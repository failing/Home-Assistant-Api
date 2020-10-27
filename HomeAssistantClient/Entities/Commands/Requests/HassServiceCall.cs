using Newtonsoft.Json;

namespace HomeAssistantClient.Messages
{
    public class HassServiceCall : HassCmdRequest
    {
        internal override dynamic Type => HassCommandType.CallService;

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("service")]
        public string Service { get; set; }
        [JsonProperty("service_data")]
        
        public dynamic? ServiceData { get; set; }
    }
}


