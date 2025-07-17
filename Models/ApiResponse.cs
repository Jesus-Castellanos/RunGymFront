using Newtonsoft.Json;

namespace RunGymFront.Models
{
    public class ApiResponse
    {
        public string mensaje { get; set; }
        public string error { get; set; }

        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class ApiResponse<T>
    {
        [JsonProperty("detalle")]
        public T Detalle { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}