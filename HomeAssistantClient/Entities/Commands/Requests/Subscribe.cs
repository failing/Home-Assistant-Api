using Newtonsoft.Json;

namespace HomeAssistantApi.Messages

{
    public class Subscribe : CommandRequest
    {
        internal override dynamic Type => HassCommandType.SubscribeEvents;

        [JsonProperty("event_type")]
        public SubscribeEventType EventType { get; set; }
    }
}
