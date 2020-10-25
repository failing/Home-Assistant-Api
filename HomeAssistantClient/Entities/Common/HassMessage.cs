using JsonSubTypes;
using Newtonsoft.Json;

namespace HomeAssistantApi.Messages

{
    [JsonConverter(typeof(JsonSubtypes), "Type")]
    [JsonSubtypes.KnownSubType(typeof(AuthenticationInvalid), HassReturnType.AuthInvalid)]
    [JsonSubtypes.KnownSubType(typeof(AuthenticationOk), HassReturnType.AuthOk)]
    [JsonSubtypes.KnownSubType(typeof(AuthenticationRequired), HassReturnType.AuthRequired)]
    [JsonSubtypes.KnownSubType(typeof(AuthenticationMessage), HassCommandType.Auth)]
    [JsonSubtypes.KnownSubType(typeof(HassResponse), HassReturnType.Result)]
    [JsonSubtypes.KnownSubType(typeof(HassEvent), HassReturnType.Event)]
    public abstract partial class HassMessage
    {
        [JsonProperty("type")]
        internal virtual dynamic Type { get; set; }
    }
}
