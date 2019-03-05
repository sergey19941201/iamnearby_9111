using System;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Iamnearby.Methods;
using Newtonsoft.Json;
using PCL.Database;
using PCL.Models;

namespace Iamnearby.Activities
{
    [Activity(Label = "AuthAfterActivity", ScreenOrientation = ScreenOrientation.Portrait/*, Theme = "@style/AppThemeLightNavbar"*/)]
    public class AuthAfterActivity : AppCompatActivity
    {
        TextView emailTV, textviewwe, textView1;
        ProgressBar activityIndicator;
        Timer timer_activate;
        Button resendBn, completeLoginBn;
        ImageView imageView1;

        LinearLayout profileLL, dialogsLL, specialistsLL;
        ISharedPreferences dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_dialog;
        UserMethods userMethods = new UserMethods();
        ISharedPreferences pref = Application.Context.GetSharedPreferences("auth_data", FileCreationMode.Private);
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.AuthAfter);
                AuthorizationMethods authorizationMethods = new AuthorizationMethods();

                profileLL = FindViewById<LinearLayout>(Resource.Id.profileLL);
                dialogsLL = FindViewById<LinearLayout>(Resource.Id.dialogsLL);
                specialistsLL = FindViewById<LinearLayout>(Resource.Id.specialistsLL);
                dialogsLL.Click += (s, e) =>
                  {
                      edit_dialog = dialog_data.Edit();
                      edit_dialog.PutString("come_from", "Came directly from bottom");
                      edit_dialog.Apply();
                      StartActivity(typeof(ChatListActivity));
                  };
                specialistsLL.Click += (s, e) =>
                  {
                      StartActivity(typeof(SpecialistsCategoryActivity));
                  };
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                emailTV = FindViewById<TextView>(Resource.Id.emailTV);
                emailTV.Text = pref.GetString("email", String.Empty);
                resendBn = FindViewById<Button>(Resource.Id.resendBn);
                completeLoginBn = FindViewById<Button>(Resource.Id.completeLoginBn);
                textviewwe = FindViewById<TextView>(Resource.Id.textviewwe);
                textView1 = FindViewById<TextView>(Resource.Id.textView1);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);

                resendBn.SetTypeface(tf, TypefaceStyle.Normal);
                completeLoginBn.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.specialistsTV).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.dialogsTV).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.profileTV).SetTypeface(tf, TypefaceStyle.Normal);
                textviewwe.SetTypeface(tf, TypefaceStyle.Normal);
                textView1.SetTypeface(tf, TypefaceStyle.Normal);
                emailTV.SetTypeface(tf, TypefaceStyle.Normal);


                completeLoginBn.Click += async (s, e) =>
                      {
                          completeLoginBn.Visibility = ViewStates.Gone;
                          resendBn.Visibility = ViewStates.Gone;
                          activityIndicator.Visibility = ViewStates.Visible;
                          var activate = await authorizationMethods.AuthActivate(pref.GetString("email", String.Empty), true);
                          completeLoginBn.Visibility = ViewStates.Visible;
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

                resendBn.Click += async (s, e) =>
                {
                    resendBn.Visibility = ViewStates.Gone;
                    completeLoginBn.Visibility = ViewStates.Gone;
                    activityIndicator.Visibility = ViewStates.Visible;
                    var reg_result = await authorizationMethods.Authorize(pref.GetString("email", String.Empty));
                    resendBn.Visibility = ViewStates.Visible;
                    completeLoginBn.Visibility = ViewStates.Visible;
                    activityIndicator.Visibility = ViewStates.Gone;
                    StartActivity(typeof(AuthAfterActivity));
                };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}