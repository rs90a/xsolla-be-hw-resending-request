using System.Text.Json.Serialization;

namespace RabbitResendingRequest.Common.Models
{
    public class Message
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }
}