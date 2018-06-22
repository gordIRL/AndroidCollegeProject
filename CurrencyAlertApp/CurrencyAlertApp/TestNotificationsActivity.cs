using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace CurrencyAlertApp
{
    [Activity(Label = "TestNotificationsActivity")]
    public class TestNotificationsActivity : Activity
    {
        TextView txtInfo1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.TestNotificationsLayout);
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
