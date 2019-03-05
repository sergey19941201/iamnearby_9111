using System;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Iamnearby.Methods;
using Newtonsoft.Json;
using PCL.Database;
using PCL.Models;

namespace Iamnearby.Activities
{
    [Activity(Label = "RegAfterActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class RegAfterActivity : Activity
    {
        TextView countdown_TV, timerTV, emailTV;
        Button resendBn, completeRegBn;
        Timer timer;
        static Timer timer_activate;
        ProgressBar activityIndicator;
        int minutes = 14;
        int seconds = 60;
        RegistrationMethods registrationMethods = new RegistrationMethods();
        UserMethods userMethods = new UserMethods();
        ISharedPreferences pref = Application.Context.GetSharedPreferences("reg_data", FileCreationMode.Private);
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.RegAfter);
                countdown_TV = FindViewById<TextView>(Resource.Id.countdown_TV);
                timerTV = FindViewById<TextView>(Resource.Id.timerTV);
                emailTV = FindViewById<TextView>(Resource.Id.emailTV);
                resendBn = FindViewById<Button>(Resource.Id.resendBn);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                completeRegBn = FindViewById<Button>(Resource.Id.completeRegBn);
                emailTV.Text = pref.GetString("email", String.Empty);

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                resendBn.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textviewwe).SetTypeface(tf, TypefaceStyle.Normal);
                countdown_TV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textView1).SetTypeface(tf, TypefaceStyle.Normal);
                emailTV.SetTypeface(tf, TypefaceStyle.Normal);
                timerTV.SetTypeface(tf, TypefaceStyle.Normal);
                completeRegBn.SetTypeface(tf, TypefaceStyle.Normal);

                completeRegBn.Click += async (s, e) =>
                {
                    completeRegBn.Visibility = ViewStates.Gone;
                    resendBn.Visibility = ViewStates.Gone;
                    activityIndicator.Visibility = ViewStates.Visible;
                    var activate = await registrationMethods.RegisterActivate(pref.GetString("email", String.Empty));
                    completeRegBn.Visibility = ViewStates.Visible;
                    resendBn.Visibility = ViewStates.Visible;
                    activityIndicator.Visibility = ViewStates.Gone;
                    if (activate.Contains("authToken"))
                    {
                        if (activate != "null" && activate != null && activate != "false")
                        {
                            var deserialized_value = JsonConvert.DeserializeObject<RegAfter>(activate.ToString());
                            if (deserialized_value.confirmed != false)
                            {
                                userMethods.InsertUser(deserialized_value.authToken, pref.GetString("email", String.Empty));
                                StartActivity(typeof(UserProfileActivity));
                            }
                            else
                                Toast.MakeText(this, GetString(Resource.String.no_confirmation_by_mail), ToastLength.Short).Show();
                        }
                    }
                };

                timer = new Timer();
                timer.Interval = 1000;
                timer.Elapsed += delegate
                 {
                     seconds--;
                     if (seconds == 0)
                     {
                         minutes--;
                         seconds = 60;
                     }
                     //this construction used here to display timer value correctly
                     if (seconds == 60)
                     {
                         if (minutes < 10)
                             RunOnUiThread(() => { timerTV.Text = $"0{minutes}:00"; });
                         else
                             RunOnUiThread(() => { timerTV.Text = $"{minutes}:00"; });
                     }
                     else if (seconds < 10)
                     {
                         if (minutes < 10)
                             RunOnUiThread(() => { timerTV.Text = $"0{minutes}:0{seconds}"; });
                         else
                             RunOnUiThread(() => { timerTV.Text = $"{minutes}:0{seconds}"; });
                     }
                     else
                     {
                         if (minutes < 10)
                             RunOnUiThread(() => { timerTV.Text = $"0{minutes}:{seconds}"; });
                         else
                             RunOnUiThread(() => { timerTV.Text = $"{minutes}:{seconds}"; });
                     }
                     if (minutes == 0 && seconds == 60)
                     {
                         timer.Stop();
                         countdown_TV.Text = GetString(Resource.String.link_expired_text);
                         resendBn.Visibility = ViewStates.Visible;
                     }
                 };
                timer.Start();
                resendBn.Click += async (s, e) =>
                  {
                      resendBn.Visibility = ViewStates.Gone;
                      activityIndicator.Visibility = ViewStates.Visible;
                      var reg_result = await registrationMethods.Resend(pref.GetString("email", String.Empty));
                      resendBn.Visibility = ViewStates.Visible;
                      activityIndicator.Visibility = ViewStates.Gone;
                      StartActivity(typeof(RegAfterActivity));
                  };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}