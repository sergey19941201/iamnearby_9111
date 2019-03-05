using System.Collections.Generic;

namespace PCL.Models
{
    public class Subcategory
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool isOnModeration { get; set; }
        public bool hasSubcategory { get; set; }
        public List<Subcategory> subcategories { get; set; }
    }

    public class SearchCategory
    {
        public string id { get; set; }
        public string name { get; set; }
        public string iconUrl { get; set; }
        public string expertsCount { get; set; }
        public bool hasSubcategory { get; set; }
        public List<Subcategory> subcategories { get; set; }
    }
}