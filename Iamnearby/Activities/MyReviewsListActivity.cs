﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Iamnearby.Adapters;
using PCL.Database;
using Iamnearby.Methods;
using PCL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MyReviewsListActivity : Activity
    {
        ProgressBar activityIndicator;
        RecyclerView recyclerView;
        ImageButton back_button;
        static Button leave_feedbackBn;
        RelativeLayout backRelativeLayout;
        Feedbacks feedbacks = new Feedbacks();
        ISharedPreferences expert_feedback_pref = Application.Context.GetSharedPreferences("expert_feedback_pref", FileCreationMode.Private);
        UserMethods userMethods = new UserMethods();
        List<ReviewsList> deserialized_review_list;
        IEnumerable<ReviewsList> reverse_list;
        ReviewListAdapter reviewListAdapter;
        LinearLayoutManager layoutManager;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.ReviewsList);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                leave_feedbackBn = FindViewById<Button>(Resource.Id.leave_feedbackBn);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                activityIndicator.Visibility = ViewStates.Visible;
                recyclerView.Visibility = ViewStates.Gone;
                leave_feedbackBn.Visibility = ViewStates.Gone;
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                leave_feedbackBn.SetTypeface(tf, TypefaceStyle.Normal);

                var reviewJson = await feedbacks.ReviewList(expert_feedback_pref.GetString("expert_id", String.Empty), /*expert_feedback_pref.GetString("expert_category_id", String.Empty),*/ userMethods.GetUsersAuthToken());
                try
                {
                    deserialized_review_list = JsonConvert.DeserializeObject<List<ReviewsList>>(reviewJson);
                }
                catch
                {
                    deserialized_review_list = null;
                }
                activityIndicator.Visibility = ViewStates.Gone;
                recyclerView.Visibility = ViewStates.Visible;
                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                if (deserialized_review_list != null)
                {
                    reverse_list = Enumerable.Reverse(deserialized_review_list);
                    reviewListAdapter = new ReviewListAdapter(reverse_list.ToList(), this, tf);
                    layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                    recyclerView.SetLayoutManager(layoutManager);
                    recyclerView.SetAdapter(reviewListAdapter);
                }
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}