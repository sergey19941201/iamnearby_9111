using System;
using Android.App;
using Firebase.Iid;
using Android.Util;

namespace Iamnearby
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : FirebaseInstanceIdService
    {
        public static string refreshedToken;
        const string TAG = "MyFirebaseIIDService";
        public override void OnTokenRefresh()
        {
            if (refreshedToken == null)
                refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "Refreshed token: " + refreshedToken);
            SendRegistrationToServer(refreshedToken);
        }
        void SendRegistrationToServer(string token)
        {
            // Add custom implementation, as needed.
        }
    }
}