using Newtonsoft.Json;
using System;

namespace HomeAssistantClient.Messages
{
    public class HassEventState
    {

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("last_changed")]
        public DateTimeOffset LastChanged { get; set; }

        [JsonProperty("last_updated")]
        public DateTimeOffset LastUpdated { get; set; }

        [JsonProperty("context")]
        public HassEventContext Context { get; set; }

    }
}