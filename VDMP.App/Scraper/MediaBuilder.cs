using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using VDMP.App.Model;
using VDMP.DBmodel;

namespace VDMP.App.Scraper
{
    /// <summary>User for building movie objects</summary>
    /// <remarks>
    ///     Called upon either by Collection picker class, or when user wants
    ///     to update a movie
    /// </remarks>
    public class MediaBuilder
    {
        /// <summary>Adds a new video asynchronous.</summary>
        /// <param name="file">A scanned file from disk.</param>
        /// <returns>A movie object matching results found</returns>
        public async Task<Movie> AddANewVideoAsync(StorageFile file)
        {
            var response = new Movie();
            // See if there is a hit for the file:
            var results = await SearchForMetaDataAsync(file).ConfigureAwait(false);
            if (results != null)
            {
                if (results.Count > 1)
                {
                    // Look up the first hit:
                    response = await LookUpKnownMediaIdAsync(results[0].id).ConfigureAwait(false);
                }
            }

            return await MovieBuilder(response, file).ConfigureAwait(true);
        }

        /// <summary>
        ///     Searches for matching titles asynchronous.
        ///     Used when the user requests to update a movie
        /// </summary>
        /// <param name="titleToSearchFor">The title passed by the user.</param>
        /// <returns>Returns a ObservableCollection with results</returns>
        public async Task<ObservableCollection<Result>> SearchForMatchingTitleAsync(string titleToSearchFor)
        {
            var response = await Response(titleToSearchFor).ConfigureAwait(false);
            if (response == null) return null;
            var results = new ObservableCollection<Result>();
            if (response.Count <= 1) return results;
            foreach (var resp in response)
            {
                resp.Poster_path = "https://image.tmdb.org/t/p/w500" + resp.Poster_path;
                results.Add(resp);
            }

            return results;
        }

        public async Task<Movie> UpdateSecondStage(Movie oldMovie, int idForNewMove)
        {
            // Look up selection from user
            var response = await LookUpKnownMediaIdAsync(idForNewMove).ConfigureAwait(true);
            return await MovieBuilderUpdater(response, oldMovie).ConfigureAwait(true);
        }


        /// <summary>Searches for meta data asynchronous.</summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        private async Task<List<Result>> SearchForMetaDataAsync(StorageFile file)
        {
            // Check if file is supported
            if (file == null) return null;
            var supportedMedia = new List<string> {".mov", ".mkv", ".avi", ".mpg"};
            if (supportedMedia.Contains(file.FileType))
            {
                return await Response(new FileNameCleaner(file.Name).ReturnTidyFilename()).ConfigureAwait(false);
            }

            // Illegal media was inserted or connection was down
            return null;
        }

        private async Task<List<Result>> Response(string titleToSearchFor)
        {
            try
            {
                var tmdbApi = new TMDBApi();
                var response = await tmdbApi.SearchForMovieAsync(titleToSearchFor)
                    .ConfigureAwait(true);
                //Search returned no matches
                if (response != null)
                    return response;
            }
            catch (HttpRequestException)
            {
                // Connection was down?
                //Console.WriteLine(e);
            }

            return null;
        }


        /// <summary>Looks up known media identifier asynchronous.</summary>
        /// <param name="idForMedia">The identifier for media.</param>
        /// <returns></returns>
        protected virtual async Task<Movie> LookUpKnownMediaIdAsync(int idForMedia)
        {
            var tmdbApi = new TMDBApi();
            if (idForMedia > -1)
            {
                var response = await tmdbApi.LookUpIdForMovieAsync(idForMedia).ConfigureAwait(false);
                return response;
            }

            // Illegal media was inserted
            return null;
        }

        /// <summary>Injects a poster image to the movie file.</summary>
        /// <param name="response">The response containing the posters.</param>
        /// <remarks>If the file can't be matched with any responses, default posters are set</remarks>
        /// <returns>Gives back the updated response with set posters.</returns>
        private async Task<Movie> PosterInserter(Movie response)
        {
            // If there are supplied values, assign posters:
            if (response.PosterPath != null && response.BackdropPath != null)
            {
                var img = new ImageDownloader();
                var img2 = new ImageDownloader();
                try
                {
                    var urlGrid = await img.ImageDownloaderTask($"https://image.tmdb.org/t/p/w500{response.PosterPath}",
                        response.PosterPath).ConfigureAwait(true);
                    await Task.Delay(100).ConfigureAwait(true);
                    var urlBack = await img2.ImageDownloaderTask(
                        $"https://image.tmdb.org/t/p/w780{response.BackdropPath}",
                        response.PosterPath).ConfigureAwait(true);
                    await Task.Delay(100).ConfigureAwait(true);
                    response.GridPosterImageSource = urlGrid;
                    response.BackdropImageSource = urlBack;
                }
                catch (HttpRequestException e)
                {
                    // Loss of connection occured
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                // If no values were given, set default:
                response.GridPosterImageSource =
                    "ms-appx:///Assets/splashsquare.png";
                response.BackdropImageSource = "ms-appx:///Assets/splashsquare.png";
                response.TitleOfMovie = response.FileName;
            }

            return response;
        }


        /// <summary>Builds a movie object by combining the file info and response info.</summary>
        /// <param name="response">The response matching the file data.</param>
        /// <param name="file">File scanned on disk.</param>
        /// <returns>A movie object</returns>
        private async Task<Movie> MovieBuilder(Movie response, StorageFile file)
        {
            // Assign file values to movie object
            response.FileName = file.Name;
            response.PathToVideo = file.Path;
            response = await PosterInserter(response).ConfigureAwait(true);
            if (response.TMDbId != 0)
            {
                response.genre = response.GenresMovie[0].IdGenre;
            }

            return response;
        }

        // When a movie is replaced
        private async Task<Movie> MovieBuilderUpdater(Movie response, Movie oldMovie)
        {
            // Get unique properties
            response.FileName = oldMovie.FileName;
            response.PathToVideo = oldMovie.PathToVideo;
            response.genre = response.GenresMovie[0].IdGenre;
            response.Rating = oldMovie.Rating;
            response.TMDbId = oldMovie.TMDbId;
            response = await PosterInserter(response).ConfigureAwait(true);
            response.LibraryId = oldMovie.LibraryId;
            response.MovieId = oldMovie.MovieId;
            return response;
        }
    }
}