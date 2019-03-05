using Android.App;
using Android.Content;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Threading.Tasks;

namespace Iamnearby.Methods
{
    public class SettingsMethods
    {
        ISharedPreferences device_info_prefs = Application.Context.GetSharedPreferences("device_info", FileCreationMode.Private);
        RestClient client = new RestClient("https://api.iamnearby.net");
        public async Task<string> Notifications(string authorization, bool notifications)
        {
            var request = new RestRequest("/v1/settings", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddJsonBody(new { notifications = notifications });
            var response = await client.ExecuteTaskAsync(request);

            return response.Content;
        }
        public async Task<string> AppFeedback(string authorization, string rating, string text)
        {
            var request = new RestRequest("/v1/feedback", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddJsonBody(new { rating = rating, text = text });
            var response = await client.ExecuteTaskAsync(request);

            return response.Content;
        }
    }
}