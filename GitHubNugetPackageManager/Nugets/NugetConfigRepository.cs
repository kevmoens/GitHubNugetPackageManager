using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GitHubNugetPackageManager.Nugets
{
    public class NugetConfigRepository
    {
        /// <summary>
        /// Read nuget.config file and return a list of all <packageSources>.<add @key> values
        /// </summary>
        /// <returns></returns>
        public List<string> ListNugetConfigs()
        {

            var path = GetPath();
            if (!File.Exists(path))
            {
                return new List<string>();
            }
            var packages = XDocument.Load(path).Descendants("packageSources")
            .Elements("add")
            .Select(x => x.Attribute("key").Value)
            .ToList();

            return packages;
        }

        public string GetPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NuGet", "NuGet.Config");
        }
    }
}
