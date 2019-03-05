using Iamnearby.Interfaces;
using PCL.Models;
using System.Collections.Generic;

namespace Iamnearby.ExpandableRecyclerClasses
{
    public class UserProfileEntryData : IExpertProfileDataType
    {
        public int entryId { set; get; }
        public string entryName { set; get; }
        public string categoryId { set; get; }
        public int upperCatIndex { set; get; }
        public List<object> photos { set; get; }
        public double rating { set; get; }
        public string reviews { set; get; }
        public string description { set; get; }
        public List<ServiceData> services { set; get; }

        public int GetItemType()
        {
            return 1;
        }

        public UserProfileEntryData(int entryId, string entryName, string categoryId, int upperCatIndex, List<ServiceData> services, List<object> photos/*, double rating, string reviews*/, string description)
        {
            this.entryId = entryId;
            this.entryName = entryName;
            this.categoryId = categoryId;
            this.upperCatIndex = upperCatIndex;
            this.services = services;
            this.photos = photos;
            this.description = description;
        }
    }
}