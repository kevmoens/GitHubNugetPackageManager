using GitHubNugetPackageManager.GitHub;
using GitHubNugetPackageManager.Nugets;
using GitHubNugetPackageManager.ViewModels;
using GitHubNugetPackageManager.Views;
using GitHubNugetPackageManager.WinCredentials;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using System;
using System.Windows;

namespace GitHubNugetPackageManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigureServices();
            MainWindow = _serviceProvider.GetService<MainWindow>();
            MainWindow.Show();
        }
        public void ConfigureServices()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            services.AddTransient<CredentialManager>();
            services.AddTransient<NugetRepository>();
            services.AddTransient<NugetConfigRepository>();
            services.AddTransient<GitHubOrganizations>();
            services.AddSingleton<Settings.Settings>();

            services.AddLogging(builder => builder.AddNLog());

            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
