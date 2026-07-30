using Pinqloq;
using Pinqloq.Sample.Api;
using Pinqloq.Sample.Api.Interfaces;
using Pinqloq.Sample.Api.Middleware;
using Pinqloq.Sample.Api.Services;

const string DemoIdentifier = "demo-user";

var builder = WebApplication.CreateBuilder(args);

var pinqloqSecretKey = builder.Configuration["Pinqloq:SecretKey"];
if (string.IsNullOrWhiteSpace(pinqloqSecretKey))
{
    throw new InvalidOperationException("Pinqloq:SecretKey is required. Get one from your pinqloq project settings.");
}

// ---- pinqloq: one-line SDK registration ----
builder.Services.AddPinqloq(options =>
{
    options.SecretKey = pinqloqSecretKey;
    options.ApiLogsCollectionName = PinqloqCollections.Http;
    options.Identifier = DemoIdentifier;
});

builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // SessionModel.Create.Request, SessionModel.Update.Request, etc. all share the nested
    // class name "Request" - disambiguate schema ids with the full type name.
    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
});

var app = builder.Build();

// ---- pinqloq: automatic HTTP request logging (demonstrated by every endpoint). "/client-events"
// is excluded because it already logs manually to PinqloqCollections.Client (see
// ClientEventsController) - without this it would be recorded twice for the same request.
//
// This must be registered BEFORE (i.e. wrap around) PinqloqExceptionLoggingMiddleware: the
// exception middleware sets the final StatusCode and swallows the exception, so the request
// logger - which reads the response after calling next() - only sees the correct 500 if the
// exception middleware runs first (further in) and finishes before control returns here. The
// reverse order silently logs every error as 200, since the logger would capture the response
// before the exception middleware ever gets a chance to set it. ----
app.UsePinqloqRequestLogging(options => options.ExcludePaths("/client-events"));

app.UseMiddleware<PinqloqExceptionLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
