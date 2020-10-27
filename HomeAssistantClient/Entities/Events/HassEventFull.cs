using Newtonsoft.Json;
using System;

namespace HomeAssistantClient.Messages
{
    public class HassEventFull
    {
        [JsonProperty("time_fired")]
        public DateTimeOffset TimeFired { get; set; }

        [JsonProperty("origin")]
        public string Origin { get; set; }

        [JsonProperty("event_type")]
        public string EventType { get; set; }

        [JsonProperty("data")]
        public HassEventData Data { get; set; }

        [JsonProperty("context")]
        public HassEventContext Context { get; set; }
    }
}
