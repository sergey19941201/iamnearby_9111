using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using PCL.Database;
using Iamnearby.Methods;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SettingsActivity : Activity
    {
        TextView on_offTV;
        ImageButton back_button;
        Button exitBn;
        RelativeLayout backRelativeLayout, aboutRL;
        RelativeLayout tint_DelLL;
        TextView noTV, yesTV;
        Switch notificationsS;
        UserMethods userMethods = new UserMethods();
        SettingsMethods settingsMethods = new SettingsMethods();
        AuthorizationMethods authorizationMethods = new AuthorizationMethods();
        ProgressBar activityIndicator;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.Settings);
                notificationsS = FindViewById<Switch>(Resource.Id.notificationsS);
                tint_DelLL = FindViewById<RelativeLayout>(Resource.Id.tint_DelLL);
                noTV = FindViewById<TextView>(Resource.Id.noTV);
                yesTV = FindViewById<TextView>(Resource.Id.yesTV);
                exitBn = FindViewById<Button>(Resource.Id.exitBn);
                on_offTV = FindViewById<TextView>(Resource.Id.on_offTV);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                exitBn.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.dsadas).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.dssadas).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.dasasssdas).SetTypeface(tf, TypefaceStyle.Normal);
                on_offTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textView5).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.your_city_valueTV).SetTypeface(tf, TypefaceStyle.Normal);
                yesTV.SetTypeface(tf, TypefaceStyle.Normal);
                noTV.SetTypeface(tf, TypefaceStyle.Normal);

                if (userMethods.GetNotifData())
                {
                    notificationsS.Checked = true;
                    on_offTV.Text = GetString(Resource.String.notification_on);
                }
                else
                {
                    notificationsS.Checked = false;
                    on_offTV.Text = GetString(Resource.String.notification_off);
                }
                tint_DelLL.Click += (s, e) =>
                {
                    tint_DelLL.Visibility = ViewStates.Gone;
                };
                yesTV.Click += async (s, e) =>
                {
                    ISharedPreferences pref = Application.Context.GetSharedPreferences("reg_data", FileCreationMode.Private);
                    ISharedPreferencesEditor edit = pref.Edit();
                    edit.PutString("surname", "");
                    edit.PutString("name", "");
                    edit.PutString("patronymic", "");
                    edit.PutString("phone", "");
                    edit.PutString("email", "");
                    edit.Apply();
                    if (checkInternetConnection())
                    {
                        activityIndicator.Visibility = ViewStates.Visible;
                        tint_DelLL.Visibility = ViewStates.Gone;
                        exitBn.Visibility = ViewStates.Gone;
                        var res = await authorizationMethods.LogOut(userMethods.GetUsersAuthToken());
                        activityIndicator.Visibility = ViewStates.Gone;
                        exitBn.Visibility = ViewStates.Visible;
                        userMethods.ClearTable();
                        userMethods.ClearUsersDataTable();
                        userMethods.ClearTableNotif();
                        StartActivity(typeof(MainActivity));
                    }
                    else
                        Toast.MakeText(this, Resource.String.turn_internet_on, ToastLength.Long).Show();
                };

                noTV.Click += (s, e) =>
                {
                    tint_DelLL.Visibility = ViewStates.Gone;
                };
                exitBn.Click += (s, e) =>
                  {
                      tint_DelLL.Visibility = ViewStates.Visible;
                  };
                aboutRL = FindViewById<RelativeLayout>(Resource.Id.aboutRL);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                aboutRL.Click += (s, e) =>
                  {
                      StartActivity(typeof(AboutActivity));
                  };
                back_button.Click += (s, e) => { OnBackPressed(); };
                backRelativeLayout.Click += (s, e) => { OnBackPressed(); };
                notificationsS.CheckedChange += async (s, e) =>
                      {
                          if (notificationsS.Checked)
                          {
                              var res = await settingsMethods.Notifications(userMethods.GetUsersAuthToken(), true);
                              userMethods.InsertNotif(true);
                              on_offTV.Text = GetString(Resource.String.notification_on);
                          }
                          else
                          {
                              var res = await settingsMethods.Notifications(userMethods.GetUsersAuthToken(), false);
                              userMethods.InsertNotif(false);
                              on_offTV.Text = GetString(Resource.String.notification_off);
                          }
                      };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
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
    }
}