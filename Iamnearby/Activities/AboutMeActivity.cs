using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Iamnearby.Methods;
using PCL.Database;

namespace Iamnearby.Activities
{
    [Activity(Label = "AboutMeActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class AboutMeActivity : Activity
    {
        RelativeLayout backRelativeLayout;
        ImageButton back_button;
        Button applyBn;
        EditText deskrET;
        ProgressBar activityIndicator;
        public static string InfoText;
        ProfileAndExpertMethods profileAndExpertMethods = new ProfileAndExpertMethods();
        UserMethods userMethods = new UserMethods();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.AboutMeAddInfo);

                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                applyBn = FindViewById<Button>(Resource.Id.applyBn);
                deskrET = FindViewById<EditText>(Resource.Id.deskrET);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                deskrET.SetTypeface(tf, TypefaceStyle.Normal);
                applyBn.SetTypeface(tf, TypefaceStyle.Normal);

                deskrET.Text = InfoText;
                InfoText = String.Empty;

                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                applyBn.Click += async (s, e) =>
                {
                    applyBn.Visibility = ViewStates.Gone;
                    activityIndicator.Visibility = ViewStates.Visible;
                    var res = profileAndExpertMethods.AboutExpert(userMethods.GetUsersAuthToken(), deskrET.Text);
                    applyBn.Visibility = ViewStates.Visible;
                    activityIndicator.Visibility = ViewStates.Gone;
                    OnBackPressed();
                };
                deskrET.EditorAction += (object sender, EditText.EditorActionEventArgs e) =>
                {
                    imm.HideSoftInputFromWindow(deskrET.WindowToken, 0);
                };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}