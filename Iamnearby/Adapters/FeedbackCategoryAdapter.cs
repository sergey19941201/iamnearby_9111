using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Iamnearby.Activities;
using Iamnearby.ViewHolders;
using PCL.Models;

namespace Iamnearby.Adapters
{
    public class FeedbackCategoryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private static Activity _context;
        private List<City> categoriesList;
        ISharedPreferences expert_feedback_pref = Application.Context.GetSharedPreferences("expert_feedback_pref", FileCreationMode.Private);
        ISharedPreferencesEditor edit_expert_feedback;
        Typeface tf;
        public FeedbackCategoryAdapter(List<City> categoriesList, Activity context, Typeface tf)
        {
            this.categoriesList = categoriesList;
            _context = context;
            this.tf = tf;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var sub_categoryViewHolder = (SubCategoryViewHolder)holder;
            sub_categoryViewHolder.specialization_nameTV.Text = categoriesList[position].name;
            sub_categoryViewHolder.specialization_nameTV.SetTypeface(tf, TypefaceStyle.Normal);
            if (categoriesList[position].id == expert_feedback_pref.GetString("expert_category_id", String.Empty))
            {
                sub_categoryViewHolder.specialization_nameTV.SetTypeface(tf, TypefaceStyle.Bold);
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SubCategoryRow, parent, false);
            return new SubCategoryViewHolder(layout, OnItemClick);
        }
        public override int ItemCount
        {
            get { try { return categoriesList.Count; } catch { return 0; } }
        }

        async void OnItemClick(int position)
        {
            edit_expert_feedback = expert_feedback_pref.Edit();
            edit_expert_feedback.PutString("expert_category_id", categoriesList[position].id);
            edit_expert_feedback.PutString("expert_category_name", categoriesList[position].name);
            edit_expert_feedback.Apply();
            _context.StartActivity(typeof(FeedbackActivity));
        }
    }
}