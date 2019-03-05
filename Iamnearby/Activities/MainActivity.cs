using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Iamnearby.Fragments;
using Iamnearby.Methods;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using PCL.Database;
using PCL.Models;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace Iamnearby.Activities
{
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        static bool from_push = false;
        static readonly string TAG = "MainActivity";
        ISharedPreferences dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_dialog;
        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static int NOTIFICATION_ID = 100;
        static Android.App.FragmentManager fragmentManager;
        static LoginRegFragment loginRegFragment;
        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            string g;
            if (resultCode != ConnectionResult.Success)
            {

                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    g = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                else
                {
                    g = "This device is not supported";
                    Finish();
                }
                return false;
            }
            else
            {
                g = "Google Play Services is available.";
                return true;
            }
        }
        void CreateNotificationChannel()
        {
            //if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            //{
            //    // Notification channels are new in API 26 (and not a part of the
            //    // support library). There is no need to create a notification
            //    // channel on older versions of Android.
            //    return;
            //}

            ////var channel = new NotificationChannel(MyFirebaseMessagingService.CHANNEL_ID,
            ////                                      "FCM Notifications",
            ////                                      NotificationImportance.Default)
            //{

            //    Description = "Firebase Cloud Messages appear in this channel"
            //};

            //var notificationManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
            //notificationManager.CreateNotificationChannel(channel);
        }

        RelativeLayout i_need_help_LL, i_want_to_help_LL, LogoRL, mainRL, tintLL;
        TextView EnterMyAccTV, getting_positionTV, your_city_valueTV, noTV, yesTV;
        ProgressBar activityIndicator;
        Button restartBn;
        ISharedPreferences loc_pref, device_info_prefs;
        ISharedPreferencesEditor edit, edit_device_info_prefs;
        UserMethods userMethods = new UserMethods();
        //MapMethods mapMethods = new MapMethods();
        CountryMethods countryMethods = new CountryMethods();
        Feedbacks feedbacks = new Feedbacks();
        Position position;

        RegistrationMethods registrationMethods = new RegistrationMethods();
        ProfileAndExpertMethods profileAndExpertMethods = new ProfileAndExpertMethods();
        SettingsMethods settingsMethods = new SettingsMethods();

        List<MyLocation> myLocDeserialized;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.main);

            PCL.HttpMethods.ProfileAndExpertMethods ksg = new PCL.HttpMethods.ProfileAndExpertMethods();



            loginRegFragment = new LoginRegFragment();
            fragmentManager = this.FragmentManager;

            edit_dialog = dialog_data.Edit();
            if (Intent.Extras != null)
            {
                string type = "";
                foreach (var key in Intent.Extras.KeySet())
                {
                    var value = Intent.Extras.GetString(key);
                    Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
                    if (key == "message_from")
                        edit_dialog.PutString("expert_id", value);
                    if (key == "name")
                        edit_dialog.PutString("expert_name", value);
                    if (key == "avatar")
                        edit_dialog.PutString("expert_avatar", value);
                    if (key == "type")
                        type = value;
                    //
                    //break;
                }
                edit_dialog.Apply();
                from_push = true;
                if (type == "message")
                {
                    StartActivity(typeof(DialogActivity));
                }
                else
                {

                    string review_id = "";
                    string reviewText = "";
                    string reviewAnswer = "";
                    string companionId = "";
                    string companionFullName = "";
                    string companionAvatar = "";
                    string myFullName = "";
                    string myId = "";
                    string myAvatar = "";
                    string is_online = "";
                    string timestamp = "";
                    string rating = "";
                    string categoryId = "";
                    string msg_cnt_new = "";

                    foreach (var key in Intent.Extras.KeySet())
                    {
                        var value = Intent.Extras.GetString(key);
                        if (key == "review_id")
                            review_id = value;
                        if (key == "reviewText")
                            reviewText = value;
                        if (key == "reviewAnswer")
                            reviewAnswer = value;
                        if (key == "companionId")
                            companionId = value;
                        if (key == "companionFullName")
                            companionFullName = value;
                        if (key == "companionAvatar")
                            companionAvatar = value;
                        if (key == "myFullName")
                            myFullName = value;
                        if (key == "myAvatar")
                            myAvatar = value;
                        if (key == "is_online")
                            is_online = value;
                        if (key == "timestamp")
                            timestamp = value;
                        if (key == "rating")
                            rating = value;
                        if (key == "categoryId")
                            categoryId = value;
                        if (key == "myId")
                            myId = value;
                        if (key == "msg_cnt_new")
                            msg_cnt_new = value;
                    }
                    var activity2 = new Intent(this, typeof(ReviewFromPushActivity));
                    activity2.PutExtra("type", type);
                    activity2.PutExtra("review_id", review_id);
                    activity2.PutExtra("reviewText", reviewText);
                    activity2.PutExtra("companionId", companionId);
                    activity2.PutExtra("companionFullName", companionFullName);
                    activity2.PutExtra("companionAvatar", companionAvatar);
                    activity2.PutExtra("myFullName", myFullName);
                    activity2.PutExtra("myAvatar", myAvatar);
                    activity2.PutExtra("reviewAnswer", reviewAnswer);
                    activity2.PutExtra("review_rating", rating);
                    activity2.PutExtra("review_date", timestamp);
                    activity2.PutExtra("review_online", is_online);
                    activity2.PutExtra("categoryId", categoryId);
                    activity2.PutExtra("myId", myId);
                    activity2.PutExtra("msg_cnt_new", msg_cnt_new);
                    StartActivity(activity2);
                }
            }
            else
                from_push = false;

            IsPlayServicesAvailable();

            CreateNotificationChannel();

            AppCenter.Start("0a86243f-1b34-4bca-bc65-133580015fc1", typeof(Analytics), typeof(Crashes));
            var context = Application.Context;
            String androidOS = Build.VERSION.Release;
            String androidId = Settings.Secure.GetString(context.ContentResolver, Settings.Secure.AndroidId);
            var VersionNumber = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionName;
            var package_name = context.PackageName;

            var device_obj = new PCL.Models.Device
            {
                platform = "Android",
                deviceToken = androidId,
                osVersion = androidOS,
                appVersion = VersionNumber,
                packageName = package_name,
            };

            var device_Json = JsonConvert.SerializeObject(device_obj);
            device_info_prefs = Application.Context.GetSharedPreferences("device_info", FileCreationMode.Private);
            edit_device_info_prefs = device_info_prefs.Edit();
            edit_device_info_prefs.PutString("device_json", device_Json);
            new MyFirebaseIIDService().OnTokenRefresh();
            var tokenFirebase = await GetToken();
            edit_device_info_prefs.PutString("firebase_token", tokenFirebase);
            edit_device_info_prefs.Apply();

            loc_pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
            edit = loc_pref.Edit();
            i_need_help_LL = FindViewById<RelativeLayout>(Resource.Id.i_need_help_LL);
            i_want_to_help_LL = FindViewById<RelativeLayout>(Resource.Id.i_want_to_help_LL);
            LogoRL = FindViewById<RelativeLayout>(Resource.Id.LogoRL);
            mainRL = FindViewById<RelativeLayout>(Resource.Id.mainRL);
            tintLL = FindViewById<RelativeLayout>(Resource.Id.tintLL);
            EnterMyAccTV = FindViewById<TextView>(Resource.Id.EnterMyAccTV);
            your_city_valueTV = FindViewById<TextView>(Resource.Id.your_city_valueTV);
            getting_positionTV = FindViewById<TextView>(Resource.Id.getting_positionTV);
            noTV = FindViewById<TextView>(Resource.Id.noTV);
            yesTV = FindViewById<TextView>(Resource.Id.yesTV);
            restartBn = FindViewById<Button>(Resource.Id.restartBn);
            activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
            activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);

            Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
            EnterMyAccTV.SetTypeface(tf, TypefaceStyle.Normal);
            your_city_valueTV.SetTypeface(tf, TypefaceStyle.Normal);
            getting_positionTV.SetTypeface(tf, TypefaceStyle.Normal);
            noTV.SetTypeface(tf, TypefaceStyle.Normal);
            yesTV.SetTypeface(tf, TypefaceStyle.Normal);
            FindViewById<TextView>(Resource.Id.textView5).SetTypeface(tf, TypefaceStyle.Normal);
            FindViewById<TextView>(Resource.Id.textView4).SetTypeface(tf, TypefaceStyle.Normal);
            FindViewById<TextView>(Resource.Id.textView3).SetTypeface(tf, TypefaceStyle.Bold);
            FindViewById<TextView>(Resource.Id.textView2).SetTypeface(tf, TypefaceStyle.Bold);
            restartBn.SetTypeface(tf, TypefaceStyle.Normal);

            restartBn.Visibility = ViewStates.Gone;
            i_need_help_LL.Click += (s, e) =>
                {
                    if (tintLL.Visibility != ViewStates.Visible)
                        StartActivity(typeof(SpecialistsCategoryActivity));
                };
            i_want_to_help_LL.Click += (s, e) =>
             {
                 if (tintLL.Visibility != ViewStates.Visible)
                     try
                     {
                         if (!userMethods.UserExists())
                             loginRegFragment.Show(fragmentManager, "fragmentManager");
                         else
                             StartActivity(typeof(UserProfileActivity));
                     }
                     catch { Toast.MakeText(this, GetString(Resource.String.you_not_logined), ToastLength.Short).Show(); StartActivity(typeof(MainActivity)); }
             };
            EnterMyAccTV.Click += (s, e) =>
              {
                  if (tintLL.Visibility != ViewStates.Visible)
                      StartActivity(typeof(AuthorizationActivity));
              };

            if (userMethods.UserExists())
                EnterMyAccTV.Visibility = ViewStates.Gone;

            if (checkInternetConnection())
            {
                //checking if GPS is enabled
                if (IsGeolocationEnabled())
                {
                    activityIndicator.Visibility = ViewStates.Visible;
                    await Geolocation();
                }
                else
                {
                    turnGPSon();
                }
            }
            else
            {
                restartBn.Visibility = ViewStates.Visible;
                activityIndicator.Visibility = ViewStates.Gone;
                getting_positionTV.Visibility = ViewStates.Gone;
                Toast.MakeText(this, Resource.String.turn_internet_on, ToastLength.Long).Show();
            }
            restartBn.Click += (s, e) =>
            {
                StartActivity(typeof(MainActivity));
            };
            var granted = await checkLocPermission();
            if (!granted)
                checkLocPermission();

            if (checkInternetConnection() && IsGeolocationEnabled())
                if (userMethods.UserExists())
                    if (!from_push)
                        StartActivity(typeof(UserProfileActivity));
        }

        private async Task<string> GetToken()
        {
            if (MyFirebaseIIDService.refreshedToken == null)
            {
                await Task.Delay(200);
                await GetToken();
            }
            return MyFirebaseIIDService.refreshedToken;
        }

        //checking internet connection
        private bool checkInternetConnection()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);

            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            if ((activeConnection != null) && activeConnection.IsConnected)
                return true;
            else
                return false;
        }
        //checking internet connection ended

        public async Task<string> Geolocation()
        {
            Timer geoTimer = new Timer();
            geoTimer.Interval = 30000;
            geoTimer.Elapsed += delegate
            {
                RunOnUiThread(() =>
                {
                    geoTimer.Stop();
                    geoTimer.Dispose();
                    if (position == null)
                        StartActivity(typeof(MainActivity));
                });
            };
            geoTimer.Start();

            Coordinates location = default(Coordinates);
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;

                position = await locator.GetLastKnownLocationAsync();
                if (position == null && locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
                    position = await locator.GetPositionAsync(TimeSpan.FromSeconds((double)30000));
            }
            catch
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                position = await locator.GetPositionAsync(TimeSpan.FromSeconds((double)30000));
            }

            //need this because different cultures put comma instead of dot so it may not work without this code
            edit.PutString("latitude", Convert.ToDouble(position.Latitude/*, (CultureInfo.InvariantCulture)*/).ToString());
            edit.PutString("longitude", Convert.ToDouble(position.Longitude/*, (CultureInfo.InvariantCulture)*/).ToString());
            edit.Apply();

            mainRL.SetBackgroundResource(Resource.Drawable.Splash);
            activityIndicator.Visibility = ViewStates.Gone;
            getting_positionTV.Visibility = ViewStates.Gone;
            restartBn.Visibility = ViewStates.Gone;
            LogoRL.Visibility = ViewStates.Visible;
            i_need_help_LL.Visibility = ViewStates.Visible;
            i_want_to_help_LL.Visibility = ViewStates.Visible;

            if (!userMethods.UserExists())
            {
                var myLocJson = await countryMethods.CurrentLocation(loc_pref.GetString("latitude", String.Empty), loc_pref.GetString("longitude", String.Empty));
                myLocDeserialized = JsonConvert.DeserializeObject<List<MyLocation>>(myLocJson);
                edit.PutString("auto_city_id", myLocDeserialized[0].city.id);
                edit.PutString("auto_city_name", myLocDeserialized[0].city.name);
                edit.PutString("auto_region_id", myLocDeserialized[0].region.id);
                edit.PutString("auto_region_name", myLocDeserialized[0].region.name);
                edit.PutString("auto_country_id", myLocDeserialized[0].country.id);
                edit.PutString("auto_country_name", myLocDeserialized[0].country.name);
                edit.Apply();

                tintLL.Visibility = ViewStates.Visible;
                your_city_valueTV.Text = myLocDeserialized[0].city.name + "?";

                yesTV.Click += (s, e) =>
                {
                    tintLL.Visibility = ViewStates.Gone;
                };

                noTV.Click += (s, e) =>
                {
                    tintLL.Visibility = ViewStates.Gone;
                    StartActivity(typeof(LocationActivity));
                };
            }

            if (position == null)
                return "";

            return "";
        }
        //My method to turn the GPS on
        private void turnGPSon()
        {
            restartBn.Visibility = ViewStates.Visible;
            activityIndicator.Visibility = ViewStates.Gone;
            getting_positionTV.Visibility = ViewStates.Gone;
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this, Resource.Style.AlertStyle);
            builder.SetTitle(Resource.String.location_services_are_disabled);
            builder.SetMessage(Resource.String.turn_them_on);
            builder.SetCancelable(false);
            builder.SetPositiveButton(Resource.String.no, (object sender1, DialogClickEventArgs e1) =>
            { });
            builder.SetNegativeButton(Resource.String.yes, (object sender1, DialogClickEventArgs e1) =>
            {
                restartBn.Visibility = ViewStates.Visible;
                StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
            });
            Android.App.AlertDialog dialog = builder.Create();
            dialog.Show();
        }
        //My method to turn the GPS on ENDED

        //method to detect if GPS is enabled
        public bool IsGeolocationEnabled()
        {
            return CrossGeolocator.Current.IsGeolocationEnabled;
        }
        async Task<bool> checkLocPermission()
        {
            PermissionStatus locationStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Location);

            if (locationStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Plugin.Permissions.Abstractions.Permission.Location });
                locationStatus = results[Plugin.Permissions.Abstractions.Permission.Location];
                request_runtime_permissions();
            }

            return true;
        }

        private const int REQUEST_PERMISSION_CODE = 1000;
        void request_runtime_permissions()
        {
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.M)
                if (
                               CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted
                          || CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted
                          || CheckSelfPermission(Manifest.Permission.AccessMockLocation) != Android.Content.PM.Permission.Granted
                          || CheckSelfPermission(Manifest.Permission.Internet) != Android.Content.PM.Permission.Granted
                          || CheckSelfPermission(Manifest.Permission.AccessNetworkState) != Android.Content.PM.Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new String[]
                    {
                                Manifest.Permission.AccessFineLocation,
                                Manifest.Permission.AccessCoarseLocation,
                                Manifest.Permission.AccessMockLocation,
                                Manifest.Permission.Internet,
                                Manifest.Permission.AccessNetworkState,
                    }, REQUEST_PERMISSION_CODE);
                }
                else
                {
                    ActivityCompat.RequestPermissions(this, new String[]
                    {
                                Manifest.Permission.AccessFineLocation,
                                Manifest.Permission.AccessCoarseLocation,
                                Manifest.Permission.AccessMockLocation,
                                Manifest.Permission.Internet,
                                Manifest.Permission.AccessNetworkState,
                    }, REQUEST_PERMISSION_CODE);
                }
        }
        bool shown = false;
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            switch (requestCode)
            {
                case REQUEST_PERMISSION_CODE:
                    {
                        if (grantResults.Length > 0 && grantResults[0] == Android.Content.PM.Permission.Granted)
                        {
                            StartActivity(typeof(MainActivity));
                        }
                        else
                        {
                            if (!shown)
                                Toast.MakeText(this, GetString(Resource.String.permissions_needed), ToastLength.Long).Show();
                            shown = true;
                            request_runtime_permissions();
                        }
                        break;
                    }
            }
        }
    }

}

