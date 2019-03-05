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
    public class RegionForProfileCityEditAdapter : RecyclerView.Adapter
    {
        private static Activity _context;
        private List<RegionSearch> regions_list;
        ISharedPreferences city_coord_for_edit_prefs = Application.Context.GetSharedPreferences("city_coord_for_edit_prefs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_city_coord_for_edit;
        Typeface tf;
        public RegionForProfileCityEditAdapter(List<RegionSearch> regions_list, Activity context, Typeface tf)
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
            edit_city_coord_for_edit = city_coord_for_edit_prefs.Edit();
            edit_city_coord_for_edit.PutString("region_id", regions_list[position].id);
            edit_city_coord_for_edit.PutString("region_name", regions_list[position].region);
            edit_city_coord_for_edit.Apply();

            var activity2 = new Intent(_context, typeof(CityInRegionProfileEditActivity));
            _context.StartActivity(activity2);
        }
    }
}