using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Views;
using Java.Lang;
using Android.Support.V4;
using Android.Support.V4.App;


namespace Alarm_Manager
{    
    [Activity(Label = "Alarm_Manager", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        // Instructions for use:
        //
        // Enter no of seconds until alarm will sound & notification displays
        //
        // OR enter time(24 hour) & minutes to set alarm & notification
        // change date etc in the code below - line 90


        // declare controls for Set Alarm via seconds
        private Button startBtn;
        private EditText timeTxt;
        TextView txtOffSetTime;
        Int32 myOffset;

        // declare controls for set alarms by time
        EditText edtTimeHours;
        EditText edtTimeMinutes;
        Button btnSubmitTime;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // wire up controls for Set Alarm via seconds
            timeTxt = FindViewById<EditText>(Resource.Id.timeTxt);
            startBtn = FindViewById<Button>(Resource.Id.startBtn);
            txtOffSetTime = FindViewById<TextView>(Resource.Id.txtOffSetTime);
            startBtn.Click += StartBtn_Click;

            // wire up controls for set alarms by time
            edtTimeHours = FindViewById<EditText>(Resource.Id.edtTimeHours);
            edtTimeMinutes = FindViewById<EditText>(Resource.Id.edtTimeMinutes);
            btnSubmitTime = FindViewById<Button>(Resource.Id.btnSubmitTime);

            btnSubmitTime.Click += BtnSubmitTime_Click;
        }


        // click event for for Set Alarm via seconds
        void StartBtn_Click(object sender, EventArgs e)
        {
            //GET TIME IN SECONDS AND INITIALIZE INTENT
            int time = Convert.ToInt32(timeTxt.Text);
            Intent i = new Intent(this, typeof(MyReceiver));

            //PASS CONTEXT,YOUR PRIVATE REQUEST CODE,INTENT OBJECT AND FLAG
            PendingIntent pi = PendingIntent.GetBroadcast(this, 0, i, 0);

            //INITIALIZE ALARM MANAGER
            AlarmManager alarmManager = (AlarmManager)GetSystemService(AlarmService);

            //SET THE ALARM
            alarmManager.Set(AlarmType.RtcWakeup, JavaSystem.CurrentTimeMillis() + (time * 1000), pi);
            Toast.MakeText(this, "Alarm set In: " + time + " seconds", ToastLength.Long).Show();

            ResetUserControls();
        }



        // click event for set alarms by time
        private void BtnSubmitTime_Click(object sender, EventArgs e)
        {
            try
            {
                txtOffSetTime.Text = "Offset = updated";
                int hours = int.Parse(edtTimeHours.Text);
                int minutes = int.Parse(edtTimeMinutes.Text);

                DateTime now = DateTime.Now;
                DateTime future = new DateTime(2018, 6, 22, hours, minutes, 0);  // Year, Month, Day, Hour(24), Minutes, Seconds

                Int32 unixTimestampNOW = (Int32)(DateTime.UtcNow.Subtract(now)).TotalSeconds;
                Int32 unixTimestampFuture = (Int32)(DateTime.UtcNow.Subtract(future)).TotalSeconds;

                myOffset = unixTimestampNOW - unixTimestampFuture;
                // now is a samaller number than future  ie. (  (future-1970)  >  (now-1970)  ) !!   

                txtOffSetTime.Text = $"Now: {now.ToString()}  \nFut: {future.ToString()}\n\n";
                txtOffSetTime.Text += $"Offset = updated {myOffset.ToString()}";

                //GET TIME IN SECONDS AND INITIALIZE INTENT
                Intent i = new Intent(this, typeof(MyReceiver));

                //PASS CONTEXT,YOUR PRIVATE REQUEST CODE,INTENT OBJECT AND FLAG
                PendingIntent pi = PendingIntent.GetBroadcast(this, 0, i, 0);

                //INITIALIZE ALARM MANAGER
                AlarmManager alarmManager = (AlarmManager)GetSystemService(AlarmService);

                //SET THE ALARM
                alarmManager.Set(AlarmType.RtcWakeup, JavaSystem.CurrentTimeMillis() + (myOffset * 1000), pi);
                Toast.MakeText(this, "Alarm set In: " + myOffset.ToString() + " seconds", ToastLength.Long).Show();

                ResetUserControls();
            }
            catch
            {
                Toast.MakeText(this, "Please enter valid time - in digits", ToastLength.Long).Show();
                ResetUserControls();
            }            
        }
        void ResetUserControls()
        {
            // reset user controls
            edtTimeHours.Text = "";
            edtTimeMinutes.Text = "";
            timeTxt.Text = "";
        }
    }
}

