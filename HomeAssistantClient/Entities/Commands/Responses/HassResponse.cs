using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HomeAssistantClient.Messages

{
    public class HassResponse : HassMessage
    {
        internal override dynamic Type => HassReturnType.Result;

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("success")]
        bool Success { get; set; }

        [JsonProperty("result")]
        public JContainer Result { get; set; }
    }
}
