using System;
using System.Collections.Generic;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using VDMP.App.Helpers;
using VDMP.App.Services;
using VDMP.App.ViewModels;

namespace VDMP.App.Views
{
    public sealed partial class MovieDetailPage : Page
    {
        public MovieDetailPage()
        {
            InitializeComponent();
        }

        public MovieDetailViewModel ViewModel { get; } = new MovieDetailViewModel();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int MovieId) ViewModel.Initialize(MovieId);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
                NavigationService.Frame.SetListDataItemForNextConnectedAnimation(ViewModel.Item);
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>Handles the OnClick event of the LoadSelectedMovie control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the movie object.</param>
        /// <remarks>Gets the movie that has been clicked "play" on. Sets up a media source file that is sent to the media player</remarks>
        private async void LoadSelectedMovie_OnClick(object sender, RoutedEventArgs e)
        {
            var queryOption = new QueryOptions
                (CommonFileQuery.OrderByTitle, new[] {".mkv", ".avi", ".mpg", ".mov"})
                {
                    FolderDepth = FolderDepth.Deep
                };
            var name = e.OriginalSource;


            var folders = new Queue<IStorageFolder>();

            var files = await KnownFolders.VideosLibrary.CreateFileQueryWithOptions
                (queryOption).GetFilesAsync();

            foreach (var file in files)
                // If we find the media we want to play:
                if (ViewModel.Item != null && file != null && file.Path.Equals(ViewModel.Item.PathToVideo))
                {
                    var Source = MediaSource.CreateFromStorageFile(file);
                    var sourceDuration = new TimeSpan(0, 0, 0, 0);

                    var item = ((Button) e.OriginalSource).Name;
                    if (item != "LoadSelectedMovie")
                    {
                        var readTimeSpan = await UserSettings.ReadPlaybackState(ViewModel.Item.TMDbId.ToString())
                            .ConfigureAwait(true);
                        if (readTimeSpan > sourceDuration) sourceDuration = readTimeSpan;
                    }

                    Source.CustomProperties.Add("startPosition", sourceDuration);
                    Source.CustomProperties.Add("name", ViewModel.Item.TMDbId.ToString());
                    Frame.Navigate(typeof(MediaPlayerPage), Source);
                }
        }

        private void UserUpdatedRating(RatingControl sender, object args)
        {
            var value = (int) sender.Value;

            ViewModel.SetUserRating(value);
        }

        private void ResumeSelected_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}
