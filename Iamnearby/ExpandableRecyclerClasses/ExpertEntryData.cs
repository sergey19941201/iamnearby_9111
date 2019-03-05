using System.Collections.Generic;
using Iamnearby.Interfaces;
using PCL.Models;

namespace Iamnearby.ExpandableRecyclerClasses
{
    public class ExpertEntryData : IExpertProfileDataType
    {
        public int entryId { set; get; }
        public string entryName { set; get; }
        public string categoryId { set; get; }
        //public int upperCatIndex { set; get; }
        public List<object> photos { set; get; }
        public double rating { set; get; }
        public string reviews { set; get; }
        public string description { set; get; }
        public List<ServiceData> services { set; get; }

        public int GetItemType()
        {
            return 1;
        }

        public ExpertEntryData(string description, List<ServiceData> services, List<object> photos)
        {
            this.services = services;
            this.photos = photos;
            this.description = description;
        }
    }
}