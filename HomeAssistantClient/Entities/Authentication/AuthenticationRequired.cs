using Newtonsoft.Json;

namespace HomeAssistantApi.Messages
{
    public class AuthenticationRequired : HassMessage
    {
        internal override dynamic Type => HassReturnType.AuthRequired;

        [JsonProperty("ha_version")]
        public string HaVersion { get; }
    }
}
