using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VDMP.App.Model;
using VDMP.DBmodel;

namespace VDMP.App.Scraper
{
    /// <summary>Used for searching and retrieving movie data from TMDb.org</summary>
    internal class TMDBApi
    {
        private const string ApiV3key = "api_key=YOUR_API_V3_KEY_GOES_HERE";
        private const string SearchOption = "search/movie?";
        private readonly Uri BaseUrl = new Uri("https://api.themoviedb.org/3/");
        private readonly HttpClient _httpClient = new HttpClient();

        /// <summary>Searches for movie.</summary>
        /// <param name="titleOfMovie">The title of movie.</param>
        /// <returns></returns>
        public async Task<List<Result>> SearchForMovieAsync(string titleOfMovie)
        {
            HttpResponseMessage resultFromSearch;

            try
            {
                using (_httpClient)
                {
                    var results = new List<Result>();
                    resultFromSearch = await _httpClient
                        .GetAsync($"{BaseUrl}{SearchOption}{ApiV3key}&query={titleOfMovie}").ConfigureAwait(false);
                    if (resultFromSearch.IsSuccessStatusCode)
                    {
                        var json = await resultFromSearch.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var resultHits = JsonConvert.DeserializeObject<RootObject>(json);
                        // If we did not get any response, skip trying to make objects:

                        foreach (var result in resultHits.Results) results.Add(result);
                        return results;
                    }
                }
            }
            catch (HttpRequestException)
            {
            }

            return null;
        }


        /// <summary>Retrieves the data for movie.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<Movie> LookUpIdForMovieAsync(int id)
        {
            using (_httpClient)
            {
                var resultFromIdLookUp =
                    await _httpClient.GetAsync($"{BaseUrl}movie/{id}?{ApiV3key}").ConfigureAwait(false);
                if (resultFromIdLookUp.IsSuccessStatusCode)
                {
                    var json = await resultFromIdLookUp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var result = JsonConvert.DeserializeObject<Movie>(json);
                    return result;
                }
            }

            return null;
        }
    }
}