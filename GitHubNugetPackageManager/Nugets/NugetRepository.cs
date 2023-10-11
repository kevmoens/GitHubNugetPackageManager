using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GitHubNugetPackageManager.WinCredentials;
using System.Net.Http;
using Newtonsoft.Json;
using GitHubNugetPackageManager.Nugets.List;

namespace GitHubNugetPackageManager.Nugets
{
    /// <summary>
    /// 
    /// use the dotnet command line 
    ///     https://learn.microsoft.com/en-us/nuget/reference/dotnet-commands
    ///     Setup nuget.config file
    ///         https://learn.microsoft.com/en-us/nuget/reference/nuget-config-file#packagesourcecredentials
    ///         Generate Token
    ///         In GitHub – User - Settings – Developer Settings – Personal Access Tokens – Generate Token (classic)
    ///             Personal Access Token needs scopes of
    ///                 write:packages(all)
    ///                 delete:packages
    ///             Add Source to nuget.config
    ///                 dotnet nuget add source --username USERNAME --password ${{ secrets.GITHUB_TOKEN }} --store-password-in-clear-text --name github "https://nuget.pkg.github.com/NAMESPACE/index.json"
    /// 
    /// </summary>
    public class NugetRepository
    {
        private readonly ILogger<NugetRepository> _logger;
        private readonly CredentialManager _credentialManager;
        private readonly Settings.Settings _settings;

        public NugetRepository(ILogger<NugetRepository> logger, CredentialManager credentialManager, Settings.Settings settings)
        {
            _logger = logger;
            _credentialManager = credentialManager;
            _settings = settings;
        }

        public async Task<List<PackageVersion>> ListPackagesDetails()
        {
            HttpClient client = new();
            List<PackageVersion> result = new();
            try
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_credentialManager.GetPassword()}");

                HttpResponseMessage response = await client.GetAsync($"https://nuget.pkg.github.com/{_settings.GitHubOrganization}/query");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                PackageSource packageSource = JsonConvert.DeserializeObject<PackageSource>(responseBody);

                foreach (var package in packageSource.data)
                {
                    foreach (var version in package.versions)
                    {
                        result.Add(new PackageVersion() { Name = package.id, Version = version.version, Url = version.id });
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request exception: {e.Message}");
            }
            return result;
        }

        /// <summary>
        /// Use the dotnet nuget command line to add the package in the source repo using a nuget.config file
        /// </summary>
        /// <param name="nupkgPath"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public string AddPackage(string nupkgPath, string source)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "dotnet",
                Arguments = $"nuget push \"{nupkgPath}\" --source \"{source}\" --api-key \"{_credentialManager.GetPassword()}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };            
            
            var process = new Process { StartInfo = startInfo };

            

            process.Start();

            process.WaitForExit();
            var errorResult = process.StandardError.ReadToEnd();
            _logger.LogInformation(errorResult);

            var streamReader = new StreamReader(process.StandardOutput.BaseStream);
            var output = process.StandardOutput.ReadToEnd();
            if (output.ToLower().Contains("error"))
            {
                errorResult = output;
            }
            _logger.LogInformation(output);


            return errorResult;
        }

        /// <summary>
        /// Use the dotnet nuget command line to remove the package in the source repo using a nuget.config file
        /// 
        /// GitHub has blocked this...
        /// </summary>
        /// <param name="nupkgPath">Path to the NuGet package</param>
        /// <param name="source">The package source URL</param>
        /// <returns>The standard error output stream as text</returns>
        public string RemovePackage(string nupkgName, string version, string source)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "dotnet",
                Arguments = $"nuget delete {nupkgName} {version} -s \"{source}\" --api-key \"{_credentialManager.GetPassword()}\" --non-interactive",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = startInfo };


            process.Start();


            process.WaitForExit();


            var streamReader = new StreamReader(process.StandardOutput.BaseStream);
            var output = process.StandardOutput.ReadToEnd();
            _logger.LogInformation(output);

            var errorResult = process.StandardError.ReadToEnd();
            _logger.LogInformation(errorResult);

            return errorResult;
        }
    }
}
