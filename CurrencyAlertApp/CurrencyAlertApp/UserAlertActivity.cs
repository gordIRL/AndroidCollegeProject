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
        // list used to populate adapter
        List<UserAlert> userAlertDisplayList = new List<UserAlert>();

        // object passed from Main Activity
        public static NewsObject selectedNewsObjectFromMainActivity;
        
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

            // Dummy Data
            userAlertDisplayList = SetUpData.DummyDataForUserAlert();


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
            // data set (List<newsobject) to manage:
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

            // display passed object from Main Activity
            if (selectedNewsObjectFromMainActivity != null)
            {
                Toast.MakeText(this, selectedNewsObjectFromMainActivity.ToString(), ToastLength.Long).Show();
            }

            //////////////////SetUpData.ConvertNewsObjectToUserAlert(selectedNewsObjectFromMainActivity);
            //////////////////SetUpData.AddNewUserAlertToDatabase();


        }// end OnCreate -----------------------------------------------------------



        private void MAdapter_ItemClick(object sender, int e)
        {
            // alert dialog for ItenClick event
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            // builder.SetMessage("Hi there!");  // usisng this disable array of menu options - good for Ok/Cancel version
            builder.SetTitle("Choose one:");

            builder.SetItems(Resource.Array.itemSelect_AddToWatchList, (sender2, e2) =>
            {
                var index = e2.Which;
                Log.Debug("DEBUG", index.ToString());
                Log.Debug("DEBUG", e2.Which.ToString());
                Toast.MakeText(this, $"You selected item no: {e}:\n", ToastLength.Long).Show();
                //Toast.MakeText(this, $"You selected item no: {e}:\n" + DisplayListOBJECT[e].ToString(), ToastLength.Long).Show();

                switch (e2.Which)
                {
                    case 0:
                        break;
                    case 1:                          
                        // call intent to start next activity
                        Intent intent = new Intent(this, typeof(PersonalAlarmsActivity));
                        StartActivity(intent);
                        break;
                    case 2:
                        break;
                    default:
                        break;
                };
            });

            builder.SetNegativeButton("Cancel", (sender2, e2) =>
            {
                Log.Debug("dbg", "Cancel clicked");
            });
            // builder.SetNeutralButton.........

            var alert = builder.Create();
            alert.Show();
        }



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

        public static void MethodToPassObject(NewsObject selectedNewsObjectInput)
        {
            selectedNewsObjectFromMainActivity = selectedNewsObjectInput;
        }
    }//
}//