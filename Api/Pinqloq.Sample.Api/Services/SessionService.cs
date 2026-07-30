using Pinqloq;
using Pinqloq.Sample.Api.Interfaces;
using Pinqloq.Sample.Api.Models;
using System.Collections.Concurrent;

namespace Pinqloq.Sample.Api.Services;

public class SessionService : ISessionService
{
    private const string DemoIdentifier = "demo-user";

    private readonly ConcurrentDictionary<int, PomodoroSession> _sessions = new();
    private readonly IPinqloqLogger _pinqloqLogger;
    private int _nextSessionId;

    public SessionService(IPinqloqLogger pinqloqLogger)
    {
        _pinqloqLogger = pinqloqLogger;
    }

    public SessionModel.GetAll.ReturnData GetAll(SessionModel.GetAll.Request request)
    {
        var sessions = _sessions.Values
            .OrderBy(session => session.Id)
            .Select(ToReturn)
            .ToList();

        return new SessionModel.GetAll.ReturnData { Status = true, StatusCode = 200, Data = sessions };
    }

    public SessionModel.Create.ReturnData Create(SessionModel.Create.Request request)
    {
        var session = new PomodoroSession(
            Interlocked.Increment(ref _nextSessionId),
            request.Type,
            request.DurationMinutes,
            DateTimeOffset.UtcNow,
            CompletedAt: null);
        _sessions[session.Id] = session;

        LogSessionEvent("SessionStarted", session);

        return new SessionModel.Create.ReturnData { Status = true, StatusCode = 200, Data = ToReturn(session) };
    }

    public SessionModel.Update.ReturnData Update(SessionModel.Update.Request request)
    {
        var session = GetSessionOrThrow(request.Id);

        var updatedSession = session with
        {
            Type = request.Type ?? session.Type,
            DurationMinutes = request.DurationMinutes ?? session.DurationMinutes,
            CompletedAt = request.IsCompleted switch
            {
                true => session.CompletedAt ?? DateTimeOffset.UtcNow,
                false => null,
                null => session.CompletedAt
            }
        };
        _sessions[request.Id] = updatedSession;

        LogSessionEvent(request.IsCompleted == true ? "SessionCompleted" : "SessionUpdated", updatedSession);

        return new SessionModel.Update.ReturnData { Status = true, StatusCode = 200, Data = ToReturn(updatedSession) };
    }

    public BaseResponseModel Delete(SessionModel.Delete.Request request)
    {
        var session = GetSessionOrThrow(request.Id);
        _sessions.TryRemove(request.Id, out _);

        LogSessionEvent("SessionDeleted", session);

        return new BaseResponseModel { Status = true, StatusCode = 200, Message = "Session deleted." };
    }

    private PomodoroSession GetSessionOrThrow(int sessionId)
    {
        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            throw new KeyNotFoundException($"Session {sessionId} was not found.");
        }

        return session;
    }

    private void LogSessionEvent(string eventName, PomodoroSession session)
    {
        _pinqloqLogger.Enqueue(new PinqloqLogEntry
        {
            Event = eventName,
            Identifier = DemoIdentifier,
            LogLevel = PinqloqLogLevel.Information,
            CollectionName = PinqloqCollections.Jobs,
            Metadata = new Dictionary<string, string>
            {
                ["sessionId"] = session.Id.ToString(),
                ["type"] = session.Type,
                ["durationMinutes"] = session.DurationMinutes.ToString()
            }
        });
    }

    private static SessionModel.Create.Return ToReturn(PomodoroSession session)
    {
        return new SessionModel.Create.Return
        {
            Id = session.Id,
            Type = session.Type,
            DurationMinutes = session.DurationMinutes,
            StartedAt = session.StartedAt,
            CompletedAt = session.CompletedAt
        };
    }
}
