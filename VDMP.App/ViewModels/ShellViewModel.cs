using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.Networking.Connectivity;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using VDMP.App.DataAccess;
using VDMP.App.Helpers;
using VDMP.App.Services;
using VDMP.App.Views;
using VDMP.DBmodel;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace VDMP.App.ViewModels
{
    public class ShellViewModel : Observable
    {
        private readonly KeyboardAccelerator _altLeftKeyboardAccelerator =
            BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);

        private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);


        private bool _isBackEnabled;
        private ICommand _itemInvokedCommand;
        private IList<KeyboardAccelerator> _keyboardAccelerators;
        private ICommand _loadedCommand;
        private WinUI.NavigationView _navigationView;
        private bool _netStatus;
        private WinUI.NavigationViewItem _selected;
        private bool _userLoggedIn;


        public bool IsBackEnabled
        {
            get => _isBackEnabled;
            set => Set(ref _isBackEnabled, value);
        }

        public bool UserLoggedIn
        {
            get => _userLoggedIn;
            set => Set(ref _userLoggedIn, value);
        }


        public bool NetStatus
        {
            get => _netStatus;
            set => Set(ref _netStatus, value);
        }

        public WinUI.NavigationViewItem Selected
        {
            get => _selected;
            set => Set(ref _selected, value);
        }

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));

        public ICommand ItemInvokedCommand => _itemInvokedCommand ?? (_itemInvokedCommand =
                                                  new RelayCommand<WinUI.NavigationViewItemInvokedEventArgs>(
                                                      OnItemInvoked));

        public void Initialize(Frame frame, WinUI.NavigationView navigationView,
            IList<KeyboardAccelerator> keyboardAccelerators)
        {
            _navigationView = navigationView;
            _keyboardAccelerators = keyboardAccelerators;
            NavigationService.Frame = frame;
            NavigationService.NavigationFailed += Frame_NavigationFailed;
            NavigationService.Navigated += Frame_Navigated;
            _navigationView.BackRequested += OnBackRequested;
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
            CheckForInternetConnectivity();
            LoginUser();
        }

        public void LoginUser()
        {
            if (!UserSettings.ReadAutoSignInState())
            {
                ShowLoginScreen("Please enter your username and password");
            }
            else
            {
                if (UserSettings.ReadUserId() != null)
                {
                    UserLoggedIn = true;
                    UserSettings.ReachDatabase = true;
                    NavigationService.Navigate(typeof(LibraryMenuPage));
                }
                else
                {
                    ShowLoginScreen("Please enter your username and password");
                }
            }
        }

        public async void ShowLoginScreen(string message)
        {
            while (true)
            {
                var logInDialog = new LogInDialog {Title = message};

                var result = await logInDialog.ShowAsync();
                if (result != ContentDialogResult.Primary) return;
                var userText = logInDialog.Username.Text;
                var userPass = logInDialog.UserPassword.Password;
                if (userText.Length > 1 && userPass.Length > 1 && NetStatus)
                {
                    // Attempt to log in
                    try
                    {
                        var userSession = new UserSession(userText, userPass);
                        var userId = await new Users().LoginUser(userSession).ConfigureAwait(true);
                        if (userId == null)
                        {
                            ShowLoginScreen("Could not find your username/password");
                        }
                        else if (userId == "fail")
                        {
                            ShowLoginScreen("Connection to the database is down. Please try again later");
                        }
                        else
                        {
                            UserSettings.WriteUserId(userId);
                            UserSettings.ReachDatabase = true;
                            UserLoggedIn = true;
                            NavigationService.Navigate(typeof(LibraryMenuPage));
                        }
                    }
                    catch (HttpRequestException)
                    {
                        ShowLoginScreen("Connection to the database is down. Please try again later");
                    }
                }
                else if (!NetStatus)
                {
                    message = "Internet connection is required to log in ";
                    continue;
                }
                else
                {
                    message = "Incorrect user input, try again";
                    continue;
                }


                break;
            }
        }

        private async void OnLoaded()
        {
            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            // More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8
            _keyboardAccelerators.Add(_altLeftKeyboardAccelerator);
            _keyboardAccelerators.Add(_backKeyboardAccelerator);
            await Task.CompletedTask.ConfigureAwait(true);
        }

        /// <summary>Event that got triggered related to that the network status changed.</summary>
        /// Updates the Upper right icon regarding status
        /// The assignment must be run on the UI thread.
        /// <param name="sender">The sender.</param>
        private void NetworkInformation_NetworkStatusChanged(object sender)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => { CheckForInternetConnectivity(); });
        }

        private void CheckForInternetConnectivity()
        {
            NetStatus = NetworkInterface.GetIsNetworkAvailable();
        }


        private async void OnItemInvoked(WinUI.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavigationService.Navigate(typeof(SettingsPage));
                return;
            }

            var item = _navigationView.MenuItems
                .OfType<WinUI.NavigationViewItem>()
                .First(menuItem => (string) menuItem.Content == (string) args.InvokedItem);
            var pageType = item.GetValue(NavHelper.NavigateToProperty) as Type;
            NavigationService.Navigate(pageType);
        }

        private void OnBackRequested(WinUI.NavigationView sender, WinUI.NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.GoBack();
        }

        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw e.Exception;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;
            if (e.SourcePageType == typeof(SettingsPage))
            {
                Selected = _navigationView.SettingsItem as WinUI.NavigationViewItem;
                return;
            }

            Selected = _navigationView.MenuItems
                .OfType<WinUI.NavigationViewItem>()
                .FirstOrDefault(menuItem => IsMenuItemForPageType(menuItem, e.SourcePageType));
        }

        private bool IsMenuItemForPageType(WinUI.NavigationViewItem menuItem, Type sourcePageType)
        {
            var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
            return pageType == sourcePageType;
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key,
            VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator {Key = key};
            if (modifiers.HasValue) keyboardAccelerator.Modifiers = modifiers.Value;

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender,
            KeyboardAcceleratorInvokedEventArgs args)
        {
            var result = NavigationService.GoBack();
            args.Handled = result;
        }
    }
}