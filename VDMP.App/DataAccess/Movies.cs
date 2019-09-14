using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VDMP.App.Exception;
using VDMP.DBmodel;

namespace VDMP.App.DataAccess
{
    public class Movies
    {
        private static readonly Uri MovieBaseUri = new Uri("http://localhost:5000/api/Movies");
        private readonly HttpClient _httpClient = new HttpClient();


        public async Task<Movie[]> GetMoviesAsync()
        {
            using (_httpClient)
            {
                if (MovieBaseUri != null)
                {
                    var response = await _httpClient.GetAsync(MovieBaseUri).ConfigureAwait(false);
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var movies = JsonConvert.DeserializeObject<Movie[]>(json);
                    return movies;
                }

                return null;
            }
        }

        public async Task<List<int>> GetMovieScoreAverage(int id)
        {
            try
            {
                using (_httpClient)
                {
                    if (MovieBaseUri != null)
                    {
                        var response = await _httpClient.GetAsync(MovieBaseUri + "/AverageScore/" + id)
                            .ConfigureAwait(false);
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                            var scores = JsonConvert.DeserializeObject<List<int>>(json);
                            return scores;
                        }
                    }
                }
            }
            catch (HttpRequestException)
            {
            }

            return null;
        }

        internal async Task<Movie> AddMovieAsync(Movie movie)
        {
            var json = JsonConvert.SerializeObject(movie);
            using (_httpClient)
            {
                if (MovieBaseUri != null)
                {
                    var response = await _httpClient
                        .PostAsync(MovieBaseUri, new StringContent(json, Encoding.UTF8, "application/json"))
                        .ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var moviedb = JsonConvert.DeserializeObject<Movie>(jsonResponse);
                        return moviedb;
                    }
                 }
            }

            throw new HttpRequestException();
        }

        /// <summary>Updates the movie asynchronous.</summary>
        /// <param name="movie">The movie.</param>
        /// <returns>Status on if the movie was updated</returns>
        /// <exception cref="ApplicationAndDatabaseMismatchException">If the application tries to update a non existent element</exception>
        internal async Task<bool> UpdateMovieAsync(Movie movie, int oldMovieId)
        {
            var json = JsonConvert.SerializeObject(movie);

            using (_httpClient)
            {
                var response = await _httpClient.PutAsync($"{MovieBaseUri}/{oldMovieId}",
                    new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.NoContent) return response.IsSuccessStatusCode;
                if (response.StatusCode == HttpStatusCode.NotFound) throw new ApplicationAndDatabaseMismatchException();
                throw new HttpRequestException();
                return false;
            }
        }

        internal async Task<bool> DeleteMovieAsync(int oldMovieId)
        {
            try
            {
                using (_httpClient)
                {
                    var response = await _httpClient.DeleteAsync(new Uri(MovieBaseUri, "Movies/" + oldMovieId))
                        .ConfigureAwait(false);
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        throw new ApplicationAndDatabaseMismatchException();
                    return response.IsSuccessStatusCode;
                }
            }
            catch (HttpRequestException e)
            {
            }

            return false;
        }
    }
}