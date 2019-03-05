using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Iamnearby.Methods;
using System;

namespace Iamnearby.Activities
{
    [Activity(Label = "NewCategoryAddActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class NewCategoryAddActivity : Activity
    {
        RelativeLayout backRelativeLayout;
        ImageButton back_button;
        Button add_categoryBn;
        EditText categET;
        ProgressBar activityIndicator;
        SpecializationMethods specializationMethods = new SpecializationMethods();
        ISharedPreferences pref = Application.Context.GetSharedPreferences("categories_data", FileCreationMode.Private);
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.NewCategoryAdd);

                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                add_categoryBn = FindViewById<Button>(Resource.Id.add_categoryBn);
                categET = FindViewById<EditText>(Resource.Id.categET);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                categET.SetTypeface(tf, TypefaceStyle.Normal);
                add_categoryBn.SetTypeface(tf, TypefaceStyle.Normal);
                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                add_categoryBn.Click += async (s, e) =>
                  {
                      if (!String.IsNullOrEmpty(categET.Text))
                      {
                          add_categoryBn.Visibility = ViewStates.Gone;
                          activityIndicator.Visibility = ViewStates.Visible;
                          var categs = await specializationMethods.AddNewSpecialization(pref.GetString("spec_id", String.Empty), categET.Text);
                          add_categoryBn.Visibility = ViewStates.Visible;
                          activityIndicator.Visibility = ViewStates.Gone;
                          StartActivity(typeof(SubCategoryActivity));
                      }
                      else
                          Toast.MakeText(this, GetString(Resource.String.enter_category_name), ToastLength.Short).Show();
                  };
                categET.EditorAction += (object sender, EditText.EditorActionEventArgs e) =>
                {
                    imm.HideSoftInputFromWindow(categET.WindowToken, 0);
                };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}