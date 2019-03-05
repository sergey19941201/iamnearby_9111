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
    public class RegionsSearchAdapter : RecyclerView.Adapter
    {
        private static Activity _context;
        private List<RegionSearch> region_list;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("reg_data", FileCreationMode.Private);
        ISharedPreferences loc_pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
        Typeface tf;
        public RegionsSearchAdapter(List<RegionSearch> region_list, Activity context, Typeface tf)
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
            ISharedPreferencesEditor edit = pref.Edit();
            ISharedPreferencesEditor edit_loc = loc_pref.Edit();
            edit.PutString("region_id", region_list[position].id);
            edit.PutString("region_name", region_list[position].region);
            edit.Apply();
            edit_loc.PutString("auto_region_id", region_list[position].id);
            edit_loc.PutString("auto_region_name", region_list[position].region);
            edit_loc.Apply();

            var activity2 = new Intent(_context, typeof(CityInRegionActivity));

            _context.StartActivity(activity2);
        }
    }
}