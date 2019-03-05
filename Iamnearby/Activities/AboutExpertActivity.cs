using System;

using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;

namespace Iamnearby.Activities
{
    [Activity(Label = "AboutExpertActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class AboutExpertActivity : Activity
    {
        RelativeLayout backRelativeLayout;
        ImageButton back_button;
        TextView infoTV;
        ProgressBar activityIndicator;
        public static string InfoText;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.AboutExpertInfo);

                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                infoTV = FindViewById<TextView>(Resource.Id.infoTV);
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                infoTV = FindViewById<TextView>(Resource.Id.infoTV);
                infoTV.Text = InfoText;
                InfoText = String.Empty;

                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}