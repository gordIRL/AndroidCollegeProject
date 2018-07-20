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

namespace CurrencyAlertApp
{
    [Activity(Label = "GordTestActivity",  Theme = "@style/MyTheme.Test")]
    // MainLauncher = true,
    public class GordTestActivity : AppCompatActivity
    {
        public static string myResultMain = string.Empty;
        List<NewsObject> DisplayListOBJECT = new List<NewsObject>();


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