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
    public class CityForRegAdapter : RecyclerView.Adapter
    {
        private static Activity _context;
        private List<CitySearch> city_list;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("reg_data", FileCreationMode.Private);
        ISharedPreferences loc_pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
        Typeface tf;

        public CityForRegAdapter(List<CitySearch> city_list, Activity context, Typeface tf)
        {
            this.city_list = city_list;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var sub_categoryViewHolder = (SubCategoryViewHolder)holder;
            sub_categoryViewHolder.specialization_nameTV.Text = city_list[position].city;
            sub_categoryViewHolder.specialization_nameTV.SetTypeface(tf, TypefaceStyle.Normal);
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SubCategoryRow, parent, false);
            return new SubCategoryViewHolder(layout, OnItemClick);
        }
        public override int ItemCount
        {
            get { try { return city_list.Count; } catch { return 0; } }
        }

        void OnItemClick(int position)
        {
            ISharedPreferencesEditor edit_loc = loc_pref.Edit();
            edit_loc.PutString("auto_city_id", city_list[position].id);
            edit_loc.PutString("auto_city_name", city_list[position].city);
            edit_loc.Apply();
            var activity2 = new Intent(_context, typeof(SpecialistsCategoryActivity));
            _context.StartActivity(activity2);
        }
    }
}