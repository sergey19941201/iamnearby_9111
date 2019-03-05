using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Iamnearby.Adapters;
using Iamnearby.Methods;
using Newtonsoft.Json;
using PCL.Database;
using PCL.Models;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustNothing)]
    public class CityInRegionActivity : Activity
    {
        EditText searchET;
        RelativeLayout tintLL;
        TextView noTV, yesTV;
        ImageButton back_button;
        Button autolocatonBn;
        ProgressBar activityIndicator, activityIndicatorSearch;
        RelativeLayout backRelativeLayout;
        RecyclerView recyclerView, search_recyclerView;
        RecyclerView.LayoutManager layoutManager, search_layoutManager;
        ImageButton close_searchBn;
        LinearLayout searchLL;
        CitySearchAdapter citySearchAdapter;
        List<CitySearch> deserialized_search;
        ImageView nothingIV, searchIV;
        TextView nothingTV, headerTV, your_city_valueTV;
        CountryMethods countryMethods = new CountryMethods();
        ISharedPreferences loc_pref;
        ISharedPreferencesEditor edit_loc;
        List<MyLocation> myLocDeserialized;
        UserMethods userMethods = new UserMethods();
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.RegionForSearching);

                loc_pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                searchET = FindViewById<EditText>(Resource.Id.searchET);
                close_searchBn = FindViewById<ImageButton>(Resource.Id.close_searchBn);
                activityIndicatorSearch = FindViewById<ProgressBar>(Resource.Id.activityIndicatorSearch);
                search_recyclerView = FindViewById<RecyclerView>(Resource.Id.search_recyclerView);
                searchLL = FindViewById<LinearLayout>(Resource.Id.searchLL);
                nothingIV = FindViewById<ImageView>(Resource.Id.nothingIV);
                searchIV = FindViewById<ImageView>(Resource.Id.searchIV);
                nothingTV = FindViewById<TextView>(Resource.Id.nothingTV);
                headerTV = FindViewById<TextView>(Resource.Id.headerTV);
                your_city_valueTV = FindViewById<TextView>(Resource.Id.your_city_valueTV);
                recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerView.SetLayoutManager(layoutManager);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                autolocatonBn = FindViewById<Button>(Resource.Id.autolocatonBn);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                tintLL = FindViewById<RelativeLayout>(Resource.Id.tintLL);
                noTV = FindViewById<TextView>(Resource.Id.noTV);
                yesTV = FindViewById<TextView>(Resource.Id.yesTV);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                activityIndicatorSearch.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                search_layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                search_recyclerView.SetLayoutManager(search_layoutManager);
                headerTV.Text = loc_pref.GetString("auto_region_name", String.Empty);
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                headerTV.SetTypeface(tf, TypefaceStyle.Bold);
                autolocatonBn.SetTypeface(tf, TypefaceStyle.Normal);
                searchET.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textView5).SetTypeface(tf, TypefaceStyle.Normal);
                your_city_valueTV.SetTypeface(tf, TypefaceStyle.Normal);
                yesTV.SetTypeface(tf, TypefaceStyle.Normal);
                noTV.SetTypeface(tf, TypefaceStyle.Normal);
                nothingTV.SetTypeface(tf, TypefaceStyle.Normal);


                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                autolocatonBn.Click += async (s, e) =>
                {
                    activityIndicator.Visibility = ViewStates.Visible;
                    var myLocJson = await countryMethods.CurrentLocation(loc_pref.GetString("latitude", String.Empty), loc_pref.GetString("longitude", String.Empty));
                    myLocDeserialized = JsonConvert.DeserializeObject<List<MyLocation>>(myLocJson);
                    your_city_valueTV.Text = myLocDeserialized[0].city.name + "?";
                    activityIndicator.Visibility = ViewStates.Gone;
                    tintLL.Visibility = ViewStates.Visible;
                };

                yesTV.Click += (s, e) =>
                {
                    edit_loc = loc_pref.Edit();
                    edit_loc.PutString("auto_city_id", myLocDeserialized[0].city.id);
                    edit_loc.PutString("auto_city_name", myLocDeserialized[0].city.name);
                    edit_loc.PutString("auto_region_id", myLocDeserialized[0].region.id);
                    edit_loc.PutString("auto_region_name", myLocDeserialized[0].region.name);
                    edit_loc.PutString("auto_country_id", myLocDeserialized[0].country.id);
                    edit_loc.PutString("auto_country_name", myLocDeserialized[0].country.name);
                    edit_loc.Apply();
                    tintLL.Visibility = ViewStates.Gone;
                    //if (userMethods.UserExists())
                        StartActivity(typeof(SpecialistsCategoryActivity));
                    //else
                    //    StartActivity(typeof(RegistrationActivity));
                };

                noTV.Click += (s, e) =>
                {
                    tintLL.Visibility = ViewStates.Gone;
                    //StartActivity(typeof(LocationActivity));
                };

                searchET.TextChanged += async (s, e) =>
                {
                    if (!String.IsNullOrEmpty(searchET.Text))
                    {
                        nothingIV.Visibility = ViewStates.Gone;
                        nothingTV.Visibility = ViewStates.Gone;
                        searchLL.Visibility = ViewStates.Visible;
                        close_searchBn.Visibility = ViewStates.Visible;
                        activityIndicatorSearch.Visibility = ViewStates.Visible;
                        search_recyclerView.Visibility = ViewStates.Gone;
                        var search_content = await countryMethods.CitySearch(loc_pref.GetString("auto_region_id", String.Empty), searchET.Text);
                        if (!search_content.ToLower().Contains("пошло не так".ToLower()) && !search_content.Contains("null"))
                        //try
                        {
                            search_recyclerView.Visibility = ViewStates.Visible;
                            deserialized_search = JsonConvert.DeserializeObject<List<CitySearch>>(search_content.ToString());
                            List<CitySearch> searchDisplayings = new List<CitySearch>();
                            foreach (var item in deserialized_search)
                            {
                                searchDisplayings.Add(new CitySearch { id = item.id, city = item.city });
                            }

                            citySearchAdapter = new CitySearchAdapter(searchDisplayings, this,tf);
                            citySearchAdapter.NotifyDataSetChanged();
                            search_recyclerView.SetAdapter(citySearchAdapter);

                            citySearchAdapter.NotifyDataSetChanged();
                        }
                        //catch
                        else
                        {
                            search_recyclerView.Visibility = ViewStates.Gone;
                            nothingIV.Visibility = ViewStates.Visible;
                            nothingTV.Visibility = ViewStates.Visible;
                        }

                        activityIndicatorSearch.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        searchIV.Visibility = ViewStates.Visible;
                        close_searchBn.Visibility = ViewStates.Gone;
                        searchLL.Visibility = ViewStates.Gone;
                    }
                };
                close_searchBn.Click += (s, e) =>
                {
                    searchET.Text = null;
                    imm.HideSoftInputFromWindow(searchET.WindowToken, 0);
                    searchLL.Visibility = ViewStates.Gone;
                };

                searchET.EditorAction += (object sender, EditText.EditorActionEventArgs e) =>
                {
                    imm.HideSoftInputFromWindow(searchET.WindowToken, 0);
                };


                activityIndicator.Visibility = ViewStates.Visible;
                var citiesJson = await countryMethods.CityList(loc_pref.GetString("auto_region_id", String.Empty));
                activityIndicator.Visibility = ViewStates.Gone;
                var deserialized_cities = JsonConvert.DeserializeObject<List<CitySearch>>(citiesJson);

                var cityForRegAdapter = new CityForRegAdapter(deserialized_cities, this, tf);
                recyclerView.SetAdapter(cityForRegAdapter);
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}
