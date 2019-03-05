using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Iamnearby.Activities;
using Iamnearby.ViewHolders;
using PCL.Models;

namespace Iamnearby.Adapters
{
    public class RegionsSearchProfileCityEditAdapter : RecyclerView.Adapter
    {
        private static Activity _context;
        private List<RegionSearch> region_list;
        ISharedPreferences city_coord_for_edit_prefs = Application.Context.GetSharedPreferences("city_coord_for_edit_prefs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_city_coord_for_edit;
        Typeface tf;
        public RegionsSearchProfileCityEditAdapter(List<RegionSearch> region_list, Activity context, Typeface tf)
        {
            this.region_list = region_list;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var dropDownSubcategsViewHolder = (DropDownSubcategsViewHolder)holder;
            dropDownSubcategsViewHolder.sub_categTV.SetTypeface(tf, TypefaceStyle.Normal);
            dropDownSubcategsViewHolder.sub_categTV.Text = region_list[position].region;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.DropDownSubcategsRow, parent, false);
            return new DropDownSubcategsViewHolder(layout, OnItemClick);
        }
        public override int ItemCount
        {
            get { try { return region_list.Count; } catch { return 0; } }
        }

        void OnItemClick(int position)
        {
            edit_city_coord_for_edit = city_coord_for_edit_prefs.Edit();
            edit_city_coord_for_edit.PutString("region_id", region_list[position].id);
            edit_city_coord_for_edit.PutString("region_name", region_list[position].region);
            edit_city_coord_for_edit.Apply();

            var activity2 = new Intent(_context, typeof(CityInRegionProfileEditActivity));

            _context.StartActivity(activity2);
        }
    }
}