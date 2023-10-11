using GitHubNugetPackageManager.WinCredentials;
using Octokit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitHubNugetPackageManager.GitHub
{
    public class GitHubOrganizations
    {
        private readonly CredentialManager _credentialManager;

        public GitHubOrganizations(CredentialManager credentialManager)
        {
            _credentialManager = credentialManager;
        }
        public async Task<List<string>> List()
        {
            List<string> result = new();

            var client = new GitHubClient(new ProductHeaderValue("GitHubNugetPackageManager"));
            client.Credentials = _credentialManager.GetCredential();
            var orgs = await client.Organization.GetAllForCurrent();
            foreach (var org in orgs)
            {
                result.Add(org.Login);
            }

            return result;
        }
    }
}
