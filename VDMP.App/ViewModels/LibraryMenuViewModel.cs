using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Animations;
using VDMP.App.DataAccess;
using VDMP.App.Exception;
using VDMP.App.Helpers;
using VDMP.App.Services;
using VDMP.App.Views;
using VDMP.DBmodel;

namespace VDMP.App.ViewModels
{
    /// <summary>Displays libraries the user has created</summary>
    /// <remarks>Any libraries the user has created, will either be retrieved from the database, or localstorage </remarks>
    public class LibraryMenuViewModel : Observable
    {
        private ICommand _itemClickCommand;

        private bool _reachDb;


        public LibraryMenuViewModel()
        {
            Source = new ObservableCollection<Library>();

            LoadLibraries();

            DeleteCommand = new RelayCommand<Library>(async param =>
            {
                try
                {
                    if (await new Libraries().DeleteLibraryAsync(param.LibraryId))
                    {
                    }

                    Source.Remove(param);
                }
                catch (ApplicationAndDatabaseMismatchException databaseMismatchException)
                {
                    // log error to disk
                    await ApplicationHasDataOutOfSync();
                }
                catch (HttpRequestException)
                {
                    // Notify user that the database is down
                }

                ;
            }, param => param != null);
        }

        public ICommand ItemClickCommand =>
            _itemClickCommand ?? (_itemClickCommand = new RelayCommand<Library>(OnItemClick));

        public ICommand DeleteCommand { get; set; }

        public ObservableCollection<Library> Source { get; }
        public static ObservableCollection<Library> SourceCopy { get; private set; }

        


        /// <summary>Loads the libraries.</summary>
        /// <remarks>Attempts first to load from Database. If that fails, it loads from disk </remarks>
        internal async void LoadLibraries()
        {
            // Call to check if internet is present
            var libraries = new Libraries();
            try
            {
                try
                {

                            var libs = await libraries.GetLibrariesAsync(UserSettings.ReadUserId())
                                .ConfigureAwait(true);
                            if (libs != null)
                                InsertLibrariesToCollection(libs);
                            else
                                await loadFromDisk(libraries).ConfigureAwait(true);

                }
                catch (HttpRequestException)
                {
                    // If the loading fails, try to load the library from disk
                    
                    await loadFromDisk(libraries);
                }
            }
            catch (System.Exception e)
            {
                ApplicationHasDataOutOfSync();
                Console.WriteLine(e);
                // Shutdown
            }
        }

        /// <summary>Loads from disk.</summary>
        /// <param name="libraries">The libraries.</param>
        /// <returns></returns>
        private async Task loadFromDisk(Libraries libraries)
        {
            //if (ReachDb) await ApplicationIsRunningInOfflineMode().ConfigureAwait(true);
            
            var libs = await libraries.GetLibrariesOffDiskAsync().ConfigureAwait(true);
            InsertLibrariesToCollection(libs);
        }

        /// <summary>Inserts the libraries to collection.</summary>
        /// <param name="libraries">The libraries.</param>
        private void InsertLibrariesToCollection(Library[] libraries)
        {
            foreach (var lib in libraries)
                Source.Add(lib);
            SourceCopy = Source;
        }

        /// <summary>Called when [item click] occurs on a library.</summary>
        /// <param name="clickedItem">The clicked item.</param>
        private void OnItemClick(Library clickedItem)
        {
            if (clickedItem != null)
            {
                NavigationService.Frame.SetListDataItemForNextConnectedAnimation(clickedItem);
                NavigationService.Navigate<MoviesPage>(clickedItem.LibraryId);
            }
        }

        /// <summary>Adds a new library asynchronous.</summary>
        /// <param name="text">Given name to the library.</param>
        /// <returns></returns>
        internal async Task AddANewLibraryAsync(string text)
        {
            var lib = new Library(text, "Movie");
            lib.UserId = UserSettings.ReadUserId();
            lib = await new Libraries().AddLibraryAsync(lib).ConfigureAwait(true);
            if (lib != null)
            {
                Source.Add(lib);
                SourceCopy = Source;
            }
        }

        /// <summary>Notify the user that the application is out of sync with the database.</summary>
        /// <returns></returns>
        private async Task ApplicationHasDataOutOfSync()
        {
            var noHitForSeachTitle = new ContentDialog
            {
                Title = "Error",
                Content = "The application has encountered an error.\n" +
                          "It will now terminate",
                CloseButtonText = "Ok"
            };
            await noHitForSeachTitle.ShowAsync();
            Application.Current.Exit();
        }

        /// <summary>Notify that the application is running in offline mode.</summary>
        /// <returns></returns>
        private async Task ApplicationIsRunningInOfflineMode()
        {
            var offlineMode = new ContentDialog
            {
                Title = "Database is not reachable",
                Content = "The application have now switched over to offline mode.\n" +
                          "You will not be able to modify your collection for this session.\n" +
                          "To get out of this mode, close the application.",
                CloseButtonText = "Ok"
            };
            await offlineMode.ShowAsync();
        }

        /// <summary>Updates the selected library asynchronous.</summary>
        /// <param name="librarySelected">The library selected.</param>
        /// <returns></returns>
        public async Task UpdateSelectedLibraryAsync(Library librarySelected)
        {
            var searchDialog = new SearchDialog
            {
                Title = "Name of library?",
                PrimaryButtonText = "Ok"
            };
            var result = await searchDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var text = searchDialog.TextInput;
                if (text.Length > 1)
                {
                    librarySelected.LibraryName = text;
                    await InsertNewLibrary(librarySelected);
                }
            }
        }

        /// <summary>Inserts the new library.</summary>
        /// <param name="librarySelected">The library selected.</param>
        /// <returns></returns>
        private async Task InsertNewLibrary(Library librarySelected)
        {
            try
            {
                if (await new Libraries().UpdateLibraryAsync(librarySelected, librarySelected.LibraryId))
                {
                    var item = Source.FirstOrDefault(i => i.LibraryId == librarySelected.LibraryId);
                    if (item != null)
                    {
                        item = librarySelected;
                        SourceCopy = Source;
                    }
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (ApplicationAndDatabaseMismatchException)
            {
                await ApplicationHasDataOutOfSync().ConfigureAwait(true);
            }
        }
    }
}