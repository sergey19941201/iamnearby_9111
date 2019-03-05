using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Iamnearby.Activities;
using PCL.Models;
using Iamnearby.ViewHolders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
using Android.OS;
using Android;
using Android.Support.V4.App;

namespace Iamnearby.Adapters
{
    public class DialogAdapter : RecyclerView.Adapter
    {
        public static MediaPlayer mediaPlayer;
        Activity _context;
        int? previous_voice_position = null;
        public static List<ChatHistoryModel> messages_list_static = new List<ChatHistoryModel>();
        string previous_date, current_date, date_today, chat_id;
        Typeface tf;

        void ImageClick(int position)
        {
            DisplayDialogPhotoActivity.image_base64 = messages_list_static[position].file_base64;
            _context.StartActivity(typeof(DisplayDialogPhotoActivity));
        }

        void LongClick(int position, View view)
        {
            try
            {
                if (messages_list_static[position].in_out == "1")
                    DialogActivity.ShowPopup(_context, view, position, chat_id, messages_list_static[position].msg_id);
            }
            catch (Exception ex)
            {

            }
        }

        async void DocumentClick(int position)
        {
            if (!messages_list_static[position].file.ToLower().Contains(".mp"))
            {
                var uri = Android.Net.Uri.Parse(messages_list_static[position].file);
                var intent = new Intent(Intent.ActionView, uri);
                _context.StartActivity(intent);
            }
            else
            {
                var granted = await checkRecordPermission();
                if (!granted)
                    return;
                Byte[] bytes = Convert.FromBase64String(messages_list_static[position].file_base64);
                var absolutePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryRingtones).AbsolutePath;
                var iamn_path = absolutePath + "/Iamnearby";
                Directory.CreateDirectory(iamn_path);
                System.IO.File.WriteAllBytes(iamn_path + "/voiceMessage.mp3", bytes);

                if (position == previous_voice_position)
                    if (mediaPlayer == null)
                    {
                        mediaPlayer = new MediaPlayer();
                        StartLastRecord(iamn_path);
                    }
                    else
                        StopLastRecord();
                else
                {
                    StopLastRecord();
                    mediaPlayer = null;
                    mediaPlayer = new MediaPlayer();
                    StartLastRecord(iamn_path);
                }

                previous_voice_position = position;
            }
        }

        private void StopLastRecord()
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Stop();
                mediaPlayer = null;
                Toast.MakeText(_context, Resource.String.audio_stopped, ToastLength.Short).Show();
            }
        }

        private void StartLastRecord(string iamn_path)
        {
            try
            {
                mediaPlayer.SetDataSource(iamn_path + "/voiceMessage.mp3");
                mediaPlayer.Prepare();
                mediaPlayer.Start();
                Toast.MakeText(_context, Resource.String.audio_playing, ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Log.Debug("DEBUG", ex.Message);
            }
        }

        public DialogAdapter(List<ChatHistoryModel> messages_list, Activity context, string chat_id, Typeface tf)
        {
            messages_list_static = messages_list;
            _context = context;
            this.chat_id = chat_id;
            this.tf = tf;
            var date_today_temp = DateTime.Now;
            string day = date_today_temp.Day.ToString();
            if (day.Length < 2)
                day = day.Insert(0, "0");
            string month = date_today_temp.Month.ToString();
            if (month.Length < 2)
                month = month.Insert(0, "0");
            date_today = date_today_temp.Year + "-" + month + "-" + day;
        }
        public override int ItemCount
        {
            get { try { return messages_list_static.Count; } catch { return 0; } }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.DialogRow, parent, false);
            return new DialogViewHolder(layout, ImageClick, DocumentClick, LongClick);
        }

        public override int GetItemViewType(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var dialogViewHolder = (DialogViewHolder)holder;
            string message_time = messages_list_static[position].timestamp.date.Substring(11, 8);
            string current_message_date = messages_list_static[position].timestamp.date.Substring(0, 10);

            dialogViewHolder.dateTV.SetTypeface(tf, TypefaceStyle.Normal);
            dialogViewHolder.message_from_meTV.SetTypeface(tf, TypefaceStyle.Normal);
            dialogViewHolder.file_from_meTV.SetTypeface(tf, TypefaceStyle.Normal);
            dialogViewHolder.time_from_meTV.SetTypeface(tf, TypefaceStyle.Normal);
            dialogViewHolder.date_from_meTV.SetTypeface(tf, TypefaceStyle.Normal);
            dialogViewHolder.message_from_expertTV.SetTypeface(tf, TypefaceStyle.Normal);
            dialogViewHolder.file_from_expertTV.SetTypeface(tf, TypefaceStyle.Normal);
            dialogViewHolder.time_from_expertTV.SetTypeface(tf, TypefaceStyle.Normal);
            dialogViewHolder.date_from_expertTV.SetTypeface(tf, TypefaceStyle.Normal);


            if (messages_list_static[position].in_out == "0")
            {
                dialogViewHolder.dateTV.Visibility = ViewStates.Gone;
                dialogViewHolder.message_from_meLL.Visibility = ViewStates.Gone;
                dialogViewHolder.message_from_expertLL.Visibility = ViewStates.Visible;
                dialogViewHolder.date_from_expertTV.Text = current_message_date;
                dialogViewHolder.time_from_expertTV.Text = message_time;

                if (messages_list_static[position].message != "message_text_empty")
                    dialogViewHolder.message_from_expertTV.Text = messages_list_static[position].message;
                else
                    dialogViewHolder.message_from_expertTV.Visibility = ViewStates.Gone;
                if (!String.IsNullOrEmpty(messages_list_static[position].file))
                    if (messages_list_static[position].file.ToLower().Contains(".jpg")
                    || messages_list_static[position].file.Contains(".png")
                    || messages_list_static[position].file.Contains(".jpeg"))
                    {
                        dialogViewHolder.activityIndicator_from_expert.IndeterminateDrawable.SetColorFilter(_context.Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                        dialogViewHolder.image_from_expertRL.Visibility = ViewStates.Visible;
                        Thread backgroundThread = new Thread(new ThreadStart(() =>
                        {
                            Glide.Get(Application.Context).ClearDiskCache();
                        }));
                        backgroundThread.IsBackground = true;
                        backgroundThread.Start();
                        Glide.Get(_context).ClearMemory();
                        Glide.With(Application.Context)
                            .Load(messages_list_static[position].file_small)
                            .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
                            .SkipMemoryCache(true))
                            .Into(dialogViewHolder.imageFromExpertIV);
                    }
                    else
                    {
                        int pos = messages_list_static[position].file.LastIndexOf("/") + 1;
                        dialogViewHolder.file_from_expertTV.PaintFlags = PaintFlags.UnderlineText;
                        dialogViewHolder.doc_from_expertLL.Visibility = ViewStates.Visible;
                        dialogViewHolder.image_from_expertRL.Visibility = ViewStates.Gone;

                        if (messages_list_static[position].file.ToLower().Contains(".mp"))
                        {
                            dialogViewHolder.doc_from_expertIV.SetBackgroundResource(Resource.Drawable.voice_attach);
                            dialogViewHolder.file_from_expertTV.Text = _context.GetString(Resource.String.voice_message);
                        }
                        else
                        {
                            dialogViewHolder.doc_from_expertIV.SetBackgroundResource(Resource.Drawable.document_pick);
                            dialogViewHolder.file_from_expertTV.Text = (messages_list_static[position].file.Substring(pos, messages_list_static[position].file.Length - pos));
                        }

                        if (String.IsNullOrEmpty(messages_list_static[position].message))
                        {
                            dialogViewHolder.message_from_expertTV.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            dialogViewHolder.message_from_expertTV.Visibility = ViewStates.Visible;
                        }
                    }
                else
                {
                    dialogViewHolder.image_from_expertRL.Visibility = ViewStates.Gone;
                    dialogViewHolder.doc_from_expertLL.Visibility = ViewStates.Gone;
                }
            }
            else if (messages_list_static[position].in_out == "1")
            {
                dialogViewHolder.dateTV.Visibility = ViewStates.Gone;
                dialogViewHolder.message_from_meLL.Visibility = ViewStates.Visible;
                dialogViewHolder.message_from_expertLL.Visibility = ViewStates.Gone;
                dialogViewHolder.date_from_meTV.Text = current_message_date;
                dialogViewHolder.time_from_meTV.Text = message_time;
                if (messages_list_static[position].message != "message_text_empty")
                    dialogViewHolder.message_from_meTV.Text = messages_list_static[position].message;
                else
                    dialogViewHolder.message_from_meTV.Visibility = ViewStates.Gone;
                if (!String.IsNullOrEmpty(messages_list_static[position].file))
                    if (messages_list_static[position].file.ToLower().Contains(".jpg")
                    || messages_list_static[position].file.Contains(".png")
                    || messages_list_static[position].file.Contains(".jpeg"))
                    {
                        dialogViewHolder.activityIndicator_from_me.IndeterminateDrawable.SetColorFilter(_context.Resources.GetColor(Resource.Color.lightLightBlueColor), Android.Graphics.PorterDuff.Mode.Multiply);
                        dialogViewHolder.image_from_meRL.Visibility = ViewStates.Visible;
                        Thread backgroundThread = new Thread(new ThreadStart(() =>
                        {
                            Glide.Get(Application.Context).ClearDiskCache();
                        }));
                        backgroundThread.IsBackground = true;
                        backgroundThread.Start();
                        Glide.Get(_context).ClearMemory();
                        Glide.With(Application.Context)
                            .Load(messages_list_static[position].file_small)
                            .Apply(new Com.Bumptech.Glide.Request.RequestOptions()
                            .SkipMemoryCache(true))
                            .Into(dialogViewHolder.imageFromMeIV);
                    }
                    else
                    {
                        int pos = messages_list_static[position].file.LastIndexOf("/") + 1;

                        dialogViewHolder.file_from_meTV.PaintFlags = PaintFlags.UnderlineText;
                        dialogViewHolder.doc_from_meLL.Visibility = ViewStates.Visible;
                        dialogViewHolder.image_from_meRL.Visibility = ViewStates.Gone;
                        if (messages_list_static[position].file.ToLower().Contains(".mp"))
                        {
                            dialogViewHolder.doc_from_meIV.SetBackgroundResource(Resource.Drawable.voice_attach);
                            dialogViewHolder.file_from_meTV.Text = _context.GetString(Resource.String.voice_message);
                        }
                        else
                        {
                            dialogViewHolder.doc_from_meIV.SetBackgroundResource(Resource.Drawable.document_pick);
                            dialogViewHolder.file_from_meTV.Text = (messages_list_static[position].file.Substring(pos, messages_list_static[position].file.Length - pos));
                        }
                        if (String.IsNullOrEmpty(messages_list_static[position].message))
                        {
                            dialogViewHolder.message_from_meTV.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            dialogViewHolder.message_from_meTV.Visibility = ViewStates.Visible;
                        }
                    }
                else
                {
                    dialogViewHolder.image_from_meRL.Visibility = ViewStates.Gone;
                    dialogViewHolder.doc_from_meLL.Visibility = ViewStates.Gone;
                }
                if (messages_list_static[position].read == "1")
                    dialogViewHolder.read_markIV.Visibility = ViewStates.Visible;
                else
                    dialogViewHolder.read_markIV.Visibility = ViewStates.Gone;
            }
            else if (messages_list_static[position].in_out == "2")
            {
                string message_date_prev = null;
                try { message_date_prev = messages_list_static[position - 1].timestamp.date.Substring(11, 8); }
                catch { message_date_prev = null; }

                if (!String.IsNullOrEmpty(message_date_prev))
                {
                    if (current_message_date == message_date_prev)
                        dialogViewHolder.dateTV.Visibility = ViewStates.Gone;
                    else
                    {
                        dialogViewHolder.dateTV.Visibility = ViewStates.Visible;
                    }
                }
                if (current_message_date != date_today)
                    dialogViewHolder.dateTV.Text = current_message_date;
                else
                    dialogViewHolder.dateTV.Text = _context.GetString(Resource.String.today);
            }
            if (position == 0)
            {
                dialogViewHolder.dateTV.Visibility = ViewStates.Visible;
                if (current_message_date != date_today)
                    dialogViewHolder.dateTV.Text = current_message_date;
                else
                    dialogViewHolder.dateTV.Text = _context.GetString(Resource.String.today);
            }
            if (position == messages_list_static.Count - 1)
                dialogViewHolder.dateTV.Visibility = ViewStates.Gone;
            if (messages_list_static[position].in_out != "2")
            {
                if (position > 0)
                    dialogViewHolder.dateTV.Visibility = ViewStates.Gone;
            }
        }
        private const int REQUEST_PERMISSION_CODE = 1000;
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
                          _context.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) != Android.Content.PM.Permission.Granted
                          || _context.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted
                          || _context.CheckSelfPermission(Manifest.Permission.RecordAudio) != Android.Content.PM.Permission.Granted
                          )
                {
                    ActivityCompat.RequestPermissions(_context, new String[]
                    {
                                Manifest.Permission.ReadExternalStorage,
                                Manifest.Permission.WriteExternalStorage,
                                Manifest.Permission.RecordAudio,
                    }, REQUEST_PERMISSION_CODE);
                }
                else
                {
                    ActivityCompat.RequestPermissions(_context, new String[]
                    {
                                Manifest.Permission.ReadExternalStorage,
                                Manifest.Permission.WriteExternalStorage,
                                Manifest.Permission.RecordAudio,
                    }, REQUEST_PERMISSION_CODE);
                }
        }
    }
}