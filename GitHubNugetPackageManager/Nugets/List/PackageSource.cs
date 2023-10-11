using System.Collections.Generic;

namespace GitHubNugetPackageManager.Nugets.List
{
    public class PackageSource
    {
        public List<Package> data { get; set; }
        public int totalHits { get; set; }
    }
}
