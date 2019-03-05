using Android.App;
using Android.Content;
using PCL.Database;
using PCL.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Iamnearby.Methods
{
    public class ProfileAndExpertMethods
    {
        static UserProfile deserialized_user_data;
        static UserMethods userMethods = new UserMethods();
        static ISharedPreferences device_info_prefs = Application.Context.GetSharedPreferences("device_info", FileCreationMode.Private);
        //public async Task<string> ExpertProfile(string id, string latitude, string longitude)
        //{
        //    var client = new RestClient("https://api.iamnearby.net");
        //    var request = new RestRequest("/v1/expert", Method.GET);
        //    if (userMethods.UserExists())
        //        client.Authenticator = new HttpBasicAuthenticator(username: "", password: userMethods.GetUsersAuthToken());
        //    request.AddHeader("Content-Type", "application/json");
        //    request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
        //    request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
        //    request.AddParameter("id", id);
        //    latitude = latitude.Replace(',', '.');
        //    longitude = longitude.Replace(',', '.');
        //    request.AddQueryParameter("latitude", latitude);
        //    request.AddQueryParameter("longitude", longitude);
        //    var response = await client.ExecuteTaskAsync(request);
        //    return response.Content;
        //}

        public async Task<string> UserProfileData(string authorization)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/expert", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> EditMyProfileImage(string authorization, string image)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/expert/avatar", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddJsonBody(new { image = image });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> EditMyProfileInfo(string authorization, string phone, string email, string surname, string name, string middlename, string cityId, string lat, string lng)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/expert/info", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);

            var coords_obj = new Coordinates
            {
                latitude = lat.Replace(',', '.'),
                longitude = lng.Replace(',', '.')
            };

            if (email != userMethods.GetUsersEmail())
                request.AddJsonBody(new { phone = phone, email = email, surname = surname, name = name, middlename = middlename, cityId = cityId, coordinates = coords_obj });
            else
                request.AddJsonBody(new { phone = phone, /*email = email,*/ surname = surname, name = name, middlename = middlename, cityId = cityId, coordinates = coords_obj });
            var response = await client.ExecuteTaskAsync(request);
            if (response.Content.Contains("с таким email уже"))
            {
                return "с таким email уже";
            }
            else
            {
                string token = userMethods.GetUsersAuthToken();
                userMethods.ClearTable();
                userMethods.InsertUser(token, email);
            }
            return response.Content;
        }
        public async Task<string> EditMyOnline(string authorization, bool online)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/expert/online", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddJsonBody(new { online = online });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public static async Task<string> EditSpecializationStatic(string authorization, string categoryId/*, string description = null, List<ServiceData> servList = null*/)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/expert/categories", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddJsonBody(new { categoryId = categoryId/*, description = description, services = servList*/ });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> EditSpecializationServices(string authorization, string categoryId, int upperCatIndex, List<ServiceData> servList)
        {
            string description = null;
            List<string> photoList = new List<string>();
            deserialized_user_data = null;
            string data = userMethods.GetUserData();
            try
            {
                deserialized_user_data = JsonConvert.DeserializeObject<UserProfile>(data);
            }
            catch
            {
                deserialized_user_data = null;
            }
            if (deserialized_user_data != null)
            {
                foreach (var categ in deserialized_user_data.mainCategories[upperCatIndex].subcategories)
                {
                    if (categ.categoryId == categoryId)
                    {
                        description = categ.description;
                        if (categ.services != null)
                            foreach (var service_category in categ.services)
                            {
                                servList.Add(service_category);
                            }
                        if (categ.photos != null)
                            foreach (var photo in categ.photos)
                            {
                                var current_img = JsonConvert.DeserializeObject<CategoryImage>(photo.ToString());
                                photoList.Add(current_img.imageUrl);
                            }
                    }
                }
            }

            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/expert/categories", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            // request.AddJsonBody(new { categoryId = categoryId, description = description, services = servList });
            request.AddJsonBody(new { categoryId = categoryId, description = description, services = servList, photos = photoList });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> EditSpecializationDescription(string authorization, string categoryId, int upperCatIndex, string description)
        {
            List<string> photoList = new List<string>();
            List<ServiceData> servList = new List<ServiceData>();
            deserialized_user_data = null;
            var data = userMethods.GetUserData();
            try
            {
                deserialized_user_data = JsonConvert.DeserializeObject<UserProfile>(data);
            }
            catch
            {
                deserialized_user_data = null;
            }
            if (deserialized_user_data != null)
            {
                foreach (var categ in deserialized_user_data.mainCategories[upperCatIndex].subcategories)
                {
                    if (categ.categoryId == categoryId)
                    {
                        if (description != null)
                        {
                            if (categ.services != null)
                                foreach (var service_category in categ.services)
                                {
                                    servList.Add(service_category);
                                }
                            if (categ.photos != null)
                                foreach (var photo in categ.photos)
                                {
                                    var current_img = JsonConvert.DeserializeObject<CategoryImage>(photo.ToString());
                                    photoList.Add(current_img.imageUrl);
                                }
                        }
                    }
                }
            }

            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/expert/categories", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddJsonBody(new { categoryId = categoryId, description = description, services = servList, photos = photoList });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> AddSpecializationPhotos(string authorization, string categoryId, int upperCatIndex, List<string> photoList)
        {
            List<ServiceData> servList = new List<ServiceData>();
            string description = null;
            deserialized_user_data = null;
            string data = userMethods.GetUserData();
            try
            {
                deserialized_user_data = JsonConvert.DeserializeObject<UserProfile>(data);
            }
            catch
            {
                deserialized_user_data = null;
            }
            if (deserialized_user_data != null)
            {
                foreach (var categ in deserialized_user_data.mainCategories[upperCatIndex].subcategories)
                {
                    if (categ.categoryId == categoryId)
                    {
                        if (categ.description != null)
                        {
                            description = categ.description;
                        }
                        if (categ.services != null)
                            foreach (var service_category in categ.services)
                            {
                                servList.Add(service_category);
                            }
                        if (categ.photos != null)
                            foreach (var photo in categ.photos)
                            {
                                var current_img = JsonConvert.DeserializeObject<CategoryImage>(photo.ToString());
                                photoList.Add(current_img.imageUrl);
                            }
                    }
                }
            }

            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/expert/categories", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddJsonBody(new { categoryId = categoryId, description = description, services = servList, photos = photoList });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> DeleteSpecialization(string authorization, string categoryId)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/expert/categories", Method.DELETE);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddJsonBody(new { categoryId = categoryId });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> AboutExpert(string authorization, string info)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/expert/about_expert", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddJsonBody(new { info = info });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> DeleteService(string authorization, string serviceId)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/expert/services", Method.DELETE);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddJsonBody(new { serviceId = serviceId });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> EditService(string authorization, string serviceId, string service_name, string service_price)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/expert/services", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddJsonBody(new { serviceId = serviceId, name = service_name, price = service_price });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> DeletePhoto(string authorization, string photoUrl, string categoryId = null)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/deletePhoto", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddParameter("photoUrl", photoUrl);
            if (!String.IsNullOrEmpty(categoryId))
                request.AddParameter("categoryId", categoryId);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
    }
}