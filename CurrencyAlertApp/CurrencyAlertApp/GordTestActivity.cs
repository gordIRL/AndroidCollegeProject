using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using CurrencyAlertApp.DataAccess;
using Android.Util;
using System.Globalization;

namespace CurrencyAlertApp
{
    [Activity(Label = "GordTestActivity", Theme = "@style/MyTheme.Base")]
    // MainLauncher = true,  //   Theme = "@style/MyTheme.Test"
    public class GordTestActivity : AppCompatActivity
    {
        public static string myResultMain = string.Empty;
        List<NewsObject> DisplayListOBJECT = new List<NewsObject>();

        // Create a single CultureInfo object (once so it can be reused) for correct Parsing of strings to DateTime object
        static CultureInfo cultureInfo = new CultureInfo("en-US");


        protected override void OnCreate(Bundle savedInstanceState)
        {            
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.gordTestLayout);
            
            Toast.MakeText(this, "Hello and welcome to TestActivity!\n" + myResultMain, ToastLength.Long).Show();

            DisplayListOBJECT = SetUpData.GetAllRawDataFromDatabase();

           

            var newsObjectListView = FindViewById<ListView>(Resource.Id.listViewTestActivityCurrency);                       

            newsObjectListView.Adapter = new NewsObjectAdapter(this, DisplayListOBJECT);            
            newsObjectListView.ItemClick += NewsObjectListView_ItemClick;
            //---------------------------------------------------------------------------------------------


            string dateString = "04-15-2018";
            string timeString = "10:45am";
            DateTime dateTime =  SetUpData.ConvertString_s_ToDateTimeObject(dateString, timeString, cultureInfo);

            Log.Debug("DEBUG", "DateTime from my method: " + dateTime.ToString("dd/MM/yyyy HH:mm:ss"));
            long ticksTime_1 = dateTime.Ticks;
            Log.Debug("DEBUG", "No of ticks: " + ticksTime_1);


            string dateString2 = "04-15-2018";
            string timeString2 = "10:45pm";
            DateTime dateTime2 = SetUpData.ConvertString_s_ToDateTimeObject(dateString2, timeString2, cultureInfo);

            Log.Debug("DEBUG", "DateTime from my method: " + dateTime2.ToString("dd/MM/yyyy HH:mm:ss"));
            long ticksTime_2 = dateTime2.Ticks;
            Log.Debug("DEBUG", "No of ticks: " + ticksTime_2);

            long timeDifference = ticksTime_2 - ticksTime_1;
            Log.Debug("DEBUG", "Time difference in ticks: " + timeDifference);

            TimeSpan elapsedSpan = new TimeSpan(timeDifference);
            Log.Debug("DEBUG", "Elapsed time: " + elapsedSpan.TotalHours);



            DateTime dateTime3 = new DateTime(2018, 03, 19, 23, 59, 00);
            long myNum = dateTime3.Ticks;
            Log.Debug("DEBUG", "No of ticks: " + myNum);
            DateTime dateTime4 = new DateTime(myNum);
            Log.Debug("DEBUG", "Re-converted dateTime: " + dateTime4.ToString("dd/MM/yyyy HH:mm:ss")); 
                    // CultureInfo.InvariantCulture
                    // tt - gives am/pm
                    // hh - 12 hour clock
                    // HH = 24 hour clock           

        }// end OnCreate














        private void NewsObjectListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Toast.MakeText(this, "Selected : " + DisplayListOBJECT[e.Position].Name, ToastLength.Short).Show();
        }

       
        public static void MethodToGetString(string myResultInput)
        {
            myResultMain = myResultInput;
        }

       
    }
}