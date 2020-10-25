using Newtonsoft.Json;

namespace HomeAssistantApi.Messages

{
    public abstract class CommandRequest : HassMessage
    {
        [JsonProperty("id")]
        internal int Id { get; set; }
    }
}
