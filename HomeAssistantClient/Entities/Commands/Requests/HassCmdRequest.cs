using Newtonsoft.Json;

namespace HomeAssistantClient.Messages
{
    public abstract class HassCmdRequest : HassMessage
    {
        [JsonProperty("id")]
        internal int Id { get; set; }
    }
}
