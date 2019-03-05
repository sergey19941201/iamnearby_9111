using System;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using PCL.Database;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LocationActivity : Activity
    {
        RelativeLayout locationRL, countryRL;
        ImageButton back_button;
        RelativeLayout backRelativeLayout;
        Button acceptBn;
        UserMethods userMethods = new UserMethods();
        TextView city_valueTV, country_valueTV;
        ISharedPreferences loc_pref;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                loc_pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
                SetContentView(Resource.Layout.Location);
                locationRL = FindViewById<RelativeLayout>(Resource.Id.locationRL);
                countryRL = FindViewById<RelativeLayout>(Resource.Id.countryRL);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                acceptBn = FindViewById<Button>(Resource.Id.acceptBn);
                city_valueTV = FindViewById<TextView>(Resource.Id.city_valueTV);
                country_valueTV = FindViewById<TextView>(Resource.Id.country_valueTV);

                city_valueTV.Text = loc_pref.GetString("auto_city_name", String.Empty);
                country_valueTV.Text = loc_pref.GetString("auto_country_name", String.Empty);
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                acceptBn.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textViesw2).SetTypeface(tf, TypefaceStyle.Normal);
                country_valueTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textVssiew2).SetTypeface(tf, TypefaceStyle.Normal);
                city_valueTV.SetTypeface(tf, TypefaceStyle.Normal);

                countryRL.Click += (s, e) =>
                  {
                      StartActivity(typeof(CountryActivity));
                  };
                locationRL.Click += (s, e) =>
                  {
                      StartActivity(typeof(RegionForSearchActivity));
                  };
                acceptBn.Click += (s, e) =>
                  {
                      StartActivity(typeof(SpecialistsCategoryActivity));
                  };
                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}