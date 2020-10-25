using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace HomeAssistantApi.Messages
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HassCommandType
    {
        [EnumMember(Value = "subscribe_events")] SubscribeEvents,
        [EnumMember(Value = "unsubscribe_events")] UnsubscribeEvents,
        [EnumMember(Value = "call_service")] CallService,
        [EnumMember(Value = "get_states")] GetStates,
        [EnumMember(Value = "get_config")] GetConfig,
        [EnumMember(Value = "get_services")] GetServices,
        [EnumMember(Value = "auth/current_user")] GetCurrentUser,
        [EnumMember(Value = "manifest/list")] GetIntergrations, 
        [EnumMember(Value = "entity/source")] Source,

        [EnumMember(Value = "manifest/get")] GetIntergration,
        [EnumMember(Value = "auth")] Auth,
        [EnumMember(Value = "get_panels")] GetPanels,
        [EnumMember(Value = "media_player_thumbnail")] MediaPlayerThumbnail,
        [EnumMember(Value = "ping")] Ping,
    }
}
