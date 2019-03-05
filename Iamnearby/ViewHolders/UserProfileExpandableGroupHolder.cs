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
    public class UserProfileExpandableGroupHolder : RecyclerView.ViewHolder
    {
        public TextView dsadas { get; set; }
        public TextView sdsddsddddwww { get; set; }
        public TextView sdsddsddddwwwwww { get; set; }

        public ImageView profile_image { get; set; }
        public RelativeLayout itemRL { set; get; }
        public ImageView profile_imageIV { set; get; }
        public ImageView onlineIV { set; get; }
        public ImageView settingsIV { get; set; }
        public TextView onlineValueTV { set; get; }
        public RelativeLayout headerRL { set; get; }
        public Switch onlyWithReviewsS { get; set; }
        public TextView rating_valueTV { get; set; }
        public ImageView choose_photoIV { get; set; }
        //public TextView txtIdNo { set; get; }
        public TextView skillNameTV { set; get; }
        public TextView expert_nameTV { set; get; }
        public TextView expertSurnameTV { set; get; }
        public TextView expert_phoneTV { set; get; }
        public TextView cityTV { set; get; }
        public TextView distanceTV { set; get; }
        public TextView reviewCountTV { set; get; }
        public ImageView bttGroup { set; get; }
        public Activity context { set; get; }
        public RelativeLayout writeRL { set; get; }
        public RelativeLayout callRL { set; get; }
        public TextView writeTV { set; get; }
        public TextView callTV { set; get; }
        public TextView textView3 { set; get; }
        public LinearLayout bottom_separatorLL { set; get; }
        public ImageView star1IV { get; set; }
        public ImageView star2IV { get; set; }
        public ImageView star3IV { get; set; }
        public ImageView star4IV { get; set; }
        public ImageView star5IV { get; set; }
        public Button editBn { get; set; }
        public RelativeLayout feedbackRL { get; set; }
        public RelativeLayout go_to_my_reviewsLL { get; set; }
        public RelativeLayout aboutMeRL { get; set; }
        ProfileAndExpertMethods profileAndExpertMethods = new ProfileAndExpertMethods();
        UserMethods userMethods = new UserMethods();

        public UserProfileExpandableGroupHolder(View view,
                                                Action<int> groupClickListener,
                                                Action<int> settingsClick,
                                                Action<int> editClick,
                                                Action<int> choose_photoIVClickListener,
                                                Action<int> OnImageClick,
                                                Action<int> feedbackClick,
                                                Action<int> reviews_by_meClick,
                                                Action<int> about_meClick,
                                                Activity context) : base(view)
        {
            profile_image = view.FindViewById<ImageView>(Resource.Id.profile_image);
            this.bttGroup = (ImageView)view.FindViewById(Resource.Id.group_bttClick);
            this.itemRL = (RelativeLayout)view.FindViewById(Resource.Id.itemRL);
            this.headerRL = (RelativeLayout)view.FindViewById(Resource.Id.headerRL);
            this.profile_imageIV = (ImageView)view.FindViewById(Resource.Id.profile_image);
            this.skillNameTV = (TextView)view.FindViewById(Resource.Id.skillNameTV);
            this.expert_nameTV = (TextView)view.FindViewById(Resource.Id.expertNameTV);
            this.expertSurnameTV = (TextView)view.FindViewById(Resource.Id.expertSurnameTV);
            this.expert_phoneTV = (TextView)view.FindViewById(Resource.Id.expert_phoneTV);
            this.cityTV = (TextView)view.FindViewById(Resource.Id.cityTV);
            this.distanceTV = (TextView)view.FindViewById(Resource.Id.distanceTV);
            this.onlineIV = (ImageView)view.FindViewById(Resource.Id.onlineIV);
            this.onlineValueTV = (TextView)view.FindViewById(Resource.Id.onlineValue_TV);
            this.reviewCountTV = (TextView)view.FindViewById(Resource.Id.reviewCountTV);
            this.onlyWithReviewsS = (Switch)view.FindViewById(Resource.Id.onlyWithReviewsS);
            aboutMeRL = (RelativeLayout)view.FindViewById(Resource.Id.aboutMeRL);

            dsadas = (TextView)view.FindViewById(Resource.Id.dsadas);
            sdsddsddddwww = (TextView)view.FindViewById(Resource.Id.sdsddsddddwww);
            sdsddsddddwwwwww = (TextView)view.FindViewById(Resource.Id.sdsddsddddwwwwww);

            feedbackRL = (RelativeLayout)view.FindViewById(Resource.Id.feedbackRL);
            go_to_my_reviewsLL = (RelativeLayout)view.FindViewById(Resource.Id.go_to_my_reviewsLL);
            choose_photoIV = view.FindViewById<ImageView>(Resource.Id.choose_photoIV);
            editBn = view.FindViewById<Button>(Resource.Id.editBn);
            settingsIV = view.FindViewById<ImageView>(Resource.Id.settingsIV);
            star1IV = view.FindViewById<ImageView>(Resource.Id.star1IV);
            star2IV = view.FindViewById<ImageView>(Resource.Id.star2IV);
            star3IV = view.FindViewById<ImageView>(Resource.Id.star3IV);
            star4IV = view.FindViewById<ImageView>(Resource.Id.star4IV);
            star5IV = view.FindViewById<ImageView>(Resource.Id.star5IV);
            rating_valueTV = view.FindViewById<TextView>(Resource.Id.rating_valueTV);
            bottom_separatorLL = (LinearLayout)view.FindViewById(Resource.Id.bottom_separatorLL);
            writeRL = (RelativeLayout)view.FindViewById(Resource.Id.writeRL);
            callRL = (RelativeLayout)view.FindViewById(Resource.Id.callRL);
            writeTV = (TextView)view.FindViewById(Resource.Id.writeTV);
            callTV = (TextView)view.FindViewById(Resource.Id.callTV);
            textView3 = (TextView)view.FindViewById(Resource.Id.textView3);
            this.context = context;
            view.Click += (sender, e) => groupClickListener(base.AdapterPosition);
            settingsIV.Click += (sender, e) => settingsClick(base.AdapterPosition);
            editBn.Click += (s, e) => editClick(Position);
            choose_photoIV.Click += (s, e) => choose_photoIVClickListener(Position);
            profile_image.Click += (s, e) => OnImageClick(Position);
            feedbackRL.Click += (s, e) => feedbackClick(Position);
            go_to_my_reviewsLL.Click += (s, e) => reviews_by_meClick(Position);
            aboutMeRL.Click += (s, e) => about_meClick(Position);

            view.Click += view_Click;
            onlyWithReviewsS.CheckedChange += async delegate
            {
                if (onlyWithReviewsS.Checked)
                {
                    onlineValueTV.Text = context.GetString(Resource.String.online_text);
                    onlineValueTV.SetTextColor(Color.Green);
                    var res = await profileAndExpertMethods.EditMyOnline(userMethods.GetUsersAuthToken(), true);
                }
                else
                {
                    onlineValueTV.SetTextColor(Color.Red);
                    onlineValueTV.Text = context.GetString(Resource.String.offline_text);
                    var res = await profileAndExpertMethods.EditMyOnline(userMethods.GetUsersAuthToken(), false);
                }
            };
        }

        private void view_Click(object sender, EventArgs e)
        {
            if (bttGroup.Rotation != 180)
            {
                bttGroup.Rotation = 180;
                //itemRL.SetBackgroundColor(new Color(ContextCompat.GetColor(context, Resource.Color.mainSeparatorColor)));
            }
            else
            {
                bttGroup.Rotation = 0;
                //itemRL.SetBackgroundColor(new Color(ContextCompat.GetColor(context, Resource.Color.transparent)));
            }
        }
    }
}