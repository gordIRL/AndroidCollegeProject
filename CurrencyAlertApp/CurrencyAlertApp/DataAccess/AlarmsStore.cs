using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace CurrencyAlertApp.DataAccess
{
    public static class AlarmsStore
    {
        //public static UserAlert mUserAlert { get; set; }

        //public static Intent intent;


        //public static void TestMethod_AlarmsStore_No_1(UserAlert userAlert)
        //{
        //    Log.Debug("DEBUG", "TEST METHOD NO 1\n\n\n\n\n");

        //    // Single alarm set from object passed from main activity  - WORKING

        //    //// start a new thread - so as not to run on the UI thread - keep the UI thread responsive
        //    //Task.Factory.StartNew(() =>
        //    //{

        //    //    // display output for testing
        //    //    Log.Debug("DEBUG", "\n\nAlarmStore  - UserAlert ID:  " + userAlert.UserAlertID.ToString());
        //    //    Log.Debug("DEBUG", "\n\nAlarmStore  - UserAlert ToString:\n" + userAlert.ToString());
        //    //    Log.Debug("DEBUG", "\n\nAlarmStore  - UserAlert DateTimeObject:\n" + userAlert.DateAndTime.ToString("dd/MM/yyyy"));
        //    //    Log.Debug("DEBUG", "AlarmStore  - UserAlert DateTimeObject:\n" + userAlert.DateAndTime.ToString("HH:mmtt"));
        //    //    Log.Debug("DEBUG", "FINISHED\n\n\n");



        //    //    //// create new DateTime object from object passed from Main Activity
        //    //    //DateTime dateTimeFromPassedObject = myResultNewsObject.DateAndTime;

        //    //    //// store the new DateTime object in list
        //    //    //dateTimesList.Add(dateTimeFromPassedObject);

        //    //    // use UserAlert ID (assigned by SQLite) as unique the alarm number for this alarm
        //    //    int alarmNumber = userAlert.UserAlertID;

        //    //    // get no of milliseconds from datetime object
        //    //    long MillisesondsOfUserAlertDateTime = new DateTimeOffset(userAlert.DateAndTime).ToUnixTimeMilliseconds();
        //    //    //long MillisesondsOfUserAlertDateTime = new DateTimeOffset(dateTimeFromPassedObject).ToUnixTimeMilliseconds();

        //    //    //// set alarm
        //    //    //var ringTime = MillisesondsOfUserAlertDateTime;
        //    //    //Intent intent = new  Intent(this, typeof(Receiver1));
        //    //    ////Intent intent = new Intent(this, typeof(Receiver1));

              

        //    //    //PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, alarmNumber, intent, 0);  //  CONTEXT,PRIVATE REQUEST CODE, INTENT, FLAG
        //    //    //AlarmManager alarmManager = (AlarmManager)GetSystemService(AlarmService);
        //    //    //alarmManager.Set(AlarmType.RtcWakeup, ringTime, pendingIntent);
        //    //    ////Log.Debug("DEBUG", "TEST");

        //    //}); // end of thread


        //}





    }//
}//