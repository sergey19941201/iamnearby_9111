using Iamnearby.Interfaces;
using System.Collections.Generic;

namespace Iamnearby.ExpandableRecyclerClasses
{
    public class UserProfileGroupData : IExpertProfileDataType
    {
        public int? idNo { set; get; }
        public string skill_name { set; get; }
        public string expert_id { set; get; }
        public string avatarUrl { get; set; }
        public string categoryId { set; get; }
        public string expert_name { set; get; }
        public string expert_phone { set; get; }
        public string expert_city { set; get; }
        public string expert_distance { set; get; }
        public string review_count { set; get; }
        public string rating { set; get; }
        public bool online { set; get; }
        public string aboutExpert { get; set; }
        public List<IExpertProfileDataType> items { set; get; }

        public int GetItemType()
        {
            return 0;
        }

        public UserProfileGroupData(int? idNo, string skill_name, string categoryId, string avatarUrl, string review_count, string rating, string expert_id, string aboutExpert, string expert_name = null, string expert_phone = null, string expert_city = null, string expert_distance = null, bool online = false)
        {
            this.idNo = idNo;
            this.skill_name = skill_name;
            this.expert_id = expert_id;
            this.avatarUrl = avatarUrl;
            this.categoryId = categoryId;
            this.expert_name = expert_name;
            this.expert_phone = expert_phone;
            this.expert_city = expert_city;
            this.expert_distance = expert_distance;
            this.online = online;
            this.review_count = review_count;
            this.rating = rating;
            this.aboutExpert = aboutExpert;
            this.items = new List<IExpertProfileDataType>();
        }
    }
}