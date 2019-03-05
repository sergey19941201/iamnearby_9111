using System;
using Android.Support.V7.Widget;

namespace Iamnearby.Methods
{
    public class ScrollUpDetector : RecyclerView.OnScrollListener
    {
        bool readyForAction;
        public Action Action;

        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);

            if (newState == RecyclerView.ScrollStateDragging)
            { //The user starts scrolling
                readyForAction = true;
            }
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);

            if (readyForAction && dy < 0)
            { //The scroll direction is down
                readyForAction = false;
                Action();
            }
        }

    }
}