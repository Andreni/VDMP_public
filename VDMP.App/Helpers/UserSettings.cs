using System;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using VDMP.DBmodel;

namespace VDMP.App.Helpers
{
    public static class UserSettings
    {
        private static StorageFolder localFolder;

        public static bool ReachDatabase { get; set; }

        public static async Task WriteApplicationLibrary(Library[] toSave)
        {
            localFolder = ApplicationData.Current.LocalFolder;
            var sampleFile = await localFolder.CreateFileAsync("appLoader.txt",
                CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(sampleFile, JsonConvert.SerializeObject(toSave));
        }

        public static async Task<Library[]> ReadLastOnlineState()
        {
            try
            {
                localFolder = ApplicationData.Current.LocalFolder;
                var sampleFile = await localFolder.GetFileAsync("appLoader.txt");
                var strings = await FileIO.ReadTextAsync(sampleFile);
                if (strings != null)
                {
                    var library = JsonConvert.DeserializeObject<Library[]>(strings);
                    if (library != null) return library;
                }
            }
            catch (JsonException)
            {
            }

            return null;
        }

        public static void WriteUserId(string userId)
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            // Write current logged in userid to storage
            localSettings.Values["userId"] = userId;
        }

        public static string ReadUserId()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            // Read data from a simple setting.
            var value = (string) localSettings.Values["userId"];

            return value;
        }

        public static void DeleteUserId()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            // Read data from a simple setting.
            localSettings.Values["userId"] = null;
        }

        public static void SetSignInState(bool condition)
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            // Write current logged in userid to storage
            localSettings.Values["autoSignIn"] = condition.ToString();
        }

        public static bool ReadAutoSignInState()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            // Read data from a simple setting.
            var value = localSettings.Values["autoSignIn"];
            bool condition;
            if (value != null)
            {
                if (!bool.TryParse(value.ToString(), out condition))
                    condition = true;
            }
            else
            {
                condition = true;
            }

            return condition;
        }

        public static async Task WritePlaybackState(TimeSpan playback, string name)
        {
            var localSettings =
                ApplicationData.Current.LocalSettings;
            var localFolder =
                ApplicationData.Current.LocalFolder;

            // Setting in a container
            var container =
                localSettings.CreateContainer(ReadUserId(), ApplicationDataCreateDisposition.Always);

            if (localSettings.Containers.ContainsKey(ReadUserId()))
                localSettings.Containers[ReadUserId()].Values[name] = playback;
        }

        public static async Task<TimeSpan> ReadPlaybackState(string name)
        {
            var localSettings =
                ApplicationData.Current.LocalSettings;
            var localFolder =
                ApplicationData.Current.LocalFolder;

            // Setting in a container
            var container =
                localSettings.CreateContainer(ReadUserId(), ApplicationDataCreateDisposition.Always);
            TimeSpan lastimeSpan;
            if (localSettings.Containers.ContainsKey(ReadUserId()))
            {
                var playback = localSettings.Containers[ReadUserId()].Values[name];
                if (playback != null) lastimeSpan = (TimeSpan) playback;
            }

            return lastimeSpan;
        }

        public static async Task DeletePlaybackState(string name)
        {
            var localSettings =
                ApplicationData.Current.LocalSettings;
            var localFolder =
                ApplicationData.Current.LocalFolder;

            // Setting in a container
            var container =
                localSettings.CreateContainer(ReadUserId(), ApplicationDataCreateDisposition.Always);

            if (localSettings.Containers.ContainsKey(ReadUserId()))
                localSettings.Containers[ReadUserId()].Values.Remove(name);
        }
    }
}