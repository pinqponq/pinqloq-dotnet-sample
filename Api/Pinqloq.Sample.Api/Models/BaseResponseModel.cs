namespace Pinqloq.Sample.Api.Models;

public class BaseResponseModel
{
    public bool Status { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
}
