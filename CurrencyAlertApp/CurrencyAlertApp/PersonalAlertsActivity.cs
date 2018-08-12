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
        // control declarations
        EditText editTxtTitle,  editTxtDescription;
        Button btnSetTime, btnSetDate, btnSetPersonalAlert, btnCancelPersonalAlert;
        TextView txtDate, txtTime, combinedDateTimeTextView;

        // variables
        public static DateTime combinedDateTimeObject;
        public static bool dateIsSet = false;
        public static bool timeIsSet = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PersonalAlerts);

            // ToolBar - Top of Screen  (method 1)
            var toolbar = FindViewById<Toolbar>(Resource.Id.personalAlertsActivity_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = GetString(Resource.String.personalAlertsActivity_top_toolbar_title);

            // wire up EditText controls
            editTxtTitle = FindViewById<EditText>(Resource.Id.personalAlertsActivity_editTxt_title);
            editTxtDescription = FindViewById<EditText>(Resource.Id.personalAlertsActivity_editTxt_description);

            // wire up display controls
            combinedDateTimeTextView = FindViewById<TextView>(Resource.Id.personalAlertsActivity_txt_combinedDateTime);

            // wire up controls for date & time pickers
            txtTime = FindViewById<TextView>(Resource.Id.personalAlertsActivity_txt_time);
            btnSetTime = FindViewById<Button>(Resource.Id.personalAlertsActivity_btn_setTime);
            btnSetTime.Click += BtnSetTime_Click;

            txtDate = FindViewById<TextView>(Resource.Id.personalAlertsActivity_txt_date);
            btnSetDate = FindViewById<Button>(Resource.Id.personalAlertsActivity_btn_setDate);                    
            btnSetDate.Click += BtnSetDate_Click;

            // wire up OK & Cancel buttons
            btnSetPersonalAlert = FindViewById<Button>(Resource.Id.personalAlertsActivity_btn_setPersonalAlert);
            btnSetPersonalAlert.Click += BtnSetPersonalAlert_Click;
            btnCancelPersonalAlert = FindViewById<Button>(Resource.Id.personalAlertsActivity_btn_cancelPersonalAlert);
            btnCancelPersonalAlert.Click += BtnCancelPersonalAlert_Click;

            ResetPersonalAlertData();

        }  // end OnCreate()



        // TOP Toolbar
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.personalAlertsActivity_topMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }


        // TOP Toolbar ('menu options')
        public override bool OnOptionsItemSelected(IMenuItem item) // 
        {
            switch (item.ItemId)
            {
                case Resource.Id.personalAlertsActivity_top_toolbar_option_marketData:
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
            // Perform Validation
            if (editTxtTitle.Text == string.Empty)
            {
                editTxtTitle.RequestFocus();
                editTxtTitle.Hint = GetString(Resource.String.personalAlertsActivity_validation_message_enterTitle);
            }
            else if (editTxtDescription.Text == string.Empty || editTxtDescription.Text.Length >200)
            {
                editTxtDescription.RequestFocus();
                editTxtDescription.Text = "";
                editTxtDescription.Hint = GetString(Resource.String.personalAlertsActivity_validation_message_enterDescription);
            }
            else if (dateIsSet == false)
            {
                txtDate.Text = GetString(Resource.String.personalAlertsActivity_validation_message_setDate);
                btnSetDate.RequestFocus();
            }
            else if (timeIsSet == false)
            {
                txtTime.Text = GetString(Resource.String.personalAlertsActivity_validation_message_setTime);
                btnSetTime.RequestFocus();
            }
            else
            {
                // validation is successful
                UserAlert userAlert = new UserAlert
                {
                    // don't add ID here - SQLite will do this automatically (auto-increment)
                    Title = editTxtTitle.Text,
                    DescriptionOfPersonalEvent = editTxtDescription.Text,
                    CountryChar = GetString(Resource.String.personalAlertsActivity_personalAlertName),
                    MarketImpact = GetString(Resource.String.personalAlertsActivity_personalAlertName_impact),
                    IsPersonalAlert = true,

                    // set time offset Property 
                    ///////////////////////////
                    DateAndTime = combinedDateTimeObject.AddMinutes(SetUpData.TimeToGoOffBeforeMarketAnnouncement),

                    // convert DateTime object to a ticks (long)
                    //DateInTicks = combinedDateTimeObject.Ticks  // original
                    DateInTicks = combinedDateTimeObject.AddMinutes(SetUpData.TimeToGoOffBeforeMarketAnnouncement).Ticks
                };
                Log.Debug("DEBUG", "\n\n\n" + userAlert.ToString() + "\n\n\n");

                // reset appropriate validation
                dateIsSet = false;
                timeIsSet = false;

                // call Property in UserAlertActivity to pass data across (newsObject)
                UserAlertsActivity.SelectedUserAlert_PassedFrom_PersonalAlertsActivity = userAlert;

                // call intent to start next activity
                Intent intent = new Intent(this, typeof(UserAlertsActivity));
                StartActivity(intent);
            }           
        }



        private void BtnCancelPersonalAlert_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, GetString(Resource.String.personalAlertsActivity_message_alertCancelled), ToastLength.Long).Show();

            Intent intent = new Intent(this, typeof(UserAlertsActivity));  
            StartActivity(intent);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            ResetPersonalAlertData();
        }

        protected override void OnResume()
        {
            base.OnResume();
            ResetPersonalAlertData();
        }

        private void ResetPersonalAlertData()
        {
            combinedDateTimeObject = new DateTime();
            dateIsSet = false;
            timeIsSet = false;
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
                TextView combinedDateTimeTextView = Activity.FindViewById<TextView>(Resource.Id.personalAlertsActivity_txt_combinedDateTime);
                combinedDateTimeTextView.Text = combinedDateTimeObject.ToString();
                timeIsSet = true;
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
                TextView combinedDateTimeTextView = Activity.FindViewById<TextView>(Resource.Id.personalAlertsActivity_txt_combinedDateTime);
                combinedDateTimeTextView.Text = combinedDateTimeObject.ToString();
                dateIsSet = true;
                
            }
        }
        //----------------------------------------------------------------------------------------------------------------------

    }//
}//