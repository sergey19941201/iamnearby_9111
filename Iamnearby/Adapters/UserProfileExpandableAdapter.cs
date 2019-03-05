using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Bumptech.Glide;
using Iamnearby.Activities;
using Iamnearby.ExpandableRecyclerClasses;
using Iamnearby.Fragments;
using Iamnearby.Interfaces;
using Iamnearby.ViewHolders;
using PCL.Database;
using PCL.Models;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace Iamnearby.Adapters
{
    public class UserProfileExpandableAdapter : RecyclerView.Adapter
    {
        static LoginRegFragment loginRegFragment;
        static Android.App.FragmentManager fragmentManager;
        ISharedPreferences expert_feedback_pref = Application.Context.GetSharedPreferences("expert_feedback_pref", FileCreationMode.Private);
        static ISharedPreferencesEditor edit_expert_feedback;
        ISharedPreferences dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_dialog;
        public static List<IExpertProfileDataType> listItems;
        private Activity context;
        private LayoutInflater inflater;
        public event EventHandler<int> ChooseImageClick;
        public event EventHandler<int> GroupClick;
        public event EventHandler<int> BttGroupClick;
        public event EventHandler<int> BttEntryClick;
        public event EventHandler<int> feedbackClick;
        UserProfileEntryData entry;
        UserProfileGroupData group;
        UserMethods userMethods = new UserMethods();
        UserProfileExpandableItemHolder holderEntry;
        UserProfileExpandableGroupHolder groupHolder;
        public static string image_url;
        static Android.Net.Uri uri;
        static Android.Graphics.Bitmap bitmap;
        IExpertProfileDataType itemNode;
        ISharedPreferences specialization_for_edit_pref = Application.Context.GetSharedPreferences("specialization_for_edit_pref", FileCreationMode.Private);
        List<ExpandableDetectPositionClickModel> expandableDetectPositionClickModelList = new List<ExpandableDetectPositionClickModel>();
        UserProfile deserialized_user_data;
        string categ_id;
        List<object> list_data_obj = new List<object>();
        public static int clicked = 0;
        Typeface tf;
        public UserProfileExpandableAdapter()
        {

        }
        public UserProfileExpandableAdapter(/*Context*/Activity context, List<IExpertProfileDataType> listItems, UserProfile deserialized_user_data, Typeface tf)
        {
            this.deserialized_user_data = deserialized_user_data;
            this.context = context;
            UserProfileExpandableAdapter.listItems = listItems;
            this.inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            this.tf = tf;
        }

        public void LoadImage(string image_url, Android.Net.Uri uri, Android.Graphics.Bitmap bitmap)
        {
            UserProfileExpandableAdapter.bitmap = bitmap;
            UserProfileExpandableAdapter.image_url = image_url;
            UserProfileExpandableAdapter.uri = uri;
        }

        void GroupOnClick(int position)
        {
            if (GroupClick != null)
                if (position != 0)
                    GroupClick(this, position);

        }


        void editOnClick(int position)
        {
            var activity2 = new Intent(context, typeof(PersonalDataActivity));
            context.StartActivity(activity2);
        }

        void GoToReviewsOnClick(int position)
        {
        }

        void WriteOnClick(int position)
        {
            context.StartActivity(new Intent(context, typeof(DialogActivity)));
        }

        public override int ItemCount
        {
            get
            {
                try { return listItems.Count; } catch { return 0; }
            }
        }

        public override int GetItemViewType(int position)
        {
            foreach (var item in listItems)
            {
                list_data_obj = listItems.ConvertAll(o => (object)o);
            }
            try
            {
                NotifyDataSetChanged();
            }
            catch { }
            return listItems[position].GetItemType();
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            loginRegFragment = new LoginRegFragment();
            fragmentManager = context.FragmentManager;
            if (viewType == 0)
            {
                View entryView = inflater.Inflate(Resource.Layout.UserProfileRow, parent, false);
                return new UserProfileExpandableGroupHolder(entryView,
                                                            GroupOnClick,
                                                            SettingsOnClick,
                                                            editOnClick,
                                                            ChooseImageOnClick,
                                                            OnImageClick,
                                                            FeedbackOnClick,
                                                            GoToReviewsClick,
                                                            AboutMeClick,
                                                            context);
            }
            else if (viewType == 1)
            {
                View sectionView = inflater.Inflate(Resource.Layout.user_profile_subcategory_entry, parent, false);
                return new UserProfileExpandableItemHolder(sectionView, EditSkillOnClick);
            }
            else
            {
                return null;
            }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            itemNode = listItems[position];

            if (itemNode.GetItemType() == 0)
            {

                group = itemNode as UserProfileGroupData;
                groupHolder = holder as UserProfileExpandableGroupHolder;


                if (position == 0)
                {
                    groupHolder.headerRL.Visibility = ViewStates.Visible;
                    groupHolder.itemRL.Visibility = ViewStates.Gone;
                    //if (image_url != null)
                    //{
                    Thread backgroundThread = new Thread(new ThreadStart(() =>
                    {
                        Glide.Get(Application.Context).ClearDiskCache();
                    }));
                    backgroundThread.IsBackground = true;
                    backgroundThread.Start();
                    Glide.Get(context).ClearMemory();
                    Glide.With(Application.Context)
                        .Load("https://api.iamnearby.net/" + group.avatarUrl)
                        .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
                        .SkipMemoryCache(true))
                        //.Placeholder(Resource.Drawable.specialization_imageIV)
                        .Into(groupHolder.profile_imageIV);
                    //}
                    if (uri != null)
                        groupHolder.profile_imageIV.SetImageURI(uri);
                    if (bitmap != null)
                    {
                        groupHolder.profile_imageIV.SetImageBitmap(bitmap);
                    }

                    if (group.online)
                    {
                        groupHolder.onlyWithReviewsS.Checked = true;
                        groupHolder.onlineValueTV.Text = context.GetString(Resource.String.online_text);
                        groupHolder.onlineValueTV.SetTextColor(Color.Green);
                    }
                    else
                    {
                        groupHolder.onlyWithReviewsS.Checked = false;
                        groupHolder.onlineValueTV.Text = context.GetString(Resource.String.offline_text);
                        groupHolder.onlineValueTV.SetTextColor(Color.Red);
                    }
                    try
                    {
                        char[] chars = { ' ' };
                        string[] split = group.expert_name.Split(chars);
                        groupHolder.expert_nameTV.Text = split[0];
                    }
                    catch
                    {
                        groupHolder.expert_nameTV.Text = group.expert_name;
                    }
                    try
                    {
                        groupHolder.reviewCountTV.Text = group.review_count + " отзывов";
                        groupHolder.rating_valueTV.Text = group.rating.ToString();
                        double rating_value = 0;
                        try
                        {
                            rating_value = Convert.ToDouble(group.rating, (CultureInfo.InvariantCulture));
                            if (rating_value >= 1)
                            {
                                groupHolder.star1IV.SetBackgroundResource(Resource.Drawable.active_star);
                                if (rating_value >= 2)
                                {
                                    groupHolder.star2IV.SetBackgroundResource(Resource.Drawable.active_star);
                                    if (rating_value >= 3)
                                    {
                                        groupHolder.star3IV.SetBackgroundResource(Resource.Drawable.active_star);
                                        if (rating_value >= 4)
                                        {
                                            groupHolder.star4IV.SetBackgroundResource(Resource.Drawable.active_star);
                                            if (rating_value >= 5)
                                            {
                                                groupHolder.star5IV.SetBackgroundResource(Resource.Drawable.active_star);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    catch { }
                }
                else
                {
                    groupHolder.headerRL.Visibility = ViewStates.Gone;
                    groupHolder.itemRL.Visibility = ViewStates.Visible;
                }
                groupHolder.skillNameTV.Text = group.skill_name;
                groupHolder.skillNameTV.SetTypeface(tf, TypefaceStyle.Bold);
                groupHolder.expert_nameTV.SetTypeface(tf, TypefaceStyle.Bold);
                groupHolder.reviewCountTV.SetTypeface(tf, TypefaceStyle.Normal);
                groupHolder.rating_valueTV.SetTypeface(tf, TypefaceStyle.Normal);
                groupHolder.expert_nameTV.SetTypeface(tf, TypefaceStyle.Bold);
                groupHolder.onlineValueTV.SetTypeface(tf, TypefaceStyle.Normal);
                groupHolder.dsadas.SetTypeface(tf, TypefaceStyle.Normal);
                groupHolder.sdsddsddddwww.SetTypeface(tf, TypefaceStyle.Normal);
                groupHolder.sdsddsddddwwwwww.SetTypeface(tf, TypefaceStyle.Normal);

                if (position != listItems.Count - 1)
                    groupHolder.bottom_separatorLL.Visibility = ViewStates.Gone;
                else
                    groupHolder.bottom_separatorLL.Visibility = ViewStates.Visible;
            }
            else if (itemNode.GetItemType() == 1)
            {
                entry = itemNode as UserProfileEntryData;
                holderEntry = holder as UserProfileExpandableItemHolder;

                holderEntry.skillNameTV.SetTypeface(tf, TypefaceStyle.Normal);
                holderEntry.skillNameTV.Text = entry.entryName;
            }
        }

        void AboutMeClick(int position)
        {
            var group = listItems[position] as UserProfileGroupData;
            AboutMeActivity.InfoText = group.aboutExpert;
            context.StartActivity(typeof(AboutMeActivity));
        }

        void OnImageClick(int position)
        {
            if (!String.IsNullOrEmpty(image_url) && image_url.Length > 29 && !image_url.Contains("DCIM"))
            {
                DisplayMyPhotoActivity.image_url = image_url;
                context.StartActivity(typeof(DisplayMyPhotoActivity));
            }
        }
        public async void ChooseImageOnClick(int position)
        {
            var granted = await checkCameraPermission();
            if (!granted)
                return;
            if (ChooseImageClick != null)
                ChooseImageClick(this, position);
            UserProfileActivity.ChangeVisibility();
            Thread backgroundThread = new Thread(new ThreadStart(() =>
            {
                Glide.Get(Application.Context).ClearDiskCache();
            }));
            backgroundThread.IsBackground = true;
            backgroundThread.Start();
            Glide.Get(context).ClearMemory();
            Glide.With(Application.Context)
            .Load(image_url)
            .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
            .SkipMemoryCache(true))
            //.Placeholder(Resource.Drawable.specialization_imageIV)
            .Into(groupHolder.profile_imageIV);
        }
        public void SettingsOnClick(int position)
        {
            var activity2 = new Intent(context, typeof(SettingsActivity));
            context.StartActivity(activity2);
        }
        public void EditOnClick(int position)
        {
            var activity2 = new Intent(context, typeof(PersonalDataActivity));
            context.StartActivity(activity2);
        }
        public void EditSkillOnClick(int position)
        {
            var entry = listItems[position] as UserProfileEntryData;
            ISharedPreferencesEditor edit = specialization_for_edit_pref.Edit();
            edit.PutString("categoryId", entry.categoryId);
            edit.PutString("name", entry.entryName);
            edit.PutString("description", entry.description);
            try
            {
                edit.PutString("photos", entry.photos.ToString());
            }
            catch { }
            ////need to put this to deserialize services
            edit.PutInt("categoryIndex", entry.upperCatIndex);
            edit.Apply();
            var activity2 = new Intent(context, typeof(AddServiceActivity));
            context.StartActivity(activity2);
        }

        public void FeedbackOnClick(int position)
        {
            edit_expert_feedback = expert_feedback_pref.Edit();
            edit_expert_feedback.PutString("expert_id", deserialized_user_data.id);
            edit_expert_feedback.PutString("expert_name", deserialized_user_data.fullName);
            edit_expert_feedback.PutString("expert_phone", deserialized_user_data.phone);
            edit_expert_feedback.PutBoolean("expert_online", deserialized_user_data.online);
            edit_expert_feedback.PutString("expert_avatar", deserialized_user_data.avatarUrl);
            edit_expert_feedback.Apply();
            var activity2 = new Intent(context, typeof(MyReviewsListActivity));
            context.StartActivity(activity2);
        }
        public void GoToReviewsClick(int position)
        {
            context.StartActivity(new Intent(context, typeof(AllReviewsByMeActivity)));
        }

        async Task<bool> checkCameraPermission()
        {
            PermissionStatus cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Camera);

            if (cameraStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Plugin.Permissions.Abstractions.Permission.Camera });
                cameraStatus = results[Plugin.Permissions.Abstractions.Permission.Camera];
                request_runtime_permissions();
                return false;
            }

            PermissionStatus mediaStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);

            if (mediaStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Plugin.Permissions.Abstractions.Permission.Storage });
                mediaStatus = results[Plugin.Permissions.Abstractions.Permission.Storage];
                request_runtime_permissions();
                return false;
            }

            return true;
        }

        private const int REQUEST_PERMISSION_CODE = 1000;
        void request_runtime_permissions()
        {
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.M)
                if (
                          context.CheckSelfPermission(Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted
                          || context.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) != Android.Content.PM.Permission.Granted
                          || context.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted
                          )
                {
                    ActivityCompat.RequestPermissions(context, new String[]
                    {
                                Manifest.Permission.Camera,
                                Manifest.Permission.ReadExternalStorage,
                                Manifest.Permission.WriteExternalStorage,
                    }, REQUEST_PERMISSION_CODE);
                }
                else
                {
                    ActivityCompat.RequestPermissions(context, new String[]
                    {
                                Manifest.Permission.Camera,
                                Manifest.Permission.ReadExternalStorage,
                                Manifest.Permission.WriteExternalStorage,
                    }, REQUEST_PERMISSION_CODE);
                }
        }
    }
}