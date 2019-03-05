using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Iamnearby.Adapters;
using Iamnearby.ExpandableRecyclerClasses;
using Iamnearby.Interfaces;
using Iamnearby.Methods;
using Newtonsoft.Json;
using PCL.Database;
using PCL.Models;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden/*, Theme = "@style/AppThemeLightNavbar"*/)]
    public class UserProfileActivity : Activity
    {
        LinearLayoutManager mLayoutManager;
        RelativeLayout i_need_help_LL, i_want_to_help_LL, ImageRL, feedbackRL, edit_skillRL;
        public static LinearLayout tintLL, gridLL;
        TextView EnterMyAccTV, expertNameTV, skillNameTV, description_text_dataTV, reviewCountTV, rating_valueTV, onlineValueTV, dialogsTV;
        ImageView profile_image, choose_photoIV, edit_skillIV;
        ImageView onlineIV, star1IV, star2IV, star3IV, star4IV, star5IV;
        static Button add_skillIV;
        UserMethods userMethods = new UserMethods();
        Methods.ProfileAndExpertMethods profileAndExpertMethods = new Methods.ProfileAndExpertMethods();
        PCL.HttpMethods.ProfileAndExpertMethods profileAndExpertMethodsPCL = new PCL.HttpMethods.ProfileAndExpertMethods();
        PictureMethods pictureMethods = new PictureMethods();
        ScrollView singleDataScrollView;
        Switch onlyWithReviewsS;
        RecyclerView recyclerView;
        ProgressBar activityIndicator, image_activityIndicator;
        static UserProfileExpandableAdapter userProfileExpandableAdapter;
        Button editBn;
        static string previous_base64;
        static string chosen_base64;
        Bitmap chosen_bitmap;
        System.Timers.Timer timer;
        List<ServiceCategory> service_categs_list = new List<ServiceCategory>();
        ISharedPreferences dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_dialog;
        ISharedPreferences specialization_for_edit_pref = Application.Context.GetSharedPreferences("specialization_for_edit_pref", FileCreationMode.Private);
        private List<IExpertProfileDataType> listItems;
        UserProfileServiceCategoriesEmpty deserialized_user_data_categs_empty;
        LinearLayout profileLL, dialogsLL, specialistsLL, go_to_my_reviewsLL;
        CardView specializationCV;
        ImageView message_indicatorIV;
        protected override async void OnResume()
        {
            base.OnResume();
            try
            {
                ISharedPreferences expert_feedback_pref = Application.Context.GetSharedPreferences("expert_feedback_pref", FileCreationMode.Private);
                ISharedPreferencesEditor edit_expert_feedback;
                ISharedPreferencesEditor edit = specialization_for_edit_pref.Edit();
                timer = new System.Timers.Timer();
                timer.Interval = 100;

                if (userMethods.UserExists())
                {
                    SetContentView(Resource.Layout.FullProfile);
                    dialogsTV = FindViewById<TextView>(Resource.Id.dialogsTV);
                    message_indicatorIV = FindViewById<ImageView>(Resource.Id.message_indicatorIV);
                    profileLL = FindViewById<LinearLayout>(Resource.Id.profileLL);
                    dialogsLL = FindViewById<LinearLayout>(Resource.Id.dialogsLL);
                    specialistsLL = FindViewById<LinearLayout>(Resource.Id.specialistsLL);
                    go_to_my_reviewsLL = FindViewById<LinearLayout>(Resource.Id.go_to_my_reviewsLL);
                    singleDataScrollView = FindViewById<ScrollView>(Resource.Id.singleDataScrollView);
                    service_categs_list.Clear();
                    editBn = FindViewById<Button>(Resource.Id.editBn);
                    profile_image = FindViewById<ImageView>(Resource.Id.profile_image);
                    choose_photoIV = FindViewById<ImageView>(Resource.Id.choose_photoIV);
                    add_skillIV = FindViewById<Button>(Resource.Id.add_skillIV);
                    edit_skillIV = FindViewById<ImageView>(Resource.Id.edit_skillIV);
                    edit_skillRL = FindViewById<RelativeLayout>(Resource.Id.edit_skillRL);
                    tintLL = FindViewById<LinearLayout>(Resource.Id.tintLL);
                    gridLL = FindViewById<LinearLayout>(Resource.Id.gridLL);
                    recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                    this.mLayoutManager = new LinearLayoutManager(this);
                    this.recyclerView.SetLayoutManager(mLayoutManager);
                    activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                    expertNameTV = FindViewById<TextView>(Resource.Id.expertNameTV);
                    skillNameTV = FindViewById<TextView>(Resource.Id.skillNameTV);
                    onlineValueTV = FindViewById<TextView>(Resource.Id.onlineValueTV);
                    rating_valueTV = FindViewById<TextView>(Resource.Id.rating_valueTV);
                    description_text_dataTV = FindViewById<TextView>(Resource.Id.description_text_dataTV);
                    reviewCountTV = FindViewById<TextView>(Resource.Id.reviewCountTV);
                    image_activityIndicator = FindViewById<ProgressBar>(Resource.Id.image_activityIndicator);
                    activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                    image_activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                    activityIndicator.Visibility = ViewStates.Visible;
                    ImageRL = FindViewById<RelativeLayout>(Resource.Id.ImageRL);
                    feedbackRL = FindViewById<RelativeLayout>(Resource.Id.feedbackRL);
                    onlineIV = FindViewById<ImageView>(Resource.Id.onlineIV);
                    onlyWithReviewsS = FindViewById<Switch>(Resource.Id.onlyWithReviewsS);
                    specializationCV = FindViewById<CardView>(Resource.Id.specializationCV);
                    specializationCV.Visibility = ViewStates.Gone;
                    go_to_my_reviewsLL.Click += (s, e) =>
                    {
                        StartActivity(typeof(AllReviewsByMeActivity));
                    };

                    Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                    FindViewById<TextView>(Resource.Id.specialistsTV).SetTypeface(tf, TypefaceStyle.Normal);
                    dialogsTV.SetTypeface(tf, TypefaceStyle.Normal);
                    FindViewById<TextView>(Resource.Id.profileTV).SetTypeface(tf, TypefaceStyle.Normal);
                    expertNameTV.SetTypeface(tf, TypefaceStyle.Bold);
                    FindViewById<TextView>(Resource.Id.dsadas).SetTypeface(tf, TypefaceStyle.Bold);
                    onlineValueTV.SetTypeface(tf, TypefaceStyle.Normal);
                    FindViewById<TextView>(Resource.Id.sdsddsddddwww).SetTypeface(tf, TypefaceStyle.Normal);
                    skillNameTV.SetTypeface(tf, TypefaceStyle.Bold);
                    FindViewById<TextView>(Resource.Id.textView1).SetTypeface(tf, TypefaceStyle.Normal);
                    description_text_dataTV.SetTypeface(tf, TypefaceStyle.Normal);
                    reviewCountTV.SetTypeface(tf, TypefaceStyle.Bold);
                    rating_valueTV.SetTypeface(tf, TypefaceStyle.Normal);
                    FindViewById<TextView>(Resource.Id.textsssView1).SetTypeface(tf, TypefaceStyle.Normal);
                    editBn.SetTypeface(tf, TypefaceStyle.Bold);
                    skillNameTV.SetTypeface(tf, TypefaceStyle.Bold);
                    add_skillIV.SetTypeface(tf, TypefaceStyle.Normal);

                    var token = userMethods.GetUsersAuthToken();
                    var user_data = await profileAndExpertMethodsPCL.UserProfileData(token);
                    edit.PutString("user_data", user_data);
                    edit.Apply();
                    if (user_data == "401")
                    {
                        Toast.MakeText(this, Resource.String.you_not_logined, ToastLength.Long).Show();
                        userMethods.ClearTable();
                        userMethods.ClearUsersDataTable();
                        userMethods.ClearTableNotif();
                        StartActivity(typeof(MainActivity));
                        return;
                    }

                    userMethods.InsertProfileData(user_data);
                    dialogsLL.Click += (s, e) =>
                    {
                        edit_dialog = dialog_data.Edit();
                        edit_dialog.PutString("come_from", "Came directly from bottom");
                        edit_dialog.Apply();
                        StartActivity(typeof(ChatListActivity));
                    };
                    specialistsLL.Click += (s, e) =>
                    {
                        StartActivity(typeof(SpecialistsCategoryActivity));
                    };
                    activityIndicator.Visibility = ViewStates.Gone;
                    try
                    {
                        var deserialized_user_data = JsonConvert.DeserializeObject<UserProfile>(user_data);

                        if (deserialized_user_data.myReviews == "0")
                            go_to_my_reviewsLL.Visibility = ViewStates.Gone;

                        if (!String.IsNullOrEmpty(deserialized_user_data.notify_alerts.msg_cnt_new.ToString()) && deserialized_user_data.notify_alerts.msg_cnt_new.ToString() != "0")
                        {
                            message_indicatorIV.Visibility = ViewStates.Visible;
                            dialogsTV.Text = GetString(Resource.String.dialogs) + " (" + deserialized_user_data.notify_alerts.msg_cnt_new + ")";
                        }
                        else
                        {
                            message_indicatorIV.Visibility = ViewStates.Gone;
                            dialogsTV.Text = GetString(Resource.String.dialogs);
                        }

                        this.listItems = new List<IExpertProfileDataType>();
                        listItems.Add(new UserProfileGroupData(
                                                                null,
                                                                null,
                                                                null,
                                                                deserialized_user_data.avatarUrl,
                                                                deserialized_user_data.reviews,
                                                                deserialized_user_data.rating,
                                                                deserialized_user_data.id,
                                                                deserialized_user_data.aboutExpert,
                                                                deserialized_user_data.fullName,
                                                                deserialized_user_data.phone,
                                                                deserialized_user_data.city.name,
                                                                deserialized_user_data.distance,
                                                                deserialized_user_data.online));
                        int count = 0;
                        foreach (var item in deserialized_user_data.mainCategories)
                        {
                            count++;
                            var spec_obj = new UserProfileGroupData(count, item.name, item.spec_id, deserialized_user_data.id, deserialized_user_data.avatarUrl, "", "", "");

                            if (item.subcategories != null)
                                foreach (var subitem in item.subcategories)
                                {
                                    UserProfileEntryData spec_data = new UserProfileEntryData(1, subitem.name, subitem.categoryId, count - 1, subitem.services, subitem.photos/*, item.rating, item.reviews*/, subitem.description);
                                    spec_obj.items.Add(spec_data);
                                }
                            listItems.Add(spec_obj);
                        };
                        userProfileExpandableAdapter = new UserProfileExpandableAdapter(this, listItems, deserialized_user_data, tf);
                        userProfileExpandableAdapter.GroupClick += OnGroupClick;
                        recyclerView.SetAdapter(userProfileExpandableAdapter);
                        singleDataScrollView.Visibility = ViewStates.Gone;
                        recyclerView.Visibility = ViewStates.Visible;
                        UserProfileExpandableAdapter.image_url = "https://api.iamnearby.net" + deserialized_user_data.avatarUrl;
                    }
                    catch
                    {
                        deserialized_user_data_categs_empty = JsonConvert.DeserializeObject<UserProfileServiceCategoriesEmpty>(user_data);
                        if (deserialized_user_data_categs_empty.myReviews == "0")
                            go_to_my_reviewsLL.Visibility = ViewStates.Gone;
                        if (!String.IsNullOrEmpty(deserialized_user_data_categs_empty.notify_alerts.msg_cnt_new.ToString()) && deserialized_user_data_categs_empty.notify_alerts.msg_cnt_new.ToString() != "0")
                        {
                            message_indicatorIV.Visibility = ViewStates.Visible;
                            dialogsTV.Text = GetString(Resource.String.dialogs) + " (" + deserialized_user_data_categs_empty.notify_alerts.msg_cnt_new + ")";
                        }
                        else
                        {
                            message_indicatorIV.Visibility = ViewStates.Gone;
                            dialogsTV.Text = GetString(Resource.String.dialogs);
                        }
                        specializationCV.Visibility = ViewStates.Gone;
                        singleDataScrollView.Visibility = ViewStates.Visible;
                        recyclerView.Visibility = ViewStates.Gone;
                        /*var d = */
                        Thread backgroundThread = new Thread(new ThreadStart(() =>
                        {
                            Glide.Get(Application.Context).ClearDiskCache();
                        }));
                        backgroundThread.IsBackground = true;
                        backgroundThread.Start();
                        Glide.Get(this).ClearMemory();
                        Glide.With(Application.Context)
                         .Load("https://api.iamnearby.net/" + deserialized_user_data_categs_empty.avatarUrl)
                         .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
                         .SkipMemoryCache(true))
                         //.Placeholder(Resource.Drawable.specialization_imageIV)
                         .Into(profile_image);

                        profile_image.Click += (s, e) =>
                        {
                            if (!String.IsNullOrEmpty(deserialized_user_data_categs_empty.avatarUrl))
                            {
                                DisplayMyPhotoActivity.image_url = "https://api.iamnearby.net" + deserialized_user_data_categs_empty.avatarUrl;
                                StartActivity(typeof(DisplayMyPhotoActivity));
                            }
                        };

                        if (deserialized_user_data_categs_empty.online)
                        {
                            onlineValueTV.SetTextColor(Color.Green);
                            onlyWithReviewsS.Checked = true;
                            onlineValueTV.Text = GetString(Resource.String.online_text);
                        }
                        else
                        {
                            onlyWithReviewsS.Checked = false;
                            onlineValueTV.Text = GetString(Resource.String.offline_text);
                            onlineValueTV.SetTextColor(Color.Red);
                        }

                        expertNameTV.Text = deserialized_user_data_categs_empty.fullName;
                        try
                        {
                            skillNameTV.Text = deserialized_user_data_categs_empty.serviceCategories[1].ToString();
                        }
                        catch { }
                    }
                    onlyWithReviewsS.CheckedChange += async (s, e) =>
                    {
                        if (onlyWithReviewsS.Checked)
                        {
                            onlineValueTV.Text = GetString(Resource.String.online_text);
                            onlineValueTV.SetTextColor(Color.Green);
                            var res = await profileAndExpertMethods.EditMyOnline(userMethods.GetUsersAuthToken(), true);
                        }
                        else
                        {
                            onlineValueTV.SetTextColor(Color.Red);
                            onlineValueTV.Text = GetString(Resource.String.offline_text);
                            var res = await profileAndExpertMethods.EditMyOnline(userMethods.GetUsersAuthToken(), false);
                        }
                    };

                    var folder = Android.OS.Environment.ExternalStorageDirectory + Java.IO.File.Separator + "DCIM/Camera";
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    //var filesList 
                    List<string> filesList = new List<string>();
                    try
                    {
                        filesList = Directory.GetFiles(folder).ToList<string>();
                    }
                    catch (Exception ex)
                    {

                    }
                    var reverseList = filesList.AsEnumerable().Reverse().ToList<string>();
                    reverseList.Insert(0, Resource.Drawable.open_camera.ToString());
                    reverseList.Insert(1, Resource.Drawable.open_gallery.ToString());

                    var gridview = FindViewById<GridView>(Resource.Id.gridview);
                    gridview.Adapter = new ImageAdapter(this, reverseList, Resources.DisplayMetrics.WidthPixels);

                    gridview.ItemClick += async delegate (object sender, AdapterView.ItemClickEventArgs args)
                    {
                        previous_base64 = "dsaads";

                        if (args.Position > 1)
                        {
                            Glide.With(Application.Context)
                                       .Load(reverseList[args.Position])
                                       //.Placeholder(Resource.Drawable.specialization_imageIV)
                                       .Into(profile_image);
                            new UserProfileExpandableAdapter().LoadImage(reverseList[args.Position], null, null);
                            try { userProfileExpandableAdapter.NotifyDataSetChanged(); } catch { }
                        }
                        else if (args.Position == 0)
                        {
                            if (pictureMethods.IsThereAnAppToTakePictures(this))
                            {
                                PictureMethods.cameraOrGalleryIndicator = "camera";
                                pictureMethods.CreateDirectoryForPictures();
                                pictureMethods.TakeAPicture(this);
                            }
                        }
                        else if (args.Position == 1)
                        {
                            PictureMethods.cameraOrGalleryIndicator = "gallery";
                            var imageIntent = new Intent();
                            imageIntent.SetType("image/*");
                            //imageIntent.SetType("file/*");
                            imageIntent.SetAction(Intent.ActionGetContent);
                            StartActivityForResult(
                                Intent.CreateChooser(imageIntent, "Select photo"), 0);
                        }

                        if (args.Position > 1)
                        {
                            if (!String.IsNullOrEmpty(previous_base64))
                            {
                                try
                                {
                                    chosen_bitmap = ((BitmapDrawable)profile_image.Drawable).Bitmap;
                                    MemoryStream chosen_memStream = new MemoryStream();
                                    chosen_bitmap.Compress(Bitmap.CompressFormat.Png, 0, chosen_memStream);
                                    byte[] chosen_ba = chosen_memStream.ToArray();
                                    chosen_base64 = Base64.EncodeToString(chosen_ba, Base64.Default);
                                }
                                catch { }
                                timer.Elapsed += delegate
                                {
                                    try
                                    {
                                        chosen_bitmap = ((BitmapDrawable)profile_image.Drawable).Bitmap;
                                        MemoryStream chosen_memStream = new MemoryStream();
                                        chosen_bitmap.Compress(Bitmap.CompressFormat.Jpeg, 90, chosen_memStream);
                                        byte[] chosen_ba = chosen_memStream.ToArray();
                                        chosen_base64 = Base64.EncodeToString(chosen_ba, Base64.Default);
                                        if (chosen_base64 != previous_base64)
                                            RunOnUiThread(async () =>
                                            {
                                                var res = await upload_avatar();
                                                if (res != null)
                                                {
                                                    timer.Stop();
                                                    var intent = new Intent(this, typeof(UserProfileActivity));
                                                    intent.SetFlags(ActivityFlags.NoHistory);
                                                    intent.SetFlags(ActivityFlags.NewTask);
                                                    StartActivity(intent);
                                                    Finish();
                                                }
                                            });
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                };
                                timer.Start();
                                timer.Enabled = true;
                            }
                        }
                    };

                    tintLL.Click += (s, e) =>
                    {
                        add_skillIV.Visibility = ViewStates.Visible;
                        tintLL.Visibility = ViewStates.Gone;
                        gridLL.Visibility = ViewStates.Gone;
                    };
                    add_skillIV.Click += (s, e) =>
                    {
                        StartActivity(typeof(YourSpecializationActivity));
                    };
                    editBn.Click += (s, e) =>
                    {
                        StartActivity(typeof(PersonalDataActivity));
                    };

                    choose_photoIV.Click += async (s, e) =>
                    {
                        var granted = await checkCameraPermission();
                        if (!granted)
                            return;
                        if (tintLL.Visibility == ViewStates.Visible)
                        {
                            tintLL.Visibility = ViewStates.Gone;
                            gridLL.Visibility = ViewStates.Gone;
                            add_skillIV.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            tintLL.Visibility = ViewStates.Visible;
                            gridLL.Visibility = ViewStates.Visible;
                            add_skillIV.Visibility = ViewStates.Gone;
                        }
                    };
                }
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }

        void OnGroupClick(object sender, int pos)
        {
            UserProfileGroupData groupSelect = (UserProfileGroupData)listItems[pos];

            if (groupSelect.items.Count == 0)
            {
                int count = 0;
                while (listItems.Count > pos + 1 && listItems[pos + 1].GetItemType() == 1)
                {
                    groupSelect.items.Add(listItems[pos + 1]);
                    listItems.RemoveAt(pos + 1);
                    userProfileExpandableAdapter.NotifyItemRemoved(pos + 1);
                    count++;
                }
            }
            else
            {
                int index = pos + 1;
                foreach (UserProfileEntryData entryNode in groupSelect.items)
                {
                    listItems.Insert(index, entryNode);
                    userProfileExpandableAdapter.NotifyItemInserted(index);
                    index++;
                }
                groupSelect.items.Clear();

                int last_group_index = 0;
                int count = 0;
                foreach (var item in UserProfileExpandableAdapter.listItems)
                {
                    if (item is UserProfileGroupData)
                        last_group_index = count;
                    count++;
                }
                if (pos == last_group_index)
                    recyclerView.SmoothScrollToPosition(UserProfileExpandableAdapter.listItems.Capacity - 1);
            }
        }


        public override void OnBackPressed()
        {
            try
            {
                if (tintLL.Visibility == ViewStates.Visible)
                {
                    tintLL.Visibility = ViewStates.Gone;
                    gridLL.Visibility = ViewStates.Gone;
                    add_skillIV.Visibility = ViewStates.Visible;
                }
                else
                    base.OnBackPressed();
            }
            catch { base.OnBackPressed(); }
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            tintLL.Visibility = ViewStates.Gone;
            gridLL.Visibility = ViewStates.Gone;
            add_skillIV.Visibility = ViewStates.Visible;
            if (PictureMethods.cameraOrGalleryIndicator == "gallery")
            {
                if (resultCode == Result.Ok)
                {
                    profile_image.SetImageURI(data.Data);

                    chosen_bitmap = ((BitmapDrawable)profile_image.Drawable).Bitmap;
                    MemoryStream chosen_memStream = new MemoryStream();
                    chosen_bitmap.Compress(Bitmap.CompressFormat.Jpeg, 15, chosen_memStream);
                    byte[] chosen_ba = chosen_memStream.ToArray();
                    chosen_base64 = Base64.EncodeToString(chosen_ba, Base64.Default);
                    upload_avatar_cam_gallery();
                }
            }
            else if (PictureMethods.cameraOrGalleryIndicator == "camera")
            {
                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
                mediaScanIntent.SetData(contentUri);
                SendBroadcast(mediaScanIntent);
                int height = Resources.DisplayMetrics.HeightPixels;
                int width = Resources.DisplayMetrics.WidthPixels;
                App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
                if (App.bitmap != null)
                {
                    profile_image.SetImageBitmap(App.bitmap);
                    new UserProfileExpandableAdapter().LoadImage(null, null, App.bitmap);

                    MemoryStream chosen_memStream = new MemoryStream();
                    App.bitmap.Compress(Bitmap.CompressFormat.Jpeg, 15, chosen_memStream);
                    byte[] chosen_ba = chosen_memStream.ToArray();
                    chosen_base64 = Base64.EncodeToString(chosen_ba, Base64.Default);
                    await upload_avatar_cam_gallery();// });

                    App.bitmap = null;
                }

                // Dispose of the Java side bitmap.
                GC.Collect();
            }
            int ldld = 09;
        }
        public static void ChangeVisibility()
        {
            if (tintLL.Visibility == ViewStates.Visible)
            {
                add_skillIV.Visibility = ViewStates.Visible;
                tintLL.Visibility = ViewStates.Gone;
                gridLL.Visibility = ViewStates.Gone;
            }
            else
            {
                add_skillIV.Visibility = ViewStates.Gone;
                tintLL.Visibility = ViewStates.Visible;
                gridLL.Visibility = ViewStates.Visible;
            }
        }
        async Task<string> upload_avatar()
        {
            if (timer.Enabled)
            {
                var lddddddddddddddddd = chosen_base64;
                image_activityIndicator.Visibility = ViewStates.Visible;
                ImageRL.Visibility = ViewStates.Gone;
                var response = await profileAndExpertMethods.EditMyProfileImage(userMethods.GetUsersAuthToken(), chosen_base64);
                image_activityIndicator.Visibility = ViewStates.Gone;
                ImageRL.Visibility = ViewStates.Visible;
                timer.Stop();
                timer.Enabled = false;
                return response;
            }
            return null;
        }
        async Task<string> upload_avatar_cam_gallery()
        {
            image_activityIndicator.Visibility = ViewStates.Visible;
            ImageRL.Visibility = ViewStates.Gone;
            var response = await profileAndExpertMethods.EditMyProfileImage(userMethods.GetUsersAuthToken(), chosen_base64);
            image_activityIndicator.Visibility = ViewStates.Gone;
            ImageRL.Visibility = ViewStates.Visible;
            timer.Enabled = false;
            timer.Stop();
            return response;
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
                          CheckSelfPermission(Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted
                          || CheckSelfPermission(Manifest.Permission.ReadExternalStorage) != Android.Content.PM.Permission.Granted
                          || CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted
                          )
                {
                    ActivityCompat.RequestPermissions(this, new String[]
                    {
                                Manifest.Permission.Camera,
                                Manifest.Permission.ReadExternalStorage,
                                Manifest.Permission.WriteExternalStorage,
                    }, REQUEST_PERMISSION_CODE);
                }
                else
                {
                    ActivityCompat.RequestPermissions(this, new String[]
                    {
                                Manifest.Permission.Camera,
                                Manifest.Permission.ReadExternalStorage,
                                Manifest.Permission.WriteExternalStorage,
                    }, REQUEST_PERMISSION_CODE);
                }
        }
        bool shown = false;
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            switch (requestCode)
            {
                case REQUEST_PERMISSION_CODE:
                    {
                        if (grantResults.Length > 0 && grantResults[0] == Android.Content.PM.Permission.Granted)
                        {
                            StartActivity(typeof(MainActivity));
                        }
                        else
                        {
                            if (!shown)
                                Toast.MakeText(this, GetString(Resource.String.permissions_needed), ToastLength.Long).Show();
                            shown = true;
                            request_runtime_permissions();
                        }
                        break;
                    }
            }
        }
    }
}