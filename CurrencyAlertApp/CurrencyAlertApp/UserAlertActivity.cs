using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Util;
using Android.Views;
using Android.Widget;
using CurrencyAlertApp.DataAccess;
using Android.Support.V7.Widget;

namespace CurrencyAlertApp
{
    [Activity(Theme = "@style/MyTheme.Base",     Label = "UserAlertActivity")]
    //  MainLauncher = true,

    public class UserAlertActivity : AppCompatActivity
    {
        // list(s) used to populate adapter
        List<UserAlert> userAlertDisplayList = new List<UserAlert>();
        List<UserAlert> tempUserAlertDisplayList = new List<UserAlert>();
            
        // Property for object passed from Main Activity
        public static NewsObject SelectedNewsObject_PassedFRom_MainActivity { get; set; }  


        // RecyclerView instance that displays the newsObject List:
        public RecyclerView mRecyclerView;

        // Layout manager that lays out each card in the RecyclerView:
        public RecyclerView.LayoutManager mLayoutManager;

        // Adapter that accesses the data set (List<newsObject>):       
        public UserAlert_RecycleAdapter mAdapter;
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
            mAdapter = new UserAlert_RecycleAdapter(userAlertDisplayList);

            //Register the item click handler(below) with the adapter:           
            mAdapter.ItemClick += MAdapter_ItemClick;

            // Plug the adapter into the RecyclerView:
            mRecyclerView.SetAdapter(mAdapter);

            //-----------------------------------------------------------------------------------

            // ToolBar - Top of Screen  (method 1)
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_Top_UserAlert);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = GetString(Resource.String.TB_Title_Top_UserAlerts);  


            // Toolbar - Bottom of Screen  (method 2)
            var toolbar_bottom = FindViewById<Toolbar>(Resource.Id.toolbar_Bottom_UserAlert);
            toolbar_bottom.Title = GetString(Resource.String.ToolbarBottom_UserAlert_Title);
            toolbar_bottom.InflateMenu(Resource.Menu.bottomMenu_UserActivity);

            toolbar_bottom.MenuItemClick += (sender, e) =>
            {
                switch (e.Item.ItemId)
                {
                    case Resource.Id.bottomMenu_UserAlertActivity_Option_1:
                        Toast.MakeText(this, "Personal Alerts Selected", ToastLength.Short).Show();
                        // call intent to start next activity
                        Intent intent = new Intent(this, typeof(PersonalAlarmsActivity));
                        StartActivity(intent);
                        break;

                    case Resource.Id.bottomMenu_UserAlertActivity_Option_2:
                        Toast.MakeText(this, "Option 2 - default selected", ToastLength.Short).Show();
                        break;
                }
            };




            // Run this code ONLY if:  
            // user selects to add an alert to a newsObject in Main Activity
            if (SelectedNewsObject_PassedFRom_MainActivity != null)
            {
                Toast.MakeText(this, SelectedNewsObject_PassedFRom_MainActivity.ToString(), ToastLength.Long).Show();

                SetUpData.CreateEmptyUserAlertTable();

                UserAlert convertedUserAlert = SetUpData.ConvertNewsObjectToUserAlert(SelectedNewsObject_PassedFRom_MainActivity);
                Log.Debug("DEBUG", convertedUserAlert.ToString());


                // need ID of UserAlert from DB at this mmoment for creating alarm code
                int userID_fromDB = SetUpData.AddNewUserAlertToDatabase(convertedUserAlert);

                Log.Debug("DEBUG", "UserAlertActivity says - new UserID from DB: " +userID_fromDB +"\n\n");
                Log.Debug("DEBUG", "FINISHED\n\n\n");

                // CALL METHOD HERE.....    SET_ALARM()
            }



            // call populate adapter
            PopulateUserAlertAdapter();
            //RefreshTxtDataLastUpdated();

        }// end OnCreate -----------------------------------------------------------




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
            builder.SetMessage("Delete this User Alert ?");  // usisng this disable array of menu options - good for Ok/Cancel version

            builder.SetPositiveButton("OK", (sender2, e2) =>
            {
                // displal ID of deleted userAlert
                Toast.MakeText(this, userAlertDisplayList[e].UserAlertID.ToString(), ToastLength.Long).Show();

                // call method to delete UserAlert from database (doesn't include AlarmManager !!!!!!)
                int rowCount = SetUpData.DeleteSelectedUserAlert(userAlertDisplayList[e].UserAlertID);

                Toast.MakeText(this, "No of rows deleted: " + rowCount, ToastLength.Long).Show();
                PopulateUserAlertAdapter();
            });


            builder.SetNegativeButton("Cancel", (sender2, e2) =>
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
            //    //Toast.MakeText(this, $"You selected item no: {e}:\n" + DisplayListOBJECT[e].ToString(), ToastLength.Long).Show();

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
            MenuInflater.Inflate(Resource.Menu.topMenu_UserAlertActivity, menu);
            return base.OnCreateOptionsMenu(menu);
        }


        // TOP Toolbar ('menu options')
        public override bool OnOptionsItemSelected(IMenuItem item) // 
        {
            switch (item.ItemId)
            {
                case Resource.Id.topMenu_UserAlertActivity_MarketData:
                    Toast.MakeText(this, "Action selected: \nMarket Data", ToastLength.Short).Show();
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);

                    break;

                case Resource.Id.topMenu_UserAlertActivity_Alerts:
                    Toast.MakeText(this, "Action selected: \nSet Alert", ToastLength.Short).Show();
                    intent = new Intent(this, typeof(PersonalAlarmsActivity));
                    StartActivity(intent);
                    break;              

                case Resource.Id.topMenu_UserAlertActivity_Preferences:
                    Toast.MakeText(this, "Action selected: \nPreferences - test Activity", ToastLength.Short).Show();
                    intent = new Intent(this, typeof(NewsObject_CustomAdapter_Test_Activity));
                    StartActivity(intent);
                    break;

                default:
                    break;
            };
            return base.OnOptionsItemSelected(item);
        }
        //-----------------------------------------------------------------------------------

      

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