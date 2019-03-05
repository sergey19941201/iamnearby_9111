using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Iamnearby.Activities;
using Iamnearby.Methods;
using Iamnearby.ViewHolders;
using PCL.Database;
using PCL.Models;

namespace Iamnearby.Adapters
{
    public class SubCategorySearchAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private static Activity _context;
        private List<SearchDisplaying> categories_list;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("categories_data", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
        UserMethods userMethods = new UserMethods();
        ProfileAndExpertMethods profileAndExpertMethods = new ProfileAndExpertMethods();
        Typeface tf;
        public SubCategorySearchAdapter(List<SearchDisplaying> categories_list, Activity context, Typeface tf)
        {
            this.categories_list = categories_list;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var specViewHolder = (UserSpecializationViewHolder)holder;
            specViewHolder.specialization_nameTV.SetTypeface(tf, TypefaceStyle.Normal);

            if (categories_list[position].isRoot)
            {
                specViewHolder.specialization_imageIV.Visibility = ViewStates.Visible;
                specViewHolder.specialization_nameTV.SetTextColor(new Color(ContextCompat.GetColor(_context, Resource.Color.buttonBackgroundColor)));
                specViewHolder.specialization_nameTV.SetAllCaps(true);
                specViewHolder.specialization_nameTV.Text = categories_list[position].name;
                Glide.With(Application.Context)
                    .Load("https://api.iamnearby.net/" + categories_list[position].iconUrl)
                    //.Placeholder(Resource.Drawable.specialization_imageIV)
                    .Into(specViewHolder.specialization_imageIV);
            }
            else
            {
                specViewHolder.specialization_nameTV.Text = "            " + categories_list[position].name;
                specViewHolder.specialization_nameTV.SetAllCaps(false);
                specViewHolder.specialization_imageIV.Visibility = ViewStates.Gone;
                specViewHolder.specialization_nameTV.SetTextColor(new Color(ContextCompat.GetColor(_context, Resource.Color.agreementTextColor)));
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SearchRow, parent, false);
            return new UserSpecializationViewHolder(layout, OnItemClick);
        }
        public override int ItemCount
        {
            get { try { return categories_list.Count; } catch { return 0; } }
        }

        async void OnItemClick(int position)
        {
            if (!categories_list[position].isRoot)
            {
                if (!userMethods.UserExists())
                {
                    SubCategoryAdapter.my_specializations_static.Add(new SubCategory() { id = categories_list[position].id, name = categories_list[position].name });
                    var activity2 = new Intent(_context, typeof(AddSpecializationActivity));
                    _context.StartActivity(activity2);
                }
                else
                {
                    var res = await SubCategoryActivity.show_activity(categories_list[position].id, userMethods.GetUsersAuthToken());

                    _context.StartActivity(new Intent(_context, typeof(UserProfileActivity)));
                }
            }
            else
            {
                Toast toast = Toast.MakeText(_context, _context.GetString(Resource.String.only_child_categs_can_be_chosen), ToastLength.Short);
                toast.SetGravity(Gravity.GetAbsoluteGravity(GravityFlags.Top, GravityFlags.Center), 0, 150);
                toast.Show();
            }
        }
    }
}