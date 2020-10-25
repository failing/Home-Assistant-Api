using Newtonsoft.Json;

namespace HomeAssistantApi.Messages
{
    public class CallService : CommandRequest
    {
        internal override dynamic Type => HassCommandType.CallService;

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("service")]
        public string Service { get; set; }

        [JsonProperty("service_data")]
        public object ServiceData { get; set; }
    }
}
