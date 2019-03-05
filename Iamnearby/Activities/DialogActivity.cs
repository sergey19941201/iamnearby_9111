using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Com.Orangegangsters.Github.Swipyrefreshlayout.Library;
using Iamnearby.Adapters;
using Iamnearby.Methods;
using Java.IO;
using Newtonsoft.Json;
using PCL.Database;
using PCL.Models;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden)]
    public class DialogActivity : AppCompatActivity, INotifyPropertyChanged
    {
        public bool dialogNotify { get; set; }
        public int categoryId { get; set; }
        //public bool is_expert { get; set; }
        int offset { get; set; }
        IEnumerable<ChatHistoryModel> reverse_history;
        ISharedPreferences dialog_data;
        ISharedPreferences expert_data;
        ISharedPreferencesEditor edit_dialog, edit_expert;
        ISharedPreferences expert_feedback_pref = Application.Context.GetSharedPreferences("expert_feedback_pref", FileCreationMode.Private);
        ISharedPreferencesEditor edit_expert_feedback;
        ISharedPreferences device_info_prefs = Application.Context.GetSharedPreferences("device_info", FileCreationMode.Private);
        static DialogMethods dialogMethods;
        static UserMethods userMethods;
        RelativeLayout contextRL, emptyRL;
        ImageView profile_image/*, add_fileIV*/;
        TextView expertNameTV, was_onlineTV, title_acceptTV, yes_acceptTV, no_acceptTV;
        ImageButton back_button;
        RelativeLayout backRelativeLayout;
        Button sendBn, attach_voiceBn, cancel_voiceBn;
        ImageView takePhotoIV,/* microphoneIV,*/ file_attachedIV;
        LinearLayout dividerLL, recordLL, add_fileLL, file_attachedLL;
        EditText messageET;
        ProgressBar activityIndicator, activityIndicatorSend, activityIndicatorAccept;
        static RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        RelativeLayout headerRelativeLayout;
        LinearLayout make_photoLL, gallery_photoLL, send_docLL;
        RelativeLayout tintLL, tintVoiceLL, tint_acceptLL;
        PictureMethods pictureMethods;
        TextView specializationTV;
        static DialogAdapter dialogsListAdapter;
        ChatHistoryRootObject deserializedChat;
        System.Timers.Timer timer;
        System.Timers.Timer timer_update;
        private const int REQUEST_PERMISSION_CODE = 1000;
        private bool isGrantedPermission;
        static bool already_requested = false;
        MediaRecorder mediaRecorder;
        string historyJson, popup_menu_value;
        static string chatId;
        static string static_path, static_filename;
        static Java.IO.File camera_file;
        static byte[] static_b;
        static FileInputStream camera_fileInputStream;
        string expert_phone, to_blacklist;
        bool expert_online;

        SwipeRefreshLayout refresher;
        SwipyRefreshLayout refresherBottom;

        static string propertyName;
        public event PropertyChangedEventHandler PropertyChanged;
        public string base64_static
        {
            get { return propertyName; }
            set
            {
                if (propertyName != value)
                {
                    propertyName = value;
                    OnPropertyChanged("file");
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (propertyName == "file")
            {
                if (String.IsNullOrEmpty(base64_static))
                {
                    file_attachedLL.Visibility = ViewStates.Gone;
                    add_fileLL.Visibility = ViewStates.Visible;
                    if (String.IsNullOrEmpty(messageET.Text))
                    {
                        sendBn.Visibility = ViewStates.Gone;
                        recordLL.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        sendBn.Visibility = ViewStates.Visible;
                        recordLL.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    file_attachedLL.Visibility = ViewStates.Visible;
                    add_fileLL.Visibility = ViewStates.Gone;
                    sendBn.Visibility = ViewStates.Visible;
                    recordLL.Visibility = ViewStates.Gone;
                    if (static_filename.ToLower().Contains(".mp3"))
                        file_attachedIV.SetBackgroundResource(Resource.Drawable.voice_attach);
                    else if (static_filename.ToLower().Contains(".jpg")
                            || static_filename.Contains(".png")
                            || static_filename.Contains(".jpeg"))
                    {
                        var bitmap = BitmapFactory.DecodeByteArray(static_b, 0, static_b.Length);
                        BitmapDrawable ob = new BitmapDrawable(Resources, bitmap);
                        file_attachedIV.SetBackgroundDrawable(ob);
                    }
                    else
                        file_attachedIV.SetBackgroundResource(Resource.Drawable.document_pick);

                }
            }

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        static string reswww = "";
        static ChatHistoryRootObject deserialized_new_content;
        static IEnumerable<ChatHistoryModel> reverse_update;
        public static async Task<string> messageReceived(string msg_id, string dialogId, bool mark_read/*, string message_text, string file, string file_base64, string timestamp*/)
        {
            if (chatId == dialogId)
            {
                reswww = await dialogMethods.ChatHistoryUpdate(userMethods.GetUsersAuthToken(), chatId, msg_id, mark_read);//update_meth();
                deserialized_new_content = JsonConvert.DeserializeObject<ChatHistoryRootObject>(reswww);
                reverse_update = Enumerable.Reverse(deserialized_new_content.messages); ;
                foreach (var item in reverse_update)
                {
                    if (item.in_out == "0")
                    {
                        try
                        {
                            if (item.file_small.ToLower().Contains(".jpg")
                                || item.file_small.Contains(".png")
                                || item.file_small.Contains(".jpeg"))
                            {
                                DialogAdapter.messages_list_static.Add(new ChatHistoryModel
                                {
                                    in_out = item.in_out,
                                    message = item.message,
                                    file_small = item.file_small,
                                    file = item.file,
                                    file_base64 = item.file_base64,
                                    timestamp = new Timestamp { date = item.timestamp.date },
                                    msg_id = item.msg_id
                                });
                            }
                            else
                            {
                                DialogAdapter.messages_list_static.Add(new ChatHistoryModel
                                {
                                    in_out = item.in_out,
                                    message = item.message,
                                    file = item.file,
                                    file_base64 = item.file_base64,
                                    timestamp = new Timestamp { date = item.timestamp.date },
                                    msg_id = item.msg_id
                                });
                            }
                            try
                            {
                                dialogsListAdapter.NotifyDataSetChanged();
                            }
                            catch (Exception ex)
                            { }
                            recyclerView.SmoothScrollToPosition(DialogAdapter.messages_list_static.Capacity - 1);
                        }
                        catch
                        {

                        }
                    }
                };
            }
            return "";
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            switch (requestCode)
            {
                case REQUEST_PERMISSION_CODE:
                    {
                        if (grantResults.Length > 0 && grantResults[0] == Android.Content.PM.Permission.Granted)
                        {
                            isGrantedPermission = true;
                        }
                        break;
                    }
            }
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try { DialogAdapter.messages_list_static.Clear(); } catch { }
            try { reverse_history = null; } catch { }
            try
            {
                SetContentView(Resource.Layout.Dialog);
                userMethods = new UserMethods();

                isGrantedPermission = false;
                pictureMethods = new PictureMethods();
                dialogMethods = new DialogMethods();
                expert_data = Application.Context.GetSharedPreferences("experts", FileCreationMode.Private);
                to_blacklist = "";
                offset = 10;
                base64_static = null;
                static_filename = null;
                file_attachedIV = FindViewById<ImageView>(Resource.Id.file_attachedIV);
                tintLL = FindViewById<RelativeLayout>(Resource.Id.tintLL);
                tint_acceptLL = FindViewById<RelativeLayout>(Resource.Id.tint_acceptLL);
                make_photoLL = FindViewById<LinearLayout>(Resource.Id.make_photoLL);
                gallery_photoLL = FindViewById<LinearLayout>(Resource.Id.gallery_photoLL);
                send_docLL = FindViewById<LinearLayout>(Resource.Id.send_docLL);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicatorSend = FindViewById<ProgressBar>(Resource.Id.activityIndicatorSend);
                activityIndicatorAccept = FindViewById<ProgressBar>(Resource.Id.activityIndicatorAccept);
                headerRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.headerRelativeLayout);
                sendBn = FindViewById<Button>(Resource.Id.sendBn);
                attach_voiceBn = FindViewById<Button>(Resource.Id.attach_voiceBn);
                cancel_voiceBn = FindViewById<Button>(Resource.Id.cancel_voiceBn);
                takePhotoIV = FindViewById<ImageView>(Resource.Id.takePhotoIV);
                dividerLL = FindViewById<LinearLayout>(Resource.Id.dividerLL);
                recordLL = FindViewById<LinearLayout>(Resource.Id.recordLL);
                add_fileLL = FindViewById<LinearLayout>(Resource.Id.add_fileLL);
                messageET = FindViewById<EditText>(Resource.Id.messageET);
                specializationTV = FindViewById<TextView>(Resource.Id.specializationTV);
                contextRL = FindViewById<RelativeLayout>(Resource.Id.contextRL);
                emptyRL = FindViewById<RelativeLayout>(Resource.Id.emptyRL);
                tintVoiceLL = FindViewById<RelativeLayout>(Resource.Id.tintVoiceLL);
                profile_image = FindViewById<ImageView>(Resource.Id.profile_image);
                file_attachedLL = FindViewById<LinearLayout>(Resource.Id.file_attachedLL);
                expertNameTV = FindViewById<TextView>(Resource.Id.expertNameTV);
                was_onlineTV = FindViewById<TextView>(Resource.Id.was_onlineTV);
                title_acceptTV = FindViewById<TextView>(Resource.Id.title_acceptTV);
                yes_acceptTV = FindViewById<TextView>(Resource.Id.yes_acceptTV);
                no_acceptTV = FindViewById<TextView>(Resource.Id.no_acceptTV);
                dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                var expert_id = dialog_data.GetString("expert_id", String.Empty);
                expertNameTV.Text = dialog_data.GetString("expert_name", String.Empty);
                recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                recyclerView.SetRecycledViewPool(new RecyclerView.RecycledViewPool());
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                expertNameTV.SetTypeface(tf, TypefaceStyle.Bold);
                messageET.SetTypeface(tf, TypefaceStyle.Normal);
                sendBn.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textView1).SetTypeface(tf, TypefaceStyle.Bold);
                FindViewById<TextView>(Resource.Id.textViews1).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textVisew2).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textVieasw3).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textViessssw1).SetTypeface(tf, TypefaceStyle.Normal);
                specializationTV.SetTypeface(tf, TypefaceStyle.Normal);
                attach_voiceBn.SetTypeface(tf, TypefaceStyle.Normal);
                cancel_voiceBn.SetTypeface(tf, TypefaceStyle.Normal);
                title_acceptTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.your_city_valueTV).SetTypeface(tf, TypefaceStyle.Normal);
                yes_acceptTV.SetTypeface(tf, TypefaceStyle.Normal);
                no_acceptTV.SetTypeface(tf, TypefaceStyle.Normal);

                Thread backgroundThread = new Thread(new ThreadStart(() =>
                {
                    Glide.Get(Application.Context).ClearDiskCache();
                }));
                backgroundThread.IsBackground = true;
                backgroundThread.Start();
                Glide.Get(this).ClearMemory();

                var expert_avatar = dialog_data.GetString("expert_avatar", String.Empty);
                if (!expert_avatar.Contains("api.iam"))
                    Glide.With(Application.Context)
                        .Load("https://api.iamnearby.net/" + expert_avatar)
                        .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
                        .SkipMemoryCache(true))
                        //.Placeholder(Resource.Drawable.specialization_imageIV)
                        .Into(profile_image);
                else
                    Glide.With(Application.Context)
                        .Load(expert_avatar)
                        .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
                        .SkipMemoryCache(true))
                        //.Placeholder(Resource.Drawable.specialization_imageIV)
                        .Into(profile_image);
                edit_expert = expert_data.Edit();
                headerRelativeLayout.Click += (s, e) =>
                {
                    edit_dialog = dialog_data.Edit();
                    edit_dialog.PutString("come_from", "Came directly from bottom");
                    edit_dialog.Apply();
                    edit_expert.PutString("expert_id", expert_id);
                    edit_expert.Apply();
                    StartActivity(typeof(ThreeLevelExpertProfileActivity));
                };
                contextRL.Click += (s, e) =>
                {
                    //dialogNotify
                    //categoryId
                    Android.Support.V7.Widget.PopupMenu popup_menu = new Android.Support.V7.Widget.PopupMenu(this, FindViewById<RelativeLayout>(Resource.Id.contextRL));
                    var menuOpts = popup_menu.Menu;
                    if (dialogNotify)
                    {
                        if (categoryId > 0)
                        {
                            edit_expert_feedback = expert_feedback_pref.Edit();
                            edit_expert_feedback.PutString("expert_id", expert_id);
                            edit_expert_feedback.PutString("expert_name", dialog_data.GetString("expert_name", String.Empty));
                            edit_expert_feedback.PutString("expert_phone", expert_phone);
                            edit_expert_feedback.PutBoolean("expert_online", expert_online);
                            edit_expert_feedback.PutString("expert_avatar", expert_avatar);
                            edit_expert_feedback.PutString("expert_category_id", categoryId.ToString());
                            edit_expert_feedback.Apply();
                            popup_menu.Inflate(Resource.Layout.dialog_popup_menu);
                            if (to_blacklist == "to_blacklist")
                            {
                                menuOpts.GetItem(2).SetTitle(Resource.String.restore_conversation);
                            }
                        }
                        else
                        {
                            popup_menu.Inflate(Resource.Layout.dialog_with_user_popup_menu);
                            if (to_blacklist == "to_blacklist")
                                menuOpts.GetItem(1).SetTitle(Resource.String.restore_conversation);
                        }
                    }
                    else
                    {
                        if (categoryId > 0)
                        {
                            edit_expert_feedback = expert_feedback_pref.Edit();
                            edit_expert_feedback.PutString("expert_id", expert_id);
                            edit_expert_feedback.PutString("expert_name", dialog_data.GetString("expert_name", String.Empty));
                            edit_expert_feedback.PutString("expert_phone", expert_phone);
                            edit_expert_feedback.PutBoolean("expert_online", expert_online);
                            edit_expert_feedback.PutString("expert_avatar", expert_avatar);
                            edit_expert_feedback.PutString("expert_category_id", categoryId.ToString());
                            edit_expert_feedback.Apply();
                            popup_menu.Inflate(Resource.Layout.dialog_popup_menu_on);
                            if (to_blacklist == "to_blacklist")
                            {
                                menuOpts.GetItem(2).SetTitle(Resource.String.restore_conversation);
                            }
                        }
                        else
                        {
                            popup_menu.Inflate(Resource.Layout.dialog_with_user_popup_menu_on);
                            if (to_blacklist == "to_blacklist")
                                menuOpts.GetItem(1).SetTitle(Resource.String.restore_conversation);
                        }
                    }
                    popup_menu.MenuItemClick += (s1, arg1) =>
                    {
                        if (arg1.Item.TitleFormatted.ToString() == GetString(Resource.String.feedback_text))
                        {
                            StartActivity(typeof(FeedbackChooseCategoryActivity));
                        }
                        if (arg1.Item.TitleFormatted.ToString() == GetString(Resource.String.clear_chat_history))
                        {
                            title_acceptTV.Text = GetString(Resource.String.clear_chat_history) + "?";
                            popup_menu_value = GetString(Resource.String.clear_chat_history);
                            tint_acceptLL.Visibility = ViewStates.Visible;
                        }
                        if (arg1.Item.TitleFormatted.ToString() == GetString(Resource.String.blacklist))
                        {
                            if (to_blacklist == "to_blacklist")
                                title_acceptTV.Text = GetString(Resource.String.remove_from_blacklist) + "?";
                            else
                                title_acceptTV.Text = GetString(Resource.String.move_to_blacklist) + "?";
                            popup_menu_value = GetString(Resource.String.blacklist);
                            tint_acceptLL.Visibility = ViewStates.Visible;
                        }
                        if (arg1.Item.TitleFormatted.ToString() == GetString(Resource.String.restore_conversation))
                        {
                            title_acceptTV.Text = GetString(Resource.String.remove_from_blacklist) + "?";
                            popup_menu_value = GetString(Resource.String.blacklist);
                            tint_acceptLL.Visibility = ViewStates.Visible;
                        }
                        if (arg1.Item.TitleFormatted.ToString() == GetString(Resource.String.turn_off_notifications))
                        {
                            title_acceptTV.Text = GetString(Resource.String.turn_off_notifications) + "?";
                            popup_menu_value = GetString(Resource.String.turn_off_notifications);
                            tint_acceptLL.Visibility = ViewStates.Visible;
                        }
                        if (arg1.Item.TitleFormatted.ToString() == GetString(Resource.String.turn_on_notifications))
                        {
                            title_acceptTV.Text = GetString(Resource.String.turn_on_notifications) + "?";
                            popup_menu_value = GetString(Resource.String.turn_on_notifications);
                            tint_acceptLL.Visibility = ViewStates.Visible;
                        }
                    };

                    try
                    {
                        popup_menu.Show();
                    }
                    catch
                    {

                    }
                };
                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                messageET.TextChanged += (s, e) =>
                {
                    if (String.IsNullOrEmpty(messageET.Text) && String.IsNullOrEmpty(base64_static))
                    {
                        sendBn.Visibility = ViewStates.Gone;
                        recordLL.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        sendBn.Visibility = ViewStates.Visible;
                        recordLL.Visibility = ViewStates.Gone;
                    }
                };
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                activityIndicatorAccept.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                activityIndicatorSend.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                activityIndicator.Visibility = ViewStates.Visible;
                string res = "";
                if (dialog_data.GetString("come_from", String.Empty) != "Came directly from bottom")
                    res = await dialogMethods.CreateChat(userMethods.GetUsersAuthToken(), dialog_data.GetString("expert_id", String.Empty), expert_data.GetString("spec_id", String.Empty));
                else
                    res = await dialogMethods.CreateChat(userMethods.GetUsersAuthToken(), dialog_data.GetString("expert_id", String.Empty));
                var deserializedId = JsonConvert.DeserializeObject<ChatId>(res);
                string auth_token_from_dialog = "";
                string category_name = "";
                try
                {
                    chatId = deserializedId.chatId;
                    historyJson = await dialogMethods.ChatHistory(userMethods.GetUsersAuthToken(), chatId);
                    activityIndicator.Visibility = ViewStates.Gone;

                    deserializedChat = JsonConvert.DeserializeObject<ChatHistoryRootObject>(historyJson);
                    dialogNotify = deserializedChat.dialogNotify;
                    categoryId = deserializedChat.categoryId;
                    category_name = deserializedChat.categoryName;
                    expert_online = deserializedChat.online;
                    expert_phone = deserializedChat.phone;
                    to_blacklist = deserializedChat.blacklist;
                    auth_token_from_dialog = deserializedChat.authToken;
                }
                catch
                {
                    auth_token_from_dialog = "null";
                    if (deserializedChat != null)
                        deserializedChat.messages = null;
                    emptyRL.Visibility = ViewStates.Gone;
                    recyclerView.Visibility = ViewStates.Visible;
                }
                specializationTV.Text = category_name;

                if (!userMethods.UserExists())
                {
                    if (/*!String.IsNullOrEmpty(auth_token_from_dialog) && */auth_token_from_dialog == "null")
                    {
                        var device_json = JsonConvert.DeserializeObject<Device>(device_info_prefs.GetString("device_json", String.Empty));
                        var fake_email = device_json.deviceToken + "@fake.iamnearby.net";
                    }
                }
                if (deserializedChat != null)
                {
                    if (deserializedChat.messages != null)
                    {
                        if (deserializedChat.messages.Count == 0)
                        {
                            emptyRL.Visibility = ViewStates.Visible;
                            recyclerView.Visibility = ViewStates.Gone;
                            try
                            {
                                DialogAdapter.messages_list_static.Clear();
                                reverse_history = null;
                            }
                            catch { }
                        }
                        else
                        {
                            emptyRL.Visibility = ViewStates.Gone;
                            recyclerView.Visibility = ViewStates.Visible;
                            try
                            {
                                DialogAdapter.messages_list_static.Clear();
                                reverse_history = null;
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        emptyRL.Visibility = ViewStates.Visible;
                        recyclerView.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    emptyRL.Visibility = ViewStates.Visible;
                    recyclerView.Visibility = ViewStates.Gone;
                }
                if (deserializedChat != null)
                    if (deserializedChat.messages != null)
                    {
                        reverse_history = Enumerable.Reverse(deserializedChat.messages);
                        dialogsListAdapter = new DialogAdapter(reverse_history.ToList<ChatHistoryModel>(), this, chatId, tf);
                    }
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerView.SetLayoutManager(layoutManager);

                recyclerView.SetAdapter(dialogsListAdapter);

                sendBn.Click += async (s, e) =>
                {
                    if (!userMethods.UserExists())
                    {
                        StartActivity(typeof(RegPartialActivity));
                    }
                    else
                    {
                        activityIndicatorSend.Visibility = ViewStates.Visible;
                        sendBn.Visibility = ViewStates.Gone;
                        string message_text;
                        if (String.IsNullOrEmpty(messageET.Text))
                            message_text = "message_text_empty";
                        else
                            message_text = messageET.Text;
                        string mes_result = "";
                        if (deserializedId != null)
                        {
                            if (deserializedId.chatId != null)
                            {
                                mes_result = await dialogMethods.SendMessage(
                                   userMethods.GetUsersAuthToken(),
                                   deserializedId.chatId,
                                   message_text,
                                   base64_static,
                                   static_filename);
                            }
                        }
                        else
                        {
                            if (dialog_data.GetString("come_from", String.Empty) != "Came directly from bottom")
                                res = await dialogMethods.CreateChat(userMethods.GetUsersAuthToken(), dialog_data.GetString("expert_id", String.Empty), expert_data.GetString("spec_id", String.Empty));
                            else
                                res = await dialogMethods.CreateChat(userMethods.GetUsersAuthToken(), dialog_data.GetString("expert_id", String.Empty));
                            var deserializedId_сhat = JsonConvert.DeserializeObject<ChatId>(res);

                            try
                            {
                                chatId = deserializedId_сhat.chatId;
                                mes_result = await dialogMethods.SendMessage(
                                       userMethods.GetUsersAuthToken(),
                                       chatId,
                                       message_text,
                                       base64_static,
                                       static_filename);
                            }
                            catch
                            {
                                Toast.MakeText(this, Resource.String.impossible_perform_operation, ToastLength.Short).Show();
                            }
                        }
                        var now = DateTime.Now;

                        if (mes_result != "false")
                        {
                            try
                            {
                                var time_stamp = JsonConvert.DeserializeObject<TimeStampModel>(mes_result);
                                string last_date;
                                try
                                {
                                    var last_timestamp = DialogAdapter.messages_list_static[DialogAdapter.messages_list_static.Count - 1].timestamp;
                                    last_date = last_timestamp.date.Substring(0, 10);
                                }
                                catch
                                {
                                    last_date = "";
                                }
                                var current_timestamp = time_stamp.timestamp;
                                var current_date = current_timestamp.Substring(0, 10);
                                if (current_date != last_date)
                                {
                                    DialogAdapter.messages_list_static.Add(new ChatHistoryModel { in_out = "2", message = "", timestamp = new Timestamp { date = time_stamp.timestamp }, msg_id = time_stamp.msg_id });
                                }
                            }
                            catch { }
                            try
                            {
                                var time_stamp = JsonConvert.DeserializeObject<TimeStampModel>(mes_result);
                                if (!String.IsNullOrEmpty(time_stamp.file_url))
                                {
                                    if (time_stamp.file_url.ToLower().Contains(".jpg")
                                    || time_stamp.file_url.Contains(".png")
                                    || time_stamp.file_url.Contains(".jpeg"))
                                    {
                                        DialogAdapter.messages_list_static.Add(new ChatHistoryModel
                                        {
                                            in_out = "1",
                                            message = messageET.Text,
                                            file_small = time_stamp.file_small,
                                            file = time_stamp.file_url,
                                            file_base64 = base64_static,
                                            timestamp = new Timestamp { date = time_stamp.timestamp },
                                            msg_id = time_stamp.msg_id
                                        });
                                    }
                                    else
                                    {
                                        DialogAdapter.messages_list_static.Add(new ChatHistoryModel
                                        {
                                            in_out = "1",
                                            message = messageET.Text,
                                            file = time_stamp.file_url,
                                            file_base64 = base64_static,
                                            timestamp = new Timestamp { date = time_stamp.timestamp },
                                            msg_id = time_stamp.msg_id
                                        });
                                    }
                                }
                                else
                                {
                                    DialogAdapter.messages_list_static.Add(new ChatHistoryModel
                                    {
                                        in_out = "1",
                                        message = messageET.Text,
                                        file = time_stamp.file_url,
                                        file_base64 = base64_static,
                                        timestamp = new Timestamp { date = time_stamp.timestamp },
                                        msg_id = time_stamp.msg_id
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                Toast.MakeText(this, GetString(Resource.String.impossible_perform_operation), ToastLength.Short).Show();
                                StartActivity(typeof(MainActivity));
                                return;
                            }
                            emptyRL.Visibility = ViewStates.Gone;
                            recyclerView.Visibility = ViewStates.Visible;
                            try
                            {
                                dialogsListAdapter.NotifyDataSetChanged();
                                recyclerView.SmoothScrollToPosition(DialogAdapter.messages_list_static.Capacity - 1);
                            }
                            catch
                            {
                                dialogsListAdapter = new DialogAdapter(DialogAdapter.messages_list_static, this, chatId, tf);
                                dialogsListAdapter.NotifyDataSetChanged();
                                recyclerView.SetAdapter(dialogsListAdapter);
                                recyclerView.SmoothScrollToPosition(DialogAdapter.messages_list_static.Capacity - 1);
                            }
                            activityIndicatorSend.Visibility = ViewStates.Gone;
                            sendBn.Visibility = ViewStates.Gone;
                            recordLL.Visibility = ViewStates.Visible;
                            messageET.Text = null;

                        }
                        else
                        {
                            try
                            {
                                var token = userMethods.GetUsersAuthToken();
                                if (dialog_data.GetString("come_from", String.Empty) != "Came directly from bottom")
                                    res = await dialogMethods.CreateChat(userMethods.GetUsersAuthToken(), dialog_data.GetString("expert_id", String.Empty), expert_data.GetString("spec_id", String.Empty));
                                else
                                    res = await dialogMethods.CreateChat(userMethods.GetUsersAuthToken(), dialog_data.GetString("expert_id", String.Empty));
                                var deserializedId_сhat = JsonConvert.DeserializeObject<ChatId>(res);
                                chatId = deserializedId_сhat.chatId;
                                activityIndicatorSend.Visibility = ViewStates.Visible;
                                sendBn.Visibility = ViewStates.Gone;

                                if (res.ToLower().Contains("с самим собой"))
                                {
                                    Toast toast = Toast.MakeText(this, GetString(Resource.String.cant_create_chat_with_yourself), ToastLength.Short);
                                    toast.SetGravity(Gravity.GetAbsoluteGravity(GravityFlags.Center, GravityFlags.Center), 0, 0);
                                    toast.Show();
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(messageET.Text))
                                        message_text = "message_text_empty";
                                    else
                                        message_text = messageET.Text;

                                    var mes_result_ = await dialogMethods.SendMessage(
                                        userMethods.GetUsersAuthToken(),
                                        chatId,
                                        message_text,
                                        base64_static,
                                        static_filename);


                                    if (mes_result_ != "false")
                                    {
                                        try
                                        {
                                            var time_stamp = JsonConvert.DeserializeObject<TimeStampModel>(mes_result_);
                                            string last_date;
                                            try
                                            {
                                                var last_timestamp = DialogAdapter.messages_list_static[DialogAdapter.messages_list_static.Count - 1].timestamp;
                                                last_date = last_timestamp.date.Substring(0, 10);
                                            }
                                            catch
                                            {
                                                last_date = "";
                                            }
                                            var current_timestamp = time_stamp.timestamp;
                                            var current_date = current_timestamp.Substring(0, 10);
                                            if (current_date != last_date)
                                            {
                                                DialogAdapter.messages_list_static.Add(new ChatHistoryModel { in_out = "2", message = "", timestamp = new Timestamp { date = time_stamp.timestamp }, msg_id = time_stamp.msg_id });
                                            }
                                        }
                                        catch { }
                                        try
                                        {
                                            var time_stamp = JsonConvert.DeserializeObject<TimeStampModel>(mes_result_);
                                            DialogAdapter.messages_list_static.Add(new ChatHistoryModel
                                            {
                                                in_out = "1",
                                                message = messageET.Text,
                                                file = time_stamp.file_url,
                                                file_base64 = base64_static,
                                                timestamp = new Timestamp { date = time_stamp.timestamp },
                                                msg_id = time_stamp.msg_id
                                            });
                                        }
                                        catch
                                        {
                                            StartActivity(typeof(MainActivity));
                                        }
                                        emptyRL.Visibility = ViewStates.Gone;
                                        recyclerView.Visibility = ViewStates.Visible;
                                        try
                                        {
                                            dialogsListAdapter.NotifyDataSetChanged();
                                            recyclerView.SmoothScrollToPosition(DialogAdapter.messages_list_static.Capacity - 1);
                                        }
                                        catch
                                        {
                                            dialogsListAdapter = new DialogAdapter(DialogAdapter.messages_list_static, this, chatId, tf);
                                            dialogsListAdapter.NotifyDataSetChanged();
                                            recyclerView.SetAdapter(dialogsListAdapter);
                                            recyclerView.SmoothScrollToPosition(DialogAdapter.messages_list_static.Capacity - 1);
                                        }
                                        activityIndicatorSend.Visibility = ViewStates.Gone;
                                        sendBn.Visibility = ViewStates.Gone;
                                        recordLL.Visibility = ViewStates.Visible;
                                        messageET.Text = null;
                                    }
                                }
                            }
                            catch { }
                            activityIndicatorSend.Visibility = ViewStates.Gone;
                            sendBn.Visibility = ViewStates.Gone;
                            recordLL.Visibility = ViewStates.Visible;
                        }
                        base64_static = null;
                        static_filename = null;
                    }
                };

                add_fileLL.Click += async (s, e) =>
                {
                    var granted = await checkCameraPermission();
                    if (!granted)
                        return;
                    if (tintLL.Visibility == ViewStates.Gone)
                        tintLL.Visibility = ViewStates.Visible;
                    else
                        tintLL.Visibility = ViewStates.Gone;
                };
                tintLL.Click += (s, e) =>
                {
                    tintLL.Visibility = ViewStates.Gone;
                };
                tint_acceptLL.Click += (s, e) =>
                {
                    tint_acceptLL.Visibility = ViewStates.Gone;
                };
                yes_acceptTV.Click += async (s, e) =>
                {
                    if (popup_menu_value == GetString(Resource.String.clear_chat_history))
                    {
                        activityIndicatorAccept.Visibility = ViewStates.Visible;
                        var clear_his = await dialogMethods.ClearHistory(userMethods.GetUsersAuthToken(), chatId);
                        activityIndicatorAccept.Visibility = ViewStates.Gone;
                        tint_acceptLL.Visibility = ViewStates.Gone;
                        DialogAdapter.messages_list_static.Clear();
                        reverse_history = null;
                        var intent = new Intent(this, typeof(DialogActivity));
                        intent.SetFlags(ActivityFlags.NewTask);
                        StartActivity(intent);
                        Finish();
                    }
                    if (popup_menu_value == GetString(Resource.String.blacklist))
                    {
                        Intent intent;
                        if (to_blacklist == "to_blacklist")
                        {
                            var end_conv = await dialogMethods.CloseChat(userMethods.GetUsersAuthToken(), chatId, false);
                            intent = new Intent(this, typeof(DialogActivity));
                            to_blacklist = "";
                        }
                        else
                        {
                            var end_conv = await dialogMethods.CloseChat(userMethods.GetUsersAuthToken(), chatId, true);
                            intent = new Intent(this, typeof(ChatListActivity));
                            to_blacklist = "to_blacklist";
                        }

                        activityIndicatorAccept.Visibility = ViewStates.Visible;
                        activityIndicatorAccept.Visibility = ViewStates.Gone;
                        tint_acceptLL.Visibility = ViewStates.Gone;
                        DialogAdapter.messages_list_static.Clear();
                        reverse_history = null;

                        intent.SetFlags(ActivityFlags.NewTask);
                        StartActivity(intent);
                        Finish();
                    }
                    if (popup_menu_value == GetString(Resource.String.turn_off_notifications))
                    {
                        activityIndicatorAccept.Visibility = ViewStates.Visible;
                        var turn_off_notifs = await dialogMethods.ToggleNotifications(userMethods.GetUsersAuthToken(), chatId, 0);
                        activityIndicatorAccept.Visibility = ViewStates.Gone;
                        tint_acceptLL.Visibility = ViewStates.Gone;
                        dialogNotify = false;
                    }
                    if (popup_menu_value == GetString(Resource.String.turn_on_notifications))
                    {
                        activityIndicatorAccept.Visibility = ViewStates.Visible;
                        var turn_on_notifs = await dialogMethods.ToggleNotifications(userMethods.GetUsersAuthToken(), chatId, 1);
                        activityIndicatorAccept.Visibility = ViewStates.Gone;
                        tint_acceptLL.Visibility = ViewStates.Gone;
                        dialogNotify = true;
                    }
                };
                no_acceptTV.Click += (s, e) =>
                {
                    tint_acceptLL.Visibility = ViewStates.Gone;
                };

                send_docLL.Click += (s, e) =>
                {
                    PictureMethods.cameraOrGalleryIndicator = "gallery";
                    var fileIntent = new Intent();
                    fileIntent.SetType("*/*");
                    tintLL.Visibility = ViewStates.Gone;
                    fileIntent.SetAction(Intent.ActionGetContent);
                    StartActivityForResult(
                        Intent.CreateChooser(fileIntent, "Select file"), 0);
                };
                gallery_photoLL.Click += (s, e) =>
                {
                    PictureMethods.cameraOrGalleryIndicator = "gallery";
                    var imageIntent = new Intent();
                    imageIntent.SetType("image/*");
                    tintLL.Visibility = ViewStates.Gone;
                    imageIntent.SetAction(Intent.ActionGetContent);
                    StartActivityForResult(
                        Intent.CreateChooser(imageIntent, "Select photo"), 0);
                };
                make_photoLL.Click += (s, e) =>
                {
                    if (pictureMethods.IsThereAnAppToTakePictures(this))
                    {
                        tintLL.Visibility = ViewStates.Gone;
                        PictureMethods.cameraOrGalleryIndicator = "camera";
                        pictureMethods.CreateDirectoryForPictures();
                        pictureMethods.TakeAPicture(this);
                    }
                };
                file_attachedLL.Click += (s, e) =>
                {
                    Android.Support.V7.Widget.PopupMenu detach_file_popup = new Android.Support.V7.Widget.PopupMenu(this, FindViewById<ImageView>(Resource.Id.file_attachedIV));

                    detach_file_popup.Inflate(Resource.Layout.detach_file_popup);

                    detach_file_popup.MenuItemClick += (s1, arg1) =>
                    {
                        if (arg1.Item.TitleFormatted.ToString() == "Открепить файл")
                        {
                            file_attachedLL.Visibility = ViewStates.Gone;
                            add_fileLL.Visibility = ViewStates.Visible;
                            base64_static = null;
                        }
                    };

                    try
                    {
                        detach_file_popup.Show();
                    }
                    catch
                    {

                    }
                };
                if (deserializedChat != null)
                    if (deserializedChat.messages != null)
                    {
                        timer = new System.Timers.Timer();
                        timer.Interval = 500;
                        timer.Elapsed += delegate
                        {
                            RunOnUiThread(() =>
                            {
                                dialogsListAdapter.NotifyDataSetChanged();
                                try
                                {
                                    recyclerView.SmoothScrollToPosition(DialogAdapter.messages_list_static.Capacity - 1);
                                }
                                catch { }
                                timer.Stop();
                                timer.Dispose();
                            });
                        };
                        timer.Start();
                    }

                recordLL.Click += delegate
                {
                    RecordAudio();
                };
                attach_voiceBn.Click += delegate
                {
                    tintVoiceLL.Visibility = ViewStates.Gone;
                    StopRecorder();
                    Toast.MakeText(this, GetString(Resource.String.audio_ready), ToastLength.Short).Show();
                };
                cancel_voiceBn.Click += delegate
                {
                    tintVoiceLL.Visibility = ViewStates.Gone;
                    CancelRecorder();
                };
                tintVoiceLL.Click += delegate
                {
                    tintVoiceLL.Visibility = ViewStates.Gone;
                    CancelRecorder();
                };

                MyFirebaseMessagingService myFirebaseMessagingService = new MyFirebaseMessagingService();


                refresherBottom = FindViewById<SwipyRefreshLayout>(Resource.Id.refresherBottom);
                refresherBottom.SetColorScheme(Resource.Color.lightBlueColor,
                                          Resource.Color.buttonBackgroundColor);
                refresherBottom.Direction = SwipyRefreshLayoutDirection.Bottom;

                refresherBottom.Refresh += async delegate
                {
                    var lastItem = DialogAdapter.messages_list_static[DialogAdapter.messages_list_static.Count - 1];
                    var udusud = await dialogMethods.ChatHistoryUpdate(userMethods.GetUsersAuthToken(), chatId, lastItem.msg_id);//update_meth();
                    refresherBottom.Refreshing = false;
                    try
                    {
                        deserialized_new_content = JsonConvert.DeserializeObject<ChatHistoryRootObject>(udusud);
                        reverse_update = Enumerable.Reverse(deserialized_new_content.messages); ;
                        foreach (var item in reverse_update)
                        {
                            if (item.in_out == "0")
                            {
                                try
                                {
                                    DialogAdapter.messages_list_static.Add(new ChatHistoryModel
                                    {
                                        msg_id = item.msg_id,
                                        in_out = item.in_out,
                                        message = item.message,
                                        file = item.file,
                                        file_base64 = item.file_base64,
                                        timestamp = new Timestamp { date = item.timestamp.date },
                                    });
                                    try
                                    {
                                        dialogsListAdapter.NotifyDataSetChanged();
                                    }
                                    catch { }
                                    recyclerView.SmoothScrollToPosition(DialogAdapter.messages_list_static.Capacity - 1);
                                }
                                catch
                                {

                                }
                            }
                        };
                    }
                    catch { }
                };

                refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
                refresher.SetColorScheme(Resource.Color.lightBlueColor,
                                          Resource.Color.buttonBackgroundColor);
                refresher.Refresh += async delegate
                {
                    try
                    {
                        var res_updated = await dialogMethods.ChatHistoryOffset(userMethods.GetUsersAuthToken(), chatId, offset);
                        if (!String.IsNullOrEmpty(res_updated) && res_updated.Length > 10)
                        {
                            var deserialized_updated_history = JsonConvert.DeserializeObject<ChatHistoryRootObject>(res_updated);
                            var reverse = Enumerable.Reverse(deserialized_updated_history.messages);
                            DialogAdapter.messages_list_static.InsertRange(0, reverse);
                            var full_new_list = DialogAdapter.messages_list_static;
                            recyclerView.SetRecycledViewPool(new RecyclerView.RecycledViewPool());
                            string previous_date = null, current_date = null;
                            int i = 0;
                            foreach (var item in full_new_list)
                            {
                                current_date = item.timestamp.date.Substring(0, 10);
                                if (!String.IsNullOrEmpty(previous_date))
                                {
                                    if (current_date != previous_date)
                                    {
                                        if (item.in_out != "2")
                                        {
                                            full_new_list.Insert(i, new ChatHistoryModel { in_out = "2", message = "", timestamp = new Timestamp { date = item.timestamp.date }, read = item.read });
                                            break;
                                        }
                                        //else
                                        //break;
                                    }
                                }
                                previous_date = item.timestamp.date.Substring(0, 10);
                                if (item.in_out == "0")
                                {

                                }
                                if (item.in_out == "2")
                                {

                                }
                                i++;
                            }

                            var dialogsListNewAdapter = new DialogAdapter(full_new_list, this, chatId, tf);
                            recyclerView.SetAdapter(dialogsListNewAdapter);
                            recyclerView.SmoothScrollToPosition(deserialized_updated_history.messages.Count - 1);
                            offset += 10;
                        }
                    }
                    catch { }
                    refresher.Refreshing = false;
                };
                activityIndicator.Visibility = ViewStates.Gone;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, GetString(Resource.String.impossible_perform_operation), ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
                return;
            }
        }

        private void CancelRecorder()
        {
            try
            {
                mediaRecorder.Stop();
            }
            catch { }
            static_path = null;
        }

        private void StopRecorder()
        {
            try
            {
                mediaRecorder.Stop();
            }
            catch
            {
                Toast.MakeText(this, "Ошибка при попытке остановить запись аудио", ToastLength.Short).Show();
            }
            camera_file = new Java.IO.File(static_path);
            int pos = static_path.LastIndexOf("/") + 1;
            static_filename = (static_path.Substring(pos, static_path.Length - pos));
            static_b = new byte[(int)camera_file.Length()];
            try
            {
                camera_fileInputStream = new FileInputStream(camera_file);
                camera_fileInputStream.Read(static_b);
                for (int i = 0; i < static_b.Length; i++)
                {
                    var byte_curr = ((char)static_b[i]);
                }
            }
            catch (Java.IO.FileNotFoundException e)
            {
                //("File Not Found.");
                //e.printStackTrace();
            }
            catch (Java.IO.IOException e1)
            {
                //System.out.println("Error Reading The File.");
                //e1.printStackTrace();
            }

            base64_static = Base64.EncodeToString(static_b, Base64.Default);
            if (String.IsNullOrEmpty(base64_static))
                base64_static = null;
        }

        private async void RecordAudio()
        {
            var granted = await checkRecordPermission();
            if (!granted)
                return;
            static_path = Android.OS.Environment.ExternalStorageDirectory
                    .AbsolutePath.ToString() + "/" + "iamnearby_voice" + "_audio.mp3";
            SetupMediaRecorder();
            try
            {
                mediaRecorder.Prepare();
                mediaRecorder.Start();
                tintVoiceLL.Visibility = ViewStates.Visible;
            }
            catch (Exception ex)
            {
                Log.Debug("DEBUG", ex.Message);
            }
        }

        private void SetupMediaRecorder()
        {
            mediaRecorder = new MediaRecorder();
            mediaRecorder.SetAudioSource(AudioSource.Mic);
            mediaRecorder.SetOutputFormat(OutputFormat.Mpeg4);
            mediaRecorder.SetAudioEncoder(AudioEncoder.HeAac);
            mediaRecorder.SetOutputFile(static_path);
        }

        public override void OnBackPressed()
        {
            if (tintLL.Visibility == ViewStates.Visible)
                tintLL.Visibility = ViewStates.Gone;
            else
                base.OnBackPressed();
        }
        private string GetActualPathFromFile(Android.Net.Uri uri)
        {
            bool isKitKat = Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat;

            if (isKitKat && DocumentsContract.IsDocumentUri(this, uri))
            {
                // ExternalStorageProvider
                if (isExternalStorageDocument(uri))
                {
                    string docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    string[] split = docId.Split(chars);
                    string type = split[0];

                    if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                    }
                }
                // DownloadsProvider
                else if (isDownloadsDocument(uri))
                {
                    string id = DocumentsContract.GetDocumentId(uri);

                    Android.Net.Uri contentUri = ContentUris.WithAppendedId(
                                    Android.Net.Uri.Parse("content://downloads/public_downloads"), long.Parse(id));

                    return getDataColumn(this, contentUri, null, null);
                }
                // MediaProvider
                else if (isMediaDocument(uri))
                {
                    String docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    String[] split = docId.Split(chars);

                    String type = split[0];

                    Android.Net.Uri contentUri = null;
                    if ("image".Equals(type))
                    {
                        contentUri = MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type))
                    {
                        contentUri = MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type))
                    {
                        contentUri = MediaStore.Audio.Media.ExternalContentUri;
                    }

                    String selection = "_id=?";
                    String[] selectionArgs = new String[]
                    {
                        split[1]
                    };

                    return getDataColumn(this, contentUri, selection, selectionArgs);
                }
                else
                {

                }
            }
            // MediaStore (and general)
            else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                // Return the remote address
                if (isGooglePhotosUri(uri))
                    return uri.LastPathSegment;

                return getDataColumn(this, uri, null, null);
            }
            // File
            else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return uri.Path;
            }

            return null;
        }

        public static String getDataColumn(Context context, Android.Net.Uri uri, String selection, String[] selectionArgs)
        {
            ICursor cursor = null;
            String column = "_data";
            String[] projection =
            { column };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(index);
                }
            }
            finally
            {
                if (cursor != null)
                    cursor.Close();
            }
            return null;
        }

        //Whether the Uri authority is ExternalStorageProvider.
        public static bool isExternalStorageDocument(Android.Net.Uri uri)
        {
            return "com.android.externalstorage.documents".Equals(uri.Authority);
        }

        //Whether the Uri authority is DownloadsProvider.
        public static bool isDownloadsDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.downloads.documents".Equals(uri.Authority);
        }

        //Whether the Uri authority is MediaProvider.
        public static bool isMediaDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.media.documents".Equals(uri.Authority);
        }

        //Whether the Uri authority is Google Photos.
        public static bool isGooglePhotosUri(Android.Net.Uri uri)
        {
            return "com.google.android.apps.photos.content".Equals(uri.Authority);
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (PictureMethods.cameraOrGalleryIndicator == "gallery")
            {
                if (data != null)
                {
                    if (requestCode == 0)
                    {

                        var uri = data.Data;
                        static_path = GetActualPathFromFile(uri);
                        if (!String.IsNullOrEmpty(static_path))
                        {
                            int pos = static_path.LastIndexOf("/") + 1;
                            //string filename = (path.Substring(pos, path.Length - pos));
                            static_filename = (static_path.Substring(pos, static_path.Length - pos));
                            try
                            {
                                camera_file = new Java.IO.File(static_path);


                                //var ggfgf = (int)camera_file.Length();

                                static_b = new byte[(int)camera_file.Length()];
                                try
                                {
                                    FileInputStream fileInputStream = new FileInputStream(camera_file);
                                    fileInputStream.Read(static_b);
                                    for (int i = 0; i < static_b.Length; i++)
                                    {
                                        var byte_curr = ((char)static_b[i]);
                                    }
                                }
                                catch (Java.IO.FileNotFoundException e)
                                {
                                    //("File Not Found.");
                                    //e.printStackTrace();
                                }
                                catch (Java.IO.IOException e1)
                                {
                                    //System.out.println("Error Reading The File.");
                                    //e1.printStackTrace();
                                }

                                if (static_b.Length < 5)
                                    Toast.MakeText(this, GetString(Resource.String.unable_upload), ToastLength.Short).Show();
                                if (static_b.Length > 16000000)
                                    Toast.MakeText(this, GetString(Resource.String.unable_upload_more_than_sixteen), ToastLength.Short).Show();
                                else
                                    base64_static = Base64.EncodeToString(static_b, Base64.Default);
                            }
                            catch
                            {
                                Toast.MakeText(this, GetString(Resource.String.unable_upload), ToastLength.Short).Show();
                            }
                        }
                        else
                            Toast.MakeText(this, GetString(Resource.String.unable_upload_from_this_folder), ToastLength.Long).Show();
                    }
                }
                else
                    base64_static = null;

            }
            else if (PictureMethods.cameraOrGalleryIndicator == "camera")
            {
                static_path = App._file.AbsoluteFile.AbsolutePath;
                camera_file = new Java.IO.File(static_path);
                int pos = static_path.LastIndexOf("/") + 1;
                static_filename = (static_path.Substring(pos, static_path.Length - pos));



                var ggfgf = (int)camera_file.Length();

                static_b = new byte[(int)camera_file.Length()];
                try
                {
                    camera_fileInputStream = new FileInputStream(camera_file);
                    camera_fileInputStream.Read(static_b);
                    for (int i = 0; i < static_b.Length; i++)
                    {
                        var byte_curr = ((char)static_b[i]);
                    }
                }
                catch (Java.IO.FileNotFoundException e)
                {
                    //("File Not Found.");
                    //e.printStackTrace();
                }
                catch (Java.IO.IOException e1)
                {
                    //System.out.println("Error Reading The File.");
                    //e1.printStackTrace();
                }

                base64_static = Base64.EncodeToString(static_b, Base64.Default);
                if (String.IsNullOrEmpty(base64_static))
                    base64_static = null;

                GC.Collect();
            }
        }

        bool we_here = false;
        System.Timers.Timer timer_read_msg = new System.Timers.Timer();

        void check_msg_read()
        {

            if (!timer_read_msg.Enabled)
            {
                var token = userMethods.GetUsersAuthToken();
                timer_read_msg.Interval = 5000;
                timer_read_msg.Stop();
                timer_read_msg = null;
                timer_read_msg = new System.Timers.Timer();
                timer_read_msg.Start();
                timer_read_msg.Elapsed += (s, e) =>
                {
                    timer_read_msg.Interval = 5000;
                    RunOnUiThread(async () =>
                    {
                        var res = await dialogMethods.CheckMsgRead(token, chatId);
                        if (res.ToString().Contains("true"))
                        {
                            int index = 0;
                            foreach (var item in DialogAdapter.messages_list_static)
                            {
                                item.read = "1";
                                DialogAdapter.messages_list_static[index].read = "1";
                                //dialogsListAdapter.NotifyDataSetChanged();
                                index++;
                            }
                            try
                            {

                                try
                                {
                                    recyclerView.SmoothScrollBy(0, 10);
                                    recyclerView.GetRecycledViewPool().Clear();
                                    RunOnUiThread(() => dialogsListAdapter.NotifyDataSetChanged());

                                    //dialogsListAdapter.OnBindViewHolder(DialogAdapter.holder1,0);
                                    //recyclerView.SetAdapter(dialogsListAdapter);
                                    //recyclerView.NotifyAll();
                                    //recyclerView.RefreshDrawableState();
                                    //var old_params = recyclerView.LayoutParameters;
                                    //LinearLayout.LayoutParams new_params = new LinearLayout.LayoutParams(100, 100);
                                    //recyclerView.LayoutParameters = new_params;
                                    //recyclerView.SetPadding(1000, 1000, 1000, 1000);
                                    //Thread.Sleep(500);
                                    //recyclerView.SetPadding(0, 0, 0, 0);
                                    //recyclerView.LayoutParameters = old_params;
                                }
                                catch (Exception ex)
                                {

                                }

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    });

                };

            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (!we_here)
            {
                timer_read_msg.Stop();
                check_msg_read();
            }
            we_here = true;

        }
        protected override void OnStop()
        {
            base.OnStop();
            we_here = false;
            timer_read_msg.Stop();
            //timer_read_msg = null;
            if (DialogAdapter.mediaPlayer != null)
                DialogAdapter.mediaPlayer.Stop();
            DialogAdapter.mediaPlayer = null;

        }
        protected override void OnPause()
        {
            base.OnPause();
            we_here = false;
            timer_read_msg.Stop();
            //timer_read_msg = null;
            if (DialogAdapter.mediaPlayer != null)
                DialogAdapter.mediaPlayer.Stop();
            DialogAdapter.mediaPlayer = null;
        }
        public static void ShowPopup(Context context, View view, int position, string chatId, string messageId)
        {
            Android.Support.V7.Widget.PopupMenu popup_menu = new Android.Support.V7.Widget.PopupMenu(context, view);
            var menuDelete = popup_menu.Menu;
            popup_menu.Inflate(Resource.Layout.delete_msg_popup);
            popup_menu.MenuItemClick += async (s1, arg1) =>
            {
                var gkkh = await dialogMethods.DeleteMsg(userMethods.GetUsersAuthToken(), chatId, messageId);
                DialogAdapter.messages_list_static.RemoveAt(position);
                dialogsListAdapter.NotifyDataSetChanged();
            };
            try
            {
                popup_menu.Show();
            }
            catch (Exception ex)
            {
            }
        }
        async Task<bool> checkRecordPermission()
        {
            PermissionStatus locationStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Microphone);

            if (locationStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Plugin.Permissions.Abstractions.Permission.Microphone });
                locationStatus = results[Plugin.Permissions.Abstractions.Permission.Microphone];
                request_runtime_record_permissions();
                return false;
            }

            return true;
        }

        //private const int REQUEST_PERMISSION_CODE = 1000;
        void request_runtime_record_permissions()
        {
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.M)
                if (
                          CheckSelfPermission(Manifest.Permission.ReadExternalStorage) != Android.Content.PM.Permission.Granted
                          || CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted
                          || CheckSelfPermission(Manifest.Permission.RecordAudio) != Android.Content.PM.Permission.Granted
                          )
                {
                    ActivityCompat.RequestPermissions(this, new String[]
                    {
                                Manifest.Permission.ReadExternalStorage,
                                Manifest.Permission.WriteExternalStorage,
                                Manifest.Permission.RecordAudio,
                    }, REQUEST_PERMISSION_CODE);
                }
                else
                {
                    ActivityCompat.RequestPermissions(this, new String[]
                    {
                                Manifest.Permission.ReadExternalStorage,
                                Manifest.Permission.WriteExternalStorage,
                                Manifest.Permission.RecordAudio,
                    }, REQUEST_PERMISSION_CODE);
                }
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
    }
}