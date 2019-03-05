using System;
using System.Globalization;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Newtonsoft.Json;
using PCL.Database;
using PCL.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace Iamnearby.Methods
{
    public class SpecializationMethods
    {
        UserMethods userMethods = new UserMethods();
        ISharedPreferences device_info_prefs = Application.Context.GetSharedPreferences("device_info", FileCreationMode.Private);
        public async Task<string> GetUpperSpecializations()
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/catalog/categories", Method.GET);
            if (userMethods.UserExists())
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> GetSubCategories(string categoryId)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/catalog/subcategories", Method.GET);
            if (userMethods.UserExists())
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            request.AddQueryParameter("categoryId", categoryId);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> ExpertsList(string categoryId, string lat, string lng, int sortType, string cityId, string distanceRadius, bool hasReviews, int offset = 0)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/catalog/experts", Method.POST);
            if (userMethods.UserExists())
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            if (String.IsNullOrEmpty(distanceRadius))
                distanceRadius = "100000";
            else if (distanceRadius == "0.5")
                distanceRadius = "500";
            else
                distanceRadius = (Convert.ToInt32(distanceRadius) * 1000).ToString();
            var coords_obj = new Coordinates
            {
                latitude = lat.Replace(',', '.'),
                longitude = lng.Replace(',', '.')
            };
            var filter_obj = new ExpertFilter
            {
                cityId = cityId,//"176",
                distanceRadius = distanceRadius,
                hasReviews = hasReviews
            };
            var center_obj = new CenterForMapFilter
            {
                latitude = /*Convert.ToDouble(*/lat/*, CultureInfo.InvariantCulture)*/,
                longitude = /*Convert.ToDouble(*/lng/*, CultureInfo.InvariantCulture)*/
            };
            //Toast.MakeText(context, "объект координат создан", ToastLength.Short).Show();
            var map_filter_obj = new MapFilter
            {
                center = center_obj,
                distanceRadius = Convert.ToInt32(distanceRadius)
            };
            request.AddJsonBody(
                new
                {
                    categoryId = categoryId,
                    offset = offset,
                    count = 100,
                    mapFilter = map_filter_obj,
                    filter = filter_obj,
                    coordinates = coords_obj,
                    sortType = sortType
                });

            var response = await client.ExecuteTaskAsync(request);
            //Toast.MakeText(context, "запрос пройден", ToastLength.Short).Show();
            return response.Content;
        }

        public async Task<string> ExpertsListMapZoom(string categoryId, string lat, string lng, string distanceRadius, bool hasReviews)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/catalog/experts", Method.POST);
            if (userMethods.UserExists())
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));

            var distanceRadiusDouble = Convert.ToDouble(distanceRadius, CultureInfo.InvariantCulture);

            var coords_obj = new Coordinates
            {
                latitude = lat.Replace(',', '.'),
                longitude = lng.Replace(',', '.')
            };
            var center_obj = new CenterForMapFilter
            {
                latitude = /*Convert.ToDouble(*/lat/*, CultureInfo.InvariantCulture)*/,
                longitude = /*Convert.ToDouble(*/lng/*, CultureInfo.InvariantCulture)*/
            };
            var map_filter_obj = new MapFilter
            {
                center = center_obj,
                distanceRadius = Convert.ToInt32(distanceRadius),
                hasReviews = hasReviews
            };
            request.AddJsonBody(
                new
                {
                    categoryId = categoryId,
                    offset = 0,
                    count = 10000,
                    mapFilter = map_filter_obj,
                    filter = map_filter_obj,
                    coordinates = coords_obj,
                });

            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> ExpertCount(string categoryId, string cityId, string distanceRadius, bool hasReviews, string lat, string lng)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/catalog/experts/count", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            if (userMethods.UserExists())
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            if (String.IsNullOrEmpty(distanceRadius))
                distanceRadius = "100000";
            else if (distanceRadius == "0.5")
                distanceRadius = "500";
            else
                distanceRadius = (Convert.ToInt32(distanceRadius) * 1000).ToString();
            var filter_obj = new ExpertFilter
            {
                cityId = cityId,
                distanceRadius = distanceRadius,
                hasReviews = hasReviews
            };
            var coords_obj = new Coordinates
            {
                latitude = lat.Replace(',', '.'),
                longitude = lng.Replace(',', '.')
            };
            var filterJson = JsonConvert.SerializeObject(filter_obj);
            request.AddJsonBody(new { categoryId = categoryId, filter = filter_obj, coordinates = coords_obj/*filterJson*/ });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> GetCities()
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/catalog/cities", Method.GET);
            if (userMethods.UserExists())
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> AddNewSpecialization(string categoryId, string name)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/catalog/subcategories/add", Method.POST);
            if (userMethods.UserExists())
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            request.AddJsonBody(new { categoryId = categoryId, name = name });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> SearchCategory(string query)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/catalog/categories/search", Method.GET);
            if (userMethods.UserExists())
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            request.AddQueryParameter("query", query);
            request.AddQueryParameter("offset", "0");
            request.AddQueryParameter("count", "100");
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
    }
}