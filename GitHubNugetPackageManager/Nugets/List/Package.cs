using Newtonsoft.Json;
using System.Collections.Generic;

namespace GitHubNugetPackageManager.Nugets.List
{
    public class Package
    {
        [JsonProperty("@type")]
        public string type { get; set; }
        public string authors { get; set; }
        public string copyright { get; set; }
        public List<DependencyGroup> dependencyGroups { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string id { get; set; }
        public bool isPrerelease { get; set; }
        public string language { get; set; }
        public string licenseUrl { get; set; }
        public string projectUrl { get; set; }
        public bool requireLicenseAcceptance { get; set; }
        public string summary { get; set; }
        public string tags { get; set; }
        public string title { get; set; }
        public int totalDownloads { get; set; }
        public bool verified { get; set; }
        public string version { get; set; }
        public List<Version> versions { get; set; }
    }
}
