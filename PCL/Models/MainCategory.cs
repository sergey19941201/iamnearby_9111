using System.Collections.Generic;

namespace PCL.Models
{
    public class MainCategory
    {
        public string spec_id { get; set; }
        public string name { get; set; }
        public List<ServiceCategory> subcategories { get; set; }
    }
}
