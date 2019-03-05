using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Iamnearby.Adapters;
using Iamnearby.Methods;
using Newtonsoft.Json;
using PCL.Models;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class FeedbackChooseCategoryActivity : Activity
    {
        ProgressBar activityIndicator;
        RecyclerView recyclerView;
        ImageButton back_button;
        RelativeLayout backRelativeLayout;
        ProfileAndExpertMethods profileAndExpertMethods = new ProfileAndExpertMethods();
        PCL.HttpMethods.ProfileAndExpertMethods profileAndExpertMethodsPCL = new PCL.HttpMethods.ProfileAndExpertMethods();
        ExpertProfile deserialized_expert_profile;
        RecyclerView.LayoutManager layoutManager;
        ISharedPreferences loc_pref;
        //Feedbacks feedbacks = new Feedbacks();
        ISharedPreferences expert_feedback_pref = Application.Context.GetSharedPreferences("expert_feedback_pref", FileCreationMode.Private);
        List<City> categoriesList = new List<City>();
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.FeedbackCategory);
                loc_pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                activityIndicator.Visibility = ViewStates.Visible;
                recyclerView.Visibility = ViewStates.Visible;
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                var expert_categ_id = expert_feedback_pref.GetString("expert_category_id", String.Empty);
                var expert_id = expert_feedback_pref.GetString("expert_id", String.Empty);
                var expert_json = await profileAndExpertMethodsPCL.ExpertProfile(expert_id, loc_pref.GetString("latitude", String.Empty), loc_pref.GetString("longitude", String.Empty));
                try
                {
                    deserialized_expert_profile = JsonConvert.DeserializeObject<ExpertProfile>(expert_json);
                    foreach (var item in deserialized_expert_profile.serviceCategories)
                    {
                        categoriesList.Add(new City { id = item.categoryId, name = item.name });
                        activityIndicator.Visibility = ViewStates.Gone;

                    }
                    layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                    recyclerView.SetLayoutManager(layoutManager);
                    var feedbackCategoryAdapter = new FeedbackCategoryAdapter(categoriesList, this, tf);
                    recyclerView.SetAdapter(feedbackCategoryAdapter);
                }
                catch
                {
                    Toast.MakeText(this, GetString(Resource.String.cannot_leave_feedback), ToastLength.Long).Show();
                    OnBackPressed();
                }
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}