using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using VDMP.App.Helpers;
using VDMP.App.Services;

namespace VDMP.App.ViewModels
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings.md
    public class SettingsViewModel : Observable
    {
        private bool _autoSignIn;
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        private ICommand _switchThemeCommand;

        private string _versionDescription;

        public SettingsViewModel()
        {
            AutoSignIn = UserSettings.ReadAutoSignInState();
        }

        public bool AutoSignIn
        {
            get => _autoSignIn;

            set
            {
                Set(ref _autoSignIn, value);
                UserSettings.SetSignInState(value);
            }
        }

        public ElementTheme ElementTheme
        {
            get => _elementTheme;

            set => Set(ref _elementTheme, value);
        }

        public string VersionDescription
        {
            get => _versionDescription;

            set => Set(ref _versionDescription, value);
        }

        public string HostnameComputer { get; set; }

        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async param =>
                        {
                            ElementTheme = param;
                            await ThemeSelectorService.SetThemeAsync(param);
                        });

                return _switchThemeCommand;
            }
        }

        public async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            HostnameComputer = Hostname();
            await Task.CompletedTask;
        }

        private string Hostname()
        {
            var hostNames = NetworkInformation.GetHostNames();
            var hostName = hostNames.FirstOrDefault(name => name.Type == HostNameType.DomainName)?.DisplayName ?? "???";

            return hostName;
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}