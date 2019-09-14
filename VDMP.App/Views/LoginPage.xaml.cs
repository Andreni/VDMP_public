using Windows.UI.Xaml.Controls;
using VDMP.App.ViewModels;

namespace VDMP.App.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        public LoginViewModel ViewModel { get; } = new LoginViewModel();
    }
}
