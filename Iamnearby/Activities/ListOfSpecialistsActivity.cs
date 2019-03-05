using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Com.Orangegangsters.Github.Swipyrefreshlayout.Library;
using Iamnearby.Adapters;
using Iamnearby.Fragments;
using Iamnearby.Methods;
using Newtonsoft.Json;
using PCL.Database;
using PCL.Models;

namespace Iamnearby.Activities
{
    // SoftInput.StateHidden attribute hides keyboard
    [Activity(Label = "ListOfSpecialistsActivity", ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden)]
    public class ListOfSpecialistsActivity : Activity
    {
        int offset { get; set; }
        ListOfSpecialistsAdapter listOfSpecialistsAdapter;
        RelativeLayout backRelativeLayout, bottomLayout, upper_layout, linearLayout7644;
        LinearLayout tintLL, searchLL;
        ImageButton back_button, close_searchBn;
        Button dropdownBn, searchBn, show_on_mapBn, sortBn, filterBn;
        TextView headerTV, typesTV;
        ProgressBar activityIndicator, activityIndicatorSearch;
        RecyclerView recyclerView, recyclerViewDropdown, search_recyclerView;
        RecyclerView.LayoutManager layoutManager, layoutManagerDropdown, search_layoutManager;
        EditText searchET;
        //sorting elements
        LinearLayout sortLL;
        RelativeLayout by_distanceLL, by_ratingLL, navbarLL;
        ImageView by_distanceIV, by_ratingIV, nothingIV;
        TextView by_distanceTV, by_ratingTV, nothingTV, dialogsTV;
        //sorting elements ENDED
        List<SearchCategory> deserialized_search;
        SpecialistsCategorySearchAdapter specialistsCategorySearchAdapter;
        InputMethodManager inputMethodManager;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
        ISharedPreferences expert_data;
        ISharedPreferencesEditor edit_expert;
        bool dropdown_closed;
        static LoginRegFragment loginRegFragment;
        static FragmentManager fragmentManager;
        LinearLayout profileLL, dialogsLL, specialistsLL;
        UserMethods userMethods = new UserMethods();
        ISharedPreferences dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_dialog;
        SwipyRefreshLayout refresher;
        List<SubCategory> deserialized_sub_categs;
        ImageView message_indicatorIV;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.ListOfSpecialists);
                dialogsTV = FindViewById<TextView>(Resource.Id.dialogsTV);
                message_indicatorIV = FindViewById<ImageView>(Resource.Id.message_indicatorIV);
                inputMethodManager = Application.GetSystemService(Context.InputMethodService) as InputMethodManager;
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                searchET = FindViewById<EditText>(Resource.Id.searchET);
                expert_data = Application.Context.GetSharedPreferences("experts", FileCreationMode.Private);
                edit_expert = expert_data.Edit();
                loginRegFragment = new LoginRegFragment();
                fragmentManager = this.FragmentManager;
                offset = 100;
                headerTV = FindViewById<TextView>(Resource.Id.headerTV);
                typesTV = FindViewById<TextView>(Resource.Id.typesTV);
                headerTV.Text = expert_data.GetString("spec_name", String.Empty); ;
                typesTV.Text = expert_data.GetString("spec_type", String.Empty);
                recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                recyclerViewDropdown = FindViewById<RecyclerView>(Resource.Id.recyclerViewDropdown);
                linearLayout7644 = FindViewById<RelativeLayout>(Resource.Id.linearLayout7644);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicatorSearch = FindViewById<ProgressBar>(Resource.Id.activityIndicatorSearch);
                nothingIV = FindViewById<ImageView>(Resource.Id.nothingIV);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                nothingTV = FindViewById<TextView>(Resource.Id.nothingTV);
                searchLL = FindViewById<LinearLayout>(Resource.Id.searchLL);
                search_recyclerView = FindViewById<RecyclerView>(Resource.Id.search_recyclerView);
                bottomLayout = FindViewById<RelativeLayout>(Resource.Id.bottomLayout);
                upper_layout = FindViewById<RelativeLayout>(Resource.Id.upper_layout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                dropdownBn = FindViewById<Button>(Resource.Id.dropdownBn);
                searchBn = FindViewById<Button>(Resource.Id.searchBn);
                sortBn = FindViewById<Button>(Resource.Id.sortBn);
                filterBn = FindViewById<Button>(Resource.Id.filterBn);
                close_searchBn = FindViewById<ImageButton>(Resource.Id.close_searchBn);
                navbarLL = FindViewById<RelativeLayout>(Resource.Id.navbarLL);
                tintLL = FindViewById<LinearLayout>(Resource.Id.tintLL);
                //sorting
                sortLL = FindViewById<LinearLayout>(Resource.Id.sortLL);
                by_distanceLL = FindViewById<RelativeLayout>(Resource.Id.by_distanceLL);
                by_ratingLL = FindViewById<RelativeLayout>(Resource.Id.by_ratingLL);
                by_distanceIV = FindViewById<ImageView>(Resource.Id.by_distanceIV);
                by_ratingIV = FindViewById<ImageView>(Resource.Id.by_ratingIV);
                by_distanceTV = FindViewById<TextView>(Resource.Id.by_distanceTV);
                by_ratingTV = FindViewById<TextView>(Resource.Id.by_ratingTV);
                //sorting ENDED
                show_on_mapBn = FindViewById<Button>(Resource.Id.show_on_mapBn);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                activityIndicatorSearch.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                layoutManagerDropdown = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                search_layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerViewDropdown.SetLayoutManager(layoutManagerDropdown);
                search_recyclerView.SetLayoutManager(search_layoutManager);
                SpecializationMethods specializationMethods = new SpecializationMethods();

                profileLL = FindViewById<LinearLayout>(Resource.Id.profileLL);
                dialogsLL = FindViewById<LinearLayout>(Resource.Id.dialogsLL);

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                headerTV.SetTypeface(tf, TypefaceStyle.Bold);
                typesTV.SetTypeface(tf, TypefaceStyle.Normal);
                dropdownBn.SetTypeface(tf, TypefaceStyle.Normal);
                searchET.SetTypeface(tf, TypefaceStyle.Normal);
                searchBn.SetTypeface(tf, TypefaceStyle.Normal);
                show_on_mapBn.SetTypeface(tf, TypefaceStyle.Bold);
                filterBn.SetTypeface(tf, TypefaceStyle.Normal);
                sortBn.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.specialistsTV).SetTypeface(tf, TypefaceStyle.Normal);
                dialogsTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.profileTV).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textVsiew1).SetTypeface(tf, TypefaceStyle.Normal);
                by_distanceTV.SetTypeface(tf, TypefaceStyle.Normal);
                by_ratingTV.SetTypeface(tf, TypefaceStyle.Normal);
                nothingTV.SetTypeface(tf, TypefaceStyle.Normal);

                dialogsLL.Click += (s, e) =>
                {
                    if (userMethods.UserExists())
                    {
                        edit_dialog = dialog_data.Edit();
                        edit_dialog.PutString("come_from", "Came directly from bottom");
                        edit_dialog.Apply();
                        StartActivity(typeof(ChatListActivity));
                    }
                    else
                        showFragment();
                };
                profileLL.Click += (s, e) =>
                {
                    if (userMethods.UserExists())
                        StartActivity(typeof(UserProfileActivity));
                    else
                        showFragment();
                };

                searchET.Visibility = ViewStates.Gone;
                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };


                searchBn.Click += (s, e) =>
                {
                    close_searchBn.Visibility = ViewStates.Visible;
                    searchET.Visibility = ViewStates.Visible;
                    searchBn.Visibility = ViewStates.Gone;
                    dropdownBn.Visibility = ViewStates.Gone;
                    headerTV.Visibility = ViewStates.Gone;
                    typesTV.Visibility = ViewStates.Gone;

                    searchET.RequestFocus();
                    showKeyboard();
                };


                sortBn.Click += (s, e) =>
                  {
                      tintLL.Visibility = ViewStates.Visible;
                      sortLL.Visibility = ViewStates.Visible;
                  };

                by_distanceLL.Click += By_distance_Click;
                by_distanceIV.Click += By_distance_Click;
                by_distanceTV.Click += By_distance_Click;
                by_ratingLL.Click += By_rating_Click;
                by_ratingIV.Click += By_rating_Click;
                by_ratingTV.Click += By_rating_Click;

                if (expert_data.GetInt("sort_meth", 1) == 1)
                {
                    by_distanceIV.Visibility = ViewStates.Visible;
                    by_ratingIV.Visibility = ViewStates.Gone;
                }
                else
                {
                    by_distanceIV.Visibility = ViewStates.Gone;
                    by_ratingIV.Visibility = ViewStates.Visible;
                }




                var sort_type = expert_data.GetInt("sort_meth", 1);
                var specs = await specializationMethods.ExpertsList(
                    expert_data.GetString("spec_id", String.Empty),
                    pref.GetString("latitude", String.Empty),
                    pref.GetString("longitude", String.Empty),
                    expert_data.GetInt("sort_meth", 1),
                    expert_data.GetString("expert_city_id", String.Empty),
                    expert_data.GetString("distance_radius", String.Empty),
                    expert_data.GetBoolean("has_reviews", false)//, this
                    );
                activityIndicator.Visibility = ViewStates.Gone;
                try
                {
                    if (specs != "null" && !String.IsNullOrEmpty(specs))
                    {
                        var deserialized_experts = JsonConvert.DeserializeObject<RootObjectExpert>(specs.ToString());
                        if (!String.IsNullOrEmpty(deserialized_experts.notify_alerts.msg_cnt_new.ToString()) && deserialized_experts.notify_alerts.msg_cnt_new.ToString() != "0")
                        {
                            message_indicatorIV.Visibility = ViewStates.Visible;
                            dialogsTV.Text = GetString(Resource.String.dialogs) + " (" + deserialized_experts.notify_alerts.msg_cnt_new + ")";
                        }
                        else
                        {
                            message_indicatorIV.Visibility = ViewStates.Gone;
                            dialogsTV.Text = GetString(Resource.String.dialogs);
                        }
                        listOfSpecialistsAdapter = new ListOfSpecialistsAdapter(deserialized_experts.experts, this, tf);
                        recyclerView.SetAdapter(listOfSpecialistsAdapter);
                        //Toast.MakeText(this, "десериализовано. адаптер задан", ToastLength.Short).Show();
                    }
                }
                catch { }

                show_on_mapBn.Click += (s, e) =>
                {
                    edit_expert.PutString("latitude", pref.GetString("latitude", String.Empty));
                    edit_expert.PutString("longitude", pref.GetString("longitude", String.Empty));
                    edit_expert.PutString("spec_id", expert_data.GetString("spec_id", String.Empty));
                    edit_expert.PutString("specs", specs);
                    edit_expert.PutString("spec_name", expert_data.GetString("spec_name", String.Empty));
                    edit_expert.PutString("spec_type", expert_data.GetString("spec_type", String.Empty));
                    edit_expert.PutBoolean("has_subcategory", expert_data.GetBoolean("has_subcategory", false));
                    edit_expert.Apply();
                    StartActivity(typeof(SpecialistsOnMapActivity));
                };

                dropdown_closed = true;
                dropdownBn.Click += (s, e) =>
                    dropdownClick();
                linearLayout7644.Click += (s, e) =>
                {
                    if (dropdownBn.Visibility == ViewStates.Visible)
                        dropdownClick();
                };
                bool types_visible = false;

                if (typesTV.Visibility == ViewStates.Visible)
                    types_visible = true;

                ScrollDownDetector downDetector = new ScrollDownDetector();
                ScrollUpDetector upDetector = new ScrollUpDetector();
                var margin_top_value = navbarLL.LayoutParameters.Height;
                LinearLayout.LayoutParams ll_down = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                LinearLayout.LayoutParams ll_up = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                ll_down.SetMargins(0, 0, 0, 0);
                ll_up.SetMargins(0, margin_top_value, 0, 0);
                System.Timers.Timer timer = new System.Timers.Timer();
                System.Timers.Timer timer2 = null;
                downDetector.Action = () =>
                {
                    if (dropdown_closed)
                    {
                        timer = new System.Timers.Timer();
                        timer.Interval = 500;
                        timer.Start();
                        timer.Elapsed += (s, e) =>
                          {
                              timer.Stop();
                              timer = null;
                          };
                        if (timer2 == null)
                        {
                            upper_layout.LayoutParameters = ll_down;
                        }
                    }
                };
                upDetector.Action = () =>
                {
                    timer2 = new System.Timers.Timer();
                    if (dropdown_closed)
                    {
                        if (timer == null)
                        {
                            timer2.Interval = 500;
                            timer2.Start();
                            timer2.Elapsed += (s, e) =>
                            {
                                timer2.Stop();
                                timer2 = null;
                            };
                            upper_layout.LayoutParameters = ll_up;
                        }
                    }
                };
                recyclerView.AddOnScrollListener(downDetector);
                recyclerView.AddOnScrollListener(upDetector);
                filterBn.Click += (s, e) =>
                  {
                      StartActivity(typeof(FilterActivity));
                  };

                searchET.EditorAction += (object sender, EditText.EditorActionEventArgs e) =>
                 {
                     imm.HideSoftInputFromWindow(searchET.WindowToken, 0);
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
                        //try
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

                            specialistsCategorySearchAdapter = new SpecialistsCategorySearchAdapter(searchDisplayings, this, tf);
                            specialistsCategorySearchAdapter.NotifyDataSetChanged();
                            search_recyclerView.SetAdapter(specialistsCategorySearchAdapter);

                            specialistsCategorySearchAdapter.NotifyDataSetChanged();
                            nothingIV.Visibility = ViewStates.Gone;
                            nothingTV.Visibility = ViewStates.Gone;
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
                        close_searchBn.Visibility = ViewStates.Gone;
                        searchLL.Visibility = ViewStates.Visible;
                        searchLL.Visibility = ViewStates.Gone;
                    }
                };
                tintLL.Click += (s, e) =>
                {
                    tintLL.Visibility = ViewStates.Gone;
                    recyclerViewDropdown.Visibility = ViewStates.Gone;
                    tintLL.Visibility = ViewStates.Gone;
                    dropdown_closed = true;
                    sortLL.Visibility = ViewStates.Gone;
                    dropdownBn.SetBackgroundResource(Resource.Drawable.dropdown);
                };
                refresher = FindViewById<SwipyRefreshLayout>(Resource.Id.refresher);
                refresher.SetColorScheme(Resource.Color.lightBlueColor,
                                          Resource.Color.buttonBackgroundColor);
                refresher.Direction = SwipyRefreshLayoutDirection.Bottom;

                refresher.Refresh += async delegate
                {
                    var specs_updated = await specializationMethods.ExpertsList(
                    expert_data.GetString("spec_id", String.Empty),
                    pref.GetString("latitude", String.Empty),
                    pref.GetString("longitude", String.Empty),
                    expert_data.GetInt("sort_meth", 1),
                    expert_data.GetString("expert_city_id", String.Empty),
                    expert_data.GetString("distance_radius", String.Empty),
                    expert_data.GetBoolean("has_reviews", false),//, this
                    offset
                    );
                    try
                    {
                        if (!String.IsNullOrEmpty(specs_updated) && specs_updated.Length > 10)
                        {
                            var deserialized_updated_specs = JsonConvert.DeserializeObject<RootObjectExpert>(specs_updated);
                            ListOfSpecialistsAdapter.experts_static.InsertRange(offset, deserialized_updated_specs.experts);
                            if (!String.IsNullOrEmpty(deserialized_updated_specs.notify_alerts.msg_cnt_new.ToString()) && deserialized_updated_specs.notify_alerts.msg_cnt_new.ToString() != "0")
                            {
                                message_indicatorIV.Visibility = ViewStates.Visible;
                                dialogsTV.Text = GetString(Resource.String.dialogs) + " (" + deserialized_updated_specs.notify_alerts.msg_cnt_new + ")";
                            }
                            else
                            {
                                message_indicatorIV.Visibility = ViewStates.Gone;
                                dialogsTV.Text = GetString(Resource.String.dialogs);
                            }
                            listOfSpecialistsAdapter.NotifyDataSetChanged();
                            recyclerView.SmoothScrollToPosition(offset);

                            offset += 100;
                        }
                    }
                    catch { }
                    refresher.Refreshing = false;
                };
                //checking if category has subcategories to load them
                if (expert_data.GetBoolean("has_subcategory", true))
                {
                    var sub_categs = await specializationMethods.GetSubCategories(expert_data.GetString("spec_id", String.Empty));
                    var deserObj = JsonConvert.DeserializeObject<SubCategoryRootObject>(sub_categs.ToString());
                    if (!String.IsNullOrEmpty(deserObj.notify_alerts.msg_cnt_new.ToString()) && deserObj.notify_alerts.msg_cnt_new.ToString() != "0")
                    {
                        message_indicatorIV.Visibility = ViewStates.Visible;
                        dialogsTV.Text = GetString(Resource.String.dialogs) + " (" + deserObj.notify_alerts.msg_cnt_new + ")";
                    }
                    else
                    {
                        message_indicatorIV.Visibility = ViewStates.Gone;
                        dialogsTV.Text = GetString(Resource.String.dialogs);
                    }
                    deserialized_sub_categs = deserObj.subcategories;
                    if (deserialized_sub_categs == null)
                        deserialized_sub_categs = new List<SubCategory>();
                    deserialized_sub_categs.Insert(0, new SubCategory { id = "-1", name = GetString(Resource.String.all_subcategs) });
                    var dropDownSubcategsAdapter = new DropDownSubcategsAdapter(deserialized_sub_categs, this, tf);
                    recyclerViewDropdown.SetAdapter(dropDownSubcategsAdapter);
                    dropdownBn.Visibility = ViewStates.Visible;
                    typesTV.Visibility = ViewStates.Visible;
                }
                else
                    dropdownBn.Visibility = ViewStates.Gone;
                bool was_drop_visible = false;
                if (dropdownBn.Visibility == ViewStates.Visible)
                    was_drop_visible = true;
                close_searchBn.Click += (s, e) =>
                {
                    searchET.Text = null;
                    imm.HideSoftInputFromWindow(searchET.WindowToken, 0);
                    searchLL.Visibility = ViewStates.Gone;
                    if (types_visible)
                        typesTV.Visibility = ViewStates.Visible;
                    if (was_drop_visible)
                        dropdownBn.Visibility = ViewStates.Visible;
                    headerTV.Visibility = ViewStates.Visible;
                    searchET.Visibility = ViewStates.Gone;
                    close_searchBn.Visibility = ViewStates.Gone;
                    searchBn.Visibility = ViewStates.Visible;
                };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
        public static void showFragment()
        {
            try { loginRegFragment.Show(fragmentManager, "fragmentManager"); }
            catch { }
        }
        public override void OnBackPressed()
        {
            if (tintLL.Visibility == ViewStates.Visible)
            {
                tintLL.Visibility = ViewStates.Gone;
                recyclerViewDropdown.Visibility = ViewStates.Gone;
                tintLL.Visibility = ViewStates.Gone;
                dropdown_closed = true;
                sortLL.Visibility = ViewStates.Gone;
                dropdownBn.SetBackgroundResource(Resource.Drawable.dropdown);
            }
            else
                base.OnBackPressed();
        }
        void dropdownClick()
        {
            sortLL.Visibility = ViewStates.Gone;
            if (!dropdown_closed)
            {
                dropdownBn.SetBackgroundResource(Resource.Drawable.dropdown);
                recyclerViewDropdown.Visibility = ViewStates.Gone;
                tintLL.Visibility = ViewStates.Gone;
                dropdown_closed = true;
            }
            else
            {
                dropdownBn.SetBackgroundResource(Resource.Drawable.dropdown_close);
                recyclerViewDropdown.Visibility = ViewStates.Visible;
                tintLL.Visibility = ViewStates.Visible;
                dropdown_closed = false;
            }
        }
        private void By_rating_Click(object sender, EventArgs e)
        {
            by_ratingIV.Visibility = ViewStates.Visible;
            by_distanceIV.Visibility = ViewStates.Gone;
            edit_expert.PutInt("sort_meth", 0);
            edit_expert.Apply();
            StartActivity(typeof(ListOfSpecialistsActivity));
        }

        private void By_distance_Click(object sender, EventArgs e)
        {
            by_distanceIV.Visibility = ViewStates.Visible;
            by_ratingIV.Visibility = ViewStates.Gone;
            edit_expert.PutInt("sort_meth", 1);
            edit_expert.Apply();
            StartActivity(typeof(ListOfSpecialistsActivity));
        }
        void showKeyboard()
        {
            inputMethodManager.ShowSoftInput(searchET, ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }
    }
}