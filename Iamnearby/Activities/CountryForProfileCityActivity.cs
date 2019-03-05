using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Iamnearby.Adapters;
using Iamnearby.Methods;
using PCL.Models;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CountryForProfileCityActivity : Activity
    {
        TextView headerTV;
        ImageButton back_button;
        RelativeLayout backRelativeLayout;
        Button searchBn;
        ProgressBar activityIndicator;
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        CountryMethods countryMethods = new CountryMethods();
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.YourSpecialization);
                recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                headerTV = FindViewById<TextView>(Resource.Id.headerTV);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                headerTV.Text = GetString(Resource.String.country);
                searchBn = FindViewById<Button>(Resource.Id.searchBn);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                searchBn.Visibility = ViewStates.Gone;
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");

                headerTV.SetTypeface(tf, TypefaceStyle.Bold);
                FindViewById<EditText>(Resource.Id.searchET).SetTypeface(tf, TypefaceStyle.Normal);
                searchBn.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.nothingTV).SetTypeface(tf, TypefaceStyle.Normal);
                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                activityIndicator.Visibility = ViewStates.Visible;
                var countriesJson = await countryMethods.GetCountries();
                activityIndicator.Visibility = ViewStates.Gone;
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerView.SetLayoutManager(layoutManager);
                var deserialized_countries = JsonConvert.DeserializeObject<List<City>>(countriesJson);
                var countryAdapter = new CountryForProfileCityAdapter(deserialized_countries, this, tf);
                recyclerView.SetAdapter(countryAdapter);
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}