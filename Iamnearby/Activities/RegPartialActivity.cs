using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using PCL.Database;
using Iamnearby.Methods;
using PCL.Models;
using Newtonsoft.Json;
using System;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait/*, Theme = "@style/AppThemeLightNavbar"*/)]
    public class RegPartialActivity : Activity
    {
        RelativeLayout backRelativeLayout;
        ImageButton back_button;
        EditText nameET;
        Button rememberBn, not_nowBn;
        ProgressBar activityIndicator;
        RegistrationMethods registrationMethods = new RegistrationMethods();
        UserMethods userMethods = new UserMethods();
        ISharedPreferences device_info_prefs = Application.Context.GetSharedPreferences("device_info", FileCreationMode.Private);
        static string name;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            {
                SetContentView(Resource.Layout.RegPartial);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                nameET = FindViewById<EditText>(Resource.Id.nameET);
                rememberBn = FindViewById<Button>(Resource.Id.rememberBn);
                not_nowBn = FindViewById<Button>(Resource.Id.not_nowBn);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                backRelativeLayout.Click += (s, e) => OnBackPressed();
                back_button.Click += (s, e) => OnBackPressed();

                if (!String.IsNullOrEmpty(name))
                    nameET.Text = name;

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                rememberBn.SetTypeface(tf, TypefaceStyle.Normal);
                nameET.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.infoTV).SetTypeface(tf, TypefaceStyle.Normal);
                not_nowBn.SetTypeface(tf, TypefaceStyle.Normal);

                nameET.TextChanged += (s, e) =>
                  {
                      name = nameET.Text;
                  };
                rememberBn.Click += async (s, e) =>
                  {
                      if (!String.IsNullOrEmpty(nameET.Text))
                      {
                          rememberBn.Visibility = ViewStates.Gone;
                          not_nowBn.Visibility = ViewStates.Gone;
                          activityIndicator.Visibility = ViewStates.Visible;
                          var res = await registrationMethods.RegisterSimple(nameET.Text);
                          rememberBn.Visibility = ViewStates.Visible;
                          not_nowBn.Visibility = ViewStates.Visible;
                          activityIndicator.Visibility = ViewStates.Gone;
                          var token = JsonConvert.DeserializeObject<SingleToken>(res).authToken;
                          var device_json = JsonConvert.DeserializeObject<Device>(device_info_prefs.GetString("device_json", String.Empty));
                          userMethods.InsertUser(token, device_json.deviceToken + "@fake.iamnearby.net");
                          OnBackPressed();
                      }
                      else
                          Toast.MakeText(this, GetString(Resource.String.enter_name), ToastLength.Short).Show();
                  };
                not_nowBn.Click += delegate
                  {
                      Toast.MakeText(this, GetString(Resource.String.message_cant_be_sent_for_unauthorized), ToastLength.Short).Show();
                      OnBackPressed();
                  };
            }
        }
    }
}