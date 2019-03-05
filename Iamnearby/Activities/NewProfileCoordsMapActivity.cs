using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using static Android.Gms.Maps.GoogleMap;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class NewProfileCoordsMapActivity : Activity, IOnMapReadyCallback, IOnMapLongClickListener
    {
        private GoogleMap _map;
        private MapFragment _mapFragment;
        private CameraUpdate cameraUpdate;
        BitmapDescriptor user_descriptor, new_coords_descriptor;
        RelativeLayout backRelativeLayout;
        ImageButton back_button;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
        ISharedPreferences city_coord_for_edit_prefs = Application.Context.GetSharedPreferences("city_coord_for_edit_prefs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_city_coord_for_edit;
        ISharedPreferencesEditor edit;
        MarkerOptions my_marker;
        Marker work_marker;
        LatLng point;
        LinearLayout apply_cancelLL;
        EditText searchET;
        Button okBn;
        TextView applyTV, cancelTV;
        string chosen_lat, chosen_lng;
        LatLng my_position = new LatLng(0, 0);
        MarkerOptions markerOptions = new MarkerOptions();
        public async void OnMapReady(GoogleMap map)
        {
            edit = pref.Edit();
            _map = map;
            if (_map != null)
            {
                var locator = CrossGeolocator.Current;
                try
                {
                    await locator.StartListeningAsync(TimeSpan.FromSeconds(30), 20);
                }
                catch { }
                my_marker = new MarkerOptions();
                my_position = new LatLng(
                                Convert.ToDouble(pref.GetString("latitude", String.Empty)/*, (CultureInfo.InvariantCulture)*/),
                                Convert.ToDouble(pref.GetString("longitude", String.Empty)/*, (CultureInfo.InvariantCulture)*/));
                my_marker.SetPosition(my_position);
                my_marker.SetIcon(user_descriptor);

                _map.AddMarker(my_marker);
                _map.UiSettings.ZoomControlsEnabled = false;
                _map.UiSettings.CompassEnabled = false;

                _map.SetOnMapLongClickListener(this);


                var kfkf = Convert.ToDouble(city_coord_for_edit_prefs.GetString("lat", String.Empty).Replace(',', '.'), CultureInfo.InvariantCulture);
                LatLng point = new LatLng(
                    Convert.ToDouble(city_coord_for_edit_prefs.GetString("lat", String.Empty).Replace(',', '.'), CultureInfo.InvariantCulture),//, (CultureInfo.InvariantCulture)),
                    Convert.ToDouble(city_coord_for_edit_prefs.GetString("lng", String.Empty).Replace(',', '.'), CultureInfo.InvariantCulture));
                this.point = point;
                locator.PositionChanged += (sender, e) =>
                {
                    var position = e.Position as Position;
                    edit.PutString("latitude", Convert.ToDouble(position.Latitude/*, (CultureInfo.InvariantCulture)*/).ToString());
                    edit.PutString("longitude", Convert.ToDouble(position.Longitude/*, (CultureInfo.InvariantCulture)*/).ToString());
                    edit.Apply();
                    map.Clear();
                    my_marker = new MarkerOptions();
                    my_position = new LatLng(
                                    Convert.ToDouble(pref.GetString("latitude", String.Empty)/*, (CultureInfo.InvariantCulture)*/),
                                    Convert.ToDouble(pref.GetString("longitude", String.Empty)/*, (CultureInfo.InvariantCulture)*/));
                    my_marker.SetPosition(my_position);
                    my_marker.SetIcon(user_descriptor);

                    _map.AddMarker(my_marker);
                    new_coords_descriptor = BitmapDescriptorFactory.FromResource(Resource.Drawable.offline_expert_marker);
                    if (work_marker != null)
                    {
                        work_marker.Remove();
                    }
                    edit_city_coord_for_edit = city_coord_for_edit_prefs.Edit();
                    apply_cancelLL.Visibility = ViewStates.Visible;
                    markerOptions = new MarkerOptions();
                    markerOptions.SetPosition(point);
                    markerOptions.SetIcon(new_coords_descriptor);

                    work_marker = _map.AddMarker(markerOptions);
                    work_marker.Title = GetString(Resource.String.work_location);
                    chosen_lat = point.Latitude.ToString();
                    chosen_lng = point.Longitude.ToString();
                };
                markerOptions = new MarkerOptions();
                markerOptions.SetPosition(point);
                new_coords_descriptor = BitmapDescriptorFactory.FromResource(Resource.Drawable.offline_expert_marker);
                markerOptions.SetIcon(new_coords_descriptor);
                work_marker = _map.AddMarker(markerOptions);
                work_marker.Title = GetString(Resource.String.work_location);

                CameraPosition.Builder target_builder = CameraPosition.InvokeBuilder();
                target_builder.Target(point);
                target_builder.Zoom(15);
                CameraPosition cameraPosition = target_builder.Build();
                cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                _map.MoveCamera(cameraUpdate);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.NewProfileCoordsMap);
                user_descriptor = BitmapDescriptorFactory.FromResource(Resource.Drawable.me_location_marker);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                apply_cancelLL = FindViewById<LinearLayout>(Resource.Id.apply_cancelLL);
                applyTV = FindViewById<TextView>(Resource.Id.applyTV);
                cancelTV = FindViewById<TextView>(Resource.Id.cancelTV);
                searchET = FindViewById<EditText>(Resource.Id.searchET);
                okBn = FindViewById<Button>(Resource.Id.okBn);
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                applyTV.SetTypeface(tf, TypefaceStyle.Normal);
                cancelTV.SetTypeface(tf, TypefaceStyle.Normal);
                searchET.SetTypeface(tf, TypefaceStyle.Normal);
                okBn.SetTypeface(tf, TypefaceStyle.Normal);
                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };
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
                applyTV.Click += delegate
                  {
                      edit_city_coord_for_edit.PutString("lat", chosen_lat);
                      edit_city_coord_for_edit.PutString("lng", chosen_lng);
                      edit_city_coord_for_edit.Apply();
                      StartActivity(typeof(ApplyCityEditActivity));
                  };
                cancelTV.Click += delegate
                  {
                      StartActivity(typeof(UserProfileActivity));
                  };
                searchET.TextChanged += (s, e) =>
                  {
                      if (!String.IsNullOrEmpty(searchET.Text))
                          okBn.Visibility = ViewStates.Visible;
                      else
                          okBn.Visibility = ViewStates.Gone;
                  };
                okBn.Click += delegate
                {
                    if (!String.IsNullOrWhiteSpace(searchET.Text))
                    {
                        //dissmissing keyboard
                        InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                        imm.HideSoftInputFromWindow(searchET.WindowToken, 0);
                        //dissmissing keyboard ENDED

                        new Thread(new ThreadStart(() =>
                        {
                            var geo = new Geocoder(this);

                            var addresses = geo.GetFromLocationName(searchET.Text, 1);

                            RunOnUiThread(() =>
                            {
                                var addressText = FindViewById<TextView>(Resource.Id.searchET);

                                addresses.ToList().ForEach((addr) =>
                                {
                                    LatLng search_location = new LatLng(
                                        Convert.ToDouble(addr.Latitude, (CultureInfo.InvariantCulture)),
                                        Convert.ToDouble(addr.Longitude, (CultureInfo.InvariantCulture)));
                                    CameraPosition.Builder builder_search = CameraPosition.InvokeBuilder();
                                    builder_search.Target(search_location);
                                    builder_search.Zoom(15);
                                    CameraPosition search_cameraPosition = builder_search.Build();
                                    cameraUpdate = CameraUpdateFactory.NewCameraPosition(search_cameraPosition);
                                    _map.MoveCamera(cameraUpdate);

                                    new_coords_descriptor = BitmapDescriptorFactory.FromResource(Resource.Drawable.offline_expert_marker);
                                    if (work_marker != null)
                                    {
                                        work_marker.Remove();
                                    }
                                    edit_city_coord_for_edit = city_coord_for_edit_prefs.Edit();
                                    apply_cancelLL.Visibility = ViewStates.Visible;
                                    MarkerOptions markerOptions = new MarkerOptions();
                                    markerOptions.SetPosition(search_location);
                                    markerOptions.SetIcon(new_coords_descriptor);
                                    work_marker = _map.AddMarker(markerOptions);
                                    work_marker.Title = GetString(Resource.String.work_location);
                                    chosen_lat = search_location.Latitude.ToString();
                                    chosen_lng = search_location.Longitude.ToString();
                                });
                            });
                        })).Start();
                    }
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

        void IOnMapLongClickListener.OnMapLongClick(LatLng point)
        {
            this.point = point;
            new_coords_descriptor = BitmapDescriptorFactory.FromResource(Resource.Drawable.offline_expert_marker);
            if (work_marker != null)
            {
                work_marker.Remove();
            }
            edit_city_coord_for_edit = city_coord_for_edit_prefs.Edit();
            apply_cancelLL.Visibility = ViewStates.Visible;
            markerOptions = new MarkerOptions();
            markerOptions.SetPosition(point);
            markerOptions.SetIcon(new_coords_descriptor);

            work_marker = _map.AddMarker(markerOptions);
            work_marker.Title = GetString(Resource.String.work_location);
            chosen_lat = point.Latitude.ToString();
            chosen_lng = point.Longitude.ToString();
        }
    }
}