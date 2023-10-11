using CredentialManagement;
using Octokit;
using System.Collections.Generic;

namespace GitHubNugetPackageManager.WinCredentials
{
    public class CredentialManager
    {
        private readonly Settings.Settings _settings;

        public CredentialManager(Settings.Settings settings)
        {
            _settings = settings;
        }
        /// <summary>
        /// Load all Windows Credentials that contain the text "github.com"
        /// </summary>
        /// <returns></returns>
        public List<string> ListCredentials()
        {
            var result = new List<string>();
            var credentials = new CredentialSet();
            credentials.Load();
            foreach (var credential in credentials)
            {
                if (credential.Target.ToLower().Contains("github.com"))
                {
                    result.Add(credential.Target);
                }
            }
            return result;
        }
        public string GetPassword()
        {
            using (var cred = new Credential())
            {
                cred.Target = _settings.GitHubCredentialName;
                cred.Load();
                return cred.Password;                
            }
        }
        public Credentials GetCredential()
        {
            using (var cred = new Credential())
            {
                cred.Target = _settings.GitHubCredentialName;
                cred.Load();
                return new Credentials(cred.Password); 
            }
        }
        public string GetUserName()
        {
            using (var cred = new Credential())
            {
                cred.Target = _settings.GitHubCredentialName;
                cred.Load();
                return cred.Username;
            }
        }
    }
}
