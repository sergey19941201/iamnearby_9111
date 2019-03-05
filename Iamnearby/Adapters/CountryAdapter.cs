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
    public class CountryAdapter : RecyclerView.Adapter
    {
        private static Activity _context;
        private List<City> countries_list;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("reg_data", FileCreationMode.Private);
        ISharedPreferences loc_pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
        Typeface tf;
        public CountryAdapter(List<City> countries_list, Activity context, Typeface tf)
        {
            this.countries_list = countries_list;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var sub_categoryViewHolder = (SubCategoryViewHolder)holder;
            sub_categoryViewHolder.specialization_nameTV.Text = countries_list[position].name;
            sub_categoryViewHolder.specialization_nameTV.SetTypeface(tf, TypefaceStyle.Normal);
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SubCategoryRow, parent, false);
            return new SubCategoryViewHolder(layout, OnItemClick);
        }
        public override int ItemCount
        {
            get { try { return countries_list.Count; } catch { return 0; } }
        }

        void OnItemClick(int position)
        {
            ISharedPreferencesEditor edit = pref.Edit();
            edit.PutString("country_id", countries_list[position].id);
            edit.PutString("country_name", countries_list[position].name);
            edit.PutString("region_id", "");
            edit.PutString("region_name", "");
            edit.Apply();
            ISharedPreferencesEditor edit_loc = loc_pref.Edit();
            edit_loc.PutString("auto_country_id", countries_list[position].id);
            edit_loc.PutString("auto_country_name", countries_list[position].name);
            edit_loc.PutString("auto_region_id", "");
            edit_loc.PutString("auto_region_name", "");
            edit_loc.PutString("auto_city_id", "");
            edit_loc.PutString("auto_city_name", "");

            edit_loc.Apply();
            var activity2 = new Intent(_context, typeof(LocationActivity));
            _context.StartActivity(activity2);
        }
    }
}