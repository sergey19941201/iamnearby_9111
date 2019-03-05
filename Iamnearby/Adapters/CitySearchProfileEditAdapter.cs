using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Iamnearby.ViewHolders;
using PCL.Models;

namespace Iamnearby.Adapters
{
    [Activity(Label = "CitySearchProfileEditAdapter")]
    public class CitySearchProfileEditAdapter : RecyclerView.Adapter
    {
        private static Activity _context;
        private List<CitySearch> city_list;
        ISharedPreferences city_coord_for_edit_prefs = Application.Context.GetSharedPreferences("city_coord_for_edit_prefs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_city_coord_for_edit;
        Action showFrag;
        Typeface tf;

        public CitySearchProfileEditAdapter(List<CitySearch> city_list, Action showFrag, Activity context, Typeface tf)
        {
            this.showFrag = showFrag;
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
            edit_city_coord_for_edit = city_coord_for_edit_prefs.Edit();
            edit_city_coord_for_edit.PutString("city_id", city_list[position].id);
            edit_city_coord_for_edit.PutString("city_name", city_list[position].city);
            edit_city_coord_for_edit.Apply();
            showFrag();
        }
    }
}