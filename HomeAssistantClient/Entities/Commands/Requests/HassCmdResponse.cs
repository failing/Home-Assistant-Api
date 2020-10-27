using JsonSubTypes;
using Newtonsoft.Json;

namespace HomeAssistantClient.Messages

{
    public class HassCmdResponse<T> : HassMessage
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        internal override dynamic Type => HassReturnType.Result;

        [JsonProperty("success")]
        bool Success { get; set; }

        [JsonProperty("result")]
        public virtual T Result { get; set; }
    }
}
