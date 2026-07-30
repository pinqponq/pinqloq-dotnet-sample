namespace Pinqloq.Sample.Api.Models;

public record ClientEventRequest(string Event, string? DeviceId, Dictionary<string, string>? Metadata);
