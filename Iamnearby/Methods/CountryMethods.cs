using Android.App;
using Android.Content;
using PCL.Database;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Threading.Tasks;

namespace Iamnearby.Methods
{
    public class CountryMethods
    {
        RestClient client = new RestClient("https://api.iamnearby.net");
        ISharedPreferences device_info_prefs = Application.Context.GetSharedPreferences("device_info", FileCreationMode.Private);
        UserMethods userMethods = new UserMethods();
        public async Task<string> GetCountries()
        {
            var request = new RestRequest("/v1/countries", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            if (userMethods.UserExists())
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
        public async Task<string> CountriesSearch(string query)
        {
            var request = new RestRequest("/v1/countries/search", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            if (userMethods.UserExists())
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
            request.AddQueryParameter("query", query);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
        public async Task<string> CurrentLocation(string latitude, string longitude)
        {
            //latitude = "59.966";
            //longitude = "30.346";

            var request = new RestRequest("/v1/currentlocation", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            if (userMethods.UserExists())
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
            latitude = latitude.Replace(',', '.');
            longitude = longitude.Replace(',', '.');
            request.AddQueryParameter("latitude", latitude);
            request.AddQueryParameter("longitude", longitude);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
        public async Task<string> GetRegions(string countryId)
        {
            var request = new RestRequest("/v1/regions", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            if (userMethods.UserExists())
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
            request.AddQueryParameter("countryId", countryId);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
        public async Task<string> RegionsSearch(string countryId, string query)
        {
            var request = new RestRequest("/v1/regions", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            if (userMethods.UserExists())
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
            request.AddQueryParameter("countryId", countryId);
            request.AddQueryParameter("query", query);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> CityList(string regionId)
        {
            var request = new RestRequest("/v1/cities", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            if (userMethods.UserExists())
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
            request.AddQueryParameter("regionId", regionId);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
        public async Task<string> CitySearch(string regionId, string query)
        {
            var request = new RestRequest("/v1/cities", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            if (userMethods.UserExists())
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
            request.AddQueryParameter("regionId", regionId);
            request.AddQueryParameter("query", query);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
    }
}