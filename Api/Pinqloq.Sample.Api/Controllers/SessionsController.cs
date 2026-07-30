using Microsoft.AspNetCore.Mvc;
using Pinqloq.Sample.Api.Interfaces;
using Pinqloq.Sample.Api.Models;

namespace Pinqloq.Sample.Api.Controllers;

[ApiController]
[Route("sessions")]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionsController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    // ---- Read: automatic HTTP request logging, nothing pinqloq-specific here at all ----
    [HttpGet]
    public IActionResult GetAll([FromQuery] SessionModel.GetAll.Request request)
    {
        return Ok(_sessionService.GetAll(request));
    }

    // ---- Create: manual business-event logging (SessionStarted) ----
    [HttpPost]
    public IActionResult Create([FromBody] SessionModel.Create.Request request)
    {
        return Ok(_sessionService.Create(request));
    }

    // ---- Update: manual business-event logging (SessionUpdated/SessionCompleted) + a real
    // not-found error path for unknown ids, doubling as the error-logging demo ----
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] SessionModel.Update.Request request)
    {
        request.Id = id;
        return Ok(_sessionService.Update(request));
    }

    // ---- Delete: manual business-event logging (SessionDeleted) ----
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var request = new SessionModel.Delete.Request { Id = id };
        return Ok(_sessionService.Delete(request));
    }
}
