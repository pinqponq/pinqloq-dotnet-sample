namespace Pinqloq.Sample.Api;

/// <summary>
/// The three pinqloq collections this sample writes to. "/client-events" is excluded from the
/// automatic HTTP request-logging middleware (see Program.cs) precisely because it already logs
/// manually to <see cref="Client"/> - without the exclusion the same request would land twice,
/// once in Http and once in Client.
/// </summary>
public static class PinqloqCollections
{
    public const string Http = "pinqloq_sample_http";
    public const string Jobs = "pinqloq_sample_jobs";
    public const string Client = "pinqloq_sample_client";
}
