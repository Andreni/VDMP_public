using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using VDMP.App.Helpers;
using VDMP.App.ViewModels;

namespace VDMP.App.Views
{
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        public SettingsViewModel ViewModel { get; } = new SettingsViewModel();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await ViewModel.InitializeAsync();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            UserSettings.DeleteUserId();
            var noHitForSeachTitle = new ContentDialog
            {
                Title = "Signing out",
                Content = "The application has now signed you out.\n" +
                          "It will now exit.",
                CloseButtonText = "Ok"
            };

            var result = await noHitForSeachTitle.ShowAsync();
            Application.Current.Exit();
        }
    }
}
