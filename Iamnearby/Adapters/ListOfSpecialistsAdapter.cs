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
using System.Globalization;

namespace Iamnearby.Adapters
{
    public class ListOfSpecialistsAdapter : RecyclerView.Adapter
    {
        ISharedPreferences expert_data, dialog_data;
        ISharedPreferencesEditor edit_expert, edit_dialog;
        public event EventHandler<int> ItemClick;
        private static Activity _context;
        public static List<Expert> experts_static;
        ListOfSpecialistsViewHolder listOfSpecialistsViewHolder;
        UserMethods userMethods = new UserMethods();
        Typeface tf;
        public ListOfSpecialistsAdapter(List<Expert> experts, Activity context, Typeface tf)
        {
            experts_static = experts;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            listOfSpecialistsViewHolder = (ListOfSpecialistsViewHolder)holder;

            listOfSpecialistsViewHolder.expert_nameTV.SetTypeface(tf, TypefaceStyle.Bold);
            listOfSpecialistsViewHolder.distanceTV.SetTypeface(tf, TypefaceStyle.Normal);
            listOfSpecialistsViewHolder.phoneTV.SetTypeface(tf, TypefaceStyle.Normal);
            listOfSpecialistsViewHolder.rating_valueTV.SetTypeface(tf, TypefaceStyle.Normal);
            listOfSpecialistsViewHolder.writeBn.SetTypeface(tf, TypefaceStyle.Bold);

            double distance_in_km;
            bool distance_in_km_parsed = System.Double.TryParse(experts_static[position].distance, out distance_in_km);
            if (distance_in_km > 999)
            {
                distance_in_km = distance_in_km / 1000;
                var final_dist = System.Math.Round(distance_in_km, 2).ToString().Replace(',', '.');
                listOfSpecialistsViewHolder.distanceTV.Text = final_dist + " км";
            }
            else
                listOfSpecialistsViewHolder.distanceTV.Text = experts_static[position].distance + " м";

            if (distance_in_km == 0)
            {
                listOfSpecialistsViewHolder.distanceTV.Visibility = ViewStates.Gone;
                listOfSpecialistsViewHolder.distanceIV.Visibility = ViewStates.Gone;
            }

            if (!System.String.IsNullOrEmpty(experts_static[position].avatarUrl))
            {
                try
                {
                    Glide.With(_context)
                   .Load("https://api.iamnearby.net/" + experts_static[position].avatarUrl)
                   .Into(listOfSpecialistsViewHolder.expert_imageIV);
                }
                catch { }
            }
            else
            {
                try
                {
                    Glide.With(_context)
                    .Load(null)
                    .Into(listOfSpecialistsViewHolder.expert_imageIV);
                }
                catch { }
            }

            if (experts_static[position].online)
                listOfSpecialistsViewHolder.onlineIV.Visibility = ViewStates.Visible;
            else
                listOfSpecialistsViewHolder.onlineIV.Visibility = ViewStates.Gone;

            try
            {
                listOfSpecialistsViewHolder.expert_nameTV.Text = experts_static[position].name;//split[0];
            }
            catch
            {
                listOfSpecialistsViewHolder.expert_nameTV.Text = experts_static[position].name;
            }
            listOfSpecialistsViewHolder.phoneTV.Text = experts_static[position].phone;
            if (experts_static[position].rating.Length > 3)
                listOfSpecialistsViewHolder.rating_valueTV.Text = experts_static[position].rating.ToString().Substring(0, 3);
            double rating_value = 0;
            try
            {
                listOfSpecialistsViewHolder.star1IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                listOfSpecialistsViewHolder.star2IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                listOfSpecialistsViewHolder.star3IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                listOfSpecialistsViewHolder.star4IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                listOfSpecialistsViewHolder.star5IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                rating_value = Convert.ToDouble(experts_static[position].rating, (CultureInfo.InvariantCulture));
                if (rating_value >= 1)
                {
                    listOfSpecialistsViewHolder.star1IV.SetBackgroundResource(Resource.Drawable.active_star);
                    if (rating_value >= 2)
                    {
                        listOfSpecialistsViewHolder.star2IV.SetBackgroundResource(Resource.Drawable.active_star);
                        if (rating_value >= 3)
                        {
                            listOfSpecialistsViewHolder.star3IV.SetBackgroundResource(Resource.Drawable.active_star);
                            if (rating_value >= 4)
                            {
                                listOfSpecialistsViewHolder.star4IV.SetBackgroundResource(Resource.Drawable.active_star);
                                if (rating_value >= 5)
                                {
                                    listOfSpecialistsViewHolder.star5IV.SetBackgroundResource(Resource.Drawable.active_star);
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            //extra chech needs to be here to prevent bug with displaying active stars
            if (System.String.IsNullOrEmpty(rating_value.ToString()) || rating_value == 0)
            {
                listOfSpecialistsViewHolder.rating_valueTV.Text = "0";
                listOfSpecialistsViewHolder.star1IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                listOfSpecialistsViewHolder.star2IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                listOfSpecialistsViewHolder.star3IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                listOfSpecialistsViewHolder.star4IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                listOfSpecialistsViewHolder.star5IV.SetBackgroundResource(Resource.Drawable.disabled_star);
            }
            //this construction needed here to prevent double actuation of click event
            if (!listOfSpecialistsViewHolder.writeBn.HasOnClickListeners)
            {
                listOfSpecialistsViewHolder.writeBn.Click += (s, e) =>
                {
                    //my_specializations.RemoveAt(position);
                    //this.NotifyDataSetChanged();
                };
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            expert_data = Application.Context.GetSharedPreferences("experts", FileCreationMode.Private);
            dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
            edit_expert = expert_data.Edit();
            edit_dialog = dialog_data.Edit();
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ExpertRow, parent, false);
            return new ListOfSpecialistsViewHolder(layout, OnItemClick, WriteBnOnClick);
        }
        public override int ItemCount
        {
            get { try { return experts_static.Count; } catch { return 0; } }
        }

        void WriteBnOnClick(int position)
        {
            edit_dialog.PutString("expert_id", experts_static[position].id);
            edit_dialog.PutString("expert_name", experts_static[position].name);
            edit_dialog.PutString("expert_avatar", experts_static[position].avatarUrl);
            edit_dialog.Apply();
            var activity2 = new Intent(_context, typeof(DialogActivity));
            _context.StartActivity(activity2);
        }

        void OnItemClick(int position)
        {
            edit_expert.PutString("expert_id", experts_static[position].id);
            edit_expert.Apply();
            var activity2 = new Intent(_context, typeof(ThreeLevelExpertProfileActivity));
            _context.StartActivity(activity2);
        }
    }
}