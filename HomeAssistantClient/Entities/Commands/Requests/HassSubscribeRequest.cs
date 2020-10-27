using Newtonsoft.Json;

namespace HomeAssistantClient.Messages

{
    public class HassSubscribeRequest : HassCmdRequest
    {
        internal override dynamic Type => HassCommandType.SubscribeEvents;

        [JsonProperty("event_type")]
        public HassSubscribeEventType EventType { get; set; }
    }
}
