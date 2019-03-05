using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Iamnearby.ViewHolders
{
    public class DropDownSubcategsViewHolder : RecyclerView.ViewHolder
    {
        public TextView sub_categTV { get; set; }

        public DropDownSubcategsViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            sub_categTV = itemView.FindViewById<TextView>(Resource.Id.sub_categTV);
            itemView.Click += (s, e) => listener(Position);
        }
    }
}