using Windows.UI.Xaml.Controls;
using VDMP.App.ViewModels;

namespace VDMP.App.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page
    {
        public ShellPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Initialize(shellFrame, navigationView, KeyboardAccelerators);
        }

        public ShellViewModel ViewModel { get; } = new ShellViewModel();
    }
}
