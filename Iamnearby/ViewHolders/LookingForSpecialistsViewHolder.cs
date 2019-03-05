using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Iamnearby.ViewHolders
{
    public class LookingForSpecialistsViewHolder : RecyclerView.ViewHolder
    {
        public TextView specialization_nameTV { get; set; }
        public TextView specialists_count { get; set; }
        public ImageView specialization_imageIV { get; set; }

        public LookingForSpecialistsViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            specialists_count = itemView.FindViewById<TextView>(Resource.Id.count_specs);
            specialization_nameTV = itemView.FindViewById<TextView>(Resource.Id.specialization_nameTV);
            specialization_imageIV = itemView.FindViewById<ImageView>(Resource.Id.specialization_imageIV);
            itemView.Click += (s, e) => listener(Position);
        }
    }
}