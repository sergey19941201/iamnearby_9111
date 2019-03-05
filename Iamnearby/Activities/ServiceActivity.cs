using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using PCL.Database;
using Iamnearby.Methods;
using PCL.Models;
using System;
using System.Collections.Generic;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class ServiceActivity : Activity
    {
        ISharedPreferences specialization_for_edit_pref = Application.Context.GetSharedPreferences("specialization_for_edit_pref", FileCreationMode.Private);
        ImageButton back_button;
        RelativeLayout backRelativeLayout;
        EditText service_nameET, service_priceET;
        ProgressBar activityIndicator;
        Button saveBn;
        ProfileAndExpertMethods profileAndExpertMethods = new ProfileAndExpertMethods();
        AddServiceActivity addServiceActivity = new AddServiceActivity();
        UserMethods userMethods = new UserMethods();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.Service);
                service_nameET = FindViewById<EditText>(Resource.Id.service_nameET);
                service_priceET = FindViewById<EditText>(Resource.Id.service_priceET);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                back_button.Click += (s, e) => { OnBackPressed(); };
                backRelativeLayout.Click += (s, e) => { OnBackPressed(); };
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                saveBn = FindViewById<Button>(Resource.Id.saveBn);

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                saveBn.SetTypeface(tf, TypefaceStyle.Normal);
                service_nameET.SetTypeface(tf, TypefaceStyle.Normal);
                service_priceET.SetTypeface(tf, TypefaceStyle.Normal);

                FindViewById<Button>(Resource.Id.removeBn).Visibility = ViewStates.Gone;

                saveBn.Click += async (s, e) =>
                  {
                      if (!String.IsNullOrEmpty(service_nameET.Text))
                      {
                          activityIndicator.Visibility = ViewStates.Visible;
                          saveBn.Visibility = ViewStates.Gone;
                          var servList = new List<ServiceData>();
                          servList.Add(new ServiceData() { name = service_nameET.Text, price = service_priceET.Text });
                          var res1 = await profileAndExpertMethods.EditSpecializationServices(
                              userMethods.GetUsersAuthToken(),
                              specialization_for_edit_pref.GetString("categoryId", String.Empty),
                              specialization_for_edit_pref.GetInt("categoryIndex", 0),
                              servList); activityIndicator.Visibility = ViewStates.Gone;
                          var res_reload = await addServiceActivity.ReloadData();
                          saveBn.Visibility = ViewStates.Visible;
                          OnBackPressed();
                      }
                      else
                          Toast.MakeText(this, GetString(Resource.String.add_name), ToastLength.Short).Show();
                  };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}