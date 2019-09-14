using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Newtonsoft.Json;
using VDMP.App.DataAccess;
using VDMP.App.Helpers;
using VDMP.App.Model;
using VDMP.App.Scraper;
using VDMP.App.Services;
using VDMP.App.Views;
using VDMP.DBmodel;

namespace VDMP.App.ViewModels
{
    public class MovieViewModel : Observable
    {
        private Library _item;
        private ICommand _itemClickCommand;

        private Movies _moviesDataAccess = new Movies();

        private bool _reachDb;


        public MovieViewModel()
        {
            DeleteCommand = new RelayCommand<Movie>(async param =>
            {
                if (await new Movies().DeleteMovieAsync(param.MovieId))
                    CollectionMovies.Remove(param);
                else DisplayLossOfConnectivity();
            }, param => param != null && UserSettings.ReachDatabase);

            ReachDb = true; // UserSettings.ReachDatabase;
        }

        public ICommand DeleteCommand { get; set; }

        public ICommand ItemClickCommand =>
            _itemClickCommand ?? (_itemClickCommand = new RelayCommand<Movie>(OnItemClick));

        public MoviesObservable<Movie> CollectionMovies { get; set; }

        public static MoviesObservable<Movie> CollectionCopy { get; set; } = new MoviesObservable<Movie>();

        public bool ReachDb
        {
            get => _reachDb;
            set => Set(ref _reachDb, value);
        }

        public Library Item
        {
            get => _item;
            set => Set(ref _item, value);
        }

        private void LoadMovies()
        {
            // Call to check if internet is present
            if (Item != null)
            {
                var json = JsonConvert.SerializeObject(Item);
                if (Item.LibraryOfMovies == null) return;
                var movies = Item.LibraryOfMovies;
                foreach (var movie in movies)
                    LoadCollectionFromDb(movie);
            }
        }

        public void Initialize(int libraryId)
        {
            var data = LibraryMenuViewModel.SourceCopy;
            Item = data.First(i => i.LibraryId == libraryId);

            CollectionMovies = new MoviesObservable<Movie>();
            LoadMovies();
        }


        // Add a folder of videos
        public async void AddCollection()
        {
            var collectionPicker = new CollectionPicker();
            var filesPickedUp = await collectionPicker.PickFolderCollection().ConfigureAwait(true);
            AddToCollectionAsync(filesPickedUp);
        }

        // Add a single video file
        public async void AddSingleItem()
        {
            var collectionPicker = new CollectionPicker();
            var filePickedUp = await collectionPicker.PickSingleItem().ConfigureAwait(true);
            AddToCollectionAsync(filePickedUp);
        }

        // Send out a search response for what the item should be updated too:
        public async Task UpdateSingleItem(Movie oldMovie, string searchTitle)
        {
            /*
            if (!UserSettings.ReachDatabase)
            {
                DisplayLossOfConnectivity();
                return;
            }
            */

            try
            {
                var mediaBuilder = new MediaBuilder();
                var response = await mediaBuilder.SearchForMatchingTitleAsync(searchTitle).ConfigureAwait(true);
                if (response == null)
                {
                    DisplayLossOfConnectivity();
                }
                else
                {
                    if (response.Count < 2)
                        DisplayNoHitForSearchTitle();
                    else
                        await PromptUserWithResultsAsync(oldMovie, response, mediaBuilder).ConfigureAwait(true);
                }
            }
            catch (HttpRequestException)
            {
                DisplayLossOfConnectivity();
            }
        }

        private async Task PromptUserWithResultsAsync(Movie oldMovie, ObservableCollection<Result> response,
            MediaBuilder mediaBuilder)
        {
            var dialog = new SearchResponse
            {
                Title = "Select one title, then push ok",
                Source = response
            };
            var result = await dialog.ShowAsync();
            // Nullrefference
            var selectionByUser = dialog.Result;
            if (selectionByUser != null)
            {
                var responseer =
                    await mediaBuilder.UpdateSecondStage(oldMovie, selectionByUser.id).ConfigureAwait(true);
                UpdateCollectionAsync(oldMovie, responseer);
            }
        }


        // Write updated changes to collection:
        private async void UpdateCollectionAsync(Movie movieOld, Movie movieUpdated)
        {
            _moviesDataAccess = new Movies();

            try
            {
                if (!await _moviesDataAccess.UpdateMovieAsync(movieUpdated, movieOld.MovieId).ConfigureAwait(true))
                {
                }
            }
            catch (HttpRequestException)
            {
                DisplayLossOfConnectivity();
                return;
            }

            var item = CollectionMovies.FirstOrDefault(i => i.PathToVideo == movieOld.PathToVideo);
            if (item != null)
            {
                PropertyMapper.CopyPropertiesTo(movieUpdated, item);
                CollectionCopy = CollectionMovies;
            }
        }

        public void LoadCollectionFromDb(Movie movie)
        {
            CollectionMovies.Add(movie);
        }

        // Insert new video file into collection
        public async void AddToCollectionAsync(IReadOnlyList<StorageFile> list)
        {
            if (list != null)
            {
                var count = 0;
                var runJobs = true;

                try
                {
                    foreach (var t in list)
                    {
                        if (t == null || !runJobs) continue;
                        var mediaBuilder = new MediaBuilder();
                        var result = await mediaBuilder.AddANewVideoAsync(t).ConfigureAwait(true);
                        result.LibraryId = Item.LibraryId;
                        var movieResponse = await new Movies().AddMovieAsync(result).ConfigureAwait(true);
                        if (movieResponse != null)
                        {
                            CollectionMovies.Add(movieResponse);
                            count++;
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    runJobs = false;
                    DisplayLossOfConnectivity();
                }

                NotificationToUser.ShowToastOfAddedMoviesNotification($"VDMP :{Item.LibraryName}",
                    "Added: " + count + " items");
            }
        }

        // When a item is clicked transfer the user to the detail page:
        private void OnItemClick(Movie clickedItem)
        {
            CollectionCopy = CollectionMovies;
            if (clickedItem != null)
            {
                NavigationService.Frame.SetListDataItemForNextConnectedAnimation(clickedItem);
                NavigationService.Navigate<MovieDetailPage>(clickedItem.MovieId);
            }
        }

        private async void DisplayNoHitForSearchTitle()
        {
            var noHitForSeachTitle = new ContentDialog
            {
                Title = "No valid match found",
                Content = "There were no hits found for the title you searched for",
                CloseButtonText = "Ok"
            };
            await noHitForSeachTitle.ShowAsync();
        }

        private async void DisplayLossOfConnectivity()
        {
            var dialog = new ContentDialog
            {
                Title = "The application has no connection to database.",
                Content = "The remaning jobs have been canceled.\n" +
                          "Please try again",
                CloseButtonText = "Ok"
            };
            await dialog.ShowAsync();
        }
    }
}