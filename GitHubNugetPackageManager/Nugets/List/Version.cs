using Newtonsoft.Json;

namespace GitHubNugetPackageManager.Nugets.List
{
    public class Version
    {
        public string version { get; set; }
        public int downloads { get; set; }

        [JsonProperty("@id")]
        public string id { get; set; }
    }
}
