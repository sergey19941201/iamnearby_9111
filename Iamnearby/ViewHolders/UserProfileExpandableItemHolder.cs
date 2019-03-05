using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;

namespace Iamnearby.ViewHolders
{
    public class UserProfileExpandableItemHolder : RecyclerView.ViewHolder
    {
        public TextView skillNameTV { set; get; }
        public RelativeLayout edit_skillRL { set; get; }

        public UserProfileExpandableItemHolder(View view, Action<int> editClickListener) : base(view)
        {
            skillNameTV = (TextView)view.FindViewById(Resource.Id.skillNameTV);
            edit_skillRL = (RelativeLayout)view.FindViewById(Resource.Id.edit_skillRL);
            edit_skillRL.Click += (s, e) => editClickListener(base.AdapterPosition);
        }
    }
}