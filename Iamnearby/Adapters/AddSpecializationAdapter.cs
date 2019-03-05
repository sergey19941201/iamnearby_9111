using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Iamnearby.Activities;
using Iamnearby.ViewHolders;

namespace Iamnearby.Adapters
{
    public class AddSpecializationAdapter : RecyclerView.Adapter
    {
        private static Activity _context;
        private List<string> my_specializations;
        Typeface tf;

        public AddSpecializationAdapter(List<string> my_specializations, Activity context, Typeface tf)
        {
            this.my_specializations = my_specializations;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var addSpecializationViewHolder = (AddSpecializationViewHolder)holder;
            addSpecializationViewHolder.specialization_titleTV.SetTypeface(tf, TypefaceStyle.Bold);
            addSpecializationViewHolder.add_specializationTV.SetTypeface(tf, TypefaceStyle.Normal);
            addSpecializationViewHolder.specializationTV.SetTypeface(tf, TypefaceStyle.Normal);
            if (position == my_specializations.Count)
            {
                addSpecializationViewHolder.specializationTV.Visibility = ViewStates.Gone;
                addSpecializationViewHolder.specialization_titleTV.Visibility = ViewStates.Gone;
                addSpecializationViewHolder.add_specializationTV.Visibility = ViewStates.Visible;
                addSpecializationViewHolder.del_specRL.Visibility = ViewStates.Gone;
            }
            else
            {
                addSpecializationViewHolder.specializationTV.Visibility = ViewStates.Visible;
                addSpecializationViewHolder.specialization_titleTV.Visibility = ViewStates.Visible;
                addSpecializationViewHolder.add_specializationTV.Visibility = ViewStates.Gone;
                if (my_specializations.Count > 1)
                    addSpecializationViewHolder.del_specRL.Visibility = ViewStates.Visible;
                else
                    addSpecializationViewHolder.del_specRL.Visibility = ViewStates.Gone;
                addSpecializationViewHolder.specializationTV.Text = my_specializations[position];
            }

        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.AddSpecializationRow, parent, false);
            return new AddSpecializationViewHolder(layout, OnItemClick, OnDelClick);
        }
        public override int ItemCount
        {
            get { try { return my_specializations.Count + 1; } catch { return 0; } }
        }

        public AssetManager Assets { get; private set; }

        void OnItemClick(int position)
        {
            if (position == my_specializations.Count)
            {
                var activity2 = new Intent(_context, typeof(YourSpecializationActivity));
                _context.StartActivity(activity2);
            }
        }
        void OnDelClick(int position)
        {
            my_specializations.RemoveAt(position);
            SubCategoryAdapter.my_specializations_static.RemoveAt(position);
            this.NotifyDataSetChanged();
        }
    }
}