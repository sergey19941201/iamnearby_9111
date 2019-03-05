using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Iamnearby.ViewHolders
{
    public class DialogsListViewHolder : RecyclerView.ViewHolder
    {
        public TextView expert_nameTV { get; set; }
        public TextView specializationTV { get; set; }
        public TextView messageTV { get; set; }
        public TextView timeTV { get; set; }
        public TextView messageCountTV { get; set; }
        public ImageView expert_imageIV { get; set; }
        public ImageView onlineIV { get; set; }
        public RelativeLayout messages_countRL { get; set; }
        public RelativeLayout linearLayout3112 { get; set; }

        public DialogsListViewHolder(View itemView, Action<int> listener, Action<int, View> long_listener) : base(itemView)
        {
            expert_nameTV = itemView.FindViewById<TextView>(Resource.Id.expert_nameTV);
            specializationTV = itemView.FindViewById<TextView>(Resource.Id.specializationTV);
            messageTV = itemView.FindViewById<TextView>(Resource.Id.messageTV);
            timeTV = itemView.FindViewById<TextView>(Resource.Id.timeTV);
            messageCountTV = itemView.FindViewById<TextView>(Resource.Id.messageCountTV);
            expert_imageIV = itemView.FindViewById<ImageView>(Resource.Id.expert_imageIV);
            onlineIV = itemView.FindViewById<ImageView>(Resource.Id.onlineIV);
            messages_countRL = itemView.FindViewById<RelativeLayout>(Resource.Id.messages_countRL);
            linearLayout3112 = itemView.FindViewById<RelativeLayout>(Resource.Id.linearLayout3112);

            itemView.Click += (s, e) => listener(Position);
            ItemView.LongClick += (s, e) => long_listener(Position, ItemView);
        }
    }
}