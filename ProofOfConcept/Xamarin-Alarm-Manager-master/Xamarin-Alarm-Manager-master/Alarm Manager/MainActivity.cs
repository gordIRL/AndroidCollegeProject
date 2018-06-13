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
    /*
     * OUR MAINACTIVITY
     * -Extends Activity
     * -Initializes our views and widgets
     * -Starts Alarm
     */
    [Activity(Label = "Alarm_Manager", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        //DECLARE WIDGETS
        private Button startBtn;
        private EditText timeTxt;
        TextView txtOffSetTime;
        Int32 myOffset;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
             SetContentView (Resource.Layout.Main);

             this.initializeViews();

        }
         /*
    INITIALIZE VIEWS
     */
    private void initializeViews()
    {
        timeTxt=  FindViewById<EditText>(Resource.Id.timeTxt);
        startBtn= FindViewById<Button>(Resource.Id.startBtn);
        txtOffSetTime = FindViewById<TextView>(Resource.Id.txtOffSetTime);

        txtOffSetTime.Text = "Offset = updated";    //  {myOffset.ToString()}

            DateTime now = DateTime.Now;
            DateTime future = new DateTime(2018, 5, 11, 10, 42, 0);

            //Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Int32 unixTimestampNOW = (Int32)(DateTime.UtcNow.Subtract(now)).TotalSeconds;
            Int32 unixTimestampFuture = (Int32)(DateTime.UtcNow.Subtract(future)).TotalSeconds;

            
            myOffset = unixTimestampNOW - unixTimestampFuture;
            // now is a samaller number than future  ie. (  (future-1970)  >  (now-1970)  ) !!   

            txtOffSetTime.Text = $"Now: {now.ToString()}  \nFuture: {future.ToString()}\n\n";

            txtOffSetTime.Text += $"Offset = updated {myOffset.ToString()}";    




            startBtn.Click += startBtn_Click;

    }

    void startBtn_Click(object sender, EventArgs e)
    {
        go();
    }

    /*
    INITIALIZE AND START OUR ALARM
     */
    private void go()
    {
        //GET TIME IN SECONDS AND INITIALIZE INTENT
        int time=Convert.ToInt32(timeTxt.Text);
        Intent i=new Intent(this,typeof(MyReceiver));

        //PASS CONTEXT,YOUR PRIVATE REQUEST CODE,INTENT OBJECT AND FLAG
        PendingIntent pi = PendingIntent.GetBroadcast(this,0,i,0);

        //INITIALIZE ALARM MANAGER
        AlarmManager alarmManager= (AlarmManager) GetSystemService(AlarmService);


            //SET THE ALARM
            //alarmManager.Set(AlarmType.RtcWakeup, JavaSystem.CurrentTimeMillis()+(time*1000),pi);
            alarmManager.Set(AlarmType.RtcWakeup, JavaSystem.CurrentTimeMillis() + (myOffset * 1000), pi);
            Toast.MakeText(this, "Alarm set In: " + time + " seconds", ToastLength.Short).Show();
    }


    }
}

