using System.Text.Json.Serialization;

namespace Pinqloq.Sample.Api.Models;

public class SessionModel
{
    public class Create
    {
        public class Request : BaseRequestModel
        {
            public required string Type { get; set; }
            public int DurationMinutes { get; set; }
        }

        public class ReturnData : BaseResponseModel
        {
            public required Return Data { get; set; }
        }

        public class Return
        {
            public int Id { get; set; }
            public required string Type { get; set; }
            public int DurationMinutes { get; set; }
            public DateTimeOffset StartedAt { get; set; }
            public DateTimeOffset? CompletedAt { get; set; }
        }
    }

    public class GetAll
    {
        public class Request : BaseRequestModel
        {
        }

        public class ReturnData : BaseResponseModel
        {
            public required List<Create.Return> Data { get; set; }
        }
    }

    public class Update
    {
        public class Request : BaseRequestModel
        {
            [JsonIgnore]
            public int Id { get; set; }

            public string? Type { get; set; }
            public int? DurationMinutes { get; set; }
            public bool? IsCompleted { get; set; }
        }

        public class ReturnData : BaseResponseModel
        {
            public required Create.Return Data { get; set; }
        }
    }

    public class Delete
    {
        public class Request : BaseRequestModel
        {
            [JsonIgnore]
            public int Id { get; set; }
        }
    }
}
