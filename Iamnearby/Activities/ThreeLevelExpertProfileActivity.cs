using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Iamnearby.Adapters;
using Iamnearby.ExpandableRecyclerClasses;
using Iamnearby.Fragments;
using Iamnearby.Interfaces;
using Iamnearby.Methods;
using Newtonsoft.Json;
using PCL.Database;
using PCL.Models;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait/*, WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden*/)]
    public class ThreeLevelExpertProfileActivity : Activity
    {
        public static RecyclerView recyclerView;
        public static ThreeLevelExpertProfileAdapter recyclerAdapter;
        static FragmentManager fragmentManager;
        private List<IExpertProfileDataType> listItems;
        private LinearLayoutManager mLayoutManager;
        TextView dialogsTV;
        ImageView message_indicatorIV;
        static LoginRegFragment loginRegFragment;
        ISharedPreferences loc_pref;
        ISharedPreferences expert_data;
        ISharedPreferencesEditor edit_expert;
        ProgressBar activityIndicator;
        LinearLayout profileLL, dialogsLL;
        ISharedPreferences dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_dialog;
        ISharedPreferences expert_feedback_pref = Application.Context.GetSharedPreferences("expert_feedback_pref", FileCreationMode.Private);
        ISharedPreferencesEditor edit_expert_feedback;
        UserMethods userMethods = new UserMethods();
        ExpertProfile deserialized_expert_profile;

        private void initWidget()
        {
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerview);
        }

        private void initData()
        {
            this.listItems = new List<IExpertProfileDataType>();
            listItems.Add(new ExpertGroupData(
                deserialized_expert_profile.id,
                deserialized_expert_profile.fullName,
                deserialized_expert_profile.phone,
                deserialized_expert_profile.aboutExpert,
                deserialized_expert_profile.rating,
                deserialized_expert_profile.distance,
                deserialized_expert_profile.city.name,
                deserialized_expert_profile.online,
                deserialized_expert_profile.avatarUrl,
                null));

            foreach (var data in deserialized_expert_profile.mainCategories)
            {
                var spec_obj = new ExpertGroupData(null, null, null, null, null, null, null, false, null, data.name);
                if (data.subcategories != null)
                    foreach (var subcategory in data.subcategories)
                    {

                        var subcat_obj = new ExpertGroupData2(subcategory.name);
                        var entry_obj = new ExpertEntryData(subcategory.description, subcategory.services, subcategory.photos);
                        subcat_obj.items.Add(entry_obj);
                        spec_obj.items.Add(subcat_obj);
                    }
                listItems.Add(spec_obj);
            }
            Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
            recyclerAdapter = new ThreeLevelExpertProfileAdapter(this, this, tf, listItems);
            recyclerAdapter.GroupClick += OnGroupClick;
            recyclerAdapter.GroupClick2 += OnGroupClick2;
            recyclerAdapter.BttEntryClick += OnBttEntryClick;
            this.mLayoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(mLayoutManager);
            recyclerView.SetAdapter(recyclerAdapter);
        }

        void OnGroupClick(object sender, int pos)
        {
            ExpertGroupData groupSelect = (ExpertGroupData)listItems[pos];
            #region closing all subitems
            if (groupSelect.items.Count == 0)
            {
                bool is_cur_header_item11111 = true;
                //loop to close EntryData items
                for (int i = pos; i < listItems.Count; i++)
                {
                    try
                    {
                        var current_item = (ExpertGroupData)listItems[i];
                        if (!is_cur_header_item11111)
                            break;
                        is_cur_header_item11111 = false;
                    }
                    catch
                    {
                        try
                        {
                            ExpertGroupData2 groupSelect2 = (ExpertGroupData2)listItems[i];
                            #region closing EntryData items 
                            if (groupSelect2.items.Count == 0)
                            {
                                int count = 0;
                                while (listItems.Count > i + 1 && listItems[i + 1].GetItemType() == 1)
                                {
                                    groupSelect2.items.Add(listItems[i + 1]);
                                    listItems.RemoveAt(i + 1);
                                    recyclerAdapter.NotifyItemRemoved(i + 1);
                                    count++;
                                }
                            }
                            #endregion closing EntryData items 
                        }
                        catch
                        {
                        }
                    }
                }

                var indexes_to_remove = new List<int>();
                bool is_cur_header_item = true;
                //loop to close GreoupData2 items
                for (int i = pos; i < listItems.Count; i++)
                {
                    try
                    {
                        var current_item = (ExpertGroupData)listItems[i];
                        if (!is_cur_header_item)
                            break;
                        is_cur_header_item = false;
                    }
                    catch
                    {
                        try
                        {
                            var current_item = (ExpertGroupData2)listItems[i];
                            groupSelect.items.Add(listItems[pos + 1]);
                            listItems.RemoveAt(pos + 1);
                            recyclerAdapter.NotifyItemRemoved(pos + 1);
                            i--;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            #endregion closing closing all subitems

            #region opening GroupData2 group items (they are subitems to main group (GroupData) items)
            else
            {
                int index = pos + 1;
                foreach (ExpertGroupData2 entryNode in groupSelect.items)
                {
                    listItems.Insert(index, entryNode);
                    recyclerAdapter.NotifyItemInserted(index);
                    index++;
                }
                groupSelect.items.Clear();

                int last_group_index = 0;
                int count = 0;
                foreach (var item in ThreeLevelExpertProfileAdapter.listItems)
                {
                    if (item is ExpertGroupData)
                        last_group_index = count;
                    count++;
                }
                if (pos == last_group_index)
                    recyclerView.SmoothScrollToPosition(ThreeLevelExpertProfileAdapter.listItems.Capacity - 1);
            }
            #endregion opening GroupData2 group items (they are subitems to main group (GroupData) items)
        }
        void OnGroupClick2(object sender, int pos)
        {
            ExpertGroupData2 groupSelect = (ExpertGroupData2)listItems[pos];
            #region closing EntryData items 
            if (groupSelect.items.Count == 0)
            {
                int count = 0;
                while (listItems.Count > pos + 1 && listItems[pos + 1].GetItemType() == 1)
                {
                    groupSelect.items.Add(listItems[pos + 1]);
                    listItems.RemoveAt(pos + 1);
                    recyclerAdapter.NotifyItemRemoved(pos + 1);
                    count++;
                }
            }
            #endregion closing EntryData items 

            #region opening EntryData items
            else
            {
                int index = pos + 1;
                foreach (ExpertEntryData entryNode in groupSelect.items)
                {
                    listItems.Insert(index, entryNode);
                    recyclerAdapter.NotifyItemInserted(index);
                    index++;
                }
                groupSelect.items.Clear();
                int last_group_index = 0;
                int count = 0;
                foreach (var item in ThreeLevelExpertProfileAdapter.listItems)
                {
                    if (item is ExpertGroupData2)
                        last_group_index = count;
                    count++;
                }
                if (pos == last_group_index)
                    recyclerView.SmoothScrollToPosition(ThreeLevelExpertProfileAdapter.listItems.Capacity - 1);
            }
            #endregion opening EntryData items
        }

        void OnBttEntryClick(object sender, int pos)
        {
            ExpertEntryData entrySelect = (ExpertEntryData)listItems[pos];
            Toast.MakeText(this, "button of entryName " + entrySelect.entryName + " clicked", ToastLength.Short).Show();
        }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.ThreeLevelUserProfile);

                dialogsTV = FindViewById<TextView>(Resource.Id.dialogsTV);
                message_indicatorIV = FindViewById<ImageView>(Resource.Id.message_indicatorIV);
                loginRegFragment = new LoginRegFragment();
                fragmentManager = this.FragmentManager;
                loc_pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                ProfileAndExpertMethods profileAndExpertMethods = new ProfileAndExpertMethods();
                PCL.HttpMethods.ProfileAndExpertMethods profileAndExpertMethodsPCL = new PCL.HttpMethods.ProfileAndExpertMethods();
                expert_data = Application.Context.GetSharedPreferences("experts", FileCreationMode.Private);
                edit_expert = expert_data.Edit();

                profileLL = FindViewById<LinearLayout>(Resource.Id.profileLL);
                dialogsLL = FindViewById<LinearLayout>(Resource.Id.dialogsLL);
                profileLL.Click += (s, e) =>
                {
                    if (userMethods.UserExists())
                        StartActivity(typeof(UserProfileActivity));
                    else
                        try { loginRegFragment.Show(fragmentManager, "fragmentManager"); }
                        catch { Toast.MakeText(this, GetString(Resource.String.you_not_logined), ToastLength.Short).Show(); StartActivity(typeof(MainActivity)); }
                };
                dialogsLL.Click += (s, e) =>
                {
                    if (userMethods.UserExists())
                    {
                        edit_dialog = dialog_data.Edit();
                        edit_dialog.PutString("come_from", "Came directly from bottom");
                        edit_dialog.Apply();
                        StartActivity(typeof(ChatListActivity));
                    }
                    else
                        try { loginRegFragment.Show(fragmentManager, "fragmentManager"); }
                        catch { Toast.MakeText(this, GetString(Resource.String.you_not_logined), ToastLength.Short).Show(); StartActivity(typeof(MainActivity)); }

                };
                initWidget();
                activityIndicator.Visibility = ViewStates.Visible;
                recyclerView.Visibility = ViewStates.Gone;
                var expert_json = await profileAndExpertMethodsPCL.ExpertProfile(expert_data.GetString("expert_id", String.Empty), loc_pref.GetString("latitude", String.Empty), loc_pref.GetString("longitude", String.Empty));
                activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                recyclerView.Visibility = ViewStates.Visible;
                deserialized_expert_profile = JsonConvert.DeserializeObject<ExpertProfile>(expert_json);
                edit_expert_feedback = expert_feedback_pref.Edit();
                edit_expert_feedback.PutString("expert_id", deserialized_expert_profile.id);
                edit_expert_feedback.PutString("expert_name", deserialized_expert_profile.fullName);
                edit_expert_feedback.PutString("expert_phone", deserialized_expert_profile.phone);
                edit_expert_feedback.PutBoolean("expert_online", deserialized_expert_profile.online);
                edit_expert_feedback.PutString("expert_avatar", deserialized_expert_profile.avatarUrl);
                try
                {
                    edit_expert_feedback.PutString("expert_category_id", deserialized_expert_profile.serviceCategories[0].categoryId);
                }
                catch { }
                edit_expert_feedback.Apply();
                initData();
            }
            catch (Exception ex)
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}