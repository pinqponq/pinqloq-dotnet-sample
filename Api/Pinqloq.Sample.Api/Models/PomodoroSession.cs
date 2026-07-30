namespace Pinqloq.Sample.Api.Models;

public record PomodoroSession(int Id, string Type, int DurationMinutes, DateTimeOffset StartedAt, DateTimeOffset? CompletedAt);
