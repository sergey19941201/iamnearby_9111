using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Iamnearby.Methods;

namespace Iamnearby.Activities
{
    [Activity(Label = "AuthorizationActivity", ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustNothing/*, Theme = "@style/AppThemeLightNavbar"*/)]
    public class AuthorizationActivity : Activity
    {
        EditText emailET;
        TextView headerTV, infoTV;
        ImageView mainImageIV;
        RelativeLayout backRelativeLayout;
        ImageButton back_button;
        Button sendBn;
        ProgressBar activityIndicator;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try { 
            SetContentView(Resource.Layout.RegEmail);
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            AuthorizationMethods authorizationMethods = new AuthorizationMethods();
            headerTV = FindViewById<TextView>(Resource.Id.headerTV);
            infoTV = FindViewById<TextView>(Resource.Id.infoTV);
            mainImageIV = FindViewById<ImageView>(Resource.Id.mainImageIV);
            backRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.backRelativeLayout);
            back_button = FindViewById<ImageButton>(Resource.Id.back_button);
            activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
            activityIndicator.IndeterminateDrawable.SetColorFilter(Resources.GetColor(Resource.Color.buttonBackgroundColor), Android.Graphics.PorterDuff.Mode.Multiply);
            ISharedPreferences pref = Application.Context.GetSharedPreferences("auth_data", FileCreationMode.Private);
            ISharedPreferencesEditor edit = pref.Edit();
            sendBn = FindViewById<Button>(Resource.Id.sendBn);
            emailET = FindViewById<EditText>(Resource.Id.emailET);
            mainImageIV.SetBackgroundResource(Resource.Drawable.mail_small2);
            infoTV.Text= GetString(Resource.String.link_for_enter);
            sendBn.Text = GetString(Resource.String.send_link);
            headerTV.Text = GetString(Resource.String.login);
            Typeface tf = Typeface.CreateFromAsset(Assets, "Roboto-Regular.ttf");
            headerTV.SetTypeface(tf, TypefaceStyle.Bold);
            sendBn.SetTypeface(tf, TypefaceStyle.Normal);
            infoTV.SetTypeface(tf, TypefaceStyle.Normal);
            emailET.SetTypeface(tf, TypefaceStyle.Normal);

            Intent intent = new Intent(this, typeof(AuthAfterActivity));
            intent.PutExtra("bottom_value", "profile");
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
                edit.PutString("email", emailET.Text);
                edit.Apply();
                sendBn.Visibility = ViewStates.Gone;
                activityIndicator.Visibility = ViewStates.Visible;
                var auth_result = await authorizationMethods.Authorize(emailET.Text);
                if (auth_result.Contains("с таким email нет в нашей базе"))
                {
                    infoTV.Text = GetString(Resource.String.email_not_exists);
                }
                else
                {
                    StartActivity(intent);
                }
                sendBn.Visibility = ViewStates.Visible;
                activityIndicator.Visibility = ViewStates.Gone;
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