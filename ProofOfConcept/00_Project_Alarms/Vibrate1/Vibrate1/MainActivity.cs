using Android.App;
using Android.Widget;
using Android.OS;
using Xamarin.Essentials;
using System;
using Android.Runtime;
using Android.Content;
using Android.Media;
using System.Threading;
using Android.Support.V4;
using Android.Support.V4.App;

namespace Vibrate1
{
    [Activity(Label = "Vibrate1", MainLauncher = true)]
    public class MainActivity : Activity
    {
        Button btnVB_On;
        Button btnVB_Off;
        Button btnScreen2;
        Button btnAlarm_On;
        Button btnAlarm_Off;
        Ringtone ringer;

        //protected override void OnCreate(Bundle savedInstanceState)  -  Original VS code
        //Xamarin.Essentials.Platform.Init(this, bundle);  - code from MS documentation

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);  // -  Original VS code 
            //Xamarin.Essentials.Platform.Init(this, bundle);


            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            btnVB_On = FindViewById<Button>(Resource.Id.btnVB_On);
            btnVB_Off = FindViewById<Button>(Resource.Id.btnVB_Off);
            btnScreen2 = FindViewById<Button>(Resource.Id.btnScreen2);
            btnAlarm_On = FindViewById<Button>(Resource.Id.btnAlarm_On);
            btnAlarm_Off = FindViewById<Button>(Resource.Id.btnAlarm_Off);

            btnVB_On.Click += BtnVB_On_Click;
            btnVB_Off.Click += BtnVB_Off_Click;
            btnScreen2.Click += BtnScreen2_Click;
            btnAlarm_On.Click += btnAlarm_On_Click;
            btnAlarm_Off.Click += BtnAlarm_Off_Click;

            ringer = RingtoneManager.GetRingtone(ApplicationContext, RingtoneManager.GetDefaultUri(RingtoneType.Alarm));



            Intent intent = new Intent(this, typeof(Screen2Activity));
            //StartActivity(intent);
            const int pendindingIntentId = 0;
            PendingIntent pendingIntent =
                PendingIntent.GetActivity(this, pendindingIntentId, intent, PendingIntentFlags.OneShot);

            // instantiate builder and set notification elements
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
            //Notification.Builder builder = new Notification.Builder(this)
                .SetContentIntent(pendingIntent)
                .SetContentTitle("My 1st Notification")
                .SetContentText("Here is the content from my new notification.")
                .SetSmallIcon(Resource.Drawable.ic_home_black_24dp)
                .SetVisibility(NotificationCompat.VisibilityPublic)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Alarm))   // ? not obsolete in NotificationCompat !!
                .SetPriority(NotificationCompat.PriorityMax)
                ;
         


            // build notification
            Notification notification = builder.Build();

            // get the notification manager
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // publish the notification
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);












        }


        private void btnAlarm_On_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, $"Alarm sounding ...", ToastLength.Short).Show();

            ringer.Play();
            //Thread.Sleep(10000);
            //ringer.Stop();
        }



        private void BtnAlarm_Off_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, $"Alarm OFF", ToastLength.Short).Show();

            ringer.Stop();            
        }

       

        private void BtnScreen2_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(Screen2Activity));
            //StartActivity(intent);
            const int pendindingIntentId = 0;
            PendingIntent pendingIntent =
                PendingIntent.GetActivity(this, pendindingIntentId, intent, PendingIntentFlags.OneShot);

            // instantiate builder and set notification elements
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
            //Notification.Builder builder = new Notification.Builder(this)
                .SetContentIntent(pendingIntent)
                .SetContentTitle("My 1st Notification")
                .SetContentText("Here is the content from my new notification.")
                .SetSmallIcon(Resource.Drawable.ic_home_black_24dp)
                .SetVisibility(NotificationCompat.VisibilityPublic)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Ringtone))   // ? not obsolete in NotificationCompat !!
                .SetPriority(NotificationCompat.PriorityMax)
                ;
            //// no lockscreen notifications before Android 5.0(API level 21)
            //if ((int)Android.OS.Build.VERSION.SdkInt >= 21)
            //{
            //    builder.SetVisibility(NotificationCompat.VisibilityPublic);
            //}

            //Obsolete code !!!!
             //.SetDefaults(NotificationDefaults.Vibrate)
             //.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Ringtone))


            // build notification
            Notification notification = builder.Build();

            // get the notification manager
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // publish the notification
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);


        }

        private void BtnVB_Off_Click(object sender, System.EventArgs e)
        {
            try
            {
                Vibration.Cancel();
                Toast.MakeText(this, $"Vibration - cancelled", ToastLength.Short).Show();
            }
            catch (FeatureNotSupportedException ex)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        private void BtnVB_On_Click(object sender, System.EventArgs e)
        {
            try
            {
                Toast.MakeText(this, $"Vibration - ON !!", ToastLength.Short).Show();
                //// Use default vibration length
                //Vibration.Vibrate();

                // Or use specified time
                var duration = TimeSpan.FromSeconds(5);
                Vibration.Vibrate(duration);
            }
            catch (FeatureNotSupportedException ex)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }




        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }//
}//

