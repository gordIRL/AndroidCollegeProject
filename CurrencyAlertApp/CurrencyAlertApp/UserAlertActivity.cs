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

namespace CurrencyAlertApp
{
    [Activity(Theme = "@style/MyTheme.Base",   Label = "UserAlertActivity")]
    //  MainLauncher = true

    public class UserAlertActivity : AppCompatActivity
    {
        //List<UserAlert> userAlertsList = new List<UserAlert>();


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.UserAlert);

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

            //SetUpData.CreateEmptyUserAlertTable();
            SetUpData.PopulateUserAlertTableWithDummyData();


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
                    intent = new Intent(this, typeof(CustomAdapter_Test_Activity));
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