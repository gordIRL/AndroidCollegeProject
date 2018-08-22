using Android.Content;
using Android.Util;

namespace CurrencyAlertApp
{
    [BroadcastReceiver]
    public class Receiver1 : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Log.Debug("DEBUG", "\n\n\n" + intent.ToString() + "\n\n\n");
            Log.Debug("DEBUG", "\n");            

            Intent myNewIntent = new Intent(context, typeof(NotificationsActivity));   // use 'context' not 'this' here!!
            context.StartActivity(myNewIntent);
        }
    }
}