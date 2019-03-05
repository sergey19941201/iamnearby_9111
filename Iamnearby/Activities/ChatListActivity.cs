using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Com.Orangegangsters.Github.Swipyrefreshlayout.Library;
using Iamnearby.Adapters;
using Iamnearby.Methods;
using Newtonsoft.Json;
using PCL.Database;
using PCL.Models;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden)]
    public class ChatListActivity : Activity
    {
        static DialogMethods dialogMethods = new DialogMethods();
        static UserMethods userMethods = new UserMethods();
        ProgressBar activityIndicator, activityIndicatorSearch;
        static ProgressBar activityIndicatorAccept;
        RecyclerView recyclerView, search_recyclerView;
        RecyclerView.LayoutManager layoutManager, search_layoutManager;
        ImageButton back_button, close_searchBn;
        ImageView nothingIV;
        Button searchBn;
        EditText searchET;
        RelativeLayout backRelativeLayout;
        TextView headerTV, dialogsTV, nothingTV;
        static TextView title_acceptTV, yes_acceptTV, no_acceptTV;
        RelativeLayout emptyRL, bottomLayout;
        static RelativeLayout tint_acceptLL;
        static string popup_menu_value;
        LinearLayout profileLL, dialogsLL, specialistsLL, searchLL;
        InputMethodManager inputMethodManager;
        ISharedPreferences dialog_data = Application.Context.GetSharedPreferences("dialogs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_dialog;
        SwipyRefreshLayout refresher;
        List<ChatListModel> deserialized_obj, deserialized_obj_updated, deserialized_obj_search;
        ImageView message_indicatorIV;
        DialogsListAdapter dialogsSearchListAdapter;
        static Activity activity;
        string date_now;
        public static void ShowPopup(Context context, string indicator, string blacklist, View view, string chatId)
        {
            Android.Support.V7.Widget.PopupMenu popup_menu = new Android.Support.V7.Widget.PopupMenu(context, view);
            var menuOpts = popup_menu.Menu;
            if (indicator == "dialog_popup_menu")
            {
                popup_menu.Inflate(Resource.Layout.dialog_popup_menu);
                if (blacklist == "to_blacklist")
                {
                    menuOpts.GetItem(2).SetTitle(Resource.String.restore_conversation);
                }
            }
            else if (indicator == "dialog_with_user_popup_menu")
            {
                popup_menu.Inflate(Resource.Layout.dialog_with_user_popup_menu);
                if (blacklist == "to_blacklist")
                    menuOpts.GetItem(1).SetTitle(Resource.String.restore_conversation);
            }
            else if (indicator == "dialog_popup_menu_on")
            {
                popup_menu.Inflate(Resource.Layout.dialog_popup_menu_on);
                if (blacklist == "to_blacklist")
                {
                    menuOpts.GetItem(2).SetTitle(Resource.String.restore_conversation);
                }
            }
            else if (indicator == "dialog_with_user_popup_menu_on")
            {
                popup_menu.Inflate(Resource.Layout.dialog_with_user_popup_menu_on);
                if (blacklist == "to_blacklist")
                    menuOpts.GetItem(1).SetTitle(Resource.String.restore_conversation);
            }
            popup_menu.MenuItemClick += (s1, arg1) =>
            {
                if (arg1.Item.TitleFormatted.ToString() == context.GetString(Resource.String.feedback_text))
                {
                    context.StartActivity(typeof(FeedbackChooseCategoryActivity));
                }
                if (arg1.Item.TitleFormatted.ToString() == context.GetString(Resource.String.clear_chat_history))
                {
                    title_acceptTV.Text = context.GetString(Resource.String.clear_chat_history) + "?";
                    popup_menu_value = context.GetString(Resource.String.clear_chat_history);
                    tint_acceptLL.Visibility = ViewStates.Visible;
                }
                if (arg1.Item.TitleFormatted.ToString() == context.GetString(Resource.String.blacklist))
                {
                    if (blacklist == "to_blacklist")
                        title_acceptTV.Text = context.GetString(Resource.String.remove_from_blacklist) + "?";
                    else
                        title_acceptTV.Text = context.GetString(Resource.String.move_to_blacklist) + "?";
                    popup_menu_value = context.GetString(Resource.String.blacklist);
                    tint_acceptLL.Visibility = ViewStates.Visible;
                }
                if (arg1.Item.TitleFormatted.ToString() == context.GetString(Resource.String.restore_conversation))
                {
                    title_acceptTV.Text = context.GetString(Resource.String.remove_from_blacklist) + "?";
                    popup_menu_value = context.GetString(Resource.String.blacklist);
                    tint_acceptLL.Visibility = ViewStates.Visible;
                }
                if (arg1.Item.TitleFormatted.ToString() == context.GetString(Resource.String.turn_off_notifications))
                {
                    title_acceptTV.Text = context.GetString(Resource.String.turn_off_notifications) + "?";
                    popup_menu_value = context.GetString(Resource.String.turn_off_notifications);
                    tint_acceptLL.Visibility = ViewStates.Visible;
                }
                if (arg1.Item.TitleFormatted.ToString() == context.GetString(Resource.String.turn_on_notifications))
                {
                    title_acceptTV.Text = context.GetString(Resource.String.turn_on_notifications) + "?";
                    popup_menu_value = context.GetString(Resource.String.turn_on_notifications);
                    tint_acceptLL.Visibility = ViewStates.Visible;
                }
                yes_acceptTV.Click += async (s, e) =>
                {
                    if (popup_menu_value == context.GetString(Resource.String.clear_chat_history))
                    {
                        activityIndicatorAccept.Visibility = ViewStates.Visible;
                        var clear_his = await dialogMethods.ClearHistory(userMethods.GetUsersAuthToken(), chatId);
                        activityIndicatorAccept.Visibility = ViewStates.Gone;
                        tint_acceptLL.Visibility = ViewStates.Gone;
                        var intent = new Intent(context, typeof(ChatListActivity));
                        intent.SetFlags(ActivityFlags.NewTask);
                        context.StartActivity(intent);
                        activity.Finish();
                    }
                    if (popup_menu_value == context.GetString(Resource.String.blacklist))
                    {
                        if (blacklist == "to_blacklist")
                        {
                            var end_conv = await dialogMethods.CloseChat(userMethods.GetUsersAuthToken(), chatId, false);
                            blacklist = "";
                        }
                        else
                        {
                            var end_conv = await dialogMethods.CloseChat(userMethods.GetUsersAuthToken(), chatId, true);
                            blacklist = "to_blacklist";
                        }

                        activityIndicatorAccept.Visibility = ViewStates.Visible;
                        activityIndicatorAccept.Visibility = ViewStates.Gone;
                        tint_acceptLL.Visibility = ViewStates.Gone;
                        var intent = new Intent(context, typeof(ChatListActivity));
                        intent.SetFlags(ActivityFlags.NewTask);
                        context.StartActivity(intent);
                        activity.Finish();
                    }
                    if (popup_menu_value == context.GetString(Resource.String.turn_off_notifications))
                    {
                        activityIndicatorAccept.Visibility = ViewStates.Visible;
                        var turn_off_notifs = await dialogMethods.ToggleNotifications(userMethods.GetUsersAuthToken(), chatId, 0);
                        activityIndicatorAccept.Visibility = ViewStates.Gone;
                        tint_acceptLL.Visibility = ViewStates.Gone;
                        var intent = new Intent(context, typeof(ChatListActivity));
                        intent.SetFlags(ActivityFlags.NewTask);
                        context.StartActivity(intent);
                        activity.Finish();
                    }
                    if (popup_menu_value == context.GetString(Resource.String.turn_on_notifications))
                    {
                        activityIndicatorAccept.Visibility = ViewStates.Visible;
                        var turn_on_notifs = await dialogMethods.ToggleNotifications(userMethods.GetUsersAuthToken(), chatId, 1);
                        activityIndicatorAccept.Visibility = ViewStates.Gone;
                        tint_acceptLL.Visibility = ViewStates.Gone;
                        var intent = new Intent(context, typeof(ChatListActivity));
                        intent.SetFlags(ActivityFlags.NewTask);
                        context.StartActivity(intent);
                        activity.Finish();
                    }
                };
            };
            try
            { popup_menu.Show(); }
            catch { }
        }
        protected override async void OnResume()
        {
            base.OnResume();

            try
            {
                SetContentView(Resource.Layout.ChatList);

                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);

                var date = DateTime.Now;
                string day = date.Day.ToString();
                if (day.Length < 2)
                    day = day.Insert(0, "0");
                string month = date.Month.ToString();
                if (month.Length < 2)
                    month = month.Insert(0, "0");
                date_now = day + "." + month + "." + date.Year;

                activity = this;
                profileLL = FindViewById<LinearLayout>(Resource.Id.profileLL);
                dialogsLL = FindViewById<LinearLayout>(Resource.Id.dialogsLL);
                specialistsLL = FindViewById<LinearLayout>(Resource.Id.specialistsLL);
                message_indicatorIV = FindViewById<ImageView>(Resource.Id.message_indicatorIV);
                inputMethodManager = Application.GetSystemService(Context.InputMethodService) as InputMethodManager;
                dialogsTV = FindViewById<TextView>(Resource.Id.dialogsTV);
                activityIndicatorSearch = FindViewById<ProgressBar>(Resource.Id.activityIndicatorSearch);
                activityIndicatorSearch.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                activityIndicatorAccept = FindViewById<ProgressBar>(Resource.Id.activityIndicatorAccept);
                activityIndicatorAccept.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                bottomLayout = FindViewById<RelativeLayout>(Resource.Id.bottomLayout);
                title_acceptTV = FindViewById<TextView>(Resource.Id.title_acceptTV);
                yes_acceptTV = FindViewById<TextView>(Resource.Id.yes_acceptTV);
                no_acceptTV = FindViewById<TextView>(Resource.Id.no_acceptTV);
                tint_acceptLL = FindViewById<RelativeLayout>(Resource.Id.tint_acceptLL);
                search_recyclerView = FindViewById<RecyclerView>(Resource.Id.search_recyclerView);
                search_layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                searchET = FindViewById<EditText>(Resource.Id.searchET);
                searchBn = FindViewById<Button>(Resource.Id.searchBn);
                close_searchBn = FindViewById<ImageButton>(Resource.Id.close_searchBn);
                searchLL = FindViewById<LinearLayout>(Resource.Id.searchLL);
                nothingIV = FindViewById<ImageView>(Resource.Id.nothingIV);
                nothingTV = FindViewById<TextView>(Resource.Id.nothingTV);
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");

                no_acceptTV.Click += (s, e) =>
                {
                    tint_acceptLL.Visibility = ViewStates.Gone;
                };
                tint_acceptLL.Click += (s, e) =>
                {
                    tint_acceptLL.Visibility = ViewStates.Gone;
                };
                searchBn.Click += (s, e) =>
                {
                    close_searchBn.Visibility = ViewStates.Visible;
                    searchET.Visibility = ViewStates.Visible;
                    searchBn.Visibility = ViewStates.Gone;
                    headerTV.Visibility = ViewStates.Gone;

                    searchET.RequestFocus();
                    showKeyboard();
                };
                close_searchBn.Click += (s, e) =>
                {
                    searchET.Text = null;
                    imm.HideSoftInputFromWindow(searchET.WindowToken, 0);
                    searchLL.Visibility = ViewStates.Gone;
                    headerTV.Visibility = ViewStates.Visible;
                    searchET.Visibility = ViewStates.Gone;
                    close_searchBn.Visibility = ViewStates.Gone;
                    searchBn.Visibility = ViewStates.Visible;
                };
                search_recyclerView.SetLayoutManager(search_layoutManager);
                searchET.TextChanged += async (s, e) =>
                {
                    if (!String.IsNullOrEmpty(searchET.Text))
                    {
                        nothingIV.Visibility = ViewStates.Gone;
                        nothingTV.Visibility = ViewStates.Gone;
                        searchLL.Visibility = ViewStates.Visible;
                        close_searchBn.Visibility = ViewStates.Visible;
                        activityIndicatorSearch.Visibility = ViewStates.Visible;
                        search_recyclerView.Visibility = ViewStates.Gone;
                        activityIndicatorSearch.Visibility = ViewStates.Visible;
                        var search_content = await dialogMethods.SearchChat(userMethods.GetUsersAuthToken(), searchET.Text);
                        if (!search_content.ToLower().Contains("пошло не так".ToLower()) && !search_content.Contains("null"))
                        //try
                        {
                            search_recyclerView.Visibility = ViewStates.Visible;
                            var deserObj = JsonConvert.DeserializeObject<RootObjectChatList>(search_content);
                            deserialized_obj_search = deserObj.chats;
                            if (deserialized_obj_search.Count > 0)
                            {
                                dialogsSearchListAdapter = new DialogsListAdapter(deserialized_obj_search, this, tf, date_now);
                                search_recyclerView.SetAdapter(dialogsSearchListAdapter);
                                dialogsSearchListAdapter.NotifyDataSetChanged();
                                nothingIV.Visibility = ViewStates.Gone;
                                nothingTV.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                search_recyclerView.Visibility = ViewStates.Gone;
                                nothingIV.Visibility = ViewStates.Visible;
                                nothingTV.Visibility = ViewStates.Visible;
                            }
                        }
                        //catch
                        else
                        {
                            search_recyclerView.Visibility = ViewStates.Gone;
                            nothingIV.Visibility = ViewStates.Visible;
                            nothingTV.Visibility = ViewStates.Visible;
                        }

                        activityIndicatorSearch.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        close_searchBn.Visibility = ViewStates.Gone;
                        searchLL.Visibility = ViewStates.Visible;
                        close_searchBn.Visibility = ViewStates.Gone;
                        searchLL.Visibility = ViewStates.Visible;
                        searchLL.Visibility = ViewStates.Gone;
                    }
                };
                specialistsLL.Click += (s, e) =>
                {
                    StartActivity(typeof(SpecialistsCategoryActivity));
                };
                profileLL.Click += (s, e) =>
                {
                    if (userMethods.UserExists())
                        StartActivity(typeof(UserProfileActivity));
                };
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                activityIndicator.Visibility = ViewStates.Visible;
                recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                headerTV = FindViewById<TextView>(Resource.Id.headerTV);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                headerTV.Text = GetString(Resource.String.dialogs);
                backRelativeLayout.Click += (s, e) => { OnBackPressed(); };
                back_button.Click += (s, e) => { OnBackPressed(); };
                activityIndicator.Visibility = ViewStates.Visible;
                emptyRL = FindViewById<RelativeLayout>(Resource.Id.emptyRL);
                refresher = FindViewById<SwipyRefreshLayout>(Resource.Id.refresher);

                headerTV.SetTypeface(tf, TypefaceStyle.Bold);
                FindViewById<EditText>(Resource.Id.searchET).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<Button>(Resource.Id.searchBn).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textView1).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textView122).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.specialistsTV).SetTypeface(tf, TypefaceStyle.Normal);
                dialogsTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.profileTV).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textView1sdds).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.by_distanceTV).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.by_ratingTV).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.nothingTV).SetTypeface(tf, TypefaceStyle.Normal);

                var res = await dialogMethods.ChatList(userMethods.GetUsersAuthToken());
                activityIndicator.Visibility = ViewStates.Gone;
                try
                {
                    var deserObj = JsonConvert.DeserializeObject<RootObjectChatList>(res);
                    if (!String.IsNullOrEmpty(deserObj.notify_alerts.msg_cnt_new.ToString()) && deserObj.notify_alerts.msg_cnt_new.ToString() != "0")
                    {
                        message_indicatorIV.Visibility = ViewStates.Visible;
                        dialogsTV.Text = GetString(Resource.String.dialogs) + " (" + deserObj.notify_alerts.msg_cnt_new + ")";
                    }
                    else
                    {
                        message_indicatorIV.Visibility = ViewStates.Gone;
                        dialogsTV.Text = GetString(Resource.String.dialogs);
                    }
                    deserialized_obj = deserObj.chats;
                }
                catch
                {
                    emptyRL.Visibility = ViewStates.Visible;
                    refresher.Visibility = ViewStates.Gone;
                }
                if (deserialized_obj.Count == 0)
                {
                    emptyRL.Visibility = ViewStates.Visible;
                    refresher.Visibility = ViewStates.Gone;
                }
                else
                {
                    emptyRL.Visibility = ViewStates.Gone;
                    refresher.Visibility = ViewStates.Visible;
                    recyclerView.Visibility = ViewStates.Visible;
                }
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerView.SetLayoutManager(layoutManager);
                if (deserialized_obj.Count > 0)
                {
                    var dialogsListAdapter = new DialogsListAdapter(deserialized_obj, this, tf, date_now);
                    recyclerView.SetAdapter(dialogsListAdapter);
                }

                refresher.SetColorScheme(Resource.Color.lightBlueColor,
                                              Resource.Color.buttonBackgroundColor);
                refresher.Direction = SwipyRefreshLayoutDirection.Both;

                refresher.Refresh += async delegate
                {
                    var res_updated = await dialogMethods.ChatList(userMethods.GetUsersAuthToken());
                    refresher.Refreshing = false;
                    try
                    {
                        var deserObj = JsonConvert.DeserializeObject<RootObjectChatList>(res_updated);
                        if (!String.IsNullOrEmpty(deserObj.notify_alerts.msg_cnt_new.ToString()) && deserObj.notify_alerts.msg_cnt_new.ToString() != "0")
                        {
                            message_indicatorIV.Visibility = ViewStates.Visible;
                            dialogsTV.Text = GetString(Resource.String.dialogs) + " (" + deserObj.notify_alerts.msg_cnt_new + ")";
                        }
                        else
                        {
                            message_indicatorIV.Visibility = ViewStates.Gone;
                            dialogsTV.Text = GetString(Resource.String.dialogs);
                        }
                        deserialized_obj_updated = deserObj.chats;
                    }
                    catch
                    {
                        emptyRL.Visibility = ViewStates.Visible;
                        refresher.Visibility = ViewStates.Gone;
                    }
                    if (deserialized_obj_updated != null)
                    {
                        if (deserialized_obj_updated.Count == 0)
                        {
                            emptyRL.Visibility = ViewStates.Visible;
                            refresher.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            emptyRL.Visibility = ViewStates.Gone;
                            refresher.Visibility = ViewStates.Visible;
                            recyclerView.Visibility = ViewStates.Visible;
                        }
                        if (deserialized_obj_updated.Count > 0)
                        {
                            var dialogsUpdatedAdapter = new DialogsListAdapter(deserialized_obj_updated, this, tf, date_now);
                            recyclerView.SetAdapter(dialogsUpdatedAdapter);
                        }
                    }
                };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
        
        void showKeyboard()
        {
            inputMethodManager.ShowSoftInput(searchET, ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }
    }
}