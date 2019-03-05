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
    public class CitySearchAdapter : RecyclerView.Adapter
    {
        private static Activity _context;
        private List<CitySearch> city_list;
        ISharedPreferences loc_pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
        Typeface tf;
        public CitySearchAdapter(List<CitySearch> city_list, Activity context, Typeface tf)
        {
            this.city_list = city_list;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var dropDownSubcategsViewHolder = (DropDownSubcategsViewHolder)holder;
            dropDownSubcategsViewHolder.sub_categTV.Text = city_list[position].city;
            dropDownSubcategsViewHolder.sub_categTV.SetTypeface(tf, TypefaceStyle.Normal);
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.DropDownSubcategsRow, parent, false);
            return new DropDownSubcategsViewHolder(layout, OnItemClick);
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