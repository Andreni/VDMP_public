using System;
using System.Linq;
using System.Threading.Tasks;
using VDMP.App.DataAccess;
using VDMP.App.Helpers;
using VDMP.DBmodel;

namespace VDMP.App.ViewModels
{
    /// <summary>Display values attached to the sent movie object</summary>
    public class MovieDetailViewModel : Observable
    {
        private int _avgScore;
        private int _avgUserAmount;
        private Movie.GenreTypes _genre;
        private Movie _item;
        private bool _showRating;
        private bool _showResume;

        public bool ShowResume
        {
            get => _showResume;
            set => Set(ref _showResume, value);
        }

        public int AvgScore
        {
            get => _avgScore;
            set => Set(ref _avgScore, value);
        }

        public int AvgUserAmount
        {
            get => _avgUserAmount;
            set => Set(ref _avgUserAmount, value);
        }

        public bool ShowRating
        {
            get => _showRating;
            set => Set(ref _showRating, value);
        }

        public Movie.GenreTypes Genre
        {
            get => _genre;
            set => Set(ref _genre, value);
        }

        public Movie Item
        {
            get => _item;
            set => Set(ref _item, value);
        }

        public void Initialize(int MovieId)
        {
            var data = MovieViewModel.CollectionCopy;
            //LibraryDataService.GetContentGridData();
            Item = data.First(i => i.MovieId == MovieId);
            if (Item.TMDbId != 0)
            {
                DeterminePlaybackAsync();
                CalculateAverageScoreAsync();
            }

            SetGenre();
        }

        private void SetGenre()
        {
            Genre = (Movie.GenreTypes) Item.genre;
        }

        /// <summary>Determines the playback asynchronous.</summary>
        public async void DeterminePlaybackAsync()
        {
            var played = await UserSettings.ReadPlaybackState(Item.TMDbId.ToString()).ConfigureAwait(true);
            if (played > new TimeSpan(0, 0, 0, 0)) ShowResume = true;
        }

        /// <summary>Sets the user rating.</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public async Task SetUserRating(int value)
        {
            Item.Rating = value;
            if (await new Movies().UpdateMovieAsync(Item, Item.MovieId).ConfigureAwait(true))
            {
            }
        }

        /// <summary>Calculates the average score asynchronous.</summary>
        public async void CalculateAverageScoreAsync()
        {
            AvgUserAmount = 0;
            // Get value of "my rating" from all users
            var scores = await new Movies().GetMovieScoreAverage(Item.TMDbId).ConfigureAwait(true);
            var sum = 0;
            if (scores != null)
            {
                ShowRating = true;
                foreach (var t in scores)
                {
                    if (t == 0) continue;
                    sum += t;
                    AvgUserAmount++;
                }

                if (sum != 0) AvgScore = sum / AvgUserAmount;
            }
            else
            {
                ShowRating = false;
            }
        }
    }
}