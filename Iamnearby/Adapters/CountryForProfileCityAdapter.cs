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
    public class CountryForProfileCityAdapter : RecyclerView.Adapter
    {
        private static Activity _context;
        private List<City> countries_list;
        ISharedPreferences city_coord_for_edit_prefs = Application.Context.GetSharedPreferences("city_coord_for_edit_prefs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_city_coord_for_edit;
        Typeface tf;

        public CountryForProfileCityAdapter(List<City> countries_list, Activity context, Typeface tf)
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
            edit_city_coord_for_edit = city_coord_for_edit_prefs.Edit();
            edit_city_coord_for_edit.PutString("country_id", countries_list[position].id);
            edit_city_coord_for_edit.PutString("country_name", countries_list[position].name);
            edit_city_coord_for_edit.Apply();
            var activity2 = new Intent(_context, typeof(RegionForProfileCityActivity));
            _context.StartActivity(activity2);
        }
    }
}