using GitHubNugetPackageManager.DragDrop;
using GitHubNugetPackageManager.GitHub;
using GitHubNugetPackageManager.MVVM;
using GitHubNugetPackageManager.Nugets;
using GitHubNugetPackageManager.Nugets.List;
using GitHubNugetPackageManager.WinCredentials;
using GitHubNugetPackageManager.WinStore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GitHubNugetPackageManager.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged, IFileDragDropTarget
    {
        private readonly CredentialManager _credentialManager;
        private readonly Settings.Settings _settings;
        private readonly NugetRepository _nugetRepository;
        private readonly NugetConfigRepository _nugetConfigRepository;
        private readonly GitHubOrganizations _gitHubOrganizations;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindowViewModel(CredentialManager credentialManager, Settings.Settings settings, NugetRepository nugetRepository, NugetConfigRepository nugetConfigRepository, GitHubOrganizations gitHubOrganizations)
        {
            _credentialManager = credentialManager;
            _settings = settings;
            _nugetRepository = nugetRepository;
            _nugetConfigRepository = nugetConfigRepository;
            _gitHubOrganizations = gitHubOrganizations;
            WindowLoadedCommand = new DelegateCommand(OnWindowLoaded);
            ConnectCommand = new DelegateCommand(OnConnect);
            OpenNugetConfigCommand = new DelegateCommand(OnOpenNugetConfig);
            LaunchExplorerCommand = new DelegateCommand(OnLaunchExplorer);
            NugetPushCommand = new DelegateCommand(OnNugetPush);
            NugetRefreshListCommand = new DelegateCommand(async () => await OnRefreshList());
            NugetRemoveCommand = new DelegateCommand(OnNugetRemove);
            NugetPackageViewCommand = new DelegateCommand(OnNugetPackageView);
        }


        public ICommand WindowLoadedCommand { get; set; }
        public ICommand ConnectCommand { get; set; }
        public ICommand LaunchExplorerCommand { get; set; }
        public ICommand OpenNugetConfigCommand { get; set; }
        public ICommand NugetPushCommand { get; set; }
        public ICommand NugetRefreshListCommand { get; set; }
        public ICommand NugetRemoveCommand { get; set; }
        public ICommand NugetPackageViewCommand { get; set; }

        private ObservableCollection<string>  _credentials = new();
        public ObservableCollection<string> Credentials
        {
            get { return _credentials; }
            set { _credentials = value; OnPropertyChanged(); }
        }


        private string _gitHubCredentialName;
        public string GitHubCredentialName
        {
            get { return _gitHubCredentialName; }
            set { _gitHubCredentialName = value; OnPropertyChanged(); }
        }
        private string _NuPkgPath;

        public string NuPkgPath
        {
            get { return _NuPkgPath; }
            set { _NuPkgPath = value; OnPropertyChanged(); }
        }
        private ObservableCollection<string> _NugetPackageSources = new();

        public ObservableCollection<string> NugetPackageSources
        {
            get { return _NugetPackageSources; }
            set { _NugetPackageSources = value; OnPropertyChanged(); }
        }
        private ObservableCollection<string> _organizations = new();

        public ObservableCollection<string> Organizations
        {
            get { return _organizations; }
            set { _organizations = value; OnPropertyChanged(); }
        }

        private ObservableCollection<PackageVersion> _NugetPackages = new();

        public ObservableCollection<PackageVersion> NugetPackages
        {
            get { return _NugetPackages; }
            set { _NugetPackages = value; OnPropertyChanged(); }
        }
        private PackageVersion _SelectedPackage;

        public PackageVersion SelectedPackage
        {
            get { return _SelectedPackage; }
            set { _SelectedPackage = value; OnPropertyChanged(); }
        }
        private string _selectedOrganization;

        public string SelectedOrganization
        {
            get { return _selectedOrganization; }
            set { _selectedOrganization = value; OnPropertyChanged(); }
        }


        private string _UploadSource = "github";

        public string UploadSource
        {
            get { return _UploadSource; }
            set { _UploadSource = value; OnPropertyChanged(); }
        }


        private void OnWindowLoaded()
        {
            _credentialManager.ListCredentials().ForEach(x => Credentials.Add(x));
            _nugetConfigRepository.ListNugetConfigs().ForEach(x => NugetPackageSources.Add(x));
        }

        private async void OnConnect()
        {
            _settings.GitHubCredentialName = GitHubCredentialName;
            await OnRefreshList();
        }

        private void OnOpenNugetConfig()
        {
            Process.Start(new ProcessStartInfo(_nugetConfigRepository.GetPath()) { UseShellExecute = true });
        }
        private void OnLaunchExplorer()
        {
            try
            {
                WindowsStoreAppLauncher.Launch("50582LuanNguyen.NuGetPackageExplorer_w6y2tyx5bpzwa!App");
                //WindowsStoreAppLauncher.Launch("50582LuanNguyen.NuGetPackageExplorer_6.0.64.0_x64__w6y2tyx5bpzwa!App");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please install Nuget Package Explorer from the Windows Store:" + ex.Message);
            }
}
        private async Task OnRefreshList()
        {
            if (string.IsNullOrEmpty(GitHubCredentialName))
            {
                MessageBox.Show("Credentials are required.");
                return;
            }
            Organizations.Clear();
            foreach (var org in await _gitHubOrganizations.List())
            {
                Organizations.Add(org);
            }

            NugetPackages.Clear();
            foreach (var package in await _nugetRepository.ListPackagesDetails())
            {
                NugetPackages.Add(package);
            }
        }
        
        private void OnNugetPush()
        {
            var error = _nugetRepository.AddPackage(NuPkgPath, UploadSource);
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error);
                return;
            }
            MessageBox.Show($"{Path.GetFileNameWithoutExtension(NuPkgPath)} pushed to {UploadSource}");
        }

        private void OnNugetRemove()
        {
            if (SelectedPackage == null)
            {
                MessageBox.Show("Please select a package to remove");
                return;
            }
            if (MessageBox.Show($"Are you sure you want to remove {SelectedPackage.Name} {SelectedPackage.Version}", "Remove Nuget Package Version", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }
            var error = _nugetRepository.RemovePackage(SelectedPackage.Name, SelectedPackage.Version, UploadSource);
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error);
                return;
            }

            MessageBox.Show($"{Path.GetFileName(NuPkgPath)} {SelectedPackage.Version} removed from to {UploadSource}");
            NugetPackages.Remove(SelectedPackage);

        }

        public void OnFileDrop(string[] filepaths)
        {
            if (filepaths?.Length > 0)
            {
                NuPkgPath = filepaths[0];
            }
        }

        private void OnNugetPackageView()
        {
            if (SelectedPackage == null)
            {
                MessageBox.Show("Please select a package to view.");
                return;
            }
            try
            {
                Process.Start(new ProcessStartInfo($"https://github.com/orgs/{_settings.GitHubOrganization}/packages/nuget/{SelectedPackage.Name}/versions") { UseShellExecute = true });
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
