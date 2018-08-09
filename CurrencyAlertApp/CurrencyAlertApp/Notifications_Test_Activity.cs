using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Widget;


namespace CurrencyAlertApp
{
    [Activity(Theme = "@style/MyTheme.Base",    Label = "TestNotificationsActivity")]
    public class Notifications_Test_Activity : AppCompatActivity
    {
        TextView txtInfo1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Notifications_Test_Layout);
            //  ??Toolbar.SetTitle = "My Test Notification Layout";
            txtInfo1 = FindViewById<TextView>(Resource.Id.txtInfo1);
            txtInfo1.Text = "Info text wired up";

            Toast.MakeText(this, "Test Notification Activity Reached", ToastLength.Long).Show();

            // instantiate builder(using compat) and set notification elements
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
                .SetContentTitle("My 1st Notification")
                .SetContentText("Here is the content from my new notification.")
                .SetSmallIcon(Resource.Mipmap.ic_album_black_24dp)
                //Obsolete code !!!!
                .SetDefaults(NotificationCompat.DefaultVibrate)    // NotificationDefaults.Vibrate)                
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Alarm));


            //// no lockscreen notifications before Android 5.0(API level 21)
            //if ((int)Android.OS.Build.VERSION.SdkInt >= 21)
            //{ builder.SetVisibility(NotificationCompat.VisibilityPublic); }


            // build notification
            Notification notification = builder.Build();

            // get the notification manager
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // publish the notification
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);
        }
    }
}
