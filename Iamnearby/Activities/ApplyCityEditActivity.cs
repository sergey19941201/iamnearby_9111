using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using PCL.Database;
using Iamnearby.Methods;
using System;

namespace Iamnearby.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, NoHistory = true)]
    public class ApplyCityEditActivity : Activity
    {
        ProfileAndExpertMethods profileAndExpertMethods = new ProfileAndExpertMethods();
        UserMethods userMethods = new UserMethods();
        ISharedPreferences city_coord_for_edit_prefs = Application.Context.GetSharedPreferences("city_coord_for_edit_prefs", FileCreationMode.Private);
        ProgressBar activityIndicator;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.ProgressScreen);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);

                var res = await profileAndExpertMethods.EditMyProfileInfo(
                    userMethods.GetUsersAuthToken(),
                    city_coord_for_edit_prefs.GetString("phone", String.Empty),
                    city_coord_for_edit_prefs.GetString("email", String.Empty),
                    city_coord_for_edit_prefs.GetString("surname", String.Empty),
                    city_coord_for_edit_prefs.GetString("name", String.Empty),
                    city_coord_for_edit_prefs.GetString("middlename", String.Empty),
                    city_coord_for_edit_prefs.GetString("city_id", String.Empty),
                    city_coord_for_edit_prefs.GetString("lat", String.Empty),
                    city_coord_for_edit_prefs.GetString("lng", String.Empty));
                StartActivity(typeof(UserProfileActivity));
            }catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}