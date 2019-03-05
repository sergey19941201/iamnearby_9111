using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using PCL.Database;
using Iamnearby.Methods;
using System;
using System.Globalization;
using System.Threading;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustNothing)]
    public class ResponseFeedbackActivity : Activity
    {
        ImageView expert_imageIV, onlineIV, star1IV, star2IV, star3IV, star4IV, star5IV;
        TextView expert_nameTV, rating_valueTV, reviewTextTV, dateTV, yesTV, noTV;
        ImageButton back_button;
        RelativeLayout backRelativeLayout, expert_data_layoutRL;
        Button sendBn;
        EditText feedbackET;
        RelativeLayout tintLL;
        Feedbacks feedbacks = new Feedbacks();
        UserMethods userMethods = new UserMethods();
        ProgressBar activityIndicator;
        ISharedPreferences expert_data = Application.Context.GetSharedPreferences("experts", FileCreationMode.Private);
        ISharedPreferencesEditor edit_expert;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.AnswerFeedback);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                expert_data_layoutRL = FindViewById<RelativeLayout>(Resource.Id.expert_data_layoutRL);
                tintLL = (RelativeLayout)FindViewById(Resource.Id.tintLL);
                expert_nameTV = (TextView)FindViewById(Resource.Id.expert_nameTV);
                activityIndicator = (ProgressBar)FindViewById(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);

                star1IV = (ImageView)FindViewById(Resource.Id.star1IV);
                star2IV = (ImageView)FindViewById(Resource.Id.star2IV);
                star3IV = (ImageView)FindViewById(Resource.Id.star3IV);
                star4IV = (ImageView)FindViewById(Resource.Id.star4IV);
                star5IV = (ImageView)FindViewById(Resource.Id.star5IV);
                onlineIV = (ImageView)FindViewById(Resource.Id.onlineIV);
                rating_valueTV = (TextView)FindViewById(Resource.Id.rating_value_TV);
                expert_imageIV = (ImageView)FindViewById(Resource.Id.expert_imageIV);
                reviewTextTV = (TextView)FindViewById(Resource.Id.reviewTextTV);
                dateTV = (TextView)FindViewById(Resource.Id.dateTV);
                yesTV = (TextView)FindViewById(Resource.Id.yesTV);
                noTV = (TextView)FindViewById(Resource.Id.noTV);
                sendBn = (Button)FindViewById(Resource.Id.sendBn);
                feedbackET = (EditText)FindViewById(Resource.Id.feedbackET);

                expert_nameTV.Text = Intent.GetStringExtra("review_name");
                star1IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                star2IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                star3IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                star4IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                star5IV.SetBackgroundResource(Resource.Drawable.disabled_star);

                rating_valueTV.Text = Intent.GetStringExtra("review_rating");
                dateTV.Text = Intent.GetStringExtra("review_date");

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                expert_nameTV.SetTypeface(tf, TypefaceStyle.Bold);
                FindViewById<TextView>(Resource.Id.count_specs).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.rating_value_TV).SetTypeface(tf, TypefaceStyle.Normal);
                dateTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.distanceTV).SetTypeface(tf, TypefaceStyle.Normal);
                reviewTextTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textVssiew1).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textView5).SetTypeface(tf, TypefaceStyle.Normal);
                feedbackET.SetTypeface(tf, TypefaceStyle.Normal);
                sendBn.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.your_city_valueTV).SetTypeface(tf, TypefaceStyle.Normal);
                yesTV.SetTypeface(tf, TypefaceStyle.Normal);
                noTV.SetTypeface(tf, TypefaceStyle.Normal);


                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };

                if (Intent.GetStringExtra("review_online") == "true")
                    onlineIV.Visibility = ViewStates.Visible;
                else
                    onlineIV.Visibility = ViewStates.Gone;

                reviewTextTV.Text = Intent.GetStringExtra("review_text");

                Thread backgroundThread = new Thread(new ThreadStart(() =>
                {
                    Glide.Get(Application.Context).ClearDiskCache();
                }));
                backgroundThread.IsBackground = true;
                backgroundThread.Start();
                Glide.Get(this).ClearMemory();
                if (!Intent.GetStringExtra("review_image_url").Contains("iamn"))
                    Glide.With(Application.Context)
                     .Load("https://api.iamnearby.net/" + Intent.GetStringExtra("review_image_url"))
                     .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
                     .SkipMemoryCache(true))
                     //.Placeholder(Resource.Drawable.specialization_imageIV)
                     .Into(expert_imageIV);
                else
                    Glide.With(Application.Context)
                     .Load(Intent.GetStringExtra("review_image_url"))
                     .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
                     .SkipMemoryCache(true))
                     //.Placeholder(Resource.Drawable.specialization_imageIV)
                     .Into(expert_imageIV);
                double rating_value = 0;
                try
                {
                    rating_value = Convert.ToDouble(Intent.GetStringExtra("review_rating"), (CultureInfo.InvariantCulture));
                    if (rating_value >= 1)
                    {
                        star1IV.SetBackgroundResource(Resource.Drawable.active_star);
                        if (rating_value >= 2)
                        {
                            star2IV.SetBackgroundResource(Resource.Drawable.active_star);
                            if (rating_value >= 3)
                            {
                                star3IV.SetBackgroundResource(Resource.Drawable.active_star);
                                if (rating_value >= 4)
                                {
                                    star4IV.SetBackgroundResource(Resource.Drawable.active_star);
                                    if (rating_value >= 5)
                                    {
                                        star5IV.SetBackgroundResource(Resource.Drawable.active_star);
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }

                sendBn.Click += (s, e) =>
                  {
                      if (!String.IsNullOrEmpty(feedbackET.Text))
                      {
                          tintLL.Visibility = ViewStates.Visible;
                      }
                  };

                yesTV.Click += async (s, e) =>
                  {
                      if (!String.IsNullOrEmpty(feedbackET.Text))
                      {
                          tintLL.Visibility = ViewStates.Gone;
                          activityIndicator.Visibility = ViewStates.Visible;
                          sendBn.Visibility = ViewStates.Gone;
                          var res = await feedbacks.ReplyReview(userMethods.GetUsersAuthToken(), Intent.GetStringExtra("review_id"), feedbackET.Text);
                          activityIndicator.Visibility = ViewStates.Gone;
                          sendBn.Visibility = ViewStates.Visible;
                          if (checkInternetConnection())
                              StartActivity(typeof(MyReviewsListActivity));
                          else
                              Toast.MakeText(this, Resource.String.turn_internet_on, ToastLength.Long).Show();
                      }

                  };
                noTV.Click += (s, e) =>
                  {
                      tintLL.Visibility = ViewStates.Gone;
                  };
                tintLL.Click += (s, e) =>
                  {
                      tintLL.Visibility = ViewStates.Gone;
                  };
                expert_data_layoutRL.Click += (s, e) =>
                  {
                      edit_expert = expert_data.Edit();
                      edit_expert.PutString("expert_id", Intent.GetStringExtra("expertId"));
                      edit_expert.Apply();
                      StartActivity(typeof(ThreeLevelExpertProfileActivity));
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