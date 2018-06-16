using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace Alarm_Manager
{
    [BroadcastReceiver]
    public class MyReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "Alarm Ringing!", ToastLength.Short).Show();

            Intent myNewIntent = new Intent(context, typeof(TestNotificationsActivity));   // use 'context' not 'this' here!!
            context.StartActivity(myNewIntent);
        }
    }
}