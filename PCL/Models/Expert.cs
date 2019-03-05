using System.Collections.Generic;

namespace PCL.Models
{
    public class Coordinates
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
    }

    public class Expert
    {
        public string id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string avatarUrl { get; set; }
        public string rating { get; set; }
        public string distance { get; set; }
        public bool online { get; set; }
        public Coordinates coordinates { get; set; }
    }
    public class RootObjectExpert
    {
        public List<Expert> experts { get; set; }
        public NotifyAlerts notify_alerts { get; set; }
    }
}