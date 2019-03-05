using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;

namespace Iamnearby.ViewHolders
{
    public class UserSpecializationViewHolder:RecyclerView.ViewHolder
    {
        public TextView specialization_nameTV { get; set; }
        public ImageView specialization_imageIV { get; set; }

        public UserSpecializationViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            specialization_nameTV = itemView.FindViewById<TextView>(Resource.Id.specialization_nameTV);
            specialization_imageIV = itemView.FindViewById<ImageView>(Resource.Id.specialization_imageIV);
            itemView.Click += (s, e) => listener(Position);
        }
    }
}