using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Iamnearby.Activities;
using Iamnearby.ViewHolders;
using PCL.Models;

namespace Iamnearby.Adapters
{
    public class DialogsListAdapter : RecyclerView.Adapter
    {
        Activity _context;
        private List<ChatListModel> dialogs_list;
        ISharedPreferences dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_dialog;
        ISharedPreferences expert_feedback_pref = Application.Context.GetSharedPreferences("expert_feedback_pref", FileCreationMode.Private);
        ISharedPreferencesEditor edit_expert_feedback;
        Typeface tf;
        string date_now;

        public DialogsListAdapter(List<ChatListModel> dialogs_list, Activity context, Typeface tf, string date_now)
        {
            this.date_now = date_now;
            this.dialogs_list = dialogs_list;
            _context = context;
            this.tf = tf;
        }
        public override int ItemCount
        {
            get { try { return dialogs_list.Count; } catch { return 0; } }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.DialogListRow, parent, false);
            return new DialogsListViewHolder(layout, OnItemClick, OnLongClick);
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var dialogsListViewHolder = (DialogsListViewHolder)holder;

            dialogsListViewHolder.messageCountTV.SetTypeface(tf, TypefaceStyle.Normal);
            dialogsListViewHolder.timeTV.SetTypeface(tf, TypefaceStyle.Normal);
            dialogsListViewHolder.expert_nameTV.SetTypeface(tf, TypefaceStyle.Bold);
            dialogsListViewHolder.specializationTV.SetTypeface(tf, TypefaceStyle.Normal);
            dialogsListViewHolder.messageTV.SetTypeface(tf, TypefaceStyle.Normal);

            try
            {
                dialogsListViewHolder.expert_nameTV.Text = dialogs_list[position].expert.name;//split[0];
            }
            catch
            {
                dialogsListViewHolder.expert_nameTV.Text = dialogs_list[position].expert.name;
            }


            if (!String.IsNullOrEmpty(dialogs_list[position].expert.categoryName))
                dialogsListViewHolder.specializationTV.Text = dialogs_list[position].expert.categoryName;
            else
            {
                if (dialogs_list[position].expert.is_expert)
                    dialogsListViewHolder.specializationTV.Text = _context.GetString(Resource.String.expert);
            }

            if (dialogs_list[position].shorttext != "message_text_empty")
                dialogsListViewHolder.messageTV.Text = dialogs_list[position].shorttext;
            else
                dialogsListViewHolder.messageTV.Text = _context.GetString(Resource.String.file);

            var lp = (RelativeLayout.LayoutParams)dialogsListViewHolder.linearLayout3112.LayoutParameters;

            if (dialogs_list[position].timestamp.Substring(0, 10) != date_now)
            {
                dialogsListViewHolder.timeTV.Text = dialogs_list[position].timestamp;
                var par = (RelativeLayout.LayoutParams)dialogsListViewHolder.timeTV.LayoutParameters;
                int right_margin = par.Width + par.RightMargin + 5;
                lp.SetMargins(lp.LeftMargin, lp.TopMargin, lp.RightMargin, lp.BottomMargin);
                dialogsListViewHolder.linearLayout3112.LayoutParameters = lp;
            }
            else
            {
                dialogsListViewHolder.timeTV.Text = dialogs_list[position].timestamp.Substring(11);
                var par = (RelativeLayout.LayoutParams)dialogsListViewHolder.timeTV.LayoutParameters;
                int right_margin = par.Width + par.RightMargin + 5;
                lp.SetMargins(lp.LeftMargin, lp.TopMargin, lp.RightMargin / 2, lp.BottomMargin);
                dialogsListViewHolder.linearLayout3112.LayoutParameters = lp;
            }
            if (dialogs_list[position].unread != "0")
            {
                dialogsListViewHolder.messageCountTV.Visibility = ViewStates.Visible;
                dialogsListViewHolder.messages_countRL.Visibility = ViewStates.Visible;
                dialogsListViewHolder.messageCountTV.Text = dialogs_list[position].unread;
            }
            else
            {
                dialogsListViewHolder.messageCountTV.Visibility = ViewStates.Gone;
                dialogsListViewHolder.messages_countRL.Visibility = ViewStates.Gone;
            }
            if (dialogs_list[position].expert.online)
                dialogsListViewHolder.onlineIV.Visibility = ViewStates.Visible;
            else
                dialogsListViewHolder.onlineIV.Visibility = ViewStates.Gone;

            if (!System.String.IsNullOrEmpty(dialogs_list[position].expert.photo))
            {
                Glide.With(_context)
               .Load("https://api.iamnearby.net/" + dialogs_list[position].expert.photo)
               .Into(dialogsListViewHolder.expert_imageIV);
            }
            else
            {
                Glide.With(_context)
                .Load(null)
                .Into(dialogsListViewHolder.expert_imageIV);
            }
        }
        void OnLongClick(int position, View view)
        {
            string menu_indicator = "";
            edit_dialog = dialog_data.Edit();
            edit_dialog.PutString("expert_id", dialogs_list[position].expert.id);
            edit_dialog.PutString("expert_name", dialogs_list[position].expert.name);
            edit_dialog.PutString("expert_avatar", dialogs_list[position].expert.photo);
            edit_dialog.Apply();
            int cat_id = dialogs_list[position].expert.category;
            if (dialogs_list[position].dialogNotify == "1")
            {
                if (cat_id > 0)
                {
                    edit_expert_feedback = expert_feedback_pref.Edit();
                    edit_expert_feedback.PutString("expert_id", dialogs_list[position].expert.id);
                    edit_expert_feedback.PutString("expert_name", dialog_data.GetString("expert_name", String.Empty));
                    edit_expert_feedback.PutString("expert_phone", dialogs_list[position].expert.phone);
                    edit_expert_feedback.PutBoolean("expert_online", dialogs_list[position].expert.online);
                    edit_expert_feedback.PutString("expert_avatar", dialogs_list[position].expert.photo);
                    edit_expert_feedback.PutString("expert_category_id", cat_id.ToString());
                    edit_expert_feedback.Apply();
                    menu_indicator = "dialog_popup_menu";
                }
                else
                    menu_indicator = "dialog_with_user_popup_menu";
            }
            else if (dialogs_list[position].dialogNotify == "0")
            {
                if (cat_id > 0)
                {
                    edit_expert_feedback = expert_feedback_pref.Edit();
                    edit_expert_feedback.PutString("expert_id", dialogs_list[position].expert.id);
                    edit_expert_feedback.PutString("expert_name", dialog_data.GetString("expert_name", String.Empty));
                    edit_expert_feedback.PutString("expert_phone", dialogs_list[position].expert.phone);
                    edit_expert_feedback.PutBoolean("expert_online", dialogs_list[position].expert.online);
                    edit_expert_feedback.PutString("expert_avatar", dialogs_list[position].expert.photo);
                    edit_expert_feedback.PutString("expert_category_id", cat_id.ToString());
                    edit_expert_feedback.Apply();
                    menu_indicator = "dialog_popup_menu_on";
                }
                else
                    menu_indicator = "dialog_with_user_popup_menu_on";
            }
            ChatListActivity.ShowPopup(_context, menu_indicator, dialogs_list[position].blacklist, view, dialogs_list[position].chatId);
        }
        void OnItemClick(int position)
        {
            edit_dialog = dialog_data.Edit();
            edit_dialog.PutString("expert_id", dialogs_list[position].expert.id);
            edit_dialog.PutString("expert_name", dialogs_list[position].expert.name);
            edit_dialog.PutString("expert_avatar", dialogs_list[position].expert.photo);
            edit_dialog.Apply();
            var activity2 = new Intent(_context, typeof(DialogActivity));
            _context.StartActivity(activity2);
        }
    }
}