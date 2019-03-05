using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class AboutActivity : Activity
    {
        ImageButton back_button;
        RelativeLayout backRelativeLayout;
        LinearLayout notissimusLL;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.About);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                notissimusLL = FindViewById<LinearLayout>(Resource.Id.notissimusLL);

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                FindViewById<TextView>(Resource.Id.dsadas).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.description_text_dataTV).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.reviewCountTV).SetTypeface(tf, TypefaceStyle.Normal);

                notissimusLL.Click += delegate
                {
                    var uri = Android.Net.Uri.Parse("https://notissimus.com");
                    var intent = new Intent(Intent.ActionView, uri);
                    StartActivity(intent);
                };
                back_button.Click += (s, e) => { OnBackPressed(); };
                backRelativeLayout.Click += (s, e) => { OnBackPressed(); };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}