using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Iamnearby.ViewHolders
{
    public class DialogViewHolder : RecyclerView.ViewHolder
    {
        public LinearLayout message_from_meLL { get; set; }
        public LinearLayout message_from_expertLL { get; set; }
        public TextView dateTV { get; set; }
        public TextView message_from_meTV { get; set; }
        public TextView time_from_meTV { get; set; }
        public TextView date_from_meTV { get; set; }
        public TextView message_from_expertTV { get; set; }
        public TextView time_from_expertTV { get; set; }
        public TextView date_from_expertTV { get; set; }
        public ImageView imageFromMeIV { get; set; }
        public ImageView imageFromExpertIV { get; set; }
        public RelativeLayout image_from_meRL { get; set; }
        public RelativeLayout image_from_expertRL { get; set; }
        public ProgressBar activityIndicator_from_expert { get; set; }
        public ProgressBar activityIndicator_from_me { get; set; }
        public TextView file_from_meTV { get; set; }
        public TextView file_from_expertTV { get; set; }
        public LinearLayout doc_from_meLL { get; set; }
        public LinearLayout doc_from_expertLL { get; set; }
        public ImageView doc_from_expertIV { get; set; }
        public ImageView doc_from_meIV { get; set; }
        public ImageView read_markIV { get; set; }

        public DialogViewHolder(View itemView, Action<int> imageClickListener, Action<int> documentClickListener, Action<int, View> longClickListener) : base(itemView)
        {
            message_from_meLL = itemView.FindViewById<LinearLayout>(Resource.Id.message_from_meLL);
            message_from_expertLL = itemView.FindViewById<LinearLayout>(Resource.Id.message_from_expertLL);
            dateTV = itemView.FindViewById<TextView>(Resource.Id.dateTV);
            message_from_meTV = itemView.FindViewById<TextView>(Resource.Id.message_from_meTV);
            time_from_meTV = itemView.FindViewById<TextView>(Resource.Id.time_from_meTV);
            date_from_meTV = itemView.FindViewById<TextView>(Resource.Id.date_from_meTV);
            date_from_expertTV = itemView.FindViewById<TextView>(Resource.Id.date_from_expertTV);
            message_from_expertTV = itemView.FindViewById<TextView>(Resource.Id.message_from_expertTV);
            time_from_expertTV = itemView.FindViewById<TextView>(Resource.Id.time_from_expertTV);
            imageFromMeIV = itemView.FindViewById<ImageView>(Resource.Id.imageFromMeIV);
            imageFromExpertIV = itemView.FindViewById<ImageView>(Resource.Id.imageFromExpertIV);
            image_from_meRL = itemView.FindViewById<RelativeLayout>(Resource.Id.image_from_meRL);
            image_from_expertRL = itemView.FindViewById<RelativeLayout>(Resource.Id.image_from_expertRL);
            activityIndicator_from_expert = itemView.FindViewById<ProgressBar>(Resource.Id.activityIndicator_from_expert);
            activityIndicator_from_me = itemView.FindViewById<ProgressBar>(Resource.Id.activityIndicator_from_me);
            file_from_meTV = itemView.FindViewById<TextView>(Resource.Id.file_from_meTV);
            file_from_expertTV = itemView.FindViewById<TextView>(Resource.Id.file_from_expertTV);
            doc_from_meLL = itemView.FindViewById<LinearLayout>(Resource.Id.doc_from_meLL);
            doc_from_expertLL = itemView.FindViewById<LinearLayout>(Resource.Id.doc_from_expertLL);
            doc_from_expertIV = itemView.FindViewById<ImageView>(Resource.Id.doc_from_expertIV);
            doc_from_meIV = itemView.FindViewById<ImageView>(Resource.Id.doc_from_meIV);
            read_markIV = itemView.FindViewById<ImageView>(Resource.Id.read_markIV);
            imageFromMeIV.Click += (s, e) => imageClickListener(base.AdapterPosition);
            imageFromExpertIV.Click += (s, e) => imageClickListener(base.AdapterPosition);
            doc_from_meLL.Click += (s, e) => documentClickListener(base.AdapterPosition);
            doc_from_expertLL.Click += (s, e) => documentClickListener(base.AdapterPosition);
            itemView.LongClick += (s, e) => longClickListener(base.AdapterPosition, message_from_meLL);
        }
    }
}