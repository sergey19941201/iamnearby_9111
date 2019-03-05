﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Iamnearby.Adapters;
using PCL.Database;
using Iamnearby.Methods;
using PCL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustNothing)]
    public class RegionForSearchActivity : Activity
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
        RegionsSearchAdapter regionsSearchAdapter;
        List<RegionSearch> deserialized_search;
        ImageView nothingIV, searchIV;
        TextView nothingTV, your_city_valueTV;
        CountryMethods countryMethods = new CountryMethods();
        ISharedPreferences pref, loc_pref;
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
                pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                searchET = FindViewById<EditText>(Resource.Id.searchET);
                close_searchBn = FindViewById<ImageButton>(Resource.Id.close_searchBn);
                activityIndicatorSearch = FindViewById<ProgressBar>(Resource.Id.activityIndicatorSearch);
                search_recyclerView = FindViewById<RecyclerView>(Resource.Id.search_recyclerView);
                searchLL = FindViewById<LinearLayout>(Resource.Id.searchLL);
                nothingIV = FindViewById<ImageView>(Resource.Id.nothingIV);
                searchIV = FindViewById<ImageView>(Resource.Id.searchIV);
                nothingTV = FindViewById<TextView>(Resource.Id.nothingTV);
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

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
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
                    var myLocJson = await countryMethods.CurrentLocation(pref.GetString("latitude", String.Empty), pref.GetString("longitude", String.Empty));
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
                    StartActivity(typeof(SpecialistsCategoryActivity));
                };

                noTV.Click += (s, e) =>
                {
                    tintLL.Visibility = ViewStates.Gone;
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
                        var search_content = await countryMethods.RegionsSearch(loc_pref.GetString("auto_country_id", String.Empty), searchET.Text);
                        if (!search_content.ToLower().Contains("пошло не так".ToLower()) && !search_content.Contains("null"))
                        {
                            search_recyclerView.Visibility = ViewStates.Visible;
                            deserialized_search = JsonConvert.DeserializeObject<List<RegionSearch>>(search_content.ToString());
                            List<RegionSearch> searchDisplayings = new List<RegionSearch>();
                            foreach (var item in deserialized_search)
                            {
                                searchDisplayings.Add(new RegionSearch { id = item.id, region = item.region });
                            }

                            regionsSearchAdapter = new RegionsSearchAdapter(searchDisplayings, this, tf);
                            regionsSearchAdapter.NotifyDataSetChanged();
                            search_recyclerView.SetAdapter(regionsSearchAdapter);

                            regionsSearchAdapter.NotifyDataSetChanged();
                        }
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
                var regionsJson = await countryMethods.GetRegions(loc_pref.GetString("auto_country_id", String.Empty));
                activityIndicator.Visibility = ViewStates.Gone;
                var deserialized_regions = JsonConvert.DeserializeObject<List<RegionSearch>>(regionsJson);

                var countryAdapter = new RegionAdapter(deserialized_regions, this, tf);
                recyclerView.SetAdapter(countryAdapter);
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}