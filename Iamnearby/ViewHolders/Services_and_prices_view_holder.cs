using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Iamnearby.ViewHolders
{
    public class Services_and_prices_view_holder : RecyclerView.ViewHolder
    {
        public TextView serviceTV { get; set; }
        public TextView priceTV { get; set; }
        public Services_and_prices_view_holder(View itemView, Action<int> listener) : base(itemView)
        {
            serviceTV = itemView.FindViewById<TextView>(Resource.Id.serviceTV);
            priceTV = itemView.FindViewById<TextView>(Resource.Id.priceTV);
            itemView.Click += (s, e) => listener(Position);
        }
    }
}