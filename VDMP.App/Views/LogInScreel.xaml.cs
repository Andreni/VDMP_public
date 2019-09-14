using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VDMP.App.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogInDialog : ContentDialog
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Enter title", typeof(string), typeof(LogInDialog), new PropertyMetadata(default(string)));

        public LogInDialog()
        {
            InitializeComponent();
        }

        public string UsernameInput
        {
            get => GetUsername(Username.Text);
            set => SetValue(TextProperty, value);
        }

        public string PasswordInput
        {
            get => GetPassword(UserPassword.Password);
            set => SetValue(TextProperty, value);
        }


        private string GetUsername(string text)
        {
            return text;
        }

        private string GetPassword(string text)
        {
            return text;
        }


        private void LogInDialog_OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            {
            }
        }

        private void LogInDialog_OnSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
