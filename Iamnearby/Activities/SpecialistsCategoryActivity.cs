using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Iamnearby.Adapters;
using PCL.Database;
using Iamnearby.Fragments;
using Iamnearby.Methods;
using PCL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Iamnearby.Activities
{
    [Activity(Label = "SpecialistsCategoryActivity", ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustResize | SoftInput.StateHidden/*, Theme = "@style/AppThemeLightNavbar"*/)]
    public class SpecialistsCategoryActivity : Activity
    {
        EditText searchET;
        ProgressBar activityIndicator, activityIndicatorSearch;
        RecyclerView recyclerView, search_recyclerView;
        RecyclerView.LayoutManager layoutManager, search_layoutManager;
        ImageButton close_searchBn;
        LinearLayout searchLL;
        SpecialistsCategorySearchAdapter specialistsCategorySearchAdapter;
        RelativeLayout bottomLayout;
        List<SearchCategory> deserialized_search;
        ImageView nothingIV, searchIV, backIV;
        TextView nothingTV, dialogsTV;
        static LoginRegFragment loginRegFragment;
        static FragmentManager fragmentManager;
        LinearLayout profileLL, dialogsLL, specialistsLL;
        ISharedPreferences dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_dialog;
        UserMethods userMethods = new UserMethods();
        ImageView message_indicatorIV;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.Specialists);
                dialogsTV = FindViewById<TextView>(Resource.Id.dialogsTV);
                message_indicatorIV = FindViewById<ImageView>(Resource.Id.message_indicatorIV);
                loginRegFragment = new LoginRegFragment();
                fragmentManager = this.FragmentManager;
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                searchET = FindViewById<EditText>(Resource.Id.searchET);
                close_searchBn = FindViewById<ImageButton>(Resource.Id.close_searchBn);
                recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicatorSearch = FindViewById<ProgressBar>(Resource.Id.activityIndicatorSearch);
                recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                search_recyclerView = FindViewById<RecyclerView>(Resource.Id.search_recyclerView);
                searchLL = FindViewById<LinearLayout>(Resource.Id.searchLL);
                nothingIV = FindViewById<ImageView>(Resource.Id.nothingIV);
                searchIV = FindViewById<ImageView>(Resource.Id.searchIV);
                backIV = FindViewById<ImageView>(Resource.Id.backIV);
                nothingTV = FindViewById<TextView>(Resource.Id.nothingTV);
                bottomLayout = FindViewById<RelativeLayout>(Resource.Id.bottomLayout);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                activityIndicatorSearch.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                search_layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerView.SetLayoutManager(layoutManager);

                search_recyclerView.SetLayoutManager(search_layoutManager);
                SpecializationMethods specializationMethods = new SpecializationMethods();
                profileLL = FindViewById<LinearLayout>(Resource.Id.profileLL);
                dialogsLL = FindViewById<LinearLayout>(Resource.Id.dialogsLL);

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                searchET.SetTypeface(tf, TypefaceStyle.Normal);
                nothingTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.specialistsTV).SetTypeface(tf, TypefaceStyle.Normal);
                dialogsTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.profileTV).SetTypeface(tf, TypefaceStyle.Normal);

                profileLL.Click += (s, e) =>
                {
                    if (userMethods.UserExists())
                        StartActivity(typeof(UserProfileActivity));
                    else
                        try { loginRegFragment.Show(fragmentManager, "fragmentManager"); }
                        catch { Toast.MakeText(this, GetString(Resource.String.you_not_logined), ToastLength.Short).Show(); StartActivity(typeof(MainActivity)); }
                };
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
                        try { loginRegFragment.Show(fragmentManager, "fragmentManager"); }
                        catch { Toast.MakeText(this, GetString(Resource.String.you_not_logined), ToastLength.Short).Show(); StartActivity(typeof(MainActivity)); }
                };

                var specs = await specializationMethods.GetUpperSpecializations();

                var deserialized_specs = JsonConvert.DeserializeObject<UpperSpecializationsRootObject>(specs.ToString());
                if (!String.IsNullOrEmpty(deserialized_specs.notify_alerts.msg_cnt_new.ToString()) && deserialized_specs.notify_alerts.msg_cnt_new.ToString() != "0")
                {
                    message_indicatorIV.Visibility = ViewStates.Visible;
                    dialogsTV.Text = GetString(Resource.String.dialogs) + " (" + deserialized_specs.notify_alerts.msg_cnt_new + ")";
                }
                else
                {
                    message_indicatorIV.Visibility = ViewStates.Gone;
                    dialogsTV.Text = GetString(Resource.String.dialogs);
                }
                var upperSpecializationAdapter = new LookingForSpecialistsAdapter(deserialized_specs.categories, this, tf);
                upperSpecializationAdapter.NotifyDataSetChanged();
                recyclerView.SetAdapter(upperSpecializationAdapter);
                activityIndicator.Visibility = ViewStates.Gone;

                backIV.Click += (s, e) =>
                  {
                      OnBackPressed();
                  };
                searchET.TextChanged += async (s, e) =>
                  {
                      if (!String.IsNullOrEmpty(searchET.Text))
                      {
                          backIV.Visibility = ViewStates.Visible;
                          searchIV.Visibility = ViewStates.Gone;
                          nothingIV.Visibility = ViewStates.Gone;
                          nothingTV.Visibility = ViewStates.Gone;
                          searchLL.Visibility = ViewStates.Visible;
                          close_searchBn.Visibility = ViewStates.Visible;
                          activityIndicatorSearch.Visibility = ViewStates.Visible;
                          search_recyclerView.Visibility = ViewStates.Gone;
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
                          backIV.Visibility = ViewStates.Gone;
                          searchIV.Visibility = ViewStates.Visible;
                          close_searchBn.Visibility = ViewStates.Gone;
                          searchLL.Visibility = ViewStates.Visible;
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
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
        bool we_here;
        protected override void OnPause()
        {
            base.OnPause();
            we_here = false;
        }
        protected override async void OnResume()
        {
            base.OnResume();
            bottomLayout.Click += (s, e) => { };
            int bottom_paddingInDp = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 58.5F, Resources.DisplayMetrics);
            recyclerView.SetPadding(0, 0, 0, bottom_paddingInDp);
            we_here = true;
            while (we_here)
            {
                await Task.Delay(500);
                recyclerView.LayoutChange += async (s, e) =>
                {

                    if (e.OldBottom > e.Bottom)
                    {
                        bottomLayout.Visibility = ViewStates.Invisible;
                        recyclerView.SetPadding(0, 0, 0, 0);
                    }
                    else
                    {
                        bottomLayout.Visibility = ViewStates.Visible;
                        recyclerView.SetPadding(0, 0, 0, bottom_paddingInDp);
                    }
                };
            }
        }

    }
}