using System.Collections.Generic;

namespace PCL.Models
{
    public class SubCategory
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool hasSubcategory { get; set; }
        public bool isOnModeration { get; set; }
    }
    public class SubCategoryRootObject
    {
        public List<SubCategory> subcategories { get; set; }
        public NotifyAlerts notify_alerts { get; set; }
    }
}