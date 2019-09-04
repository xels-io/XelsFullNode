using Newtonsoft.Json;
using Xels.Bitcoin.Features.Apps.Interfaces;

namespace Xels.Bitcoin.Features.Apps
{
    /// <summary>
    /// Instances created from xelsApp.json
    /// </summary>
    public class XelsApp : IXelsApp
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        
        public string Location { get; set; }

        [JsonProperty("webRoot")]
        public string WebRoot { get; set; } = "wwwroot";

        public string Address { get; set; }

        public bool IsSinglePageApp { get; set; } = true;
    }
}
