using System.Collections.Generic;

namespace PCL.Models
{
    public class UpperSpecializations
    {
        public string id { get; set; }
        public string name { get; set; }
        public string iconUrl { get; set; }
        public string expertsCount { get; set; }
        public bool hasSubcategory { get; set; }
        public bool isOnModeration { get; set; }
    }
    public class UpperSpecializationsRootObject
    {
        public List<UpperSpecializations> categories { get; set; }
        public NotifyAlerts notify_alerts { get; set; }
    }
}