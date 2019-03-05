using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Newtonsoft.Json;

namespace PCL.HttpMethods
{
    public class RegistrationMethods
    {
        ISharedPreferences device_info_prefs = Application.Context.GetSharedPreferences("device_info", FileCreationMode.Private);
        ISharedPreferences loc_pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
        public async Task<string> Register(string phone, string email, string surname, string name, string middlename, string cityId, List<string> categories = null)
        {
            using (HttpClient client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorization);
                var myContent = JsonConvert.SerializeObject(new { phone = phone, email = email, surname = surname, name = name, middlename = middlename, categories = categories, cityId = cityId });
                var content = new StringContent(myContent.ToString(), Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.Add("Device", device_info_prefs.GetString("device_json", String.Empty));
                content.Headers.Add("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
                var res = await client.PostAsync("https://api.iamnearby.net/v1/register", content);
                var content_response = res.Content.ReadAsStringAsync();
                var response_result = content_response.Result;
                return response_result;
            }
        }
    }
}
