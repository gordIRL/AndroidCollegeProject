using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Util;
using Android.Views;
using System.Threading.Tasks;
using CurrencyAlertApp.DataAccess;

//using System.Linq;
//using System.Text;
//using Android.Runtime;

namespace CurrencyAlertApp
{
    [Activity(Theme = "@style/MyTheme.Base",     Label = "UserAlertActivity")]
    //  MainLauncher = true,

    public class UserAlertsActivity : AppCompatActivity
    {
        // list(s) used to populate adapter
        List<UserAlert> userAlertDisplayList = new List<UserAlert>();
        List<UserAlert> tempUserAlertDisplayList = new List<UserAlert>();
            
        // Property for NewsObject passed from Main Activity
        public static NewsObject SelectedNewsObject_PassedFrom_MainActivity { get; set; }

        // Property for UserAlert object passed from PersonalAlerts Activity
        public static UserAlert SelectedUserAlert_PassedFrom_PersonalAlertsActivity { get; set; }


        // RecyclerView instance that displays the newsObject List:
        public RecyclerView mRecyclerView;

        // Layout manager that lays out each card in the RecyclerView:
        public RecyclerView.LayoutManager mLayoutManager;

        // Adapter that accesses the data set (List<newsObject>):       
        public UserAlerts_RecycleAdapter mAdapter;
        //public NewsObject_RecycleAdapter mAdapter;


     

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.UserAlert);

            //// Dummy Data
            //userAlertDisplayList = SetUpData.DummyDataForUserAlert();


            // Get our RecyclerView layout:
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView_UserAlert);

            //............................................................
            // Layout Manager Setup:

            // Use the built-in linear layout manager:
            mLayoutManager = new LinearLayoutManager(this);

            // Or use the built-in grid layout manager (two horizontal rows):
            //mLayoutManager = new GridLayoutManager
            //       (this, 2, GridLayoutManager.Horizontal, false);

            // Plug the layout manager into the RecyclerView:
            mRecyclerView.SetLayoutManager(mLayoutManager);

            //............................................................
            // Adapter Setup:

            // Create an adapter for the RecyclerView, and pass it the
            // data set (List<userAlert>) to manage:
            mAdapter = new UserAlerts_RecycleAdapter(userAlertDisplayList);

            //Register the item click handler(below) with the adapter:           
            mAdapter.ItemClick += MAdapter_ItemClick;

            // Plug the adapter into the RecyclerView:
            mRecyclerView.SetAdapter(mAdapter);

            //-----------------------------------------------------------------------------------

            // ToolBar - Top of Screen  (method 1)
            var toolbar = FindViewById<Toolbar>(Resource.Id.userAlertsActivity_top_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = GetString(Resource.String.userAlertsActivity_top_toolbar_title);  


            // Toolbar - Bottom of Screen  (method 2)
            var toolbar_bottom = FindViewById<Toolbar>(Resource.Id.userAlertsActivity_bottom_toolbar);
            toolbar_bottom.Title = GetString(Resource.String.userAlertsActivity_bottom_toolbar_title);
            toolbar_bottom.InflateMenu(Resource.Menu.userAlertsActivity_bottomMenu);

            toolbar_bottom.MenuItemClick += (sender, e) =>
            {
                switch (e.Item.ItemId)
                {
                    case Resource.Id.userAlertsActivity_bottom_toolbar_option_personalAlerts:                        
                        // call intent to start next activity
                        Intent intent = new Intent(this, typeof(PersonalAlertsActivity));
                        StartActivity(intent);
                        break;
                }
            };




            // This code ONLY runs if - user selects to add an alert to a newsObject in Main Activity
            if (SelectedNewsObject_PassedFrom_MainActivity != null)
            {
                Log.Debug("DEBUG", "\n\n\n" + SelectedNewsObject_PassedFrom_MainActivity.ToString() + "\n\n\n");

                // avoid null error if UserAlertTable doesn't exist - won't overwrite if it does
                SetUpData.CreateEmptyUserAlertTable();

                UserAlert convertedUserAlert = SetUpData.ConvertNewsObjectToUserAlert(SelectedNewsObject_PassedFrom_MainActivity);
                Log.Debug("DEBUG", convertedUserAlert.ToString());

                // store UserAlert in database & get its ID number - 
                // need ID of UserAlert from DB at this mmoment for creating alarm code
                int userID_fromDB = SetUpData.AddNewUserAlertToDatabase(convertedUserAlert);

                Log.Debug("DEBUG", "UserAlertActivity says - new UserID from DB: " +userID_fromDB +"\n\n");
                Log.Debug("DEBUG", "FINISHED\n\n\n");

                // call SetAlarm() here .....   
                SetAlarm(convertedUserAlert);
                Log.Debug("DEBUG", "\n\n\npause for breakpoint\n\n\n");
            }




            // This code ONLY runs if - user selects to add a Personal Alert in PersonalAlerts Activity
            if (SelectedUserAlert_PassedFrom_PersonalAlertsActivity != null)
            {
                Log.Debug("DEBUG", "\n\n\n" + SelectedUserAlert_PassedFrom_PersonalAlertsActivity.ToString() + "\n\n\n");

                // avoid null error if UserAlertTable doesn't exist - won't overwrite if it does
                SetUpData.CreateEmptyUserAlertTable();

                // before calling AddNewUserAlertToDatabase() make a copy of UserAlert as its corresponding 
                // property will be set to null - use copy for SetAlarm()
                UserAlert tempUserAlertToSetAlarmWith = SelectedUserAlert_PassedFrom_PersonalAlertsActivity;

                // store UserAlert in database & get its ID number - 
                // need ID of UserAlert from DB at this mmoment for creating alarm code
                int userID_fromDB = SetUpData.AddNewUserAlertToDatabase(SelectedUserAlert_PassedFrom_PersonalAlertsActivity);
                Log.Debug("DEBUG", "\n\n\nUserAlertActivity says - new UserID from DB: " + userID_fromDB + "\n\n\n");

                // // call SetAlarm() here .....   
                SetAlarm(tempUserAlertToSetAlarmWith);
                Log.Debug("DEBUG", "\n\n\npause for breakpoint\n\n\n");
            }

            PopulateUserAlertAdapter();
        }// end OnCreate 



        void DeleteAlarm(UserAlert userAlert)
        {
            // start a new thread - so as not to run on the UI thread - keep the UI thread responsive
            Task.Factory.StartNew(() =>
            {
                try
                {
                    int alarmNo_takenFrom_UserAlertID = userAlert.UserAlertID;

                    Intent intent = new Intent(this, typeof(Receiver1));
                    //  CONTEXT,PRIVATE REQUEST CODE, INTENT, FLAG
                    PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, alarmNo_takenFrom_UserAlertID, intent, 0);
                    AlarmManager alarmManager = (AlarmManager)GetSystemService(AlarmService);

                    alarmManager.Cancel(pendingIntent);
                    //alarmManager.Set(AlarmType.RtcWakeup, ringTime, pendingIntent);

                    Log.Debug("DEBUG", "\n\n\nAlarm deleted from Alarm Manager - ID: " + alarmNo_takenFrom_UserAlertID);
                }
                catch
                {
                    Log.Debug("DEBUG", "Alarm NOT deleted from Alarm Manager\n\n\n");
                }               
                Log.Debug("DEBUG", "FINISHED\n\n\n");
            }); // end of thread
        }





        void SetAlarm(UserAlert userAlert)
        {
            // start a new thread - so as not to run on the UI thread - keep the UI thread responsive
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // use UserAlert ID (assigned by SQLite) as unique the alarm number for this alarm
                    int alarmNumber = userAlert.UserAlertID;

                    // get no of milliseconds from datetime object
                    long MillisesondsOfUserAlertDateTime = new DateTimeOffset(userAlert.DateAndTime).ToUnixTimeMilliseconds();

                    // set alarm
                    var ringTime = MillisesondsOfUserAlertDateTime;
                    Intent intent = new Intent(this, typeof(Receiver1));

                    //  CONTEXT,PRIVATE REQUEST CODE, INTENT, FLAG
                    PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, alarmNumber, intent, 0);

                    AlarmManager alarmManager = (AlarmManager)GetSystemService(AlarmService);
                    alarmManager.Set(AlarmType.RtcWakeup, ringTime, pendingIntent);

                    Log.Debug("DEBUG", "\n\n\n\nAlarm set by Alarm Manager - ID: " + alarmNumber.ToString());
                }
                catch
                {
                    Log.Debug("DEBUG", "Alarm NOT set by Alarm Manager\n\n\n\n");
                }              
                Log.Debug("DEBUG", "End Of SetAlarm()\n\n\n\n");
            }); // end of thread

            //// display output for testing - still works
            //Log.Debug("DEBUG", "\n\nSetAlarm()  - UserAlert ID:  " + userAlert.UserAlertID.ToString());
            //Log.Debug("DEBUG", "\n\nSetAlarm()  - UserAlert ToString:\n" + userAlert.ToString());
            //Log.Debug("DEBUG", "\n\nSetAlarm()  - UserAlert DateTimeObject:\n" + userAlert.DateAndTime.ToString("dd/MM/yyyy"));
            //Log.Debug("DEBUG", "SetAlarm()  - UserAlert DateTimeObject:\n" + userAlert.DateAndTime.ToString("HH:mmtt"));
            //Log.Debug("DEBUG", "FINISHED\n\n\n");
        }




        protected override void OnResume()
        {
            base.OnResume();
            PopulateUserAlertAdapter();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            PopulateUserAlertAdapter();
        }




        private void MAdapter_ItemClick(object sender, int e)
        {
            // alert dialog for ItemClick event
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            // using  builder.SetMessage()   disables array of menu options - good for Ok/Cancel version
            
            builder.SetMessage(GetString(Resource.String.userAlertsActivity_dialogOption_deleteUserAlert_message));            
            builder.SetPositiveButton(GetString(Resource.String.userAlertsActivity_dialogOption_deleteUserAlert_positiveButton),
                (sender2, e2) =>
            {
                // display ID of deleted userAlert
                Log.Debug("DEBUG", "\n\n\nUserAlert ID: " + userAlertDisplayList[e].UserAlertID.ToString() + "\n\n\n");
                //Toast.MakeText(this, userAlertDisplayList[e].UserAlertID.ToString(), ToastLength.Long).Show();

                // call method to delete UserAlert from database (doesn't include AlarmManager !!!!!!)
                int rowCount = SetUpData.DeleteSelectedUserAlert(userAlertDisplayList[e].UserAlertID);

                Log.Debug("dbg", "\n\n\nNo of rows deleted: " + rowCount + "\n\n\n");
                
                // delete alarm via deleting the associated pending intent 
                // pass in currently selected userAlert
                DeleteAlarm(userAlertDisplayList[e]);   

                PopulateUserAlertAdapter();
            });


            builder.SetNegativeButton(GetString(Resource.String.userAlertsActivity_dialogOption_deleteUserAlert_negativeButton),
                (sender2, e2) =>
            {
                Log.Debug("dbg", "Cancel clicked");
            });
            // builder.SetNeutralButton.........

            var alert = builder.Create();
            alert.Show();
        }

            //// Multiple options version
            //builder.SetTitle("Choose one:");
            //builder.SetItems(Resource.Array.itemSelect_AddToWatchList, (sender2, e2) =>
            //{
            //    var index = e2.Which;
            //    Log.Debug("DEBUG", index.ToString());
            //    Log.Debug("DEBUG", e2.Which.ToString());
            //    Toast.MakeText(this, $"You selected item no: {e}:\n", ToastLength.Long).Show();
            //    //Toast.MakeText(this, $"You selected item no: {e}:\n" 
            //         + DisplayListOBJECT[e].ToString(), ToastLength.Long).Show();

            //    switch (e2.Which)
            //    {
            //        case 0:
            //            break;
            //        case 1:                          
            //            // call intent to start next activity
            //            Intent intent = new Intent(this, typeof(PersonalAlarmsActivity));
            //            StartActivity(intent);
            //            break;
            //        case 2:
            //            break;
            //        default:
            //            break;
            //    };
            //});           
        



        // TOP Toolbar
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.userAlertsActivity_topMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }


        // TOP Toolbar ('menu options')
        public override bool OnOptionsItemSelected(IMenuItem item) // 
        {
            switch (item.ItemId)
            {
                case Resource.Id.userAlertsActivity_top_toolbar_menu_MainActivity:                    
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                    break;

                default:
                    break;
            };
            return base.OnOptionsItemSelected(item);
        }
      

        void PopulateUserAlertAdapter()
        {
            // clear List & get all userAlert(s) data from database  
            tempUserAlertDisplayList.Clear();
            tempUserAlertDisplayList = SetUpData.GetAllUserAlertDataFromDatabase();

            // Refresh adapter by running a 'forEach' through tempList to 
            // repopulate the same DisplayListObect that adapter has memory reference to  
            userAlertDisplayList.Clear();
            foreach (var item in tempUserAlertDisplayList)
            {
                userAlertDisplayList.Add(item);
            }
            mAdapter.NotifyDataSetChanged();
        }


    }//
}//