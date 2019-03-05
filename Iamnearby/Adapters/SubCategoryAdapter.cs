using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Iamnearby.Activities;
using Iamnearby.ViewHolders;
using PCL.Database;
using PCL.Models;

namespace Iamnearby.Adapters
{
    public class SubCategoryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private static Activity _context;
        private List<SubCategory> sub_category_list;
        public static List<SubCategory> my_specializations_static = new List<SubCategory>();
        public static List<int> checked_positions = new List<int>();
        ISharedPreferences pref = Application.Context.GetSharedPreferences("categories_data", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
        UserMethods userMethods = new UserMethods();
        Typeface tf;

        public SubCategoryAdapter(List<SubCategory> sub_category_list, Activity context, Typeface tf)
        {
            this.sub_category_list = sub_category_list;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var sub_categoryViewHolder = (CheckSubCategoryViewHolder)holder;
            sub_categoryViewHolder.specialization_nameTV.SetTypeface(tf, TypefaceStyle.Normal);
            sub_categoryViewHolder.specialization_nameTV.Text = sub_category_list[position].name;
            if (sub_category_list[position].isOnModeration)
                sub_categoryViewHolder.specialization_nameTV.SetTextColor(new Color(ContextCompat.GetColor(_context, Resource.Color.lightBlueColor)));
            if (sub_category_list[position].name == "Другое")
                sub_categoryViewHolder.specialization_nameTV.SetTextColor(Color.Black);

            if (position == sub_category_list.Count - 1)
                sub_categoryViewHolder.spec_checkBox.Visibility = ViewStates.Gone;
            else
                sub_categoryViewHolder.spec_checkBox.Visibility = ViewStates.Visible;

            if (checked_positions.Contains(position))
                sub_categoryViewHolder.spec_checkBox.Checked = true;
            else
                sub_categoryViewHolder.spec_checkBox.Checked = false;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CheckSubCategoryRow, parent, false);
            return new CheckSubCategoryViewHolder(layout, OnItemClick, CheckBoxClicked);
        }
        public override int ItemCount
        {
            get { try { return sub_category_list.Count; } catch { return 0; } }
        }

        void CheckBoxClicked(object sender, int position)
        {
            var checkBox = (CheckBox)sender;
            if (checkBox.Checked)
            {
                checked_positions.Add(position);
            }
            else
            {
                checked_positions.RemoveAt(checked_positions.FindIndex(x => x == position));
            }
        }

        async void OnItemClick(int position)
        {
            if (position + 1 == sub_category_list.Count)
            {
                edit = pref.Edit();
                var activity2 = new Intent(_context, typeof(NewCategoryAddActivity));
                var n = sub_category_list[position].name;
                edit.Apply();
                _context.StartActivity(activity2);
            }
        }
    }
}