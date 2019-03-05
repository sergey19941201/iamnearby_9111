using Android.App;
using Android.Content;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Threading.Tasks;

namespace Iamnearby.Methods
{
    public class AuthorizationMethods
    {
        ISharedPreferences device_info_prefs = Application.Context.GetSharedPreferences("device_info", FileCreationMode.Private);
        RestClient client = new RestClient("https://api.iamnearby.net");
        public async Task<string> Authorize(string email)
        {
            var request = new RestRequest("/v1/auth", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            request.AddJsonBody(new { email = email });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
        public async Task<string> AuthActivate(string email, bool is_cycle = false)
        {
            var request = new RestRequest("/v1/auth/activate", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("iscycle", is_cycle.ToString());
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            request.AddJsonBody(new { email = email });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
        public async Task<string> LogOut(string authorization)
        {
            var request = new RestRequest("/v1/auth/deactivate", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
    }
}