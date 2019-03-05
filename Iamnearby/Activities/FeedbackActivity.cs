using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Com.Bumptech.Glide;
using Iamnearby.Methods;
using PCL.Database;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait/*, WindowSoftInputMode = SoftInput.AdjustNothing*/)]
    public class FeedbackActivity : Activity
    {
        ISharedPreferences expert_feedback_pref = Application.Context.GetSharedPreferences("expert_feedback_pref", FileCreationMode.Private);
        UserMethods userMethods = new UserMethods();
        Feedbacks feedbacks = new Feedbacks();
        TextView expert_nameTV, phoneTV, specializationTV;
        ImageView expert_imageIV, onlineIV;
        ImageButton back_button;
        RelativeLayout backRelativeLayout, expert_data_layoutRL;
        ImageView star1IV, star2IV, star3IV, star4IV, star5IV;
        EditText feedbackET;
        ProgressBar activityIndicator;
        Button sendBn;
        ISharedPreferences expert_data = Application.Context.GetSharedPreferences("experts", FileCreationMode.Private);
        ISharedPreferencesEditor edit_expert;


        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.Feedback);
                expert_nameTV = FindViewById<TextView>(Resource.Id.expert_nameTV);
                expert_data_layoutRL = FindViewById<RelativeLayout>(Resource.Id.expert_data_layoutRL);
                phoneTV = FindViewById<TextView>(Resource.Id.phoneTV);
                specializationTV = FindViewById<TextView>(Resource.Id.specializationTV);
                expert_imageIV = FindViewById<ImageView>(Resource.Id.expert_imageIV);
                star1IV = FindViewById<ImageView>(Resource.Id.star1IV);
                star2IV = FindViewById<ImageView>(Resource.Id.star2IV);
                star3IV = FindViewById<ImageView>(Resource.Id.star3IV);
                star4IV = FindViewById<ImageView>(Resource.Id.star4IV);
                star5IV = FindViewById<ImageView>(Resource.Id.star5IV);
                onlineIV = FindViewById<ImageView>(Resource.Id.onlineIV);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                feedbackET = FindViewById<EditText>(Resource.Id.feedbackET);
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                sendBn = FindViewById<Button>(Resource.Id.sendBn);
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                expert_nameTV.SetTypeface(tf, TypefaceStyle.Bold);
                FindViewById<TextView>(Resource.Id.count_specs).SetTypeface(tf, TypefaceStyle.Normal);
                phoneTV.SetTypeface(tf, TypefaceStyle.Normal);
                specializationTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.distanceTV).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textView111).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textVwiew1).SetTypeface(tf, TypefaceStyle.Normal);
                feedbackET.SetTypeface(tf, TypefaceStyle.Normal);
                sendBn.SetTypeface(tf, TypefaceStyle.Normal);

                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                string[] name = expert_feedback_pref.GetString("expert_name", String.Empty).Split(new char[] { ' ' });
                expert_nameTV.Text = name[0];
                phoneTV.Text = expert_feedback_pref.GetString("expert_phone", String.Empty);
                specializationTV.Text = expert_feedback_pref.GetString("expert_category_name", String.Empty);
                if (expert_feedback_pref.GetBoolean("expert_online", false))
                    onlineIV.Visibility = ViewStates.Visible;
                else
                    onlineIV.Visibility = ViewStates.Gone;
                Thread backgroundThread = new Thread(new ThreadStart(() =>
                {
                    Glide.Get(Application.Context).ClearDiskCache();
                }));
                backgroundThread.IsBackground = true;
                backgroundThread.Start();
                Glide.Get(this).ClearMemory();


                var expert_avatar = expert_feedback_pref.GetString("expert_avatar", String.Empty);
                if (!expert_avatar.Contains("api.iam"))
                    Glide.With(Application.Context)
                        .Load("https://api.iamnearby.net/" + expert_avatar)
                        .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
                        .SkipMemoryCache(true))
                        //.Placeholder(Resource.Drawable.specialization_imageIV)
                        .Into(expert_imageIV);
                else
                    Glide.With(Application.Context)
                        .Load(expert_avatar)
                        .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
                        .SkipMemoryCache(true))
                        //.Placeholder(Resource.Drawable.specialization_imageIV)
                        .Into(expert_imageIV);

                int mark = 0;

                star1IV.Click += (s, e) =>
                {
                    star1IV.SetBackgroundResource(Resource.Drawable.active_star);
                    star2IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                    star3IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                    star4IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                    star5IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                    mark = 1;
                };
                star2IV.Click += (s, e) =>
                {
                    star1IV.SetBackgroundResource(Resource.Drawable.active_star);
                    star2IV.SetBackgroundResource(Resource.Drawable.active_star);
                    star3IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                    star4IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                    star5IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                    mark = 2;
                };
                star3IV.Click += (s, e) =>
                {
                    star1IV.SetBackgroundResource(Resource.Drawable.active_star);
                    star2IV.SetBackgroundResource(Resource.Drawable.active_star);
                    star3IV.SetBackgroundResource(Resource.Drawable.active_star);
                    star4IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                    star5IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                    mark = 3;
                };
                star4IV.Click += (s, e) =>
                {
                    star1IV.SetBackgroundResource(Resource.Drawable.active_star);
                    star2IV.SetBackgroundResource(Resource.Drawable.active_star);
                    star3IV.SetBackgroundResource(Resource.Drawable.active_star);
                    star4IV.SetBackgroundResource(Resource.Drawable.active_star);
                    star5IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                    mark = 4;
                };
                star5IV.Click += (s, e) =>
                {
                    star1IV.SetBackgroundResource(Resource.Drawable.active_star);
                    star2IV.SetBackgroundResource(Resource.Drawable.active_star);
                    star3IV.SetBackgroundResource(Resource.Drawable.active_star);
                    star4IV.SetBackgroundResource(Resource.Drawable.active_star);
                    star5IV.SetBackgroundResource(Resource.Drawable.active_star);
                    mark = 5;
                };
                feedbackET.EditorAction += (object sender, EditText.EditorActionEventArgs e) =>
                {
                    imm.HideSoftInputFromWindow(feedbackET.WindowToken, 0);
                };
                sendBn.Click += async (s, e) =>
                  {
                      if (mark > 0)
                      {
                          if (!String.IsNullOrEmpty(feedbackET.Text))
                          {
                              activityIndicator.Visibility = ViewStates.Visible;
                              sendBn.Visibility = ViewStates.Gone;

                              var send_new_review = await feedbacks.SendNewReview(userMethods.GetUsersAuthToken(), expert_feedback_pref.GetString("expert_id", String.Empty), feedbackET.Text, expert_feedback_pref.GetString("expert_category_id", String.Empty), mark.ToString());
                              activityIndicator.Visibility = ViewStates.Gone;
                              sendBn.Visibility = ViewStates.Visible;

                              if (!send_new_review.ToLower().Contains("ельзя оставить от"))
                              {
                                  if (checkInternetConnection())
                                      StartActivity(typeof(FeedbackSentActivity));
                                  else
                                      Toast.MakeText(this, Resource.String.turn_internet_on, ToastLength.Long).Show();
                              }
                              else
                                  Toast.MakeText(this, GetString(Resource.String.cannot_leave_feedback_for_me), ToastLength.Short).Show();
                          }
                      }
                      else
                          Toast.MakeText(this, GetString(Resource.String.leave_rating_please), ToastLength.Short).Show();
                  };
                expert_data_layoutRL.Click += (s, e) =>
                  {
                      edit_expert = expert_data.Edit();
                      edit_expert.PutString("expert_id", expert_feedback_pref.GetString("expert_id", String.Empty));
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