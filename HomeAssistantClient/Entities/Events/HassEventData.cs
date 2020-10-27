using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HomeAssistantClient.Messages
{
    public class HassEventData
    {
        [JsonProperty("entity_id")]
        public string EntityId { get; set; }

        [JsonProperty("new_state")]
        public HassState NewState { get; set; }

        [JsonProperty("old_state")]
        public HassState OldState { get; set; }
    }
}