using System;
using Android.Content.Res;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Iamnearby.ViewHolders
{
    public class AddSpecializationViewHolder : RecyclerView.ViewHolder
    {
        public TextView add_specializationTV { get; set; }
        public TextView specializationTV { get; set; }
        public TextView specialization_titleTV { get; set; }
        public RelativeLayout del_specRL { get; set; }
        public AssetManager Assets { get; }

        public AddSpecializationViewHolder(View itemView, Action<int> listener, Action<int> del_listiner) : base(itemView)
        {
            specialization_titleTV = itemView.FindViewById<TextView>(Resource.Id.specialization_titleTV);
            add_specializationTV = itemView.FindViewById<TextView>(Resource.Id.add_specializationTV);
            specializationTV = itemView.FindViewById<TextView>(Resource.Id.specializationTV);
            del_specRL = itemView.FindViewById<RelativeLayout>(Resource.Id.del_specRL);
            itemView.Click += (s, e) => listener(Position);
            del_specRL.Click += (s, e) => del_listiner(Position);
        }
    }
}