using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Iamnearby.Adapters;
using Newtonsoft.Json;
using PCL.Models;

namespace Iamnearby.Activities
{
    [Activity(Label = "RegEmailActivity", ScreenOrientation = ScreenOrientation.Portrait/*, Theme = "@style/AppThemeLightNavbar"*/)]
    public class RegEmailActivity : Activity
    {
        EditText emailET;
        RelativeLayout backRelativeLayout;
        ImageButton back_button;
        Button sendBn;
        ProgressBar activityIndicator;
        ISharedPreferences loc_pref;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.RegEmail);
                loc_pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                emailET = FindViewById<EditText>(Resource.Id.emailET);
                sendBn = FindViewById<Button>(Resource.Id.sendBn);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                ISharedPreferences pref = Application.Context.GetSharedPreferences("reg_data", FileCreationMode.Private);
                ISharedPreferencesEditor edit = pref.Edit();
                emailET.Text = pref.GetString("email", String.Empty);

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                sendBn.SetTypeface(tf, TypefaceStyle.Normal);
                emailET.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.infoTV).SetTypeface(tf, TypefaceStyle.Normal);

                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                sendBn.Click += async (s, e) =>
                 {
                     List<string> ids_list = new List<string>();
                     int i = 0;
                     foreach (var item in SubCategoryAdapter.my_specializations_static)
                     {
                         ids_list.Add(SubCategoryAdapter.my_specializations_static[i].id);
                         i++;
                     }
                     edit.PutString("email", emailET.Text);
                     edit.Apply();
                     sendBn.Visibility = ViewStates.Gone;
                     activityIndicator.Visibility = ViewStates.Visible;

                     PCL.HttpMethods.RegistrationMethods reg_meths = new PCL.HttpMethods.RegistrationMethods();
                     var reg_result = await reg_meths.Register(
                         pref.GetString("phone", String.Empty),
                         pref.GetString("email", String.Empty),
                         pref.GetString("surname", String.Empty),
                         pref.GetString("name", String.Empty),
                         pref.GetString("patronymic", String.Empty),
                         loc_pref.GetString("auto_city_id", String.Empty),
                         ids_list
                         );
                     sendBn.Visibility = ViewStates.Visible;
                     activityIndicator.Visibility = ViewStates.Gone;

                     if (reg_result == "[]" || reg_result == "" || reg_result.ToLower().Contains("выслано повторное письмо"))
                     {
                         StartActivity(typeof(RegAfterActivity));
                     }
                     else
                     {
                         var deserialized_value = JsonConvert.DeserializeObject<RegisterFailedContent>(reg_result.ToString());
                         Toast.MakeText(this, deserialized_value.message, ToastLength.Short).Show();
                         StartActivity(typeof(RegistrationActivity));
                     }
                 };
                emailET.EditorAction += (object sender, EditText.EditorActionEventArgs e) =>
                {
                    imm.HideSoftInputFromWindow(emailET.WindowToken, 0);
                };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}