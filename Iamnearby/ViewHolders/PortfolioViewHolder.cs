using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;

namespace Iamnearby.ViewHolders
{
    public class PortfolioViewHolder : RecyclerView.ViewHolder
    {
        public ImageView portfolioIV { get; set; }
        public ProgressBar activityIndicator { get; set; }

        public PortfolioViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            portfolioIV = itemView.FindViewById<ImageView>(Resource.Id.portfolioIV);
            activityIndicator = itemView.FindViewById<ProgressBar>(Resource.Id.activityIndicator);
            activityIndicator.IndeterminateDrawable.SetColorFilter(Color.Argb(255, 7, 40, 131), Android.Graphics.PorterDuff.Mode.Multiply);
            itemView.Click += (s, e) => listener(Position);
        }
    }
}