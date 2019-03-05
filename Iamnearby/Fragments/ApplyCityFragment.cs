using System;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Iamnearby.Activities;

namespace Iamnearby.Fragments
{
    public class ApplyCityFragment : DialogFragment
    {
        ISharedPreferences city_coord_for_edit_prefs = Application.Context.GetSharedPreferences("city_coord_for_edit_prefs", FileCreationMode.Private);
        TextView cityValueTV, applyTV, cancelTV;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.ApplyCityFragment, container, false);

            cityValueTV = rootView.FindViewById<TextView>(Resource.Id.cityValueTV);
            cityValueTV.Text = city_coord_for_edit_prefs.GetString("city_name", String.Empty);
            applyTV = rootView.FindViewById<TextView>(Resource.Id.applyTV);
            cancelTV = rootView.FindViewById<TextView>(Resource.Id.cancelTV);

            Typeface tf = Typeface.CreateFromAsset(this.Activity.Assets, "Roboto-Regular.ttf");
            rootView.FindViewById<TextView>(Resource.Id.textView1).SetTypeface(tf, TypefaceStyle.Normal);
            cityValueTV.SetTypeface(tf, TypefaceStyle.Normal);
            applyTV.SetTypeface(tf, TypefaceStyle.Normal);
            cancelTV.SetTypeface(tf, TypefaceStyle.Normal);

            applyTV.Click += (s, e) =>
            {
                StartActivity(new Intent(Activity, typeof(ApplyCityEditActivity)));
            };
            cancelTV.Click += (s, e) =>
            {
                StartActivity(new Intent(Activity, typeof(UserProfileActivity)));
            };

            return rootView;
        }
    }
}