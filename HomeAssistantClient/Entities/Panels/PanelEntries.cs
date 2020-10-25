using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeAssistantApi.Messages
{
    public class PanelEntries
    {
        [JsonProperty("component_name")]
        public string ComponentName { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("config")]
        public dynamic Config { get; set; }

        [JsonProperty("url_path")]
        public string UrlPath { get; set; }

        [JsonProperty("require_admin")]
        public bool RequireAdmin { get; set; }
    }
}
