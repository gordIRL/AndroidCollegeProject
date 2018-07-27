using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using System.Collections.Generic;


using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using CurrencyAlertApp.DataAccess;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// to pass a XDocument add reference:    System.Xml.Linq.
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;
using System.Reflection;
using Android.Content.Res;
using Android.Util;

namespace CurrencyAlertApp
{
    [Activity(Label = "RecyclerViewer", MainLauncher = true, Icon = "@drawable/icon",
               Theme = "@android:style/Theme.Material.Light.DarkActionBar")]  // MainLauncher = true,

    public class TestRecycleActivity : Activity
    {        
        List<string> DisplayListSTRING = new List<string>();
        List<NewsObject> DisplayListOBJECT = new List<NewsObject>();    
        
        // RecyclerView instance that displays the newsObject List:
        RecyclerView mRecyclerView;

        // Layout manager that lays out each card in the RecyclerView:
        RecyclerView.LayoutManager mLayoutManager;

        // Adapter that accesses the data set (List<newsObject>):
        NewsObject_RecycleAdapter_NEW mAdapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //// Instantiate - taken from 'Get All Data' 
            // clear List & get raw newsObject data from database
            //ClearDisplayListAndAdapter();
            //adapter.Clear();
            DisplayListSTRING.Clear();

            //RefreshTxtDataLastUpdated();  // not using in this experiment

            DisplayListOBJECT.Clear();
            DisplayListOBJECT = SetUpData.GetAllRawDataFromDatabase();

            // convert this newsObject data to a List<string>
            DisplayListSTRING = ConvertObjectsListToStringList(DisplayListOBJECT);
            
            // Set our view from the "main" layout resource:
            SetContentView(Resource.Layout.TestRecycleLayout);

            // Get our RecyclerView layout:
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);      


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
            mAdapter = new NewsObject_RecycleAdapter_NEW(DisplayListOBJECT);

            ////Register the item click handler(below) with the adapter:
            //mAdapter.ItemClick += OnItemClick;
            mAdapter.ItemClick += MAdapter_ItemClick;
            mAdapter.ItemLongClick += MAdapter_ItemLongClick;

            // Plug the adapter into the RecyclerView:
            mRecyclerView.SetAdapter(mAdapter);           
        }// end onCreate

        private void MAdapter_ItemLongClick(object sender, NewsObject_RecycleAdapter_NEWClickEventArgs e)
        {
            int photoNum = e.Position + 1;
            Toast.MakeText(this, "LONGGGGGGGGGGG Click\nThis is photo number " + photoNum, ToastLength.Short).Show();
        }

        private void MAdapter_ItemClick(object sender, NewsObject_RecycleAdapter_NEWClickEventArgs e)
        {
            int photoNum =  e.Position + 1;
            Toast.MakeText(this, "This is photo number " + photoNum, ToastLength.Short).Show();
        }



        public List<string> ConvertObjectsListToStringList(List<NewsObject> newsObjects)
        {
            List<string> stringList = new List<string>();

            foreach (var tempNewsObject in newsObjects)
            {
                // convert individual newsObject to a single string
                string tempStringItem = (string.Format(
                        "{0}:  {1}\nDate: {2}   {3} \n{4}",
                        tempNewsObject.CountryChar.TrimEnd(),
                        tempNewsObject.MarketImpact.TrimEnd(),
                        tempNewsObject.DateAndTime.ToString("dd/MM/yyyy"),
                        tempNewsObject.DateAndTime.ToString("HH:mm tt"),
                        tempNewsObject.Name.TrimEnd()));

                // add string to string list
                stringList.Add(tempStringItem);
            }
            return stringList;
        }
    }// end MainActivity
}// end Namespace