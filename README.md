# pinqloq .NET Sample

A minimal ASP.NET Core Web API showing how to integrate the [pinqloq](https://pinqloq.pinqponq.io) logging SDK. It's a tiny Pomodoro-timer API (in-memory, no database), each part of it demonstrating a different pinqloq logging mechanism.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![NuGet](https://img.shields.io/nuget/v/pinqloq?label=pinqloq&color=004880&logo=nuget&logoColor=white)](https://www.nuget.org/packages/pinqloq)
[![Documentation](https://img.shields.io/badge/docs-pinqloq.pinqponq.io-1E88E5)](https://pinqloq.pinqponq.io/documentation.html)

## What this sample demonstrates

- **Automatic HTTP request logging** (`UsePinqloqRequestLogging()`) — no pinqloq code in the handler at all
- **Manual business-event logging** (`IPinqloqLogger.Enqueue`) — including the error-logging path on a real not-found case
- **Device/client log forwarding** (`LogSourceType.Device`) — the secret key never reaches a client, only this backend holds it
- **Unhandled-exception logging**, via `PinqloqExceptionLoggingMiddleware`, which logs at `PinqloqLogLevel.Error` with the exception type, message, and stack trace in `Detail`, then returns a generic `500`

## Collections

pinqloq's dashboard organizes logs into named collections. This sample uses three, each with a distinct purpose:

| Collection | Constant | Contents |
|---|---|---|
| `pinqloq_sample_http` | `PinqloqCollections.Http` | Automatic HTTP request logs (the `ApiLogsCollectionName` default) |
| `pinqloq_sample_jobs` | `PinqloqCollections.Jobs` | Manual backend business events and unhandled exceptions |
| `pinqloq_sample_client` | `PinqloqCollections.Client` | Events forwarded from the client-events endpoint, tagged `LogSourceType.Device` |

You need to create all three collections in your pinqloq project before running the app, or writes to a missing/unauthorized collection will get a `403`.

### Avoiding duplicate logs

`/client-events` is **excluded** from the automatic HTTP request logger:

```csharp
app.UsePinqloqRequestLogging(options => options.ExcludePaths("/client-events"));
```

Without this, the same client-forwarded request would be recorded twice — once generically by the HTTP middleware (into `Http`) and once meaningfully by the controller (into `Client`). `ExcludePaths` matches by path segment and is case-insensitive.

### Middleware order matters

`UsePinqloqRequestLogging()` is registered **before** `PinqloqExceptionLoggingMiddleware`:

```csharp
app.UsePinqloqRequestLogging(options => options.ExcludePaths("/client-events"));
app.UseMiddleware<PinqloqExceptionLoggingMiddleware>();
```

The exception middleware sets the final `StatusCode` and swallows the exception (it doesn't rethrow), so the request logger — which reads the response after calling `next()` — only sees the correct `500` if the exception middleware runs first (further in) and finishes before control returns to the logger. Registering them the other way around silently logs every error as `200`, because the logger would capture the response before the exception middleware ever got a chance to set it.

## Running it

### 1. Dashboard setup (one-time, at pinqloq.pinqponq.io)

1. Create a project and copy its secret key.
2. Create the three collections listed above.
3. Under Team Members, edit your account and set a panel password (8+ characters) so you can view logs at the [log panel](https://pinqloq-panel.pinqponq.io).

### 2. Local setup

```bash
cp Api/Pinqloq.Sample.Api/appsettings.Development.json.example Api/Pinqloq.Sample.Api/appsettings.Development.json
```

Edit `appsettings.Development.json` and set `Pinqloq:SecretKey` to your real secret key. This file is gitignored — it never gets committed.

The `Pinqloq.Sample.Api.csproj` already references the [`pinqloq` NuGet package](https://www.nuget.org/packages/pinqloq); `dotnet restore` pulls it in automatically. See the [full SDK documentation](https://pinqloq.pinqponq.io/documentation.html) for configuration options beyond what this sample uses.

### 3. Run

```bash
dotnet run --project Api/Pinqloq.Sample.Api
```

Swagger UI is available at `http://localhost:5200/swagger` in the Development environment — use it to try every endpoint.

## Requirements

- A pinqloq account and project (free tier is enough, but check your plan's collection limit against the three collections this sample needs)
