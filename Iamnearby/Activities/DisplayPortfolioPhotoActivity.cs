﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using PCL.Database;
using Iamnearby.Methods;
using System;
using System.Threading;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class DisplayPortfolioPhotoActivity : Activity
    {
        RelativeLayout backRelativeLayout;
        ImageButton back_button;
        Button removeBn;
        ImageView fullIV;
        ProgressBar activityIndicator, activityIndicatorDel;
        RelativeLayout tint_DelLL;
        TextView noTV, yesTV;
        public static string image_url;
        public static bool from_expert;
        ProfileAndExpertMethods profileAndExpertMethods = new ProfileAndExpertMethods();
        UserMethods userMethods = new UserMethods();
        AddServiceActivity addServiceActivity = new AddServiceActivity();
        ISharedPreferences specialization_for_edit_pref = Application.Context.GetSharedPreferences("specialization_for_edit_pref", FileCreationMode.Private);
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.DisplayPortfolioPhoto);

                activityIndicatorDel = FindViewById<ProgressBar>(Resource.Id.activityIndicatorDel);
                tint_DelLL = FindViewById<RelativeLayout>(Resource.Id.tint_DelLL);
                noTV = FindViewById<TextView>(Resource.Id.noTV);
                yesTV = FindViewById<TextView>(Resource.Id.yesTV);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                activityIndicatorDel.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                removeBn = FindViewById<Button>(Resource.Id.removeBn);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                fullIV = FindViewById<ImageView>(Resource.Id.fullIV);

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                removeBn.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textView5).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.your_city_valueTV).SetTypeface(tf, TypefaceStyle.Normal);
                yesTV.SetTypeface(tf, TypefaceStyle.Normal);
                noTV.SetTypeface(tf, TypefaceStyle.Normal);

                tint_DelLL.Click += (s, e) =>
                {
                    tint_DelLL.Visibility = ViewStates.Gone;
                };
                yesTV.Click += async (s, e) =>
                {
                    activityIndicatorDel.Visibility = ViewStates.Visible;
                    var res = await profileAndExpertMethods.DeletePhoto(userMethods.GetUsersAuthToken(), image_url, specialization_for_edit_pref.GetString("categoryId", String.Empty));
                    var res_reload = await addServiceActivity.ReloadData();
                    activityIndicatorDel.Visibility = ViewStates.Gone;
                    tint_DelLL.Visibility = ViewStates.Gone;
                    //StartActivity(typeof(UserProfileActivity));
                    OnBackPressed();
                };

                noTV.Click += (s, e) =>
                {
                    tint_DelLL.Visibility = ViewStates.Gone;
                };

                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };

                Thread backgroundThread = new Thread(new ThreadStart(() =>
                {
                    Glide.Get(Application.Context).ClearDiskCache();
                }));
                backgroundThread.IsBackground = true;
                backgroundThread.Start();
                Glide.Get(this).ClearMemory();
                Glide.With(Application.Context)
                    .Load(image_url)
                    .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
                    .SkipMemoryCache(true))
                    //.Placeholder(Resource.Drawable.specialization_imageIV)
                    .Into(fullIV);

                removeBn.Click += (s, e) =>
                {
                    tint_DelLL.Visibility = ViewStates.Visible;
                };
                if (!from_expert)
                    removeBn.Visibility = ViewStates.Visible;
                else
                    removeBn.Visibility = ViewStates.Gone;
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}