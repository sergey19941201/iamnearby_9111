using System.Collections.Generic;

namespace PCL.Models
{
    public class ChatListModel
    {
        public string chatId { get; set; }
        public string timestamp { get; set; }
        public string shorttext { get; set; }
        public ChatListExpert expert { get; set; }
        public string unread { get; set; }
        public string blacklist { get; set; }
        public string dialogNotify { get; set; }
    }
    public class ChatListExpert
    {
        public string id { get; set; }
        public string photo { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public int category { get; set; }
        public bool is_expert { get; set; }
        public bool online { get; set; }
        public string categoryName { get; set; }
    }
    public class RootObjectChatList
    {
        public List<ChatListModel> chats { get; set; }
        public NotifyAlerts notify_alerts { get; set; }
    }
}