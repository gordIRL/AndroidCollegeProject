using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Widget;
using Android.Support.V7.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;


namespace CurrencyAlertApp
{
    [Activity(Theme = "@style/MyTheme.Base",    Label = "Notifications!!!")]
    public class Notifications_Test_Activity : AppCompatActivity
    {
        TextView txtInfo1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Notifications_Test_Layout);


            // ToolBar - Top of Screen  (method 1)
            var toolbar = FindViewById<Toolbar>(Resource.Id.notificationsActivity_top_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = GetString(Resource.String.notificationsActivity_top_toolbar_title);



            txtInfo1 = FindViewById<TextView>(Resource.Id.notificatonsActivity_txt_Info1);
            txtInfo1.Text = "Warning\nAlert Activated\nTake Appropriate Action";

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
