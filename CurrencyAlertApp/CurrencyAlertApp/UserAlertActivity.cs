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
    [Activity(Theme = "@style/MyTheme.Base", MainLauncher = true,    Label = "UserAlertActivity")]
    //  MainLauncher = true,

    public class UserAlertActivity : AppCompatActivity
    {
        List<UserAlert> DisplayListOBJECT = new List<UserAlert>();


        // RecyclerView instance that displays the newsObject List:
        public RecyclerView mRecyclerView;

        // Layout manager that lays out each card in the RecyclerView:
        public RecyclerView.LayoutManager mLayoutManager;

        // Adapter that accesses the data set (List<newsObject>):       
        public UserAlert_RecycleAdapter mAdapter;
        //public NewsObject_RecycleAdapter mAdapter;


        //List<UserAlert> userAlertsList = new List<UserAlert>();


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.UserAlert);


            //SetUpData.CreateEmptyUserAlertTable();
            DisplayListOBJECT =  SetUpData.DummyDataForUserAlert();




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
            mAdapter = new UserAlert_RecycleAdapter(DisplayListOBJECT);

            ////////////////////////////Register the item click handler(below) with the adapter:           
            //////////////////////////mAdapter.ItemClick += MAdapter_ItemClick1;

            // Plug the adapter into the RecyclerView:
            mRecyclerView.SetAdapter(mAdapter);

            //-----------------------------------------------------------------------------------

            // ToolBar - Top of Screen  (method 1)
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_Top_UserAlert);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = GetString(Resource.String.ToolbarTopTitle);


            // Toolbar - Bottom of Screen  (method 2)
            var toolbar_bottom = FindViewById<Toolbar>(Resource.Id.toolbar_Bottom_UserAlert);
            toolbar_bottom.Title = GetString(Resource.String.ToolbarBottom_UserAlert_Title);
            toolbar_bottom.InflateMenu(Resource.Menu.bottomMenu_UserActivity);

            toolbar_bottom.MenuItemClick += (sender, e) =>
            {
                switch (e.Item.ItemId)
                {
                    case Resource.Id.bottomMenu_UserAlertActivity_Option_1:
                        Toast.MakeText(this, "Option 1 - default selected", ToastLength.Short).Show();
                        break;
                        
                    case Resource.Id.bottomMenu_UserAlertActivity_Option_2:
                        Toast.MakeText(this, "Option 2 - default selected", ToastLength.Short).Show();
                    break;
                }
            };

     


        }// end OnCreate -----------------------------------------------------------



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
    }//
}//