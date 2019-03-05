using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;

namespace Iamnearby.ViewHolders
{
    class CheckSubCategoryViewHolder : RecyclerView.ViewHolder
    {
        public TextView specialization_nameTV { get; set; }
        public CheckBox spec_checkBox { get; set; }

        public CheckSubCategoryViewHolder(View itemView, Action<int> listener, Action<object, int> check_box_listener) : base(itemView)
        {
            specialization_nameTV = itemView.FindViewById<TextView>(Resource.Id.specialization_nameTV);
            spec_checkBox = itemView.FindViewById<CheckBox>(Resource.Id.spec_checkBox);
            itemView.Click += (s, e) => listener(Position);
            spec_checkBox.Click += (s, e) => check_box_listener(s, Position);
        }
    }
}