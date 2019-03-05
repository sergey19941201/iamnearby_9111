using System.Collections.Generic;

namespace PCL.Models
{
    public class Timestamp
    {
        public string date { get; set; }
        public int timezone_type { get; set; }
        public string timezone { get; set; }
    }

    public class ChatHistoryModel
    {
        public string read { get; set; }
        public string in_out { get; set; }
        public string message { get; set; }
        public Timestamp timestamp { get; set; }
        public string file { get; set; }
        public string file_base64 { get; set; }
        public string file_small { get; set; }
        public string file_type { get; set; }
        public string msg_id { get; set; }
    }
    public class ChatHistoryRootObject
    {
        public bool dialogNotify { get; set; }
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public string phone { get; set; }
        public bool is_expert { get; set; }
        public bool online { get; set; }
        public string blacklist { get; set; }
        public List<ChatHistoryModel> messages { get; set; }
        public string authToken { get; set; }
    }
}