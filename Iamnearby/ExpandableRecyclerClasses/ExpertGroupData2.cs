using System.Collections.Generic;
using Iamnearby.Interfaces;

namespace Iamnearby.ExpandableRecyclerClasses
{

    public class ExpertGroupData2 : IExpertProfileDataType
    {
        public string subcategory { get; set; }
        public List<IExpertProfileDataType> items { set; get; }
        public List<IExpertProfileDataType> items_child { set; get; }

        public int GetItemType()
        {
            return 2;
        }

        public ExpertGroupData2(string subcategory)
        {
            this.subcategory = subcategory;
            this.items = new List<IExpertProfileDataType>();
            this.items_child = new List<IExpertProfileDataType>();
        }
    }
}