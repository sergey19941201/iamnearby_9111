using Android.App;
using Android.Content;
using PCL.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Iamnearby.Methods
{
    public class RegistrationMethods
    {
        ISharedPreferences device_info_prefs = Application.Context.GetSharedPreferences("device_info", FileCreationMode.Private);
        ISharedPreferences loc_pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
        RestClient client = new RestClient("https://api.iamnearby.net");
        //public async Task<string> Register(string phone, string email, string surname, string name, string middlename, string cityId, List<string> categories = null)
        //{
        //    var request = new RestRequest("/v1/register", Method.POST);
        //    request.AddHeader("Content-Type", "application/json");
        //    request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
        //    request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));

        //    //string[] categories1 = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };


        //    request.AddJsonBody(new { phone = phone, email = email, surname = surname, name = name, middlename = middlename, categories = categories, cityId = cityId });
        //    var response = await client.ExecuteTaskAsync(request);

        //    return response.Content;
        //}
        public async Task<string> RegisterSimple(string name)
        {
            var request = new RestRequest("/v1/register/simple", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            var device_json = JsonConvert.DeserializeObject<Device>(device_info_prefs.GetString("device_json", String.Empty));
            request.AddJsonBody(new { name = name, email = device_json.deviceToken + "@fake.iamnearby.net", cityId = loc_pref.GetString("auto_city_id", String.Empty) });
            var response = await client.ExecuteTaskAsync(request);

            return response.Content;
        }
        public async Task<string> Resend(string email)
        {
            var request = new RestRequest("/v1/register/resend", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            request.AddJsonBody(new { email = email });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
        public async Task<string> RegisterActivate(string email)
        {
            var request = new RestRequest("/v1/register/activate", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            request.AddJsonBody(new { email = email });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
    }
}