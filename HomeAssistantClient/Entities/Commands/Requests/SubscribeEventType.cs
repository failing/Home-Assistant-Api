using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace HomeAssistantApi.Messages

{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SubscribeEventType
    {
        [EnumMember(Value = "state_changed")] StateChange,
        [EnumMember(Value = "component_loaded")] ComponentLoaded,
        [EnumMember(Value = "core_config_updated")] CoreConfigUpdates,
        [EnumMember(Value = "service_registered")] NewService,
        [EnumMember(Value = "service_removed")] ServiceRemoved,
        [EnumMember(Value = "panels_updated")] PanelUpdates,
        [EnumMember(Value = "themes_updated")] ThemeUpdates,
    }
}