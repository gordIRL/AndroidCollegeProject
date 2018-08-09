using Android.Widget;
using Android.Content;


namespace CurrencyAlertApp
{
    [BroadcastReceiver]
    public class Receiver1 : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "Received intent!", ToastLength.Short).Show();

            Toast.MakeText(context, "Alarm Ringing!", ToastLength.Short).Show();

            Intent myNewIntent = new Intent(context, typeof(Notifications_Test_Activity));   // use 'context' not 'this' here!!
            context.StartActivity(myNewIntent);
        }
    }
}