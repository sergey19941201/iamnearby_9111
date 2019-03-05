using System;
using System.Collections.Generic;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Bumptech.Glide;
using Iamnearby.Activities;
using Iamnearby.ViewHolders;
using PCL.Models;

namespace Iamnearby.Adapters
{
    public class ReviewListAdapter : RecyclerView.Adapter
    {
        private static Activity _context;
        List<ReviewsList> reviews_list;
        ISharedPreferencesEditor edit_expert;
        ISharedPreferences expert_data = Application.Context.GetSharedPreferences("experts", FileCreationMode.Private);
        Typeface tf;
        public ReviewListAdapter(List<ReviewsList> reviews_list, Activity context, Typeface tf)
        {
            this.reviews_list = reviews_list;
            _context = context;
            this.tf = tf;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.FeedbackRow, parent, false);
            return new ReviewViewHolder(layout, ReplyOnClick, itemClick);
        }

        void ReplyOnClick(int position)
        {
            var activity2 = new Intent(_context, typeof(ResponseFeedbackActivity));
            activity2.PutExtra("expertId", reviews_list[position].userId);
            activity2.PutExtra("review_id", reviews_list[position].id);
            activity2.PutExtra("review_rating", reviews_list[position].rating);
            activity2.PutExtra("review_date", reviews_list[position].reviewDate);
            activity2.PutExtra("review_text", reviews_list[position].reviewText);
            activity2.PutExtra("review_image_url", reviews_list[position].avatarUrl);
            activity2.PutExtra("review_name", reviews_list[position].name);
            activity2.PutExtra("review_online", reviews_list[position].online);
            _context.StartActivity(activity2);
        }

        void itemClick(int position)
        {
            edit_expert = expert_data.Edit();
            edit_expert.PutString("expert_id", reviews_list[position].userId);
            edit_expert.Apply();
            _context.StartActivity(typeof(ThreeLevelExpertProfileActivity));
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var reviewViewHolder = (ReviewViewHolder)holder;

            reviewViewHolder.expert_nameTV.SetTypeface(tf, TypefaceStyle.Bold);
            reviewViewHolder.rating_valueTV.SetTypeface(tf, TypefaceStyle.Normal);
            reviewViewHolder.dateTV.SetTypeface(tf, TypefaceStyle.Normal);
            reviewViewHolder.distanceTV.SetTypeface(tf, TypefaceStyle.Normal);
            reviewViewHolder.reviewTextTV.SetTypeface(tf, TypefaceStyle.Normal);
            reviewViewHolder.replyTextTV.SetTypeface(tf, TypefaceStyle.Normal);

            reviewViewHolder.expert_nameTV.Text = reviews_list[position].name;
            reviewViewHolder.star1IV.SetBackgroundResource(Resource.Drawable.disabled_star);
            reviewViewHolder.star2IV.SetBackgroundResource(Resource.Drawable.disabled_star);
            reviewViewHolder.star3IV.SetBackgroundResource(Resource.Drawable.disabled_star);
            reviewViewHolder.star4IV.SetBackgroundResource(Resource.Drawable.disabled_star);
            reviewViewHolder.star5IV.SetBackgroundResource(Resource.Drawable.disabled_star);

            reviewViewHolder.rating_valueTV.Text = reviews_list[position].rating;
            reviewViewHolder.dateTV.Text = reviews_list[position].reviewDate;

            if (reviews_list[position].canAnswer)
                reviewViewHolder.replyBn.Visibility = ViewStates.Visible;
            else
            {
                if (!String.IsNullOrEmpty(reviews_list[position].reviewAnswer))
                {
                    reviewViewHolder.replyTextTV.Visibility = ViewStates.Visible;
                    reviewViewHolder.replyTextTV.Text = _context.GetString(Resource.String.feedback_response) + ": " + reviews_list[position].reviewAnswer;
                }
                else
                    reviewViewHolder.replyTextTV.Visibility = ViewStates.Gone;
                reviewViewHolder.replyBn.Visibility = ViewStates.Gone;
            }
            if (reviews_list[position].online == "true")
                reviewViewHolder.onlineIV.Visibility = ViewStates.Visible;
            else
                reviewViewHolder.onlineIV.Visibility = ViewStates.Gone;

            reviewViewHolder.reviewTextTV.Text = reviews_list[position].reviewText;

            if (!System.String.IsNullOrEmpty(reviews_list[position].avatarUrl))
            {
                Glide.With(_context)
               .Load("https://api.iamnearby.net/" + reviews_list[position].avatarUrl)
               .Into(reviewViewHolder.expert_imageIV);
            }
            else
            {
                Glide.With(_context)
                .Load(null)
                .Into(reviewViewHolder.expert_imageIV);
            }
            double rating_value = 0;
            try
            {
                rating_value = Convert.ToDouble(reviews_list[position].rating, (CultureInfo.InvariantCulture));
                if (rating_value >= 1)
                {
                    reviewViewHolder.star1IV.SetBackgroundResource(Resource.Drawable.active_star);
                    if (rating_value >= 2)
                    {
                        reviewViewHolder.star2IV.SetBackgroundResource(Resource.Drawable.active_star);
                        if (rating_value >= 3)
                        {
                            reviewViewHolder.star3IV.SetBackgroundResource(Resource.Drawable.active_star);
                            if (rating_value >= 4)
                            {
                                reviewViewHolder.star4IV.SetBackgroundResource(Resource.Drawable.active_star);
                                if (rating_value >= 5)
                                {
                                    reviewViewHolder.star5IV.SetBackgroundResource(Resource.Drawable.active_star);
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }
        public override int ItemCount
        {
            get
            {
                try { return reviews_list.Count; } catch { return 0; }
            }
        }
    }
}