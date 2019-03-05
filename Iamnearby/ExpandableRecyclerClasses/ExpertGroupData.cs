using System.Collections.Generic;
using Iamnearby.Interfaces;

namespace Iamnearby.ExpandableRecyclerClasses
{
    public class ExpertGroupData : IExpertProfileDataType
    {
        public List<IExpertProfileDataType> items { set; get; }
        public List<IExpertProfileDataType> items_child { set; get; }

        public string expertId { set; get; }
        public string expertName { set; get; }
        public string expert_phone { set; get; }
        public string aboutExpertInfo { set; get; }
        public string rating_value { set; get; }
        public string distance { set; get; }
        public string city { set; get; }
        public bool online { get; set; }
        public string profile_image { get; set; }
        public string upper_categ { get; set; }

        public int GetItemType()
        {
            return 0;
        }

        public ExpertGroupData(string expertId,
                                string expertName,
                                string expert_phone,
                                string aboutExpertInfo,
                                string rating_value,
                                string distance,
                                string city,
                                bool online,
                                string profile_image,
                                string upper_categ)
        {
            this.expertId = expertId;
            this.expertName = expertName;
            this.expert_phone = expert_phone;
            this.aboutExpertInfo = aboutExpertInfo;
            this.rating_value = rating_value;
            this.distance = distance;
            this.city = city;
            this.online = online;
            this.profile_image = profile_image;
            this.upper_categ = upper_categ;
            this.items = new List<IExpertProfileDataType>();
            this.items_child = new List<IExpertProfileDataType>();
        }
    }
}