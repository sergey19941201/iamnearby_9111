using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Iamnearby.Adapters;
using Iamnearby.Methods;
using PCL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Iamnearby.Activities
{
    [Activity(Label = "FilterActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class FilterActivity : Activity
    {
        RelativeLayout backRelativeLayout;
        static RelativeLayout distanceRL;
        ImageButton back_button;
        Button cancellationBn;
        static Button findBn, resetBn;
        TextView distance_value;
        //static because we need to control this element from recyclerview adapter
        static TextView inform_processTV, city_value;
        SeekBar distanceSB;
        Switch onlyWithReviewsS;
        static ProgressBar activityIndicator;
        static LinearLayout city_chooseLL;
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        static ISharedPreferences expert_data;
        static ISharedPreferencesEditor edit_expert;
        static bool reset_pressed = false;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("coordinates", FileCreationMode.Private);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.Filter);

                expert_data = Application.Context.GetSharedPreferences("experts", FileCreationMode.Private);
                edit_expert = expert_data.Edit();
                recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                findBn = FindViewById<Button>(Resource.Id.findBn);
                resetBn = FindViewById<Button>(Resource.Id.resetBn);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                distanceRL = FindViewById<RelativeLayout>(Resource.Id.distanceRL);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                cancellationBn = FindViewById<Button>(Resource.Id.cancellationBn);
                city_value = FindViewById<TextView>(Resource.Id.city_value);
                distance_value = FindViewById<TextView>(Resource.Id.distance_value);
                inform_processTV = FindViewById<TextView>(Resource.Id.inform_processTV);
                distanceSB = FindViewById<SeekBar>(Resource.Id.distanceSB);
                onlyWithReviewsS = FindViewById<Switch>(Resource.Id.onlyWithReviewsS);
                activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
                city_chooseLL = FindViewById<LinearLayout>(Resource.Id.city_chooseLL);
                activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
                SpecializationMethods specializationMethods = new SpecializationMethods();
                recyclerView.SetLayoutManager(layoutManager);
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                resetBn.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textView1).SetTypeface(tf, TypefaceStyle.Normal);
                city_value.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textViesw1).SetTypeface(tf, TypefaceStyle.Normal);
                distance_value.SetTypeface(tf, TypefaceStyle.Normal);
                FindViewById<TextView>(Resource.Id.textVsiew1).SetTypeface(tf, TypefaceStyle.Normal);
                inform_processTV.SetTypeface(tf, TypefaceStyle.Normal);
                findBn.SetTypeface(tf, TypefaceStyle.Normal);
                cancellationBn.SetTypeface(tf, TypefaceStyle.Normal);

                backRelativeLayout.Click += (s, e) =>
                {
                    OnBackPressed();
                };
                back_button.Click += (s, e) =>
                {
                    OnBackPressed();
                };

                if (String.IsNullOrEmpty(expert_data.GetString("expert_city_name", String.Empty)))
                    city_value.Text = GetString(Resource.String.not_chosen);
                else
                {
                    city_value.Text = expert_data.GetString("expert_city_name", String.Empty);
                    resetBn.Enabled = true;
                    resetBn.SetTextColor(Android.Graphics.Color.White);
                }

                if (String.IsNullOrEmpty(expert_data.GetString("distance_radius", String.Empty)))
                {
                    distance_value.Text = "100 " + GetString(Resource.String.km);
                    distanceSB.Progress = 100;
                }
                else
                {
                    distance_value.Text = expert_data.GetString("distance_radius", String.Empty);
                    distanceSB.Progress = Convert.ToInt32(expert_data.GetString("distance_radius", String.Empty));
                    resetBn.Enabled = true;
                    resetBn.SetTextColor(Android.Graphics.Color.White);
                }
                if (expert_data.GetBoolean("has_reviews", false) == false)
                    onlyWithReviewsS.Checked = false;
                else
                {
                    onlyWithReviewsS.Checked = true;
                    resetBn.Enabled = true;
                    resetBn.SetTextColor(Android.Graphics.Color.White);
                }

                distanceSB.ProgressChanged += async (s, e) =>
                {
                    if (!reset_pressed)
                    {
                        resetBn.Enabled = true;
                        resetBn.SetTextColor(Android.Graphics.Color.White);
                    }
                    if (e.Progress == 0)
                        distance_value.Text = "0.5" + " " + GetString(Resource.String.km);
                    else
                        distance_value.Text = e.Progress.ToString() + " " + GetString(Resource.String.km);

                    if (e.Progress == 0)
                        edit_expert.PutString("distance_radius", "0.5");
                    else
                        edit_expert.PutString("distance_radius", e.Progress.ToString());
                    edit_expert.Apply();
                    findBn.Visibility = ViewStates.Gone;
                    activityIndicator.Visibility = ViewStates.Visible;
                    inform_processTV.Visibility = ViewStates.Visible;
                    inform_processTV.Text = "Получение количества специалистов";
                    var filtered = await specializationMethods.ExpertCount(
                          expert_data.GetString("spec_id", String.Empty),
                          expert_data.GetString("expert_city_id", String.Empty),
                          expert_data.GetString("distance_radius", String.Empty),
                          expert_data.GetBoolean("has_reviews", false),
                          pref.GetString("latitude", String.Empty),
                          pref.GetString("longitude", String.Empty));
                    var deserialized_value = JsonConvert.DeserializeObject<ExpertCount>(filtered.ToString());
                    activityIndicator.Visibility = ViewStates.Gone;
                    findBn.Visibility = ViewStates.Visible;
                    inform_processTV.Visibility = ViewStates.Gone;
                    findBn.Text = "Показать " + deserialized_value.count.ToString() + " специалистов";
                    reset_pressed = false;
                };

                onlyWithReviewsS.CheckedChange += async (s, e) =>
                 {
                     if (!reset_pressed)
                     {
                         resetBn.Enabled = true;
                         resetBn.SetTextColor(Android.Graphics.Color.White);
                     }
                     if (onlyWithReviewsS.Checked)
                         edit_expert.PutBoolean("has_reviews", true);
                     else
                         edit_expert.PutBoolean("has_reviews", false);
                     edit_expert.Apply();
                     findBn.Visibility = ViewStates.Gone;
                     activityIndicator.Visibility = ViewStates.Visible;
                     inform_processTV.Visibility = ViewStates.Visible;
                     inform_processTV.Text = "Получение количества специалитов";
                     var filtered = await specializationMethods.ExpertCount(
                           expert_data.GetString("spec_id", String.Empty),
                           expert_data.GetString("expert_city_id", String.Empty),
                           expert_data.GetString("distance_radius", String.Empty),
                           expert_data.GetBoolean("has_reviews", false),
                           pref.GetString("latitude", String.Empty),
                           pref.GetString("longitude", String.Empty)
                           );
                     var deserialized_value = JsonConvert.DeserializeObject<ExpertCount>(filtered.ToString());
                     activityIndicator.Visibility = ViewStates.Gone;
                     findBn.Visibility = ViewStates.Visible;
                     inform_processTV.Visibility = ViewStates.Gone;
                     findBn.Text = "Показать " + deserialized_value.count.ToString() + " специалистов";
                     reset_pressed = false;
                 };
                city_value.Click += async (s, e) =>
                {

                    findBn.Visibility = ViewStates.Gone;
                    activityIndicator.Visibility = ViewStates.Visible;
                    inform_processTV.Visibility = ViewStates.Visible;
                    inform_processTV.Text = GetString(Resource.String.getting_cities);
                    var cities = await specializationMethods.GetCities();
                    city_chooseLL.Visibility = ViewStates.Visible;
                    var deserialized_cities = JsonConvert.DeserializeObject<List<City>>(cities.ToString());
                    var listOfCitiesAdapter = new CityAdapter(deserialized_cities, this, tf);
                    recyclerView.SetAdapter(listOfCitiesAdapter);
                };
                cancellationBn.Click += (s, e) =>
                  {
                      city_chooseLL.Visibility = ViewStates.Gone;
                      inform_processTV.Visibility = ViewStates.Gone;
                      findBn.Visibility = ViewStates.Visible;
                  };
                findBn.Click += (s, e) =>
                  {
                      StartActivity(typeof(ListOfSpecialistsActivity));
                  };
                resetBn.Click += (s, e) =>
                  {
                      city_chooseLL.Visibility = ViewStates.Gone;
                      reset_pressed = true;
                      resetBn.Enabled = false;
                      resetBn.SetTextColor(new Color(ContextCompat.GetColor(this, Resource.Color.lightBlueColor)));
                      distanceRL.Visibility = ViewStates.Visible;
                      distance_value.Text = "100 " + GetString(Resource.String.km);
                      distanceSB.Progress = 100;
                      //setting values to default
                      edit_expert.PutString("expert_city_id", "");
                      edit_expert.PutString("expert_city_name", "");
                      edit_expert.PutString("distance_radius", "");
                      edit_expert.PutBoolean("has_reviews", false);
                      edit_expert.Apply();
                      city_value.Text = GetString(Resource.String.not_chosen);
                      onlyWithReviewsS.Checked = false;
                  };
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }

        public async void city_click()
        {
            SpecializationMethods specializationMethods = new SpecializationMethods();

            resetBn.Enabled = true;
            resetBn.SetTextColor(Android.Graphics.Color.White);
            distanceRL.Visibility = ViewStates.Gone;
            edit_expert.PutString("distance_radius", "");
            edit_expert.Apply();
            //USING RESOURCES IN HERE CAUSES EXCEPTION
            inform_processTV.Text = "Подсчет количества специалистов";//GetString(Resource.String.getting_exps_count);
            city_chooseLL.Visibility = ViewStates.Gone;
            city_value.Text = expert_data.GetString("expert_city_name", String.Empty);
            var filtered = await specializationMethods.ExpertCount(
                expert_data.GetString("spec_id", String.Empty),
                expert_data.GetString("expert_city_id", String.Empty),
                expert_data.GetString("distance_radius", String.Empty),
                expert_data.GetBoolean("has_reviews", false),
                pref.GetString("latitude", String.Empty),
                pref.GetString("longitude", String.Empty)
                );
            var deserialized_value = JsonConvert.DeserializeObject<ExpertCount>(filtered.ToString());
            activityIndicator.Visibility = ViewStates.Gone;
            findBn.Visibility = ViewStates.Visible;
            inform_processTV.Visibility = ViewStates.Gone;
            findBn.Text = "Показать " + deserialized_value.count.ToString() + " специалистов";
            reset_pressed = false;
        }
    }
}