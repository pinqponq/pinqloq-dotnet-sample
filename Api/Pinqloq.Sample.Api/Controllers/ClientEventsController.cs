using Microsoft.AspNetCore.Mvc;
using Pinqloq;
using Pinqloq.Sample.Api.Models;

namespace Pinqloq.Sample.Api.Controllers;

[ApiController]
[Route("client-events")]
public class ClientEventsController : ControllerBase
{
    private readonly IPinqloqLogger _pinqloqClient;

    public ClientEventsController(IPinqloqLogger pinqloqClient)
    {
        _pinqloqClient = pinqloqClient;
    }

    // ---- 4. Client/device event forwarding: the secret key never leaves this backend ----
    [HttpPost]
    public IActionResult Forward([FromBody] ClientEventRequest request)
    {
        _pinqloqClient.Enqueue(new PinqloqLogEntry
        {
            Event = request.Event,
            Identifier = request.DeviceId ?? "unknown-device",
            LogLevel = PinqloqLogLevel.Information,
            LogSourceType = PinqloqLogSourceType.Device,
            CollectionName = PinqloqCollections.Client,
            Metadata = request.Metadata
        });

        return Accepted();
    }
}
