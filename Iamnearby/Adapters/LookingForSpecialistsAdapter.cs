using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Bumptech.Glide;
using Iamnearby.Activities;
using Iamnearby.ViewHolders;
using PCL.Models;

namespace Iamnearby.Adapters
{
    public class LookingForSpecialistsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private static Activity _context;
        private List<UpperSpecializations> specializations_list;
        public static string id_of_highest_categ, name_of_highest_cat;
        public static bool has_subcategory_of_highest_cat;
        ISharedPreferences expert_data = Application.Context.GetSharedPreferences("experts", FileCreationMode.Private);
        ISharedPreferencesEditor edit_expert;
        ISharedPreferences dialog_data;
        ISharedPreferencesEditor edit_dialog;
        Typeface tf;
        public LookingForSpecialistsAdapter(List<UpperSpecializations> specializations_list, Activity context, Typeface tf)
        {
            this.specializations_list = specializations_list;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var specViewHolder = (LookingForSpecialistsViewHolder)holder;

            specViewHolder.specialization_nameTV.SetTypeface(tf, TypefaceStyle.Bold);
            specViewHolder.specialists_count.SetTypeface(tf, TypefaceStyle.Bold);
            specViewHolder.specialization_nameTV.Text = specializations_list[position].name;
            specViewHolder.specialists_count.Visibility = ViewStates.Visible;
            specViewHolder.specialists_count.Text = specializations_list[position].expertsCount;
            if (!System.String.IsNullOrEmpty(specializations_list[position].iconUrl))
            {
                Glide.With(_context)
               .Load("https://api.iamnearby.net/" + specializations_list[position].iconUrl)
               .Into(specViewHolder.specialization_imageIV);
            }
            else
            {
                Glide.With(_context)
                .Load(null)
                .Into(specViewHolder.specialization_imageIV);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.YourSpecializationRow, parent, false);
            return new LookingForSpecialistsViewHolder(layout, OnItemClick);
        }

        public override int ItemCount
        {
            get { try { return specializations_list.Count; } catch { return 0; } }
        }

        void OnItemClick(int position)
        {
            edit_expert = expert_data.Edit();
            edit_dialog = dialog_data.Edit();
            var activity2 = new Intent(_context, typeof(ListOfSpecialistsActivity));
            edit_dialog.PutString("come_from", specializations_list[position].id);
            edit_dialog.Apply();
            edit_expert.PutString("spec_id", specializations_list[position].id);
            id_of_highest_categ = specializations_list[position].id;
            name_of_highest_cat = specializations_list[position].name;
            has_subcategory_of_highest_cat = specializations_list[position].hasSubcategory;
            edit_expert.PutString("spec_name", specializations_list[position].name);
            edit_expert.PutString("spec_type", _context.GetString(Resource.String.all_subcategs));
            edit_expert.PutBoolean("has_subcategory", specializations_list[position].hasSubcategory);

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