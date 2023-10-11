using Newtonsoft.Json;

namespace GitHubNugetPackageManager.Nugets.List
{
    public class DependencyGroup
    {
        [JsonProperty("@id")]
        public string id { get; set; }

        [JsonProperty("@type")]
        public string type { get; set; }
        public object dependencies { get; set; }
    }
}
