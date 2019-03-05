using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Support.V4.App;
using Android.Util;
using Firebase.Messaging;
using Iamnearby.Activities;

namespace Iamnearby
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        ISharedPreferences expert_feedback_pref = Application.Context.GetSharedPreferences("expert_feedback_pref", FileCreationMode.Private);
        ISharedPreferencesEditor edit_expert_feedback;
        DialogActivity dialogActivity = new DialogActivity();

        const string TAG = "MyFirebaseMsgService";
        public override async void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);

            string message_text = "";
            string name = "";
            string msg_id = "";
            string dialogId = "";
            string expert_id = "";
            string expert_category_id = "";
            string expert_avatar = "";
            string phone = "";
            string is_online = "";
            string companionFullName = "";
            string type = "";
            string reviewText = "";
            string reviewAnswer = "";
            //string file_base64 = "";
            //string timestamp = "";
            var keys = message.Data.GetEnumerator();
            foreach (var key in message.Data)
            {
                var k = key.Key;
                if (key.Key == "name")
                    name = key.Value;
                if (key.Key == "msg_text")
                    message_text = key.Value;
                if (key.Key == "msg_id")
                    msg_id = key.Value;
                if (key.Key == "dialogId")
                    dialogId = key.Value;
                if (key.Key == "message_from")
                    expert_id = key.Value;
                if (key.Key == "categoryId")
                    expert_category_id = key.Value;
                if (key.Key == "avatar")
                    expert_avatar = key.Value;
                if (key.Key == "phone")
                    phone = key.Value;
                if (key.Key == "is_online")
                    is_online = key.Value;
                if (key.Key == "companionFullName")
                    companionFullName = key.Value;
                if (key.Key == "type")
                    type = key.Value;
                if (key.Key == "reviewText")
                    reviewText = key.Value;
                if (key.Key == "reviewAnswer")
                    reviewAnswer = key.Value;
                //if (key.Key == "file_base64")
                //    file_base64 = key.Value;
                //if (key.Key == "timestamp")
                //    timestamp = key.Value;
                //break;
                //var v = key.Value;
            }
            bool online_bool;
            try
            {
                online_bool = Convert.ToBoolean(is_online);
            }
            catch
            {
                online_bool = false;
            }
            edit_expert_feedback = expert_feedback_pref.Edit();
            edit_expert_feedback.PutString("expert_id", expert_id);
            edit_expert_feedback.PutString("expert_name", name);
            edit_expert_feedback.PutString("expert_phone", phone);
            edit_expert_feedback.PutBoolean("expert_online", online_bool);
            edit_expert_feedback.PutString("expert_avatar", expert_avatar);

            edit_expert_feedback.Apply();
            var res = await DialogActivity.messageReceived(msg_id, dialogId, false);
            string main_body = "";
            if (type == "message")
                main_body = name + ": " + message_text;
            else if (type == "review")
                main_body = companionFullName + ": " + reviewText;
            else
                main_body = companionFullName + ": " + reviewAnswer;
            SendNotification(main_body, type, message.Data);
        }

        void SendNotification(string messageBody, string type, IDictionary<string, string> data)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            foreach (var key in data.Keys)
            {
                intent.PutExtra(key, data[key]);
            }

            var default_uri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);

            var pendingIntent = PendingIntent.GetActivity(this,
                                                         MainActivity.NOTIFICATION_ID,
                                                         intent,
                                                         PendingIntentFlags.OneShot);
            string title;
            if (type == "message")
                title = GetString(Resource.String.iamn_notif);
            else if (type == "review")
                title = GetString(Resource.String.new_review);
            else
                title = GetString(Resource.String.answer_review);
            //Check if notification channel exists and if not create one
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.O)
            {
                NotificationCompat.Builder mBuilder;
                mBuilder = new NotificationCompat.Builder(ApplicationContext);
                mBuilder.SetSmallIcon(Resource.Drawable.online_expert_marker);
                mBuilder.SetContentTitle(title)
                        .SetColor(Resource.Color.buttonBackgroundColor)
                        .SetContentText(messageBody)
                        .SetContentIntent(pendingIntent)
                        .SetAutoCancel(true)
                        .SetSound(default_uri);
                //.SetContentIntent(resultPendingIntent);

                var importance = NotificationManager.ImportanceHigh;
                NotificationChannel notificationChannel = new NotificationChannel(MainActivity.NOTIFICATION_ID.ToString(), "NOTIFICATION_CHANNEL_NAME", importance);
                notificationChannel.EnableLights(true);
                notificationChannel.LightColor = Color.Red;
                notificationChannel.EnableVibration(true);

                notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });
                mBuilder.SetChannelId(MainActivity.NOTIFICATION_ID++.ToString());

                NotificationManager _notificationManager =
                (NotificationManager)GetSystemService(NotificationService);
                _notificationManager.CreateNotificationChannel(notificationChannel);
                _notificationManager.Notify(MainActivity.NOTIFICATION_ID++, mBuilder.Build());
            }
            else
            {

                long[] v = { 200, 200 };
                var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                                          .SetSmallIcon(Resource.Drawable.online_expert_marker)
                                          .SetColor(Resource.Color.buttonBackgroundColor)
                                          .SetContentTitle(title)
                                          .SetContentText(messageBody)
                                          .SetAutoCancel(true)
                                          .SetContentIntent(pendingIntent)
                                          .SetSound(default_uri)
                                          .SetVibrate(v)
                                          .SetLights(Color.Blue, 1, 1);


                var notificationManager = NotificationManagerCompat.From(this);
                notificationManager.Notify(MainActivity.NOTIFICATION_ID++, notificationBuilder.Build());
            }
        }
    }
}