using Newtonsoft.Json;

namespace HomeAssistantApi.Messages
{
    public class HassTheme
    {
        [JsonProperty("themes")]
        public dynamic Themes { get; set; }

        [JsonProperty("default_theme")]
        public string DefaultTheme { get; set; }

        [JsonProperty("default_dark_theme")]
        public dynamic DefaultDarkTheme { get; set; }
    }
}
