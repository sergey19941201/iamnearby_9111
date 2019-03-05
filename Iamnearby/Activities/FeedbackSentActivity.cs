using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, NoHistory = true/*, Theme = "@style/AppThemeLightNavbar"*/)]
    public class FeedbackSentActivity : Activity
    {
        Button continueBn;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.FeedbackSent);

                continueBn = FindViewById<Button>(Resource.Id.continueBn);

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                continueBn.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.infoTV).SetTypeface(tf, TypefaceStyle.Normal);

                continueBn.Click += (s, e) =>
                  {
                      StartActivity(typeof(ReviewsListActivity));
                  };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}