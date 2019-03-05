using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views.InputMethods;
using Android.Widget;
using System;

namespace Iamnearby.Activities
{
    [Activity(Label = "RegistrationActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class RegistrationActivity : Activity
    {
        RelativeLayout backRelativeLayout, countryRL, locationRL;
        ImageButton back_button;
        CheckBox agree_licenseCB;
        Button continueBn;
        EditText surnameET, nameET, patronymicET, phoneET;
        TextView city_valueTV, country_valueTV;
        ISharedPreferences loc_pref;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.Registration);

                loc_pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);

                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                agree_licenseCB = FindViewById<CheckBox>(Resource.Id.agree_licenseCB);
                continueBn = FindViewById<Button>(Resource.Id.continueBn);
                surnameET = FindViewById<EditText>(Resource.Id.surnameET);
                nameET = FindViewById<EditText>(Resource.Id.nameET);
                patronymicET = FindViewById<EditText>(Resource.Id.patronymicET);
                phoneET = FindViewById<EditText>(Resource.Id.phoneET);
                countryRL = FindViewById<RelativeLayout>(Resource.Id.countryRL);
                locationRL = FindViewById<RelativeLayout>(Resource.Id.locationRL);
                city_valueTV = FindViewById<TextView>(Resource.Id.city_valueTV);
                country_valueTV = FindViewById<TextView>(Resource.Id.country_valueTV);
                //we use shared preferences to pass data between activities
                loc_pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
                ISharedPreferences pref = Application.Context.GetSharedPreferences("reg_data", FileCreationMode.Private);
                ISharedPreferencesEditor edit = pref.Edit();

                city_valueTV.Text = loc_pref.GetString("auto_city_name", String.Empty);
                country_valueTV.Text = loc_pref.GetString("auto_country_name", String.Empty);
                surnameET.Text = pref.GetString("surname", String.Empty);
                nameET.Text = pref.GetString("name", String.Empty);
                patronymicET.Text = pref.GetString("patronymic", String.Empty);
                phoneET.Text = pref.GetString("phone", String.Empty);
                edit.PutString("surname", "");
                edit.PutString("name", "");
                edit.PutString("patronymic", "");
                edit.PutString("phone", "");
                edit.Apply();

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                surnameET.SetTypeface(tf, TypefaceStyle.Normal);
                nameET.SetTypeface(tf, TypefaceStyle.Normal);
                patronymicET.SetTypeface(tf, TypefaceStyle.Normal);
                phoneET.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textdsdsView1).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textVssdiew2).SetTypeface(tf, TypefaceStyle.Normal);
                country_valueTV.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textVisdsdew2).SetTypeface(tf, TypefaceStyle.Normal);
                city_valueTV.SetTypeface(tf, TypefaceStyle.Normal);
                continueBn.SetTypeface(tf, TypefaceStyle.Normal);

                continueBn.SetBackgroundResource(Resource.Drawable.button_corners_disabled);
                agree_licenseCB.Click += (s, e) =>
                  {
                      if (agree_licenseCB.Checked)
                      {
                          continueBn.Enabled = true;
                          continueBn.SetBackgroundResource(Resource.Drawable.button_corners);
                      }
                      else
                      {
                          continueBn.Enabled = false;
                          continueBn.SetBackgroundResource(Resource.Drawable.button_corners_disabled);
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
                continueBn.Click += (s, e) =>
                  {
                      if (/*!String.IsNullOrEmpty(surnameET.Text) && */!String.IsNullOrEmpty(nameET.Text) && !String.IsNullOrEmpty(phoneET.Text)/* && !String.IsNullOrEmpty(patronymicET.Text)*/)
                      {
                          edit.PutString("surname", surnameET.Text);
                          edit.PutString("name", nameET.Text);
                          edit.PutString("patronymic", patronymicET.Text);
                          edit.PutString("phone", phoneET.Text);
                          edit.Apply();
                          StartActivity(typeof(YourSpecializationActivity));
                      }
                      else
                          Toast.MakeText(this, GetString(Resource.String.fill_all_entries), ToastLength.Short).Show();
                  };
                phoneET.EditorAction += (object sender, EditText.EditorActionEventArgs e) =>
                 {
                     imm.HideSoftInputFromWindow(phoneET.WindowToken, 0);
                 };
                locationRL.Click += (s, e) =>
                  {
                      StartActivity(typeof(RegionForSearchActivity));
                  };
                countryRL.Click += (s, e) =>
                  {
                      StartActivity(typeof(CountryActivity));
                  };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}