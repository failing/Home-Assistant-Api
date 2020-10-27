using Newtonsoft.Json;

namespace HomeAssistantClient.Messages
{
    public class HassUnsubscribeRequest : HassCmdRequest
    {
        internal override dynamic Type => HassCommandType.UnsubscribeEvents;

        [JsonProperty("subscription")]
        public int Subscription { get; set; }
    }
}
