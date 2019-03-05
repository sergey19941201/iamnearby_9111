using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Bumptech.Glide;
using Iamnearby.Activities;
using PCL.Database;
using PCL.Models;
using Iamnearby.ViewHolders;
using System;
using System.Collections.Generic;

namespace Iamnearby.Adapters
{
    public class UpperSpecializationAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private static Activity _context;
        private List<UpperSpecializations> specializations_list;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("categories_data", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
        UserMethods userMethods = new UserMethods();
        Typeface tf;

        public UpperSpecializationAdapter(List<UpperSpecializations> specializations_list, Activity context, Typeface tf)
        {
            this.specializations_list = specializations_list;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var specViewHolder = (UserSpecializationViewHolder)holder;
            specViewHolder.specialization_nameTV.Text = specializations_list[position].name;
            specViewHolder.specialization_nameTV.SetTypeface(tf, TypefaceStyle.Bold);
            Glide.With(Application.Context)
                .Load("https://api.iamnearby.net/" + specializations_list[position].iconUrl)
                //.Placeholder(Resource.Drawable.specialization_imageIV)
                .Into(specViewHolder.specialization_imageIV);
            if (!userMethods.UserExists())
            {
                if (position == 0)
                    specViewHolder.specialization_imageIV.Visibility = ViewStates.Gone;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.YourSpecializationRow, parent, false);
            return new UserSpecializationViewHolder(layout, OnItemClick);
        }

        public override int ItemCount
        {
            get { try { return specializations_list.Count; } catch { return 0; } }
        }

        void OnItemClick(int position)
        {
            edit = pref.Edit();
            var activity2 = new Intent(_context, typeof(SubCategoryActivity));
            edit.PutString("spec_id", specializations_list[position].id);
            edit.PutString("spec_name", specializations_list[position].name);
            edit.Apply();
            if (!userMethods.UserExists())
            {
                if (position != 0)
                    _context.StartActivity(activity2);
                else if (position == 0)
                    _context.StartActivity(new Intent(_context, typeof(RegEmailActivity)));
            }
            else
                _context.StartActivity(activity2);
        }
    }
}