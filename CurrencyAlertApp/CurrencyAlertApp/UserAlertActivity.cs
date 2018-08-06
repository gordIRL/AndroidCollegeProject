using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using CurrencyAlertApp.DataAccess;

namespace CurrencyAlertApp
{
    [Activity(Theme = "@style/MyTheme.Base",   Label = "UserAlertActivity",   MainLauncher = true)]
    //  MainLauncher = true

    public class UserAlertActivity : AppCompatActivity
    {
        //List<UserAlert> userAlertsList = new List<UserAlert>();


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.UserAlert);

           
            //SetUpData.CreateEmptyUserAlertTable();
            SetUpData.PopulateUserAlertTableWithDummyData();


        }// end OnCreate
    }//
}//