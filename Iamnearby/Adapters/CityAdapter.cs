using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Iamnearby.Activities;
using PCL.Models;
using Iamnearby.ViewHolders;
using System;
using System.Collections.Generic;

namespace Iamnearby.Adapters
{
    public class CityAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private static Activity _context;
        private List<City> city_list;
        ISharedPreferences expert_data = Application.Context.GetSharedPreferences("experts", FileCreationMode.Private);
        ISharedPreferencesEditor edit_expert;
        FilterActivity filterActivity = new FilterActivity();
        Typeface tf;

        public CityAdapter(List<City> city_list, Activity context, Typeface tf)
        {
            this.city_list = city_list;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var dropDownSubcategsViewHolder = (DropDownSubcategsViewHolder)holder;
            dropDownSubcategsViewHolder.sub_categTV.Text = city_list[position].name;
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
            edit_expert = expert_data.Edit();
            edit_expert.PutString("expert_city_id", city_list[position].id);
            edit_expert.PutString("expert_city_name", city_list[position].name);
            edit_expert.Apply();
            filterActivity.city_click();
        }
    }
}