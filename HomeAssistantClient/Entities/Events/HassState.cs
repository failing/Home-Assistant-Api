using Newtonsoft.Json;
using System;

namespace HomeAssistantApi.Messages

{
    public class HassState
    {
        [JsonProperty("entity_id")]
        public string EntityId { get; set; }

        [JsonProperty("attributes")]
        public dynamic Attributes { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("last_changed")]
        public DateTimeOffset LastChanged { get; set; }

        [JsonProperty("last_updated")]
        public DateTimeOffset LastUpdated { get; set; }

        [JsonProperty("context")]
        public HassEventContext HassEventContext { get; set; }
    }
}
