using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using PCL.Database;
using System;
using System.Globalization;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class ReviewFromPushActivity : Activity
    {
        UserMethods userMethods = new UserMethods();
        LinearLayout specialistsLL, dialogsLL, profileLL, new_reviewLL;
        ImageView expert_imageIV;
        TextView expert_nameTV, reviewTextTV;
        Button replyBn;
        ImageView onlineIV;
        ImageView star1IV, star2IV, star3IV, star4IV, star5IV;
        TextView rating_valueTV;
        TextView dateTV;
        ISharedPreferences dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_dialog;
        ISharedPreferences expert_data = Application.Context.GetSharedPreferences("experts", FileCreationMode.Private);
        ISharedPreferencesEditor edit_expert;
        LinearLayout new_replyLL;
        ImageView my_imageIV, expert_resp_imageIV;
        TextView my_nameTV, my_reviewTextTV, exp_resp_nameTV, expert_responseTextTV, dialogsTV;
        ISharedPreferences expert_feedback_pref = Application.Context.GetSharedPreferences("expert_feedback_pref", FileCreationMode.Private);
        ISharedPreferencesEditor edit_expert_feedback;
        RelativeLayout expdsertdLL, expertLL;
        ImageView message_indicatorIV;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.ReviewFromPush);
                dialogsTV = FindViewById<TextView>(Resource.Id.dialogsTV);
                message_indicatorIV = FindViewById<ImageView>(Resource.Id.message_indicatorIV);
                replyBn = FindViewById<Button>(Resource.Id.replyBn);
                expert_imageIV = FindViewById<ImageView>(Resource.Id.expert_imageIV);
                expert_nameTV = FindViewById<TextView>(Resource.Id.expert_nameTV);
                reviewTextTV = FindViewById<TextView>(Resource.Id.reviewTextTV);
                new_reviewLL = FindViewById<LinearLayout>(Resource.Id.new_reviewLL);
                specialistsLL = FindViewById<LinearLayout>(Resource.Id.specialistsLL);
                dialogsLL = FindViewById<LinearLayout>(Resource.Id.dialogsLL);
                profileLL = FindViewById<LinearLayout>(Resource.Id.profileLL);
                onlineIV = FindViewById<ImageView>(Resource.Id.online_IV);
                star1IV = FindViewById<ImageView>(Resource.Id.star1_IV);
                star2IV = FindViewById<ImageView>(Resource.Id.star2_IV);
                star3IV = FindViewById<ImageView>(Resource.Id.star3_IV);
                star4IV = FindViewById<ImageView>(Resource.Id.star4_IV);
                star5IV = FindViewById<ImageView>(Resource.Id.star5_IV);
                rating_valueTV = FindViewById<TextView>(Resource.Id.rating_value_TV);
                dateTV = FindViewById<TextView>(Resource.Id.date_TV);
                my_imageIV = FindViewById<ImageView>(Resource.Id.my_imageIV);
                expert_resp_imageIV = FindViewById<ImageView>(Resource.Id.expert_resp_imageIV);
                my_nameTV = FindViewById<TextView>(Resource.Id.my_nameTV);
                my_reviewTextTV = FindViewById<TextView>(Resource.Id.my_reviewTextTV);
                exp_resp_nameTV = FindViewById<TextView>(Resource.Id.exp_resp_nameTV);
                expert_responseTextTV = FindViewById<TextView>(Resource.Id.expert_responseTextTV);
                new_replyLL = FindViewById<LinearLayout>(Resource.Id.new_replyLL);
                expdsertdLL = FindViewById<RelativeLayout>(Resource.Id.expdsertdLL);
                expertLL = FindViewById<RelativeLayout>(Resource.Id.expertLL);

                try
                {
                    var msg_cnt_new = Intent.GetStringExtra("msg_cnt_new");
                    if (!String.IsNullOrEmpty(msg_cnt_new) && msg_cnt_new != "0")
                    {
                        message_indicatorIV.Visibility = ViewStates.Visible;
                        dialogsTV.Text = GetString(Resource.String.dialogs) + " (" + msg_cnt_new + ")";
                    }
                    else
                    {
                        message_indicatorIV.Visibility = ViewStates.Gone;
                        dialogsTV.Text = GetString(Resource.String.dialogs);
                    }
                }
                catch
                {
                    message_indicatorIV.Visibility = ViewStates.Gone;
                    dialogsTV.Text = GetString(Resource.String.dialogs);
                }

                specialistsLL.Click += (s, e) =>
                {
                    StartActivity(typeof(SpecialistsCategoryActivity));
                };
                dialogsLL.Click += (s, e) =>
                {
                    edit_dialog = dialog_data.Edit();
                    edit_dialog.PutString("come_from", "Came directly from bottom");
                    edit_dialog.Apply();
                    StartActivity(typeof(ChatListActivity));
                };
                profileLL.Click += (s, e) =>
                {
                    if (userMethods.UserExists())
                        StartActivity(typeof(UserProfileActivity));
                };

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.specialistsTV).SetTypeface(tf, TypefaceStyle.Normal);
                dialogsTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.profileTV).SetTypeface(tf, TypefaceStyle.Normal);
                expert_nameTV.SetTypeface(tf, TypefaceStyle.Bold);
                FindViewById<TextView>(Resource.Id.rating_value_TV).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.date_TV).SetTypeface(tf, TypefaceStyle.Normal);
                reviewTextTV.SetTypeface(tf, TypefaceStyle.Normal);
                my_nameTV.SetTypeface(tf, TypefaceStyle.Normal);
                my_reviewTextTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.my_saasd).SetTypeface(tf, TypefaceStyle.Normal);
                exp_resp_nameTV.SetTypeface(tf, TypefaceStyle.Bold);
                expert_responseTextTV.SetTypeface(tf, TypefaceStyle.Normal);
                replyBn.SetTypeface(tf, TypefaceStyle.Normal);

                var type = Intent.GetStringExtra("type");
                var review_id = Intent.GetStringExtra("review_id");
                var reviewText = Intent.GetStringExtra("reviewText");
                var companionId = Intent.GetStringExtra("companionId");
                var companionFullName = Intent.GetStringExtra("companionFullName");
                var companionAvatar = Intent.GetStringExtra("companionAvatar");
                var myFullName = Intent.GetStringExtra("myFullName");
                var myAvatar = Intent.GetStringExtra("myAvatar");
                var reviewAnswer = Intent.GetStringExtra("reviewAnswer");
                var review_rating = Intent.GetStringExtra("review_rating");
                var review_date = Intent.GetStringExtra("review_date");
                var review_online = Intent.GetStringExtra("review_online");
                var categoryId = Intent.GetStringExtra("categoryId");
                var myId = Intent.GetStringExtra("myId");
                if (type == "review")
                {
                    new_reviewLL.Visibility = ViewStates.Visible;
                    Glide.With(this)
                     .Load(companionAvatar)
                     .Into(expert_imageIV);
                    expert_nameTV.Text = companionFullName;
                    reviewTextTV.Text = reviewText;

                    star1IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                    star2IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                    star3IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                    star4IV.SetBackgroundResource(Resource.Drawable.disabled_star);
                    star5IV.SetBackgroundResource(Resource.Drawable.disabled_star);

                    rating_valueTV.Text = Intent.GetStringExtra("review_rating");
                    dateTV.Text = Intent.GetStringExtra("review_date");
                    if (Intent.GetStringExtra("review_online") == "true")
                        onlineIV.Visibility = ViewStates.Visible;
                    else
                        onlineIV.Visibility = ViewStates.Gone;

                    double rating_value = 0;
                    try
                    {
                        rating_value = Convert.ToDouble(review_rating, (CultureInfo.InvariantCulture));
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

                    replyBn.Click += delegate
                      {
                          edit_expert_feedback = expert_feedback_pref.Edit();
                          edit_expert_feedback.PutString("expert_id", myId);
                          edit_expert_feedback.PutString("expert_category_id", categoryId);
                          edit_expert_feedback.Apply();
                          var activity2 = new Intent(this, typeof(ResponseFeedbackActivity));
                          activity2.PutExtra("expertId", companionId);
                          activity2.PutExtra("review_id", review_id);
                          activity2.PutExtra("review_rating", review_rating);
                          activity2.PutExtra("review_date", review_date);
                          activity2.PutExtra("review_online", review_online);
                          activity2.PutExtra("review_text", reviewText);
                          activity2.PutExtra("review_image_url", companionAvatar);
                          activity2.PutExtra("review_name", companionFullName);
                          StartActivity(activity2);
                      };
                }
                else if (type == "review_answer")
                {
                    new_replyLL.Visibility = ViewStates.Visible;
                    Glide.With(this)
                     .Load(companionAvatar)
                     .Into(expert_resp_imageIV);
                    Glide.With(this)
                     .Load(myAvatar)
                     .Into(my_imageIV);
                    my_nameTV.Text = myFullName;
                    my_reviewTextTV.Text = reviewText;
                    exp_resp_nameTV.Text = companionFullName;
                    expert_responseTextTV.Text = reviewAnswer;
                }
                else
                {
                    StartActivity(typeof(MainActivity));
                }
                expdsertdLL.Click += ExpdsertdLL_Click;
                expertLL.Click += ExpdsertdLL_Click;
                void ExpdsertdLL_Click(object sender, EventArgs e)
                {
                    edit_expert = expert_data.Edit();
                    edit_expert.PutString("expert_id", companionId);
                    edit_expert.Apply();
                    StartActivity(typeof(ThreeLevelExpertProfileActivity));
                }
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}