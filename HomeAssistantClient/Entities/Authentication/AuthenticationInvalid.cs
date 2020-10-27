using Newtonsoft.Json;

namespace HomeAssistantClient.Messages

{
    public class AuthenticationInvalid : HassMessage
    {
        internal override dynamic Type => HassReturnType.AuthInvalid;

        [JsonProperty("message")]
        public string Message { get; }
    }
}
