using System.Collections.Generic;

namespace PCL.Models
{
    public class ServiceCategory
    {
        public string name { get; set; }
        public double rating { get; set; }
        public string reviews { get; set; }
        public string description { get; set; }
        public List<object> photos { get; set; }
        public string categoryId { get; set; }
        public List<ServiceData> services { get; set; }
    }

    public class ExpertProfile
    {
        public string id { get; set; }
        public string fullName { get; set; }
        public string phone { get; set; }
        public string avatarUrl { get; set; }
        public bool online { get; set; }
        public bool notifications { get; set; }
        public City city { get; set; }
        public string address { get; set; }
        public string distance { get; set; }
        public string reviews { get; set; }
        public string rating { get; set; }
        public Coordinates coordinates { get; set; }
        public string aboutExpert { get; set; }
        public List<ServiceCategory> serviceCategories { get; set; }
        public List<MainCategory> mainCategories { get; set; }
        public NotifyAlerts notify_alerts { get; set; }
    }
}