using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Android.Text.Format;
using CurrencyAlertApp.DataAccess;
using System.Threading.Tasks;

namespace CurrencyAlertApp
{
    [Activity(Label = "PersonalAlarmsActivity",    Theme = "@style/MyTheme.Base")]
    // MainLauncher = true, 
    public class PersonalAlarmsActivity : Activity
    {
        // declare controls for Set Alarm via seconds
        private Button startBtn;       
        private EditText timeTxt;
        TextView txtOffSetTime;
        Int32 myOffset;

        // declare controls for set alarms by time
        Button btnSetTime;
        Button btnSetDate;
        TextView txtDate;
        TextView txtTime;
        Button btnSetPersonalAlert;

        public static DateTime combinedDateTimeObject;
        public static NewsObject myResultNewsObject;

        //public static int alarmNumber;
        public static List<DateTime> dateTimesList = new List<DateTime>();



        //// MSDN code
        ////DateTime centuryBegin = new DateTime(2001, 1, 1);
        ////DateTime currentDate = DateTime.Now;

        ////long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
        ////TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PersonalAlarms);

            // wire up controls for Set Alarm via seconds
            timeTxt = FindViewById<EditText>(Resource.Id.timeTxt);
            startBtn = FindViewById<Button>(Resource.Id.startBtn);
            txtOffSetTime = FindViewById<TextView>(Resource.Id.txtOffSetTime);
            startBtn.Click += StartBtn_Click;

            // wire up controls for set alarms by time
            btnSetTime = FindViewById<Button>(Resource.Id.btnSetTime);
            btnSetDate = FindViewById<Button>(Resource.Id.btnSetDate);
            txtDate = FindViewById<TextView>(Resource.Id.txtDate);
            txtTime = FindViewById<TextView>(Resource.Id.txtTime);
            btnSetPersonalAlert = FindViewById<Button>(Resource.Id.btnSetPersonalAlert);

            btnSetPersonalAlert.Click += BtnSetPersonalAlert_Click;
            btnSetTime.Click += BtnSetTime_Click;
            btnSetDate.Click += BtnSetDate_Click;

            if (myResultNewsObject != null)
            {
                Toast.MakeText(this, "PERSONAL ALARMS\n" + myResultNewsObject.ToString(), ToastLength.Long).Show();
            }
        }

       


        // click event for for Set Alarm via seconds
        void StartBtn_Click(object sender, EventArgs e)
        {

            //// Original
            ////GET TIME IN SECONDS AND INITIALIZE INTENT
            //int time = Convert.ToInt32(timeTxt.Text);
            //Intent intent = new Intent(this, typeof(Receiver1));

            ////PASS CONTEXT,YOUR PRIVATE REQUEST CODE,INTENT OBJECT AND FLAG  
            //// ??? private request code - must be different for each alarm ???
            //PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, 0, intent, 0); //  CONTEXT,PRIVATE REQUEST CODE, INTENT, FLAG 

            ////INITIALIZE ALARM MANAGER
            //AlarmManager alarmManager = (AlarmManager)GetSystemService(AlarmService);

            ////SET THE ALARM
            //alarmManager.Set(AlarmType.RtcWakeup, JavaSystem.CurrentTimeMillis() + (time * 1000), pendingIntent);
            //Toast.MakeText(this, "Alarm set In: " + time + " seconds", ToastLength.Long).Show();
            //timeTxt.Text = "";
            //----------------------------------------------------------------------------------------------




            // Single alarm set from object passed from main activity  - WORKING

            // start a new thread - so as not to run on the UI thread - keep the UI thread responsive
            Task.Factory.StartNew(() =>
            {
                //// experiment to demonstrate threading
                //for (int i = 0; i < 500000; i++)
                //{
                //    Log.Debug("DEBUG", "TEST: " + i);
                //    //Thread.Sleep(1000);  // issues with System.Threading                   
                //}    
                // create new DateTime object from object passed from Main Activity
                DateTime dateTimeFromPassedObject = myResultNewsObject.DateAndTime;

                // store the new DateTime object in list
                dateTimesList.Add(dateTimeFromPassedObject);

                // use current no of items in list as the alarm number for this alarm
                int alarmNumber = dateTimesList.Count;

                // get no of milliseconds from datetime object
                long yourDateTimeMilliseconds4 = new DateTimeOffset(dateTimeFromPassedObject).ToUnixTimeMilliseconds();

                // set alarm
                var ringTime = yourDateTimeMilliseconds4;
                Intent intent = new Intent(this, typeof(Receiver1));
                PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, alarmNumber, intent, 0);  //  CONTEXT,PRIVATE REQUEST CODE, INTENT, FLAG
                AlarmManager alarmManager = (AlarmManager)GetSystemService(AlarmService);
                alarmManager.Set(AlarmType.RtcWakeup, ringTime, pendingIntent);
                //Log.Debug("DEBUG", "TEST");

            }); // end of thread





            //--------------------------------------------------------------------------------------------------------------

            //// Experiment with multiple alarms
            //DateTime dateTime_1 = DateTime.Now.AddSeconds(0);
            //DateTime dateTime_2 = dateTime_1.AddSeconds(10);
            //DateTime dateTime_3 = dateTime_1.AddSeconds(20);

            //long yourDateTimeMilliseconds1 = new DateTimeOffset(dateTime_1).ToUnixTimeMilliseconds();
            //long yourDateTimeMilliseconds2 = new DateTimeOffset(dateTime_2).ToUnixTimeMilliseconds();
            //long yourDateTimeMilliseconds3 = new DateTimeOffset(dateTime_3).ToUnixTimeMilliseconds();

            //Log.Debug("DEBUG", "Time is: " + dateTime_1.ToLongTimeString());
            //Log.Debug("DEBUG", "Time is: " + dateTime_2.ToLongTimeString());
            //Log.Debug("DEBUG", "Time is: " + dateTime_3.ToLongTimeString());

            //Log.Debug("DEBUG", "Time is: " + yourDateTimeMilliseconds1);
            //Log.Debug("DEBUG", "Time is: " + yourDateTimeMilliseconds2);
            //Log.Debug("DEBUG", "Time is: " + yourDateTimeMilliseconds3);


            //var ringTime = yourDateTimeMilliseconds1;
            //Intent intent = new Intent(this, typeof(Receiver1));
            //PendingIntent pendingIntent1 = PendingIntent.GetBroadcast(this, 1, intent, 0); //  CONTEXT,PRIVATE REQUEST CODE, INTENT, FLAG 
            //AlarmManager alarmManager = (AlarmManager)GetSystemService(AlarmService);
            //alarmManager.Set(AlarmType.RtcWakeup, ringTime, pendingIntent1);  

            //ringTime = yourDateTimeMilliseconds2;
            //intent = new Intent(this, typeof(Receiver1));
            //PendingIntent pendingIntent2 = PendingIntent.GetBroadcast(this, 2, intent, 0); //  CONTEXT,PRIVATE REQUEST CODE, INTENT, FLAG 
            //alarmManager = (AlarmManager)GetSystemService(AlarmService);
            //alarmManager.Set(AlarmType.RtcWakeup, ringTime, pendingIntent2);

            //ringTime = yourDateTimeMilliseconds3;
            //intent = new Intent(this, typeof(Receiver1));
            //PendingIntent pendingIntent3 = PendingIntent.GetBroadcast(this, 3, intent, 0); //  CONTEXT,PRIVATE REQUEST CODE, INTENT, FLAG 
            //alarmManager = (AlarmManager)GetSystemService(AlarmService);
            //alarmManager.Set(AlarmType.RtcWakeup, ringTime, pendingIntent3);

            //alarmManager.Cancel(pendingIntent1);
            //alarmManager.Cancel(pendingIntent2);



        } //////////////////////////////////////////////////////////////












        // click event for set alarms by time
        private void BtnSetPersonalAlert_Click(object sender, EventArgs e)
        {
            try
            {
                txtOffSetTime.Text = "Offset = updated";
                //int hours = int.Parse(edtTimeHours.Text);
                //int minutes = int.Parse(edtTimeMinutes.Text);

                DateTime now = DateTime.Now;
                DateTime future = combinedDateTimeObject;  // Year, Month, Day, Hour(24), Minutes, Seconds

                Int32 unixTimestampNOW = (Int32)(DateTime.UtcNow.Subtract(now)).TotalSeconds;
                Int32 unixTimestampFuture = (Int32)(DateTime.UtcNow.Subtract(future)).TotalSeconds;

                myOffset = unixTimestampNOW - unixTimestampFuture;
                // now is a samaller number than future  ie. (  (future-1970)  >  (now-1970)  ) !!   

                txtOffSetTime.Text = $"Now: {now.ToString()}  \nFut: {future.ToString()}\n\n";
                txtOffSetTime.Text += $"Offset = updated {myOffset.ToString()}";

                //GET TIME IN SECONDS AND INITIALIZE INTENT
                Intent i = new Intent(this, typeof(Receiver1));

                //PASS CONTEXT,YOUR PRIVATE REQUEST CODE,INTENT OBJECT AND FLAG
                PendingIntent pi = PendingIntent.GetBroadcast(this, 100, i, 0);

                //INITIALIZE ALARM MANAGER
                AlarmManager alarmManager = (AlarmManager)GetSystemService(AlarmService);

                //SET THE ALARM
                alarmManager.Set(AlarmType.RtcWakeup, JavaSystem.CurrentTimeMillis() + (myOffset * 1000), pi);
                Toast.MakeText(this, "Alarm set In: " + myOffset.ToString() + " seconds", ToastLength.Long).Show();
            }
            catch
            {
                Toast.MakeText(this, "Please enter valid time - in digits", ToastLength.Long).Show();
            }
        }

       


//----TIME-----------------------------------------------------------------------------------------------
       
        private void BtnSetTime_Click(object sender, EventArgs e)
        {            
            TimePickerFragment frag = TimePickerFragment.NewInstance(
                delegate (DateTime time)
                {
                    txtTime.Text = time.ToShortTimeString();
                });
            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }//


        public class TimePickerFragment : DialogFragment, TimePickerDialog.IOnTimeSetListener
        {
            public static readonly string TAG = "MyTimePickerFragment";
            Action<DateTime> timeSelectedHandler = delegate { };

            public static TimePickerFragment NewInstance(Action<DateTime> onTimeSelected)
            {
                TimePickerFragment frag = new TimePickerFragment();
                frag.timeSelectedHandler = onTimeSelected; return frag;
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                DateTime currentTime = DateTime.Now;
                bool is24HourFormat = DateFormat.Is24HourFormat(Activity);
                //is24HourFormat = true;
                TimePickerDialog dialog = new TimePickerDialog
                    (Activity, this, currentTime.Hour, currentTime.Minute, is24HourFormat);
                return dialog;
            }

            public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
            {
                DateTime currentTime = DateTime.Now;
                DateTime selectedTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hourOfDay, minute, 0);
                Log.Debug(TAG, selectedTime.ToLongTimeString());
                timeSelectedHandler(selectedTime);

                //  my stuff
                combinedDateTimeObject = new DateTime(combinedDateTimeObject.Year, combinedDateTimeObject.Month, combinedDateTimeObject.Day, hourOfDay, minute, 0);
                TextView combinedDateTimeTextView = Activity.FindViewById<TextView>(Resource.Id.txtcombinedDateTime);
                combinedDateTimeTextView.Text = combinedDateTimeObject.ToString();
            }
        }


        



        //--DATE----------------------------------------------------------------------------------------------------

        private void BtnSetDate_Click(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                txtDate.Text = time.ToLongDateString();
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }//


        public class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
        {
            // TAG can be any string of your choice.     
            public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();
            // Initialize this value to prevent NullReferenceExceptions.     
            Action<DateTime> _dateSelectedHandler = delegate { };

            public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected)
            {
                DatePickerFragment frag = new DatePickerFragment();
                frag._dateSelectedHandler = onDateSelected;
                return frag;
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                DateTime currently = DateTime.Now;
                DatePickerDialog dialog = new DatePickerDialog(Activity,
                                                                this,
                                                                currently.Year,
                                                                currently.Month - 1,
                                                                currently.Day);
                return dialog;
            }

            public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
            {
                // Note: monthOfYear is a value between 0 and 11, not 1 and 12!         
                DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
                Log.Debug(TAG, selectedDate.ToLongDateString());
                _dateSelectedHandler(selectedDate);

                //  my stuff
                combinedDateTimeObject = new DateTime(year, monthOfYear + 1, dayOfMonth, combinedDateTimeObject.Hour, combinedDateTimeObject.Minute, combinedDateTimeObject.Second);
                TextView combinedDateTimeTextView = Activity.FindViewById<TextView>(Resource.Id.txtcombinedDateTime);
                combinedDateTimeTextView.Text = combinedDateTimeObject.ToString();
            }
        }


        public static void MethodToPassObject(NewsObject myNewsObjectInput)
        {
            myResultNewsObject = myNewsObjectInput;
        }


    }//
}//