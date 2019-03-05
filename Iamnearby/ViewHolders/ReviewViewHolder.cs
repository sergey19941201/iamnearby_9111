using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Iamnearby.ViewHolders
{
    public class ReviewViewHolder : RecyclerView.ViewHolder
    {
        public TextView expert_nameTV { get; set; }
        public Button replyBn { get; set; }
        public ImageView expert_imageIV { get; set; }
        public ImageView onlineIV { get; set; }
        public ImageView star1IV, star2IV, star3IV, star4IV, star5IV;
        public TextView rating_valueTV { set; get; }
        public TextView dateTV { get; set; }
        public TextView reviewTextTV { get; set; }
        public TextView replyTextTV { get; set; }
        public TextView distanceTV { get; set; }

        public ReviewViewHolder(View view, Action<int> replyClickListener, Action<int> viewClickListener) : base(view)
        {
            expert_nameTV = (TextView)view.FindViewById(Resource.Id.expert_nameTV);
            replyBn = (Button)view.FindViewById(Resource.Id.replyBn);
            star1IV = (ImageView)view.FindViewById(Resource.Id.star1IV);
            star2IV = (ImageView)view.FindViewById(Resource.Id.star2IV);
            star3IV = (ImageView)view.FindViewById(Resource.Id.star3IV);
            star4IV = (ImageView)view.FindViewById(Resource.Id.star4IV);
            star5IV = (ImageView)view.FindViewById(Resource.Id.star5IV);
            onlineIV = (ImageView)view.FindViewById(Resource.Id.onlineIV);
            rating_valueTV = (TextView)view.FindViewById(Resource.Id.rating_value_TV);
            expert_imageIV = (ImageView)view.FindViewById(Resource.Id.expert_imageIV);
            reviewTextTV = (TextView)view.FindViewById(Resource.Id.reviewTextTV);
            dateTV = (TextView)view.FindViewById(Resource.Id.dateTV);
            distanceTV = (TextView)view.FindViewById(Resource.Id.distanceTV);
            replyTextTV = (TextView)view.FindViewById(Resource.Id.replyTextTV);
            replyBn.Click += (s, e) => replyClickListener(base.AdapterPosition);
            view.Click += (s, e) => viewClickListener(base.AdapterPosition);
            //this.bttEntry = (Button)view.FindViewById(Resource.Id.entry_bttClick);

            //bttEntry.Click += (sender, e) => bttEntryClickListener(base.AdapterPosition);
        }
    }
}