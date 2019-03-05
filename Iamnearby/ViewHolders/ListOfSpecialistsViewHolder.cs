using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Iamnearby.ViewHolders
{
    public class ListOfSpecialistsViewHolder : RecyclerView.ViewHolder
    {
        public TextView expert_nameTV { get; set; }
        public TextView phoneTV { get; set; }
        public TextView rating_valueTV { get; set; }
        public TextView distanceTV { get; set; }
        public ImageView expert_imageIV { get; set; }
        public ImageView distanceIV { get; set; }
        public ImageView onlineIV { get; set; }
        public ImageView star1IV { get; set; }
        public ImageView star2IV { get; set; }
        public ImageView star3IV { get; set; }
        public ImageView star4IV { get; set; }
        public ImageView star5IV { get; set; }
        public Button writeBn { get; set; }

        public ListOfSpecialistsViewHolder(View itemView, Action<int> listener, Action<int> writeBnClick) : base(itemView)
        {
            onlineIV = itemView.FindViewById<ImageView>(Resource.Id.onlineIV);
            expert_nameTV = itemView.FindViewById<TextView>(Resource.Id.expert_nameTV);
            phoneTV = itemView.FindViewById<TextView>(Resource.Id.phoneTV);
            rating_valueTV = itemView.FindViewById<TextView>(Resource.Id.rating_valueTV);
            distanceTV = itemView.FindViewById<TextView>(Resource.Id.distanceTV);
            expert_imageIV = itemView.FindViewById<ImageView>(Resource.Id.expert_imageIV);
            distanceIV = itemView.FindViewById<ImageView>(Resource.Id.distanceIV);
            star1IV = itemView.FindViewById<ImageView>(Resource.Id.star1IV);
            star2IV = itemView.FindViewById<ImageView>(Resource.Id.star2IV);
            star3IV = itemView.FindViewById<ImageView>(Resource.Id.star3IV);
            star4IV = itemView.FindViewById<ImageView>(Resource.Id.star4IV);
            star5IV = itemView.FindViewById<ImageView>(Resource.Id.star5IV);
            writeBn = itemView.FindViewById<Button>(Resource.Id.writeBn);
            writeBn.Click += (s, e) => writeBnClick(Position);
            itemView.Click += (s, e) => listener(Position);
        }
    }
}