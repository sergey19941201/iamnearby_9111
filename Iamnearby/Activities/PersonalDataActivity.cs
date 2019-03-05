using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views.InputMethods;
using Android.Widget;
using PCL.Database;
using Iamnearby.Methods;
using PCL.Models;
using Newtonsoft.Json;
using System;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait/*, WindowSoftInputMode = SoftInput.AdjustNothing*/)]
    public class PersonalDataActivity : Activity
    {
        EditText nameET, last_nameET, patronimicET, emailET, phoneET;
        TextView cityTV, coordinatesTV;
        Button saveBn;
        ImageButton back_button;
        RelativeLayout backRelativeLayout;
        InputMethodManager imm;
        RelativeLayout mainRL;
        ProgressBar activityIndicator;
        ScrollView singleDataScrollView;
        PCL.HttpMethods.ProfileAndExpertMethods profileAndExpertMethodsPCL = new PCL.HttpMethods.ProfileAndExpertMethods();
        ProfileAndExpertMethods profileAndExpertMethods = new ProfileAndExpertMethods();
        UserMethods userMethods = new UserMethods();
        UserProfileServiceCategoriesEmpty deserialized_user_data_categs_empty;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);
        ISharedPreferences city_coord_for_edit_prefs = Application.Context.GetSharedPreferences("city_coord_for_edit_prefs", FileCreationMode.Private);
        ISharedPreferencesEditor edit_city_coord_for_edit;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.PersonalData);
                edit_city_coord_for_edit = city_coord_for_edit_prefs.Edit();
                nameET = FindViewById<EditText>(Resource.Id.nameET);
                last_nameET = FindViewById<EditText>(Resource.Id.last_nameET);
                patronimicET = FindViewById<EditText>(Resource.Id.patronimicET);
                emailET = FindViewById<EditText>(Resource.Id.emailET);
                phoneET = FindViewById<EditText>(Resource.Id.phoneET);
                cityTV = FindViewById<TextView>(Resource.Id.cityTV);
                saveBn = FindViewById<Button>(Resource.Id.saveBn);
                mainRL = FindViewById<RelativeLayout>(Resource.Id.mainRL);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                coordinatesTV = FindViewById<TextView>(Resource.Id.coordinatesTV);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                singleDataScrollView = FindViewById<ScrollView>(Resource.Id.singleDataScrollView);
                back_button.Click += (s, e) => { OnBackPressed(); };
                backRelativeLayout.Click += (s, e) => { OnBackPressed(); };
                activityIndicator.Visibility = Android.Views.ViewStates.Visible;
                singleDataScrollView.Visibility = Android.Views.ViewStates.Gone;
                saveBn.Visibility = Android.Views.ViewStates.Gone;
                var user_data = await profileAndExpertMethodsPCL.UserProfileData(userMethods.GetUsersAuthToken());
                if (user_data == "401")
                {
                    Toast.MakeText(this, Resource.String.you_not_logined, ToastLength.Long).Show();
                    userMethods.ClearTable();
                    userMethods.ClearUsersDataTable();
                    userMethods.ClearTableNotif();
                    StartActivity(typeof(MainActivity));
                    return;
                }
                activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                singleDataScrollView.Visibility = Android.Views.ViewStates.Visible;
                saveBn.Visibility = Android.Views.ViewStates.Visible;

                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                FindViewById<TextView>(Resource.Id.removeBn).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textViesw2).SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textVssiew2).SetTypeface(tf, TypefaceStyle.Normal);
                nameET.SetTypeface(tf, TypefaceStyle.Normal);
                last_nameET.SetTypeface(tf, TypefaceStyle.Normal);
                patronimicET.SetTypeface(tf, TypefaceStyle.Normal);
                cityTV.SetTypeface(tf, TypefaceStyle.Normal);
                coordinatesTV.SetTypeface(tf, TypefaceStyle.Normal);
                saveBn.SetTypeface(tf, TypefaceStyle.Normal);
                emailET.SetTypeface(tf, TypefaceStyle.Normal);
                phoneET.SetTypeface(tf, TypefaceStyle.Normal);

                try
                {
                    var deserialized_user_data = JsonConvert.DeserializeObject<UserProfile>(user_data);
                    try
                    {
                        string[] words = deserialized_user_data.fullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        nameET.Text = words[0];
                        patronimicET.Text = words[1];
                        last_nameET.Text = words[2];
                        edit_city_coord_for_edit.PutString("surname", words[2]);
                        edit_city_coord_for_edit.PutString("name", words[0]);
                        edit_city_coord_for_edit.PutString("middlename", words[1]);
                    }
                    catch { }
                    emailET.Text = userMethods.GetUsersEmail();
                    phoneET.Text = deserialized_user_data.phone;
                    cityTV.Text = deserialized_user_data.city.name;
                    edit_city_coord_for_edit.PutString("city_id", deserialized_user_data.city.id);
                    edit_city_coord_for_edit.PutString("city_name", deserialized_user_data.city.name);
                    edit_city_coord_for_edit.PutString("lat", deserialized_user_data.coordinates.latitude);
                    edit_city_coord_for_edit.PutString("lng", deserialized_user_data.coordinates.longitude);
                    try
                    {
                        if (Convert.ToInt32(deserialized_user_data.coordinates.latitude) == 0)
                            edit_city_coord_for_edit.PutString("lat", pref.GetString("latitude", String.Empty));
                    }
                    catch { }
                    try
                    {
                        if (Convert.ToInt32(deserialized_user_data.coordinates.longitude) == 0)
                            edit_city_coord_for_edit.PutString("lng", pref.GetString("longitude", String.Empty));
                    }
                    catch { }
                    edit_city_coord_for_edit.PutString("phone", deserialized_user_data.phone);
                    edit_city_coord_for_edit.PutString("email", userMethods.GetUsersEmail());
                    edit_city_coord_for_edit.Apply();
                }
                catch
                {
                    deserialized_user_data_categs_empty = JsonConvert.DeserializeObject<UserProfileServiceCategoriesEmpty>(user_data);
                    try
                    {
                        string[] words = deserialized_user_data_categs_empty.fullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        nameET.Text = words[0];
                        patronimicET.Text = words[1];
                        last_nameET.Text = words[2];
                        edit_city_coord_for_edit.PutString("surname", words[2]);
                        edit_city_coord_for_edit.PutString("name", words[0]);
                        edit_city_coord_for_edit.PutString("middlename", words[1]);
                    }
                    catch { }
                    emailET.Text = userMethods.GetUsersEmail();
                    phoneET.Text = deserialized_user_data_categs_empty.phone;
                    cityTV.Text = deserialized_user_data_categs_empty.city.name;
                    edit_city_coord_for_edit.PutString("city_id", deserialized_user_data_categs_empty.city.id);
                    edit_city_coord_for_edit.PutString("city_name", deserialized_user_data_categs_empty.city.name);
                    edit_city_coord_for_edit.PutString("lat", deserialized_user_data_categs_empty.coordinates.latitude);
                    edit_city_coord_for_edit.PutString("lng", deserialized_user_data_categs_empty.coordinates.longitude);
                    try
                    {
                        if (Convert.ToInt32(deserialized_user_data_categs_empty.coordinates.latitude) == 0)
                            edit_city_coord_for_edit.PutString("lat", pref.GetString("latitude", String.Empty));
                    }
                    catch { }
                    try
                    {
                        if (Convert.ToInt32(deserialized_user_data_categs_empty.coordinates.longitude) == 0)
                            edit_city_coord_for_edit.PutString("lng", pref.GetString("longitude", String.Empty));
                    }
                    catch { }
                    edit_city_coord_for_edit.PutString("phone", deserialized_user_data_categs_empty.phone);
                    edit_city_coord_for_edit.PutString("email", userMethods.GetUsersEmail());
                    edit_city_coord_for_edit.Apply();
                }
                FindViewById<RelativeLayout>(Resource.Id.cityRL).Click += (s, e) =>
                  {
                      StartActivity(typeof(CountryForProfileCityActivity));
                  };
                FindViewById<RelativeLayout>(Resource.Id.locationRL).Click += (s, e) =>
                  {
                      StartActivity(typeof(NewProfileCoordsMapActivity));
                  };
                saveBn.Click += async (s, e) =>
                  {
                      if (
                      !String.IsNullOrEmpty(last_nameET.Text) &&
                      !String.IsNullOrEmpty(nameET.Text) &&
                      !String.IsNullOrEmpty(phoneET.Text) &&
                      !String.IsNullOrEmpty(patronimicET.Text) &&
                      !String.IsNullOrEmpty(emailET.Text))
                      {
                          activityIndicator.Visibility = Android.Views.ViewStates.Visible;
                          singleDataScrollView.Visibility = Android.Views.ViewStates.Gone;
                          saveBn.Visibility = Android.Views.ViewStates.Gone;
                          var res = await profileAndExpertMethods.EditMyProfileInfo(
                              userMethods.GetUsersAuthToken(),
                              phoneET.Text,
                              emailET.Text,
                              last_nameET.Text,
                              nameET.Text,
                              patronimicET.Text,
                              city_coord_for_edit_prefs.GetString("city_id", String.Empty),
                              city_coord_for_edit_prefs.GetString("lat", String.Empty),
                              city_coord_for_edit_prefs.GetString("lng", String.Empty));
                          activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                          singleDataScrollView.Visibility = Android.Views.ViewStates.Visible;
                          saveBn.Visibility = Android.Views.ViewStates.Visible;
                          if (res.Contains("с таким email уже"))
                              Toast.MakeText(this, GetString(Resource.String.email_already_exists), ToastLength.Short).Show();
                          else if (res.Contains("или неверно заполнен те"))
                              Toast.MakeText(this, GetString(Resource.String.wrong_phone), ToastLength.Short).Show();
                          else
                          {
                              ISharedPreferences pref = Application.Context.GetSharedPreferences("reg_data", FileCreationMode.Private);
                              ISharedPreferencesEditor edit = pref.Edit();
                              edit.PutString("surname", "");
                              edit.PutString("name", "");
                              edit.PutString("patronymic", "");
                              edit.PutString("phone", "");
                              edit.PutString("email", "");
                              edit.Apply();
                              var token = JsonConvert.DeserializeObject<SingleToken>(res).authToken;
                              userMethods.InsertUser(token, emailET.Text);

                              StartActivity(typeof(UserProfileActivity));
                          }
                      }
                      else
                          Toast.MakeText(this, GetString(Resource.String.fill_all_entries), ToastLength.Short).Show();
                  };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}