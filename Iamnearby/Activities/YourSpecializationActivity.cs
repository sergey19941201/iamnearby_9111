using Android.App;
using Android.Content;
using Android.Content.PM;
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
    [Activity(Label = "YourSpecializationActivity", ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden)]
    public class YourSpecializationActivity : Activity
    {
        RelativeLayout backRelativeLayout;
        ImageButton back_button, close_searchBn;
        LinearLayout searchLL;
        Button searchBn;
        ImageView nothingIV;
        TextView headerTV, nothingTV;
        ProgressBar activityIndicator, activityIndicatorSearch;
        RecyclerView recyclerView, search_recyclerView;
        RecyclerView.LayoutManager layoutManager, search_layoutManager;
        EditText searchET;
        List<SearchCategory> deserialized_search;
        SubCategorySearchAdapter subCategorySearchAdapter;
        InputMethodManager inputMethodManager;
        UserMethods userMethods = new UserMethods();
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.YourSpecialization);
                inputMethodManager = Application.GetSystemService(Context.InputMethodService) as InputMethodManager;
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                SpecializationMethods specializationMethods = new SpecializationMethods();
                searchET = FindViewById<EditText>(Resource.Id.searchET);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                activityIndicatorSearch = FindViewById<ProgressBar>(Resource.Id.activityIndicatorSearch);
                searchLL = FindViewById<LinearLayout>(Resource.Id.searchLL);
                search_recyclerView = FindViewById<RecyclerView>(Resource.Id.search_recyclerView);
                nothingTV = FindViewById<TextView>(Resource.Id.nothingTV);
                nothingIV = FindViewById<ImageView>(Resource.Id.nothingIV);
                headerTV = FindViewById<TextView>(Resource.Id.headerTV);
                searchBn = FindViewById<Button>(Resource.Id.searchBn);
                close_searchBn = FindViewById<ImageButton>(Resource.Id.close_searchBn);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                activityIndicatorSearch.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                search_layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                search_recyclerView.SetLayoutManager(search_layoutManager);
                recyclerView.SetLayoutManager(layoutManager);
                searchET.Visibility = ViewStates.Gone;

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                headerTV.SetTypeface(tf, TypefaceStyle.Bold);
                nothingTV.SetTypeface(tf, TypefaceStyle.Normal);
                searchET.SetTypeface(tf, TypefaceStyle.Normal);
                searchBn.SetTypeface(tf, TypefaceStyle.Normal);

                searchBn.Click += (s, e) =>
                {
                    close_searchBn.Visibility = ViewStates.Visible;
                    searchET.Visibility = ViewStates.Visible;
                    searchBn.Visibility = ViewStates.Gone;
                    headerTV.Visibility = ViewStates.Gone;

                    searchET.RequestFocus();
                    showKeyboard();
                };

                searchET.EditorAction += (object sender, EditText.EditorActionEventArgs e) =>
                {
                    imm.HideSoftInputFromWindow(searchET.WindowToken, 0);
                };

                close_searchBn.Click += (s, e) =>
                {
                    searchET.Text = null;
                    imm.HideSoftInputFromWindow(searchET.WindowToken, 0);
                    searchLL.Visibility = ViewStates.Gone;
                    headerTV.Visibility = ViewStates.Visible;
                    searchET.Visibility = ViewStates.Gone;
                    close_searchBn.Visibility = ViewStates.Gone;
                    searchBn.Visibility = ViewStates.Visible;
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
                        activityIndicatorSearch.Visibility = ViewStates.Visible;
                        var search_content = await specializationMethods.SearchCategory(searchET.Text);
                        if (!search_content.ToLower().Contains("пошло не так".ToLower()) && !search_content.Contains("null"))
                        {
                            search_recyclerView.Visibility = ViewStates.Visible;
                            deserialized_search = JsonConvert.DeserializeObject<List<SearchCategory>>(search_content.ToString());
                            List<SearchDisplaying> searchDisplayings = new List<SearchDisplaying>();
                            foreach (var item in deserialized_search)
                            {
                                if (item.hasSubcategory)
                                {
                                    searchDisplayings.Add(new SearchDisplaying { id = item.id, name = item.name, iconUrl = item.iconUrl, isRoot = true, hasSubcategory = true, rootId = item.id });
                                    if (item.subcategories != null)
                                    {
                                        foreach (var item1 in item.subcategories)
                                        {
                                            if (item1.hasSubcategory)
                                            {
                                                searchDisplayings.Add(new SearchDisplaying { id = item1.id, name = item1.name, iconUrl = null, isRoot = false, hasSubcategory = true, rootId = item.id });
                                                if (item1.subcategories != null)
                                                {
                                                    foreach (var item2 in item1.subcategories)
                                                    {
                                                        if (item2.hasSubcategory)
                                                        {
                                                            searchDisplayings.Add(new SearchDisplaying { id = item2.id, name = item2.name, iconUrl = null, isRoot = false, hasSubcategory = true, rootId = item.id });
                                                            if (item2.subcategories != null)
                                                            {
                                                                foreach (var item3 in item2.subcategories)
                                                                {

                                                                    if (item3.subcategories != null)
                                                                    {
                                                                        searchDisplayings.Add(new SearchDisplaying { id = item3.id, name = item3.name, iconUrl = null, isRoot = false, hasSubcategory = true, rootId = item.id });
                                                                        foreach (var item4 in item3.subcategories)
                                                                        {
                                                                            searchDisplayings.Add(new SearchDisplaying { id = item4.id, name = item4.name, iconUrl = null, isRoot = false, hasSubcategory = true, rootId = item.id });
                                                                        }
                                                                    }
                                                                    else
                                                                        searchDisplayings.Add(new SearchDisplaying { id = item3.id, name = item3.name, iconUrl = null, isRoot = false, hasSubcategory = false, rootId = item.id });
                                                                }
                                                            }
                                                        }
                                                        else
                                                            searchDisplayings.Add(new SearchDisplaying { id = item2.id, name = item2.name, iconUrl = null, isRoot = false, hasSubcategory = false, rootId = item.id });
                                                    }
                                                }
                                            }
                                            else
                                                searchDisplayings.Add(new SearchDisplaying { id = item1.id, name = item1.name, iconUrl = null, isRoot = false, hasSubcategory = false, rootId = item.id });
                                        }
                                    }
                                }
                                else
                                    searchDisplayings.Add(new SearchDisplaying { id = item.id, name = item.name, iconUrl = item.iconUrl, isRoot = true, hasSubcategory = false, rootId = item.id });
                            }

                            subCategorySearchAdapter = new SubCategorySearchAdapter(searchDisplayings, this, tf);
                            subCategorySearchAdapter.NotifyDataSetChanged();
                            search_recyclerView.SetAdapter(subCategorySearchAdapter);

                            subCategorySearchAdapter.NotifyDataSetChanged();
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
                        close_searchBn.Visibility = ViewStates.Gone;
                        searchLL.Visibility = ViewStates.Visible;
                    }
                };
                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                var specs = await specializationMethods.GetUpperSpecializations();
                activityIndicator.Visibility = ViewStates.Gone;
                if (specs != "null")
                {
                    var deserialized_specs = JsonConvert.DeserializeObject<UpperSpecializationsRootObject>(specs.ToString());
                    if (!userMethods.UserExists())
                        deserialized_specs.categories.Insert(0, new UpperSpecializations { name = GetString(Resource.String.skip_choosing_specialization) });
                    var upperSpecializationAdapter = new UpperSpecializationAdapter(deserialized_specs.categories, this, tf);

                    recyclerView.SetAdapter(upperSpecializationAdapter);
                }
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
        void showKeyboard()
        {
            inputMethodManager.ShowSoftInput(searchET, ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }
    }
}