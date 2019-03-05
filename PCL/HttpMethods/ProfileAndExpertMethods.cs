using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using PCL.Database;
using PCL.Models;

namespace PCL.HttpMethods
{
    public class ProfileAndExpertMethods
    {
        static UserProfile deserialized_user_data;
        static UserMethods userMethods = new UserMethods();
        static ISharedPreferences device_info_prefs = Application.Context.GetSharedPreferences("device_info", FileCreationMode.Private);
        public async Task<string> UserProfileData(string authorization)
        {
            using (HttpClient client = new HttpClient())
            {
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("" + ":" + authorization);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                var device_info = device_info_prefs.GetString("device_json", String.Empty);
                var device_token = device_info_prefs.GetString("firebase_token", String.Empty);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Add("content-type", "application/json");
                client.DefaultRequestHeaders.Add("device", device_info);
                client.DefaultRequestHeaders.Add("deviceToken", device_token);

                string response = "";
                try
                {
                    response = await client.GetStringAsync("https://api.iamnearby.net/v1/expert");
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("401"))
                        return "401";
                }
                return response;
            }
        }
        public async Task<string> ExpertProfile(string id, string latitude, string longitude)
        {
            using (HttpClient client = new HttpClient())
            {
                if (userMethods.UserExists())
                {
                    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("" + ":" + userMethods.GetUsersAuthToken());
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                }
                var device_info = device_info_prefs.GetString("device_json", String.Empty);
                var device_token = device_info_prefs.GetString("firebase_token", String.Empty);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("device", device_info);
                client.DefaultRequestHeaders.Add("deviceToken", device_token);

                string response = "";
                try
                {
                    latitude = latitude.Replace(',', '.');
                    longitude = longitude.Replace(',', '.');
                    var url = "https://api.iamnearby.net/v1/expert?" + "id=" + id + "&latitude=" + latitude + "&longitude=" + longitude;
                    response = await client.GetStringAsync(url);
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("401"))
                        return "401";
                }
                return response;
            }
        }
    }
}
