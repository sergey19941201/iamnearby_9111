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
using System.Threading.Tasks;

namespace Iamnearby.Activities
{
    [Activity(Label = "SubCategoryActivity", ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden)]
    public class SubCategoryActivity : Activity
    {
        List<SubCategory> deserialized_sub_categs;
        RelativeLayout backRelativeLayout;
        LinearLayout searchLL;
        Button searchBn;
        ImageButton back_button, close_searchBn;
        static ProgressBar activityIndicator;
        static ProgressBar activityIndicatorSearch;
        static RecyclerView recyclerView, search_recyclerView;
        RecyclerView.LayoutManager layoutManager, search_layoutManager;
        ImageView nothingIV;
        EditText searchET;
        Button applyBn;
        TextView headerTV, nothingTV;
        List<SearchCategory> deserialized_search;
        SubCategorySearchAdapter subCategorySearchAdapter;
        InputMethodManager inputMethodManager;
        public static List<string> my_specializations;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("categories_data", FileCreationMode.Private);
        static ProfileAndExpertMethods profileAndExpertMethods = new ProfileAndExpertMethods();
        UserMethods userMethods = new UserMethods();
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.YourSpecializationCheckBox);
                inputMethodManager = Application.GetSystemService(Context.InputMethodService) as InputMethodManager;
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                searchET = FindViewById<EditText>(Resource.Id.searchET);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicatorSearch = FindViewById<ProgressBar>(Resource.Id.activityIndicatorSearch);
                searchLL = FindViewById<LinearLayout>(Resource.Id.searchLL);
                search_recyclerView = FindViewById<RecyclerView>(Resource.Id.search_recyclerView);
                nothingTV = FindViewById<TextView>(Resource.Id.nothingTV);
                nothingIV = FindViewById<ImageView>(Resource.Id.nothingIV);
                headerTV = FindViewById<TextView>(Resource.Id.headerTV);
                searchBn = FindViewById<Button>(Resource.Id.searchBn);
                applyBn = FindViewById<Button>(Resource.Id.applyBn);
                close_searchBn = FindViewById<ImageButton>(Resource.Id.close_searchBn);
                recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                activityIndicatorSearch.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                search_layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                search_recyclerView.SetLayoutManager(search_layoutManager);
                recyclerView.SetLayoutManager(layoutManager);

                SubCategoryAdapter.checked_positions.Clear();

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                headerTV.SetTypeface(tf, TypefaceStyle.Bold);
                searchET.SetTypeface(tf, TypefaceStyle.Normal);
                searchBn.SetTypeface(tf, TypefaceStyle.Normal);
                nothingTV.SetTypeface(tf, TypefaceStyle.Normal);
                applyBn.SetTypeface(tf, TypefaceStyle.Normal);

                SpecializationMethods specializationMethods = new SpecializationMethods();

                searchET.Visibility = ViewStates.Gone;
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
                headerTV.Text = pref.GetString("spec_name", String.Empty);

                var sub_categs = await specializationMethods.GetSubCategories(pref.GetString("spec_id", String.Empty));

                activityIndicator.Visibility = ViewStates.Gone;
                if (sub_categs != "null")
                {
                    var deserObj = JsonConvert.DeserializeObject<SubCategoryRootObject>(sub_categs.ToString());
                    deserialized_sub_categs = deserObj.subcategories;
                    deserialized_sub_categs.Add(new SubCategory { id = "-1", name = GetString(Resource.String.other) });

                    var subCategoryAdapter = new SubCategoryAdapter(deserialized_sub_categs, this, tf);

                    recyclerView.SetAdapter(subCategoryAdapter);
                }
                applyBn.Click += async (s, e) =>
                {
                    foreach (var index in SubCategoryAdapter.checked_positions)
                    {

                        var kjg = deserialized_sub_categs[index];
                    }
                    if (!userMethods.UserExists())
                    {
                        foreach (var index in SubCategoryAdapter.checked_positions)
                        {
                            if (!SubCategoryAdapter.my_specializations_static.Contains(deserialized_sub_categs[index]))
                                SubCategoryAdapter.my_specializations_static.Add(deserialized_sub_categs[index]);
                        }
                        StartActivity(new Intent(this, typeof(AddSpecializationActivity)));
                    }
                    else
                    {

                        foreach (var index in SubCategoryAdapter.checked_positions)
                        {
                            var res = await SubCategoryActivity.show_activity(deserialized_sub_categs[index].id, userMethods.GetUsersAuthToken());
                        }

                        StartActivity(new Intent(this, typeof(UserProfileActivity)));
                    }
                };
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
        public static async Task<bool> show_activity(string categId, string auth_token)
        {
            try
            {
                if (search_recyclerView.Visibility == ViewStates.Visible)
                    activityIndicatorSearch.Visibility = ViewStates.Visible;
                if (recyclerView.Visibility == ViewStates.Visible)
                    activityIndicator.Visibility = ViewStates.Visible;
                recyclerView.Visibility = ViewStates.Gone;
                search_recyclerView.Visibility = ViewStates.Gone;
            }
            catch { }

            var res = await ProfileAndExpertMethods.EditSpecializationStatic(auth_token, categId);
            return true;
        }
    }
}