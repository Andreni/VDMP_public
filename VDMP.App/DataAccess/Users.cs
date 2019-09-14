using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VDMP.DBmodel;

namespace VDMP.App.DataAccess
{
    public class Users
    {
        private static readonly Uri UsersBaseUri = new Uri("http://localhost:5000/api/users/");

        private readonly HttpClient _httpClient = new HttpClient();


        internal async Task<string> LoginUser(UserSession userSession)
        {
            var json = JsonConvert.SerializeObject(userSession);
            using (_httpClient)
            {
                if (UsersBaseUri == null) return null;
                var response = await _httpClient
                    .PostAsync(UsersBaseUri + "login/", new StringContent(json, Encoding.UTF8, "application/json"))
                    .ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var userId = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return userId;
                }

                if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    return "fail";
                return null;
            }
        }

        // TODO Remove?
        /*
        internal async Task<bool> CanReachDatabase(string userId)
        {
            using (_httpClient)
            {
                _httpClient.Timeout = new TimeSpan(0, 0, 0, 5);

                try
                {
                    var response = await _httpClient.GetAsync(UsersBaseUri + userId).ConfigureAwait(true);
                    if (response.IsSuccessStatusCode) return true;
                }
                catch (TaskCanceledException)
                {
                    // Can not reach database fast enough
                }
            }

            _httpClient.Dispose();
            return false;
        }
        */
    }
}