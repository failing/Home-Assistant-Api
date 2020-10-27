using Newtonsoft.Json;

namespace HomeAssistantClient.Messages
{
    public sealed class HassEvent : HassMessage
    {
        internal override dynamic Type => HassReturnType.Event;

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("event")]
        public HassEventFull Event { get; set; }
    }
}
