using System;
using Android.App;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Iamnearby.Methods;
using PCL.Database;

namespace Iamnearby.ViewHolders
{
    public class UserProfileViewHolder : RecyclerView.ViewHolder
    {
        public ImageView profile_image { get; set; }
        public ImageView choose_photoIV { get; set; }
        public ImageView settingsIV { get; set; }
        public ImageView add_skillIV { get; set; }
        public ImageView edit_skillIV { get; set; }
        public RelativeLayout headerRL { get; set; }
        public LinearLayout gridLL { get; set; }
        public LinearLayout tintLL { get; set; }
        public Button editBn { get; set; }
        public Switch onlyWithReviewsS { get; set; }
        public TextView onlineValueTV { get; set; }
        public LinearLayout go_to_my_reviewsLL { get; set; }
        public RelativeLayout linearLayout3 { get; set; }
        public ImageView star1IV { get; set; }
        public ImageView star2IV { get; set; }
        public ImageView star3IV { get; set; }
        public ImageView star4IV { get; set; }
        public ImageView star5IV { get; set; }
        public TextView rating_valueTV { get; set; }
        public TextView expertNameTV { get; set; }
        public TextView skillNameTV { get; set; }
        public TextView description_text_dataTV { get; set; }
        public TextView reviewCountTV { get; set; }
        public CardView itemCV { get; set; }
        public RelativeLayout feedbackRL { get; set; }
        public TextView dsadas { get; set; }
        public TextView sdsddsddddwww { get; set; }
        public TextView textView1 { get; set; }
        public TextView textVsiew1 { get; set; }
        public RelativeLayout edit_skillRL { get; set; }
        UserMethods userMethods = new UserMethods();
        ProfileAndExpertMethods profileAndExpertMethods = new ProfileAndExpertMethods();
        Activity _context;
        public UserProfileViewHolder(View itemView, Action<int> listener, Action<int> choose_photoIVClickListener, Action<int> settingsClick, Action<int> editClick, Action<int> editSkillClick, Action<int> feedbackClick, Action<int> OnImageClick, Action<int> GoToReviewsClick, Activity _context) : base(itemView)
        {
            this._context = _context;
            go_to_my_reviewsLL = itemView.FindViewById<LinearLayout>(Resource.Id.go_to_my_reviewsLL);
            profile_image = itemView.FindViewById<ImageView>(Resource.Id.profile_image);
            choose_photoIV = itemView.FindViewById<ImageView>(Resource.Id.choose_photoIV);
            settingsIV = itemView.FindViewById<ImageView>(Resource.Id.settingsIV);
            add_skillIV = itemView.FindViewById<ImageView>(Resource.Id.add_skillIV);
            edit_skillIV = itemView.FindViewById<ImageView>(Resource.Id.edit_skillIV);
            edit_skillRL = itemView.FindViewById<RelativeLayout>(Resource.Id.edit_skillRL);
            headerRL = itemView.FindViewById<RelativeLayout>(Resource.Id.headerRL);
            gridLL = itemView.FindViewById<LinearLayout>(Resource.Id.gridLL);
            tintLL = itemView.FindViewById<LinearLayout>(Resource.Id.tintLL);
            editBn = itemView.FindViewById<Button>(Resource.Id.editBn);
            onlyWithReviewsS = itemView.FindViewById<Switch>(Resource.Id.onlyWithReviewsS);
            onlineValueTV = itemView.FindViewById<TextView>(Resource.Id.onlineValue_TV);
            star1IV = itemView.FindViewById<ImageView>(Resource.Id.star1IV);
            star2IV = itemView.FindViewById<ImageView>(Resource.Id.star2IV);
            star3IV = itemView.FindViewById<ImageView>(Resource.Id.star3IV);
            star4IV = itemView.FindViewById<ImageView>(Resource.Id.star4IV);
            star5IV = itemView.FindViewById<ImageView>(Resource.Id.star5IV);
            rating_valueTV = itemView.FindViewById<TextView>(Resource.Id.rating_valueTV);
            expertNameTV = itemView.FindViewById<TextView>(Resource.Id.expertNameTV);
            feedbackRL = itemView.FindViewById<RelativeLayout>(Resource.Id.feedbackRL);
            skillNameTV = itemView.FindViewById<TextView>(Resource.Id.skillNameTV);
            description_text_dataTV = itemView.FindViewById<TextView>(Resource.Id.description_text_dataTV);
            reviewCountTV = itemView.FindViewById<TextView>(Resource.Id.reviewCountTV);
            dsadas = itemView.FindViewById<TextView>(Resource.Id.dsadas);
            sdsddsddddwww = itemView.FindViewById<TextView>(Resource.Id.sdsddsddddwww);
            textView1 = itemView.FindViewById<TextView>(Resource.Id.textView1);
            textVsiew1 = itemView.FindViewById<TextView>(Resource.Id.textVsiew1);
            linearLayout3 = itemView.FindViewById<RelativeLayout>(Resource.Id.linearLayout31);
            choose_photoIV.Click += (sender, e) => choose_photoIVClickListener(base.AdapterPosition);
            itemView.Click += (s, e) => listener(Position);
            settingsIV.Click += (s, e) => settingsClick(Position);
            editBn.Click += (s, e) => editClick(Position);
            edit_skillRL.Click += (s, e) => editSkillClick(Position);
            feedbackRL.Click += (s, e) => feedbackClick(Position);
            profile_image.Click += (s, e) => OnImageClick(Position);
            onlyWithReviewsS.CheckedChange += async delegate
              {
                  if (onlyWithReviewsS.Checked)
                  {
                      onlineValueTV.Text = _context.GetString(Resource.String.online_text);
                      onlineValueTV.SetTextColor(Color.Green);
                      var res = await profileAndExpertMethods.EditMyOnline(userMethods.GetUsersAuthToken(), true);
                  }
                  else
                  {
                      onlineValueTV.SetTextColor(Color.Red);
                      onlineValueTV.Text = _context.GetString(Resource.String.offline_text);
                      var res = await profileAndExpertMethods.EditMyOnline(userMethods.GetUsersAuthToken(), false);
                  }
              };
            go_to_my_reviewsLL.Click += (s, e) => GoToReviewsClick(Position);
        }
    }
}