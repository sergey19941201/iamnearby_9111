using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Iamnearby.Adapters;
using System.Collections.Generic;

namespace Iamnearby.Activities
{
    [Activity(Label = "AddSpecializationActivity", ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden)]
    public class AddSpecializationActivity : Activity
    {
        RelativeLayout backRelativeLayout;
        ImageButton back_button;
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        Button continueBn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.AddSpecialization);

                continueBn = FindViewById<Button>(Resource.Id.continueBn);
                backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
                back_button = FindViewById<ImageButton>(Resource.Id.back_button);
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                recyclerView.SetLayoutManager(layoutManager);
                Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
                FindViewById<TextView>(Resource.Id.headerTV).SetTypeface(tf, TypefaceStyle.Bold);
                FindViewById<Button>(Resource.Id.continueBn).SetTypeface(tf, TypefaceStyle.Normal);

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
                      StartActivity(typeof(AddOneMoreSpecActivity));
                  };
                List<string> temp_list = new List<string>();
                int i = 0;
                foreach (var item in SubCategoryAdapter.my_specializations_static)
                {
                    temp_list.Add(SubCategoryAdapter.my_specializations_static[i].name);
                    i++;
                }
                var upperSpecializationAdapter = new AddSpecializationAdapter(temp_list, this, tf);

                recyclerView.SetAdapter(upperSpecializationAdapter);
            }
            catch
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}