using GitHubNugetPackageManager.ViewModels;
using System.Windows;

namespace GitHubNugetPackageManager.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
        public MainWindow()
        {
            InitializeComponent();
        }

    }
}
