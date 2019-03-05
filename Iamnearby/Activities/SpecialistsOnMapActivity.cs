using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Com.Bumptech.Glide;
using Iamnearby.Adapters;
using PCL.Database;
using Iamnearby.Fragments;
using Iamnearby.Methods;
using PCL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using static Android.Gms.Maps.GoogleMap;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace Iamnearby.Activities
{
    //SoftInput.StateHidden attribute hides keyboard
    [Activity(Label = "SpecialistsOnMapActivity", ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden)]
    public class SpecialistsOnMapActivity : Activity, IOnMapReadyCallback, IOnCameraIdleListener
    {
        RelativeLayout backRelativeLayout, bottomLayout, upper_layout, expert_data_layoutRL;
        LinearLayout tintLL, mapLL, searchLL, tint_expertLL;
        TextView expert_nameTV, phoneTV, rating_valueTV, distanceTV;
        ImageView expert_imageIV, onlineIV, star1IV, star2IV, star3IV, star4IV, star5IV, distanceIV;
        ImageButton close_searchBn;
        Button writeBn, sortBn, filterBn;
        ImageButton back_button;
        Button dropdownBn, searchBn, show_listBn;
        TextView headerTV, typesTV;
        ProgressBar activityIndicator, activityIndicatorSearch;
        RecyclerView recyclerViewDropdown, search_recyclerView;
        RecyclerView.LayoutManager layoutManagerDropdown, search_layoutManager;
        EditText searchET;
        //sorting elements
        LinearLayout sortLL;
        RelativeLayout by_distanceLL, by_ratingLL, navbarLL, linearLayout7644;
        ImageView by_distanceIV, by_ratingIV, nothingIV;
        TextView by_distanceTV, by_ratingTV, nothingTV, dialogsTV;
        //sorting elements ENDED
        List<SearchCategory> deserialized_search;
        SpecialistsOnMapCategorySearchAdapter specialistsOnMapCategorySearchAdapter;
        InputMethodManager inputMethodManager;
        private GoogleMap _map;
        private MapFragment _mapFragment;
        private CameraUpdate cameraUpdate;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
        ISharedPreferences expert_data;
        ISharedPreferencesEditor edit_expert;
        ISharedPreferences dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_dialog;
        //ISharedPreferences loc_pref;
        ISharedPreferencesEditor edit;
        BitmapDescriptor expert_offline_marker, expert_online_marker, expert_clicked_marker, user_marker;
        LinearLayout profileLL, dialogsLL, specialistsLL;
        static LoginRegFragment loginRegFragment;
        static FragmentManager fragmentManager;
        UserMethods userMethods = new UserMethods();
        Marker prevMarker;
        List<Expert> deserialized_experts;
        bool dropdown_closed;
        List<SubCategory> deserialized_sub_categs;
        ImageView message_indicatorIV;
        SpecializationMethods specializationMethods = new SpecializationMethods();
        LatLng expert_position;
        LatLng clicked_expert_position;
        LatLng my_position;
        MarkerOptions my_marker;
        bool first_zoom;

        public async void OnMapReady(GoogleMap map)
        {
            edit = pref.Edit();
            _map = map;
            if (_map != null)
            {
                _map.UiSettings.ZoomControlsEnabled = false;
                _map.UiSettings.CompassEnabled = false;
                _map.MoveCamera(cameraUpdate);
                _map.SetMaxZoomPreference(17f);
                _map.CameraIdle += (s, e) => OnCameraIdle();
                var locator = CrossGeolocator.Current;
                try
                {
                    await locator.StartListeningAsync(TimeSpan.FromSeconds(30), 20);
                }
                catch { }
                locator.PositionChanged += (sender, e) =>
                {
                    // Position position=new Position();
                    var position = e.Position as Position;
                    edit.PutString("latitude", Convert.ToDouble(position.Latitude/*, (CultureInfo.InvariantCulture)*/).ToString());
                    edit.PutString("longitude", Convert.ToDouble(position.Longitude/*, (CultureInfo.InvariantCulture)*/).ToString());
                    edit.Apply();
                    my_position = new LatLng(
                               Convert.ToDouble(pref.GetString("latitude", String.Empty)/*, (CultureInfo.InvariantCulture)*/),
                               Convert.ToDouble(pref.GetString("longitude", String.Empty)/*, (CultureInfo.InvariantCulture)*/));

                    //OnCameraIdle();
                };
                _map.MarkerClick += (s, e) =>
                  {
                      var lat_clicked = e.Marker.Position.Latitude;
                      var lng_clicked = e.Marker.Position.Longitude;
                      map.Clear();
                      my_marker.SetPosition(my_position);
                      my_marker.SetIcon(user_marker);
                      _map.AddMarker(my_marker);
                      foreach (var expert in deserialized_experts)
                      {
                          if (lat_clicked == Convert.ToDouble(expert.coordinates.latitude, CultureInfo.InvariantCulture) &&
                          lng_clicked == Convert.ToDouble(expert.coordinates.longitude, CultureInfo.InvariantCulture))
                          {
                              clicked_expert_position = (new LatLng(
                                    Convert.ToDouble(expert.coordinates.latitude, (CultureInfo.InvariantCulture)),
                                    Convert.ToDouble(expert.coordinates.longitude, (CultureInfo.InvariantCulture))));
                              edit_expert.PutString("expert_id", expert.id);
                              edit_expert.Apply();
                              edit_dialog = dialog_data.Edit();
                              edit_dialog.PutString("expert_id", expert.id);
                              edit_dialog.PutString("expert_name", expert.name);
                              edit_dialog.PutString("expert_avatar", expert.avatarUrl);
                              edit_dialog.Apply();

                              star1IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                              star2IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                              star3IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                              star4IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                              star5IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                              tint_expertLL.Visibility = ViewStates.Visible;
                              expert_data_layoutRL.Visibility = ViewStates.Visible;
                              if (expert.online)
                                  onlineIV.Visibility = ViewStates.Visible;
                              else
                                  onlineIV.Visibility = ViewStates.Gone;
                              expert_imageIV = null;
                              expert_imageIV = FindViewById<ImageView>(Resource.Id.expert_imageIV);

                              Thread backgroundThread = new Thread(new ThreadStart(() =>
                              {
                                  Glide.Get(Application.Context).ClearDiskCache();
                              }));
                              backgroundThread.IsBackground = true;
                              backgroundThread.Start();
                              Glide.Get(this).ClearMemory();
                              if (!String.IsNullOrEmpty(expert.avatarUrl))
                                  Glide.With(Application.Context)
                                      .Load("https://api.iamnearby.net/" + expert.avatarUrl)
                                      //.Placeholder(Resource.Drawable.job_placeholder)
                                      .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
                                      .SkipMemoryCache(true))
                                      .Into(expert_imageIV);
                              if (String.IsNullOrEmpty(expert.avatarUrl))
                              {
                                  Glide.With(Application.Context)
                                      .Load(Resource.Drawable.photo_placeholder)
                                      //.Placeholder(Resource.Drawable.job_placeholder)
                                      .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
                                      .SkipMemoryCache(true))
                                      .Into(expert_imageIV);
                              }
                              expert_nameTV.Text = expert.name;
                              phoneTV.Text = expert.phone;
                              double distance_in_km;
                              bool distance_in_km_parsed = Double.TryParse(expert.distance, out distance_in_km);
                              if (distance_in_km > 999)
                              {
                                  distance_in_km = distance_in_km / 1000;
                                  var final_dist = Math.Round(distance_in_km, 2).ToString().Replace(',', '.');
                                  distanceTV.Text = final_dist + " км";
                              }
                              else
                                  distanceTV.Text = expert.distance + " м";
                              if (distance_in_km == 0)
                              {
                                  distanceTV.Visibility = ViewStates.Gone;
                                  distanceIV.Visibility = ViewStates.Gone;
                              }

                              if (expert.rating.Length > 3)
                                  rating_valueTV.Text = expert.rating.ToString().Substring(0, 3);
                              double rating_value = 0;
                              try
                              {
                                  rating_value = Convert.ToDouble(expert.rating, (CultureInfo.InvariantCulture));
                                  if (rating_value >= 1)
                                  {
                                      star1IV.SetBackgroundResource(Resource.Drawable.active_star);
                                      if (rating_value >= 2)
                                      {
                                          star2IV.SetBackgroundResource(Resource.Drawable.active_star);
                                          if (rating_value >= 3)
                                          {
                                              star3IV.SetBackgroundResource(Resource.Drawable.active_star);
                                              if (rating_value >= 4)
                                              {
                                                  star4IV.SetBackgroundResource(Resource.Drawable.active_star);
                                                  if (rating_value >= 5)
                                                  {
                                                      star5IV.SetBackgroundResource(Resource.Drawable.active_star);
                                                  }
                                              }
                                          }
                                      }
                                  }
                              }
                              catch { }
                              //extra chech needs to be here to prevent bug with displaying active stars
                              if (String.IsNullOrEmpty(rating_value.ToString()) || rating_value == 0)
                              {
                                  rating_valueTV.Text = "0";
                                  star1IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                                  star2IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                                  star3IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                                  star4IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                                  star5IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                              }
                              if (!String.IsNullOrWhiteSpace(expert.coordinates.latitude) && !String.IsNullOrWhiteSpace(expert.coordinates.longitude))
                              {
                                  MarkerOptions expert_marker = new MarkerOptions();
                                  expert_marker.SetPosition(new LatLng(
                                      Convert.ToDouble(expert.coordinates.latitude, (CultureInfo.InvariantCulture)),
                                      Convert.ToDouble(expert.coordinates.longitude, (CultureInfo.InvariantCulture))));

                                  expert_marker.SetIcon(expert_clicked_marker);

                                  _map.AddMarker(expert_marker);
                              }
                          }
                          else
                          {
                              if (!String.IsNullOrWhiteSpace(expert.coordinates.latitude) && !String.IsNullOrWhiteSpace(expert.coordinates.longitude))
                              {
                                  MarkerOptions expert_marker = new MarkerOptions();
                                  expert_marker.SetPosition(new LatLng(
                                      Convert.ToDouble(expert.coordinates.latitude, (CultureInfo.InvariantCulture)),
                                      Convert.ToDouble(expert.coordinates.longitude, (CultureInfo.InvariantCulture))));
                                  if (expert.online)
                                  {
                                      expert_marker.SetIcon(expert_online_marker);
                                      //set this to mark marker that it is online
                                      expert_marker.SetRotation(0.00001F);
                                  }
                                  else
                                  {
                                      expert_marker.SetIcon(expert_offline_marker);
                                  }
                                  _map.AddMarker(expert_marker);
                              }
                          }
                      }
                  };
            }
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                first_zoom = true;
                SetContentView(Resource.Layout.SpecialistsOnMap);
                dialogsTV = FindViewById<TextView>(Resource.Id.dialogsTV);
                message_indicatorIV = FindViewById<ImageView>(Resource.Id.message_indicatorIV);
                distanceIV = FindViewById<ImageView>(Resource.Id.distanceIV);

                inputMethodManager = Application.GetSystemService(Context.InputMethodService) as InputMethodManager;
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                expert_data = Application.Context.GetSharedPreferences("experts", FileCreationMode.Private);
                edit_expert = expert_data.Edit();

                expert_offline_marker = BitmapDescriptorFactory.FromResource(Resource.Drawable.offline_expert_marker);
                expert_online_marker = BitmapDescriptorFactory.FromResource(Resource.Drawable.online_expert_marker);
                expert_clicked_marker = BitmapDescriptorFactory.FromResource(Resource.Drawable.expert_clicked);
                user_marker = BitmapDescriptorFactory.FromResource(Resource.Drawable.me_location_marker);

                _mapFragment =
                    FragmentManager.FindFragmentByTag("map") as MapFragment;
                if (_mapFragment == null)
                {
                    GoogleMapOptions mapOptions = new GoogleMapOptions()
                        .InvokeMapType(GoogleMap.MapTypeNormal)
                        .InvokeZoomControlsEnabled(false)
                        .InvokeCompassEnabled(true);

                    FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                    _mapFragment = MapFragment.NewInstance(mapOptions);
                    fragTx.Add(Resource.Id.map, _mapFragment, "map");
                    fragTx.Commit();
                }
                _mapFragment.GetMapAsync(this);

                my_marker = new MarkerOptions();
                my_position = new LatLng(
                                Convert.ToDouble(pref.GetString("latitude", String.Empty)/*, (CultureInfo.InvariantCulture)*/),
                                Convert.ToDouble(pref.GetString("longitude", String.Empty)/*, (CultureInfo.InvariantCulture)*/));
                my_marker.SetPosition(my_position);
                my_marker.SetIcon(user_marker);
                CameraPosition.Builder target_builder = CameraPosition.InvokeBuilder();
                target_builder.Target(my_position);
                target_builder.Zoom(15);
                CameraPosition cameraPosition = target_builder.Build();
                cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

                //sorting
                sortBn = FindViewById<Button>(Resource.Id.sortBn);
                sortLL = FindViewById<LinearLayout>(Resource.Id.sortLL);
                by_distanceLL = FindViewById<RelativeLayout>(Resource.Id.by_distanceLL);
                by_ratingLL = FindViewById<RelativeLayout>(Resource.Id.by_ratingLL);
                by_distanceIV = FindViewById<ImageView>(Resource.Id.by_distanceIV);
                by_ratingIV = FindViewById<ImageView>(Resource.Id.by_ratingIV);
                by_distanceTV = FindViewById<TextView>(Resource.Id.by_distanceTV);
                by_ratingTV = FindViewById<TextView>(Resource.Id.by_ratingTV);
                //sorting ENDED
                loginRegFragment = new LoginRegFragment();
                fragmentManager = this.FragmentManager;
                nothingIV = FindViewById<ImageView>(Resource.Id.nothingIV);
                nothingTV = FindViewById<TextView>(Resource.Id.nothingTV);
                searchET = FindViewById<EditText>(Resource.Id.searchET);
                searchLL = FindViewById<LinearLayout>(Resource.Id.searchLL);
                search_recyclerView = FindViewById<RecyclerView>(Resource.Id.search_recyclerView);
                searchBn = FindViewById<Button>(Resource.Id.searchBn);
                close_searchBn = FindViewById<ImageButton>(Resource.Id.close_searchBn);
                filterBn = FindViewById<Button>(Resource.Id.filterBn);
                expert_imageIV = FindViewById<ImageView>(Resource.Id.expert_imageIV);
                expert_nameTV = FindViewById<TextView>(Resource.Id.expert_nameTV);
                phoneTV = FindViewById<TextView>(Resource.Id.phoneTV);
                rating_valueTV = FindViewById<TextView>(Resource.Id.rating_valueTV);
                distanceTV = FindViewById<TextView>(Resource.Id.distanceTV);
                expert_imageIV = FindViewById<ImageView>(Resource.Id.expert_imageIV);
                onlineIV = FindViewById<ImageView>(Resource.Id.onlineIV);
                star1IV = FindViewById<ImageView>(Resource.Id.star1IV);
                star2IV = FindViewById<ImageView>(Resource.Id.star2IV);
                star3IV = FindViewById<ImageView>(Resource.Id.star3IV);
                star4IV = FindViewById<ImageView>(Resource.Id.star4IV);
                star5IV = FindViewById<ImageView>(Resource.Id.star5IV);
                writeBn = FindViewById<Button>(Resource.Id.writeBn);
                mapLL = FindViewById<LinearLayout>(Resource.Id.mapLL);
                headerTV = FindViewById<TextView>(Resource.Id.headerTV);
                typesTV = FindViewById<TextView>(Resource.Id.typesTV);
                headerTV.Text = expert_data.GetString("spec_name", String.Empty);
                typesTV.Text = expert_data.GetString("spec_type", String.Empty);
                recyclerViewDropdown = FindViewById<RecyclerView>(Resource.Id.recyclerViewDropdown);
                linearLayout7644 = FindViewById<RelativeLayout>(Resource.Id.linearLayout7644);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicatorSearch = FindViewById<ProgressBar>(Resource.Id.activityIndicatorSearch);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                bottomLayout = FindViewById<RelativeLayout>(Resource.Id.bottomLayout);
                upper_layout = FindViewById<RelativeLayout>(Resource.Id.upper_layout);
                expert_data_layoutRL = FindViewById<RelativeLayout>(Resource.Id.expert_data_layoutRL);
                tint_expertLL = FindViewById<LinearLayout>(Resource.Id.tint_expertLL);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                dropdownBn = FindViewById<Button>(Resource.Id.dropdownBn);
                searchBn = FindViewById<Button>(Resource.Id.searchBn);
                navbarLL = FindViewById<RelativeLayout>(Resource.Id.navbarLL);
                tintLL = FindViewById<LinearLayout>(Resource.Id.tintLL);
                show_listBn = FindViewById<Button>(Resource.Id.show_listBn);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                activityIndicatorSearch.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                layoutManagerDropdown = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                search_layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerViewDropdown.SetLayoutManager(layoutManagerDropdown);
                search_recyclerView.SetLayoutManager(search_layoutManager);
                searchET.Visibility = ViewStates.Gone;
                profileLL = FindViewById<LinearLayout>(Resource.Id.profileLL);
                dialogsLL = FindViewById<LinearLayout>(Resource.Id.dialogsLL);

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                headerTV.SetTypeface(tf, TypefaceStyle.Bold);
                typesTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.specialistsTV).SetTypeface(tf, TypefaceStyle.Normal);
                dialogsTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.profileTV).SetTypeface(tf, TypefaceStyle.Normal);
                expert_nameTV.SetTypeface(tf, TypefaceStyle.Bold);
                FindViewById<TextView>(Resource.Id.count_specs).SetTypeface(tf, TypefaceStyle.Normal);
                phoneTV.SetTypeface(tf, TypefaceStyle.Normal);
                rating_valueTV.SetTypeface(tf, TypefaceStyle.Normal);
                distanceTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textView1).SetTypeface(tf, TypefaceStyle.Normal);
                by_distanceTV.SetTypeface(tf, TypefaceStyle.Normal);
                by_ratingTV.SetTypeface(tf, TypefaceStyle.Normal);
                nothingTV.SetTypeface(tf, TypefaceStyle.Normal);
                searchET.SetTypeface(tf, TypefaceStyle.Normal);
                dropdownBn.SetTypeface(tf, TypefaceStyle.Normal);
                searchBn.SetTypeface(tf, TypefaceStyle.Normal);
                show_listBn.SetTypeface(tf, TypefaceStyle.Bold);
                filterBn.SetTypeface(tf, TypefaceStyle.Normal);
                sortBn.SetTypeface(tf, TypefaceStyle.Normal);
                writeBn.SetTypeface(tf, TypefaceStyle.Bold);

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
                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                writeBn.Click += (s, e) =>
                  {
                      StartActivity(typeof(DialogActivity));
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
                show_listBn.Click += (s, e) =>
                {
                    StartActivity(typeof(ListOfSpecialistsActivity));
                };
                sortBn.Click += (s, e) =>
                {
                    tintLL.Visibility = ViewStates.Visible;
                    sortLL.Visibility = ViewStates.Visible;
                };
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
                mapLL.Visibility = ViewStates.Gone;

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


                var specs = await specializationMethods.ExpertsList(
                   expert_data.GetString("spec_id", String.Empty),
                   pref.GetString("latitude", String.Empty),
                   pref.GetString("longitude", String.Empty),
                   1,
                   expert_data.GetString("expert_city_id", String.Empty),
                   expert_data.GetString("distance_radius", String.Empty),
                   expert_data.GetBoolean("has_reviews", false)/*, this*/);
                activityIndicator.Visibility = ViewStates.Gone;
                mapLL.Visibility = ViewStates.Visible;

                try
                {
                    var deserObj = JsonConvert.DeserializeObject<RootObjectExpert>(specs.ToString());
                    deserialized_experts = deserObj.experts;
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
                    int i = 0;
                    foreach (var expert in deserialized_experts)
                    {
                        if (!String.IsNullOrWhiteSpace(expert.coordinates.latitude) && !String.IsNullOrWhiteSpace(expert.coordinates.longitude))
                        {
                            MarkerOptions expert_marker = new MarkerOptions();
                            expert_marker.SetPosition(new LatLng(
                                Convert.ToDouble(expert.coordinates.latitude, (CultureInfo.InvariantCulture)),
                                Convert.ToDouble(expert.coordinates.longitude, (CultureInfo.InvariantCulture))));

                            if (i < 1)
                            {
                                expert_position = (new LatLng(
                                    Convert.ToDouble(expert.coordinates.latitude, (CultureInfo.InvariantCulture)),
                                    Convert.ToDouble(expert.coordinates.longitude, (CultureInfo.InvariantCulture))));
                                CameraPosition.Builder target_builder_expert = CameraPosition.InvokeBuilder();
                                target_builder_expert.Target(expert_position);
                                target_builder_expert.Zoom(15);
                                cameraPosition = target_builder_expert.Build();
                                if (!String.IsNullOrEmpty(expert_data.GetString("expert_city_id", String.Empty)))
                                {
                                    cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                                    _map.MoveCamera(cameraUpdate);
                                }
                                i++;
                            }

                            if (expert.online)
                            {
                                expert_marker.SetIcon(expert_online_marker);
                                //set this to mark marker that it is online
                                expert_marker.SetRotation(0.00001F);
                            }
                            else
                            {
                                expert_marker.SetIcon(expert_offline_marker);
                            }
                            _map.AddMarker(expert_marker);
                        }
                    }
                }
                catch { }

                _map.AddMarker(my_marker);

                dropdown_closed = true;
                dropdownBn.Click += (s, e) =>
                    dropdownClick();
                linearLayout7644.Click += (s, e) =>
                {
                    if (dropdownBn.Visibility == ViewStates.Visible)
                        dropdownClick();
                };

                bool types_visible = false;
                bool dropdown_visible = false;
                if (typesTV.Visibility == ViewStates.Visible)
                    types_visible = true;

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

                            specialistsOnMapCategorySearchAdapter = new SpecialistsOnMapCategorySearchAdapter(searchDisplayings, this, tf);
                            specialistsOnMapCategorySearchAdapter.NotifyDataSetChanged();
                            search_recyclerView.SetAdapter(specialistsOnMapCategorySearchAdapter);

                            specialistsOnMapCategorySearchAdapter.NotifyDataSetChanged();
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
                expert_data_layoutRL.Click += Expert_data_layoutRL_Click;
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
                    var dropDownSubcategsAdapter = new DropDownSubCategsAdapterForMap(deserialized_sub_categs, this, tf);
                    recyclerViewDropdown.SetAdapter(dropDownSubcategsAdapter);
                    dropdownBn.Visibility = ViewStates.Visible;
                    typesTV.Visibility = ViewStates.Visible;
                }
                else
                    dropdownBn.Visibility = ViewStates.Gone;
                if (dropdownBn.Visibility == ViewStates.Visible)
                    dropdown_visible = true;
                close_searchBn.Click += (s, e) =>
                {
                    searchET.Text = null;
                    imm.HideSoftInputFromWindow(searchET.WindowToken, 0);
                    searchLL.Visibility = ViewStates.Gone;
                    if (types_visible)
                        typesTV.Visibility = ViewStates.Visible;
                    if (dropdown_visible)
                        dropdownBn.Visibility = ViewStates.Visible;
                    headerTV.Visibility = ViewStates.Visible;
                    searchET.Visibility = ViewStates.Gone;
                    close_searchBn.Visibility = ViewStates.Gone;
                    searchBn.Visibility = ViewStates.Visible;
                };
            }
            catch
            {
                var available = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(ApplicationContext);
                if (available != 0)
                    Toast.MakeText(this, GetString(Resource.String.install_google_play_services), ToastLength.Long).Show();
                StartActivity(typeof(MainActivity));
            }
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
                if (expert_data_layoutRL.Visibility != ViewStates.Visible)
                    tintLL.Visibility = ViewStates.Visible;
                dropdown_closed = false;
            }
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
        private void Expert_data_layoutRL_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(ThreeLevelExpertProfileActivity));
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
        protected override void OnResume()
        {
            base.OnResume();
        }
        protected override void OnPostResume()
        {
            base.OnPostResume();
        }

        public async void OnCameraIdle()
        {
            if (_map.CameraPosition.Zoom < 9f)
                _map.AnimateCamera(CameraUpdateFactory.ZoomTo(9f));
            LatLngBounds bounds = _map.Projection.VisibleRegion.LatLngBounds;
            var ne = bounds.Northeast;
            var sw = bounds.Southwest;
            var lat_center = (ne.Latitude + sw.Latitude) / 2;
            var lng_center = (ne.Longitude + sw.Longitude) / 2;
            Location loc1 = new Location("");
            loc1.Latitude = ne.Latitude;
            loc1.Longitude = ne.Longitude;

            Location loc2 = new Location("");
            loc2.Latitude = lat_center;
            loc2.Longitude = lng_center;

            var distanceInMeters = loc1.DistanceTo(loc2);
            string distanceInString = Math.Truncate(distanceInMeters).ToString();

            var specs = await specializationMethods.ExpertsListMapZoom(
                   expert_data.GetString("spec_id", String.Empty),
                   lat_center.ToString(),
                   lng_center.ToString(),
                   distanceInString,
                   expert_data.GetBoolean("has_reviews", false)/*, this*/);

            try
            {
                var deserObj = JsonConvert.DeserializeObject<RootObjectExpert>(specs.ToString());
                deserialized_experts = deserObj.experts;
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
                if (!first_zoom)
                    _map.Clear();
                first_zoom = false;

                my_marker.SetPosition(my_position);
                my_marker.SetIcon(user_marker);
                _map.AddMarker(my_marker);

                foreach (var expert in deserialized_experts)
                {
                    if (!String.IsNullOrWhiteSpace(expert.coordinates.latitude) && !String.IsNullOrWhiteSpace(expert.coordinates.longitude))
                    {
                        MarkerOptions expert_marker = new MarkerOptions();
                        expert_marker.SetPosition(new LatLng(
                            Convert.ToDouble(expert.coordinates.latitude, (CultureInfo.InvariantCulture)),
                            Convert.ToDouble(expert.coordinates.longitude, (CultureInfo.InvariantCulture))));
                        if (expert.online)
                        {
                            expert_marker.SetIcon(expert_online_marker);
                            //set this to mark marker that it is online
                            expert_marker.SetRotation(0.00001F);
                        }
                        else if (!expert.online)
                            expert_marker.SetIcon(expert_offline_marker);
                        if (clicked_expert_position != null)
                            if (expert_marker.Position.Latitude == clicked_expert_position.Latitude && expert_marker.Position.Longitude == clicked_expert_position.Longitude)
                            {
                                expert_marker.SetIcon(expert_clicked_marker);
                            }
                        _map.AddMarker(expert_marker);
                    }
                }
            }
            catch { }
        }
    }
}