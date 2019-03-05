using System.Threading;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;

namespace Iamnearby.Activities
{
    [Activity(Label = "DisplayPortfolioPhotoExpertActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class DisplayPortfolioPhotoExpertActivity : Activity
    {
        RelativeLayout backRelativeLayout;
        ImageButton back_button;
        Button removeBn;
        ImageView fullIV;
        ProgressBar activityIndicator;
        public static string image_url;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.DisplayPortfolioPhoto);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                removeBn = FindViewById<Button>(Resource.Id.removeBn);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                fullIV = FindViewById<ImageView>(Resource.Id.fullIV);
                removeBn.Visibility = ViewStates.Gone;

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                removeBn.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textView5).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.your_city_valueTV).SetTypeface(tf, TypefaceStyle.Normal);

                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };

                Thread backgroundThread = new Thread(new ThreadStart(() =>
                {
                    Glide.Get(Application.Context).ClearDiskCache();
                }));
                backgroundThread.IsBackground = true;
                backgroundThread.Start();
                Glide.Get(this).ClearMemory();
                if (!string.IsNullOrEmpty(image_url))
                    Glide.With(Application.Context)
                        .Load(image_url)
                        .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
                        .SkipMemoryCache(true))
                        //.Placeholder(Resource.Drawable.specialization_imageIV)
                        .Into(fullIV);
                image_url = string.Empty;
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}