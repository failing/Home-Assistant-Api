using Newtonsoft.Json;

namespace HomeAssistantClient.Messages
{
    public class AuthenticationRequired : HassMessage
    {
        internal override dynamic Type => HassReturnType.AuthRequired;

        [JsonProperty("ha_version")]
        public string HaVersion { get; }
    }
}
