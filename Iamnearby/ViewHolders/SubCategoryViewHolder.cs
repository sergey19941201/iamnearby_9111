using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;

namespace Iamnearby.ViewHolders
{
    public class SubCategoryViewHolder : RecyclerView.ViewHolder
    {
        public TextView specialization_nameTV { get; set; }

        public SubCategoryViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            specialization_nameTV = itemView.FindViewById<TextView>(Resource.Id.specialization_nameTV);
            itemView.Click += (s, e) => listener(Position);
        }
    }
}