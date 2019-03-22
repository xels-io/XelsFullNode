using Newtonsoft.Json;

namespace Xels.Bitcoin.Apps.Browser.Dto
{
    public class XelsApp
    {      
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("webRoot")]
        public string WebRoot { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
