using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Iamnearby.Activities;
using Iamnearby.ViewHolders;
using PCL.Models;

namespace Iamnearby.Adapters
{
    public class DropDownSubcategsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private static Activity _context;
        private List<SubCategory> dropdown_sub_category_list;
        ISharedPreferences expert_data = Application.Context.GetSharedPreferences("experts", FileCreationMode.Private);
        ISharedPreferencesEditor edit_expert;
        ISharedPreferences dialog_data;
        ISharedPreferencesEditor edit_dialog;
        LinearLayout.LayoutParams ll_up = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        Typeface tf;
        public DropDownSubcategsAdapter(List<SubCategory> dropdown_sub_category_list, Activity context, Typeface tf)
        {
            this.dropdown_sub_category_list = dropdown_sub_category_list;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var dropDownSubcategsViewHolder = (DropDownSubcategsViewHolder)holder;
            dropDownSubcategsViewHolder.sub_categTV.Text = dropdown_sub_category_list[position].name;
            dropDownSubcategsViewHolder.sub_categTV.SetTypeface(tf, TypefaceStyle.Normal);
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.DropDownSubcategsRow, parent, false);
            return new DropDownSubcategsViewHolder(layout, OnItemClick);
        }
        public override int ItemCount
        {
            get { try { return dropdown_sub_category_list.Count; } catch { return 0; } }
        }

        void OnItemClick(int position)
        {
            edit_expert = expert_data.Edit();
            edit_dialog = dialog_data.Edit();
            var activity2 = new Intent(_context, typeof(ListOfSpecialistsActivity));
            edit_dialog.PutString("come_from", dropdown_sub_category_list[position].id);
            edit_dialog.Apply();
            edit_expert.PutString("spec_id", dropdown_sub_category_list[position].id);
            edit_expert.PutString("spec_type", dropdown_sub_category_list[position].name);
            edit_expert.PutString("spec_name", LookingForSpecialistsAdapter.name_of_highest_cat);
            edit_expert.PutBoolean("has_subcategory", dropdown_sub_category_list[position].hasSubcategory);
            if (dropdown_sub_category_list[position].name == _context.GetString(Resource.String.all_subcategs))
            {
                edit_expert.PutString("spec_id", LookingForSpecialistsAdapter.id_of_highest_categ);
                edit_expert.PutBoolean("has_subcategory", LookingForSpecialistsAdapter.has_subcategory_of_highest_cat);
                edit_expert.PutString("spec_name", LookingForSpecialistsAdapter.name_of_highest_cat);
            }
            edit_expert.Apply();
            _context.StartActivity(activity2);
        }
    }
}