using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Iamnearby.Methods;
using PCL.Database;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class EditServiceActivity : Activity
    {
        public static int service_id;
        public static string service_name, service_price;
        ISharedPreferences specialization_for_edit_pref = Application.Context.GetSharedPreferences("specialization_for_edit_pref", FileCreationMode.Private);
        ImageButton back_button;
        RelativeLayout backRelativeLayout;
        EditText service_nameET, service_priceET;
        RelativeLayout tintDelServiceLL;
        ProgressBar activityIndicator, activityIndicatorServiceDel;
        Button saveBn;
        TextView headerTV;
        ProfileAndExpertMethods profileAndExpertMethods = new ProfileAndExpertMethods();
        AddServiceActivity addServiceActivity = new AddServiceActivity();
        UserMethods userMethods = new UserMethods();
        Button removeBn;
        TextView yesServiceTV, noServiceTV;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.Service);

                tintDelServiceLL = FindViewById<RelativeLayout>(Resource.Id.tintDelServiceLL);
                service_nameET = FindViewById<EditText>(Resource.Id.service_nameET);
                service_priceET = FindViewById<EditText>(Resource.Id.service_priceET);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                back_button.Click += (s, e) => { OnBackPressed(); };
                backRelativeLayout.Click += (s, e) => { OnBackPressed(); };
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicatorServiceDel = FindViewById<ProgressBar>(Resource.Id.activityIndicatorServiceDel);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                activityIndicatorServiceDel.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                yesServiceTV = FindViewById<TextView>(Resource.Id.yesServiceTV);
                noServiceTV = FindViewById<TextView>(Resource.Id.noServiceTV);
                saveBn = FindViewById<Button>(Resource.Id.saveBn);
                removeBn = FindViewById<Button>(Resource.Id.removeBn);
                headerTV = FindViewById<TextView>(Resource.Id.headerTV);
                service_nameET.Text = service_name;
                service_priceET.Text = service_price;

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                headerTV.SetTypeface(tf, TypefaceStyle.Bold);
                headerTV.Text = GetString(Resource.String.service_editing);
                FindViewById<TextView>(Resource.Id.textViewService5).SetTypeface(tf, TypefaceStyle.Bold);
                saveBn.SetTypeface(tf, TypefaceStyle.Normal);
                service_nameET.SetTypeface(tf, TypefaceStyle.Normal);
                service_priceET.SetTypeface(tf, TypefaceStyle.Normal);
                yesServiceTV.SetTypeface(tf, TypefaceStyle.Normal);
                noServiceTV.SetTypeface(tf, TypefaceStyle.Normal);
                saveBn.Click += async (s, e) =>
                {
                    if (!String.IsNullOrEmpty(service_nameET.Text))
                    {
                        activityIndicator.Visibility = ViewStates.Visible;
                        saveBn.Visibility = ViewStates.Gone;
                        var res1 = await profileAndExpertMethods.EditService(
                            userMethods.GetUsersAuthToken(),
                            service_id.ToString(),
                            service_nameET.Text,
                            service_priceET.Text);
                        activityIndicator.Visibility = ViewStates.Gone;

                        var res_reload = await addServiceActivity.ReloadData();
                        saveBn.Visibility = ViewStates.Visible;
                        OnBackPressed();
                    }
                    else
                        Toast.MakeText(this, GetString(Resource.String.add_name), ToastLength.Short).Show();
                };
                removeBn.Click += (s, e) =>
                  {
                      tintDelServiceLL.Visibility = ViewStates.Visible;
                  };
                tintDelServiceLL.Click += (s, e) =>
                {
                    tintDelServiceLL.Visibility = ViewStates.Gone;
                };
                noServiceTV.Click += (s, e) =>
                {
                    tintDelServiceLL.Visibility = ViewStates.Gone;
                };
                yesServiceTV.Click += async (s, e) =>
                {
                    activityIndicatorServiceDel.Visibility = ViewStates.Visible;
                    var res = await profileAndExpertMethods.DeleteService(userMethods.GetUsersAuthToken(), service_id.ToString());
                    var res_reload = await addServiceActivity.ReloadData();
                    System.Threading.Thread.Sleep(100);
                    tintDelServiceLL.Visibility = ViewStates.Gone;
                    OnBackPressed();
                };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}