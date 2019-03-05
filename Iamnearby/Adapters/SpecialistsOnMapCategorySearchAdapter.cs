using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Bumptech.Glide;
using Iamnearby.Activities;
using Iamnearby.ViewHolders;
using PCL.Models;

namespace Iamnearby.Adapters
{
    public class SpecialistsOnMapCategorySearchAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private static Activity _context;
        private List<SearchDisplaying> categories_list;
        ISharedPreferences expert_data = Application.Context.GetSharedPreferences("experts", FileCreationMode.Private);
        ISharedPreferencesEditor edit_expert;
        ISharedPreferences dialog_data;
        ISharedPreferencesEditor edit_dialog;
        Typeface tf;
        public SpecialistsOnMapCategorySearchAdapter(List<SearchDisplaying> categories_list, Activity context, Typeface tf)
        {
            this.categories_list = categories_list;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var specViewHolder = (UserSpecializationViewHolder)holder;
            specViewHolder.specialization_nameTV.SetTypeface(tf, TypefaceStyle.Normal);
            //specViewHolder.count_specs.SetTypeface(tf, TypefaceStyle.Normal);
            if (categories_list[position].isRoot)
            {
                specViewHolder.specialization_imageIV.Visibility = ViewStates.Visible;
                specViewHolder.specialization_nameTV.SetTextColor(new Color(ContextCompat.GetColor(_context, Resource.Color.buttonBackgroundColor)));
                specViewHolder.specialization_nameTV.SetAllCaps(true);
                specViewHolder.specialization_nameTV.Text = categories_list[position].name;
                if (!System.String.IsNullOrEmpty(categories_list[position].iconUrl))
                {
                    Glide.With(_context)
                   .Load("https://api.iamnearby.net/" + categories_list[position].iconUrl)
                   .Into(specViewHolder.specialization_imageIV);
                }
                else
                {
                    Glide.With(_context)
                    .Load(null)
                    .Into(specViewHolder.specialization_imageIV);
                }
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
            dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SearchRow, parent, false);
            return new UserSpecializationViewHolder(layout, OnItemClick);
        }
        public override int ItemCount
        {
            get { try { return categories_list.Count; } catch { return 0; } }
        }

        void OnItemClick(int position)
        {
            edit_expert = expert_data.Edit();
            edit_dialog = dialog_data.Edit();
            var activity2 = new Intent(_context, typeof(SpecialistsOnMapActivity));
            edit_dialog.PutString("come_from", categories_list[position].id);
            edit_dialog.Apply();
            edit_expert.PutString("spec_id", categories_list[position].id);
            LookingForSpecialistsAdapter.id_of_highest_categ = categories_list[position].rootId;
            LookingForSpecialistsAdapter.name_of_highest_cat = categories_list[position].name;
            LookingForSpecialistsAdapter.has_subcategory_of_highest_cat = categories_list[position].hasSubcategory;
            edit_expert.PutString("spec_name", categories_list[position].name);
            edit_expert.PutString("spec_type", _context.GetString(Resource.String.all_subcategs));
            edit_expert.PutBoolean("has_subcategory", categories_list[position].hasSubcategory);

            //setting values to default
            edit_expert.PutString("expert_city_id", "");
            edit_expert.PutString("expert_city_name", "");
            edit_expert.PutString("distance_radius", "");
            edit_expert.PutBoolean("has_reviews", false);

            edit_expert.Apply();
            _context.StartActivity(activity2);
        }
    }
}