using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RestSharp;
using RestSharp.Authenticators;

namespace Iamnearby.Methods
{
    public class DialogMethods
    {
        ISharedPreferences device_info_prefs = Application.Context.GetSharedPreferences("device_info", FileCreationMode.Private);
        RestClient client = new RestClient("https://api.iamnearby.net");

        public async Task<string> CreateChat(string authorization, string expertId, string categoryId = null)
        {
            var request = new RestRequest("v1/chat/create", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            if (!String.IsNullOrEmpty(categoryId))
                request.AddJsonBody(new { expertId = expertId, categoryId = categoryId });
            else
                request.AddJsonBody(new { expertId = expertId });
            var response = await client.ExecuteTaskAsync(request);

            return response.Content;
        }

        public async Task<string> ChatHistory(string authorization, string chatId, bool mark_read = true)
        {
            var request = new RestRequest("/v1/chat/history", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddParameter("chatId", chatId);
            request.AddParameter("count", 10);
            request.AddParameter("mark_read", mark_read);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> ChatHistoryOffset(string authorization, string chatId, int offset, bool mark_read = true)
        {
            var request = new RestRequest("/v1/chat/history", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddParameter("chatId", chatId);
            request.AddParameter("count", 10);
            request.AddParameter("offset", offset);
            request.AddParameter("mark_read", mark_read);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> ChatHistoryUpdate(string authorization, string chatId, string lastId, bool mark_read = true)
        {
            var request = new RestRequest("/v1/chat/history", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddParameter("chatId", chatId);
            request.AddParameter("lastId", lastId);
            request.AddParameter("mark_read", mark_read);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> SendMessage(string authorization, string chatId, string message, string file = null, string filename = null)//byte[] file = null)
        {
            var request = new RestRequest("v1/chat/send", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            if (file == null)
                request.AddJsonBody(new { chatId = chatId, message = message });
            else
                request.AddJsonBody(new { chatId = chatId, message = message, file = file, filename = filename });
            var response = await client.ExecuteTaskAsync(request);

            return response.Content;
        }

        public async Task<string> ChatList(string authorization)
        {
            var request = new RestRequest("/v1/chats/", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
        public async Task<string> SearchChat(string authorization, string query)
        {
            var request = new RestRequest("/v1/chats/", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddParameter("query", query);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<string> ClearHistory(string authorization, string chatId)
        {
            var request = new RestRequest("/v1/chat/clearhistory", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddParameter("chatId", chatId);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
        public async Task<string> CloseChat(string authorization, string chatId, bool to_bl)
        {
            var request = new RestRequest("/v1/chat/close", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddParameter("chatId", chatId);
            request.AddParameter("to_bl", to_bl);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
        public async Task<string> ToggleNotifications(string authorization, string chatId, int notifications)
        {
            var request = new RestRequest("/v1/chat/notifications", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            //request.AddJsonBody(new { chatId = chatId, notifications = notifications });
            request.AddParameter("chatId", chatId);
            request.AddParameter("notifications", notifications);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
        public async Task<string> CheckMsgRead(string authorization, string chatId)
        {
            var request = new RestRequest("/v1/chat/checkreadmsg", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddParameter("chatId", chatId);
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
        public async Task<string> DeleteMsg(string authorization, string chatId, string msgId)
        {
            var client = new RestClient("https://api.iamnearby.net");
            var request = new RestRequest("/v1/chat/msgdelete", Method.DELETE);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Device", device_info_prefs.GetString("device_json", String.Empty));
            request.AddHeader("deviceToken", device_info_prefs.GetString("firebase_token", String.Empty));
            client.Authenticator = new HttpBasicAuthenticator(username: "", password: authorization);
            request.AddJsonBody(new
            {
                chatId = chatId,
                msgId = msgId
            });
            var response = await client.ExecuteTaskAsync(request);
            return response.Content;
        }
    }
}
