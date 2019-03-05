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
using Newtonsoft.Json;
using System;

namespace Iamnearby.Activities
{
    [Activity(Name = "iamnearby.iamnearby.url_entry_activity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class url_entry_activity : Activity
    {
        TextView infoTV;
        ProgressBar activityIndicator;
        RegistrationMethods registrationMethods = new RegistrationMethods();
        AuthorizationMethods authorizationMethods = new AuthorizationMethods();
        UserMethods userMethods = new UserMethods();
        ISharedPreferences pref_reg = Application.Context.GetSharedPreferences("reg_data", FileCreationMode.Private);
        ISharedPreferences pref_auth = Application.Context.GetSharedPreferences("auth_data", FileCreationMode.Private);
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.UrlEntry);
                infoTV = FindViewById<TextView>(Resource.Id.infoTV);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                var outside_data = Intent.Data.EncodedAuthority;

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                infoTV.SetTypeface(tf, TypefaceStyle.Normal);

                if (outside_data.ToString().Contains("register_activate"))
                {
                    activityIndicator.Visibility = ViewStates.Visible;
                    infoTV.Text = GetString(Resource.String.reg_confirmation);
                    var activate = await registrationMethods.RegisterActivate(pref_reg.GetString("email", String.Empty));
                    activityIndicator.Visibility = ViewStates.Gone;

                    var deserialized_value = JsonConvert.DeserializeObject<RegAfter>(activate.ToString());
                    userMethods.InsertUser(deserialized_value.authToken, pref_reg.GetString("email", String.Empty));
                    StartActivity(typeof(UserProfileActivity));
                }
                else if (outside_data.ToString().Contains("auth_activate"))
                {
                    infoTV.Text = GetString(Resource.String.auth_confirmation);
                    activityIndicator.Visibility = ViewStates.Visible;
                    var activate = await authorizationMethods.AuthActivate(pref_auth.GetString("email", String.Empty));
                    activityIndicator.Visibility = ViewStates.Gone;

                    if (activate != "null" && activate != null && activate != "false")
                    {
                        var deserialized_value = JsonConvert.DeserializeObject<RegAfter>(activate.ToString());
                        userMethods.InsertUser(deserialized_value.authToken, pref_auth.GetString("email", String.Empty));
                        StartActivity(typeof(UserProfileActivity));
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.activation_aborted), ToastLength.Short).Show();
                        StartActivity(typeof(AuthorizationActivity));
                    }
                };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}