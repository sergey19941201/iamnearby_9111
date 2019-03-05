using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Iamnearby.Activities;

namespace Iamnearby.Fragments
{
    public class LoginRegFragment : DialogFragment
    {
        TextView regTV;
        TextView authTV;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.LoginAuthFragment, container, false);

            regTV = rootView.FindViewById<TextView>(Resource.Id.regTV);
            authTV = rootView.FindViewById<TextView>(Resource.Id.authTV);

            Typeface tf = Typeface.CreateFromAsset(this.Activity.Assets, "Roboto-Regular.ttf");
            rootView.FindViewById<TextView>(Resource.Id.textView1).SetTypeface(tf, TypefaceStyle.Normal);
            regTV.SetTypeface(tf, TypefaceStyle.Normal);
            authTV.SetTypeface(tf, TypefaceStyle.Normal);

            regTV.Click += (s, e) =>
            {
                StartActivity(new Intent(Activity, typeof(RegistrationActivity)));
            };
            authTV.Click += (s, e) =>
            {
                StartActivity(new Intent(Activity, typeof(AuthorizationActivity)));
            };

            return rootView;
        }
    }
}