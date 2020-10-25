using Newtonsoft.Json;

namespace HomeAssistantApi.Messages

{
    public class Unsubscribe : CommandRequest
    {
        internal override dynamic Type => HassCommandType.UnsubscribeEvents;

        [JsonProperty("subscription")]
        public int Subscription { get; set; }
    }
}
