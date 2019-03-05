using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;

namespace Iamnearby.Activities
{
    [Activity(Label = "AddOneMoreSpecActivity", ScreenOrientation = ScreenOrientation.Portrait/*, Theme = "@style/AppThemeLightNavbar"*/)]
    public class AddOneMoreSpecActivity : Activity
    {
        Button add_specBn, not_nowBn;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.AddOneMoreSpec);
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                add_specBn = FindViewById<Button>(Resource.Id.add_specBn);
                not_nowBn = FindViewById<Button>(Resource.Id.not_nowBn);
                add_specBn.SetTypeface(tf, TypefaceStyle.Normal);
                not_nowBn.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textView1).SetTypeface(tf, TypefaceStyle.Normal);
                add_specBn.Click += (s, e) =>
                  {
                      StartActivity(typeof(YourSpecializationActivity));
                  };
                not_nowBn.Click += (s, e) =>
                  {
                      StartActivity(typeof(RegEmailActivity));
                  };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}