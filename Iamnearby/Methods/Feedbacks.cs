using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using RestSharp;
using RestSharp.Authenticators;

namespace Iamnearby.Methods
{
    public class Feedbacks
    {
        ISharedPreferences device_info_prefs = Application.Context.GetSharedPreferences("device_info", FileCreationMode.Private);
        RestClient client = new RestClient("https://api.iamnearby.net");
        public async Task<string> ReviewList(string id, /*string categoryId,*/ string authorization = null)
        {
            var request = new RestRequest("/v1/reviews", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            if (authorization != null)
                client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddParameter("id", id);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
        public async Task<string> ByMeReviewList(string authorization)
        {
            var request = new RestRequest("/v1/reviews", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
        public async Task<string> SendNewReview(string authorization, string expertId, string text, string serviceCategoryId, string rating)
        {
            var request = new RestRequest("/v1/reviews/new", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddJsonBody(new { /*serviceCategoryId = serviceCategoryId,*/ expertId = expertId, text = text, rating = rating });
            var response = await client.ExecuteTaskAsync(request);

            return response.Content;
        }
        public async Task<string> ReplyReview(string authorization, string reviewId, string text)
        {
            var request = new RestRequest("/v1/reviews/reply", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddJsonBody(new { reviewId = reviewId, text = text });
            var response = await client.ExecuteTaskAsync(request);

            return response.Content;
        }
    }
}