using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Text.Format;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using CurrencyAlertApp.DataAccess;


namespace CurrencyAlertApp
{
    [Activity(Label = "PersonalAlarmsActivity",  Theme = "@style/MyTheme.Base")]
    // MainLauncher = true, 
    public class PersonalAlertsActivity : AppCompatActivity
    {
        // new declarations
        EditText editTxtTitle;
        EditText editTxtDescription;

        Button btnSetTime;
        Button btnSetDate;
        Button btnSetPersonalAlert;
        Button btnCancelPersonalAlert;

        TextView txtDate;
        TextView txtTime;
        TextView combinedDateTimeTextView;

        // variables
        public static DateTime combinedDateTimeObject;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PersonalAlerts);

            // ToolBar - Top of Screen  (method 1)
            var toolbar = FindViewById<Toolbar>(Resource.Id.personalAlerts_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = GetString(Resource.String.personalAlerts_top_toolbar_title);

            // wire up EditText controls
            editTxtTitle = FindViewById<EditText>(Resource.Id.personalAlerts_editTxt_title);
            editTxtDescription = FindViewById<EditText>(Resource.Id.personalAlerts_editTxt_description);

            // wire up display controls
            combinedDateTimeTextView = FindViewById<TextView>(Resource.Id.personalAlerts_txt_combinedDateTime);

            // wire up controls for date & time pickers
            txtTime = FindViewById<TextView>(Resource.Id.personalAlerts_txt_time);
            btnSetTime = FindViewById<Button>(Resource.Id.personalAlerts_btn_setTime);
            btnSetTime.Click += BtnSetTime_Click;

            txtDate = FindViewById<TextView>(Resource.Id.personalAlerts_txt_date);
            btnSetDate = FindViewById<Button>(Resource.Id.personalAlerts_btn_setDate);                    
            btnSetDate.Click += BtnSetDate_Click;

            // wire up OK & Cancel buttons
            btnSetPersonalAlert = FindViewById<Button>(Resource.Id.personalAlerts_btn_setPersonalAlert);
            btnSetPersonalAlert.Click += BtnSetPersonalAlert_Click;
            btnCancelPersonalAlert = FindViewById<Button>(Resource.Id.personalAlerts_btn_cancelPersonalAlert);
            btnCancelPersonalAlert.Click += BtnCancelPersonalAlert_Click;        

        }  // end OnCreate()



        // TOP Toolbar
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.topMenu_PersonalAlertsActivity, menu);
            return base.OnCreateOptionsMenu(menu);
        }


        // TOP Toolbar ('menu options')
        public override bool OnOptionsItemSelected(IMenuItem item) // 
        {
            switch (item.ItemId)
            {
                case Resource.Id.topMenu_PersonalAlertsActivity_MarketData:
                    Toast.MakeText(this, "Action selected: \nGo to MarketData", ToastLength.Short).Show();
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                    break;
                default:
                    break;
            };
            return base.OnOptionsItemSelected(item);
        }


        // populate UserAlert object with data from screen controls & set Property with this UserAlert object
        private void BtnSetPersonalAlert_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "OK - Set Personal Alert Selected", ToastLength.Short).Show();

            UserAlert userAlert = new UserAlert
            {
                // don't add ID here - SQLite will do this automatically (auto-increment)
                Title = editTxtTitle.Text,
                DescriptionOfPersonalEvent = editTxtDescription.Text,
                CountryChar = "Personal: ",                  // Resource.String.personalAlerts_personalAlertName,
                MarketImpact = "High",
                IsPersonalAlert = true,
                DateAndTime = combinedDateTimeObject,

                // convert DateTime object to a ticks (long)
                DateInTicks = combinedDateTimeObject.Ticks
            };
            Log.Debug("DEBUG", "\n\n\n" + userAlert.ToString() + "\n\n\n");

            // remember to set property !!!!
            // call Property in UserAlertActivity to pass data across (newsObject)
            UserAlertActivity.SelectedUserAlert_PassedFrom_PersonalAlertsActivity = userAlert;

            // call intent to start next activity
            Intent intent = new Intent(this, typeof(UserAlertActivity));
            StartActivity(intent);
        }//



        private void BtnCancelPersonalAlert_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "CANCEL Alert Selected", ToastLength.Short).Show();

            Intent intent = new Intent(this, typeof(MainActivity));  // nice to go back to calling Activity (?? back statck ??)
            StartActivity(intent);
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
                TimePickerFragment frag = new TimePickerFragment
                {
                    timeSelectedHandler = onTimeSelected
                };
                return frag;
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
                // reference needed because outside of OnCreate()
                TextView combinedDateTimeTextView = Activity.FindViewById<TextView>(Resource.Id.personalAlerts_txt_combinedDateTime);
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
                DatePickerFragment frag = new DatePickerFragment
                {
                    _dateSelectedHandler = onDateSelected
                };
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
                // reference needed because outside of OnCreate()
                TextView combinedDateTimeTextView = Activity.FindViewById<TextView>(Resource.Id.personalAlerts_txt_combinedDateTime);
                combinedDateTimeTextView.Text = combinedDateTimeObject.ToString();
            }
        }
        //----------------------------------------------------------------------------------------------------------------------

    }//
}//