using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Iamnearby.Activities;
using PCL.Models;
using Iamnearby.ViewHolders;
using System.Collections.Generic;

namespace Iamnearby.Adapters
{
    public class RegionAdapter : RecyclerView.Adapter
    {
        private static Activity _context;
        private List<RegionSearch> regions_list;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("reg_data", FileCreationMode.Private);
        ISharedPreferences loc_pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
        Typeface tf;
        public RegionAdapter(List<RegionSearch> regions_list, Activity context, Typeface tf)
        {
            this.regions_list = regions_list;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var sub_categoryViewHolder = (SubCategoryViewHolder)holder;
            sub_categoryViewHolder.specialization_nameTV.SetTypeface(tf, TypefaceStyle.Normal);
            sub_categoryViewHolder.specialization_nameTV.Text = regions_list[position].region;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SubCategoryRow, parent, false);
            return new SubCategoryViewHolder(layout, OnItemClick);
        }
        public override int ItemCount
        {
            get { try { return regions_list.Count; } catch { return 0; } }
        }

        void OnItemClick(int position)
        {
            ISharedPreferencesEditor edit = pref.Edit();
            ISharedPreferencesEditor edit_loc = loc_pref.Edit();
            edit.PutString("region_id", regions_list[position].id);
            edit.PutString("region_name", regions_list[position].region);
            edit.Apply();
            edit_loc.PutString("auto_region_id", regions_list[position].id);
            edit_loc.PutString("auto_region_name", regions_list[position].region);
            edit_loc.Apply();


            var activity2 = new Intent(_context, typeof(CityInRegionActivity));
            _context.StartActivity(activity2);
        }
    }
}