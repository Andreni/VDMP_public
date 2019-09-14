using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VDMP.App.Exception;
using VDMP.App.Helpers;
using VDMP.DBmodel;

namespace VDMP.App.DataAccess
{
    internal class Libraries
    {
        private static readonly Uri LibrariesBaseUri = new Uri("http://localhost:5000/api/libraries/");
        private readonly HttpClient _httpClient = new HttpClient();


        internal async Task<Library[]> GetLibrariesAsync(string userId)
        {
            using (_httpClient)
            {
                if (LibrariesBaseUri != null)
                {
                    var response = await _httpClient.GetAsync(LibrariesBaseUri + "UsersLibraries/" + userId)
                        .ConfigureAwait(true);
                    if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var libraries = JsonConvert.DeserializeObject<Library[]>(json);
                        await UserSettings.WriteApplicationLibrary(libraries);
                        return libraries;
                    }
                }
            }

            return null;
        }

        /// <summary>For use when Internet or database connection is down</summary>
        /// <summary>Gets the libraries off disk asynchronous.</summary>
        /// <returns></returns>
        internal async Task<Library[]> GetLibrariesOffDiskAsync()
        {
            return await UserSettings.ReadLastOnlineState().ConfigureAwait(false);
        }

        internal async void WriteLibrariesToDiskAsync(Library[] libraries)
        {
            await UserSettings.WriteApplicationLibrary(libraries);
        }

        internal async Task<Library> AddLibraryAsync(Library library)
        {
            var json = JsonConvert.SerializeObject(library);
            using (_httpClient)
            {
                if (LibrariesBaseUri != null)
                {
                    var response = await _httpClient
                        .PostAsync(LibrariesBaseUri, new StringContent(json, Encoding.UTF8, "application/json"))
                        .ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var libraryResp = JsonConvert.DeserializeObject<Library>(json);
                        return libraryResp;
                    }
                }
            }

            return null;
        }

        /// <exception cref="VDMP.App.Exception.ApplicationAndDatabaseMismatchException">
        ///     If the application tries to update a non
        ///     existent element
        /// </exception>
        internal async Task<bool> UpdateLibraryAsync(Library library, int libraryId)
        {
            var json = JsonConvert.SerializeObject(library);
            using (_httpClient)
            {
                var response = await _httpClient.PutAsync($"{LibrariesBaseUri}{libraryId}",
                    new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(true);
                if (response.StatusCode == HttpStatusCode.NotFound) throw new ApplicationAndDatabaseMismatchException();
                if (response.IsSuccessStatusCode) return response.IsSuccessStatusCode;

                return false;
            }
        }

        internal async Task<bool> DeleteLibraryAsync(int libraryId)
        {
            using (_httpClient)
            {
                var response = await _httpClient.DeleteAsync(new Uri(LibrariesBaseUri + libraryId.ToString()))
                    .ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.NotFound) throw new ApplicationAndDatabaseMismatchException();
                return response.IsSuccessStatusCode;
            }
        }
    }
}