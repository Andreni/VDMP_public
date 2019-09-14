using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using VDMP.App.Helpers;

namespace VDMP.App.Services
{
    public static class ThemeSelectorService
    {
        private const string SettingsKey = "AppBackgroundRequestedTheme";

        public static ElementTheme Theme { get; set; } = ElementTheme.Default;

        public static async Task InitializeAsync()
        {
            Theme = await LoadThemeFromSettingsAsync();
        }

        public static async Task SetThemeAsync(ElementTheme theme)
        {
            Theme = theme;

#pragma warning disable CA2007 // Do not directly await a Task
            await SetRequestedThemeAsync();
#pragma warning restore CA2007 // Do not directly await a Task
#pragma warning disable CA2007 // Do not directly await a Task
            await SaveThemeInSettingsAsync(Theme);
#pragma warning restore CA2007 // Do not directly await a Task
        }

        public static async Task SetRequestedThemeAsync()
        {
            foreach (var view in CoreApplication.Views)
                await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (Window.Current.Content is FrameworkElement frameworkElement)
                        frameworkElement.RequestedTheme = Theme;
                });
        }

        private static async Task<ElementTheme> LoadThemeFromSettingsAsync()
        {
            var cacheTheme = ElementTheme.Default;
#pragma warning disable CA2007 // Do not directly await a Task
            var themeName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);
#pragma warning restore CA2007 // Do not directly await a Task

            if (!string.IsNullOrEmpty(themeName)) Enum.TryParse(themeName, out cacheTheme);

            return cacheTheme;
        }

        private static async Task SaveThemeInSettingsAsync(ElementTheme theme)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, theme.ToString()).ConfigureAwait(false);
        }
    }
}