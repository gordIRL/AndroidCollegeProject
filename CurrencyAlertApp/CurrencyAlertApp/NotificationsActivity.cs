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
    public class NotificationsActivity : AppCompatActivity
    {
        TextView txtInfo1;
        Button btnBack;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Notifications);


            // ToolBar - Top of Screen  (method 1)
            var toolbar = FindViewById<Toolbar>(Resource.Id.notificationsActivity_top_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = GetString(Resource.String.notificationsActivity_top_toolbar_title);
                       

            txtInfo1 = FindViewById<TextView>(Resource.Id.notificatonsActivity_txt_Info1);
            //txtInfo1.Text = "Warning\nAlert Activated\nTake Appropriate Action";
            txtInfo1.Text = GetString(Resource.String.notificationsActivity_information);

            btnBack = FindViewById<Button>(Resource.Id.notificationsActivity_btn_back);
            btnBack.Click += BtnBack_Click;

            // instantiate builder(using compat) and set notification elements
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
                .SetContentTitle(GetString(Resource.String.notificationsActivity_notification_content_title))
                .SetContentText(GetString(Resource.String.notificationsActivity_notification_content_text))
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

        private void BtnBack_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(UserAlertsActivity));
            StartActivity(intent);
        }
    }
}
