using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Content;
using Android.Views;
using System.Collections.Generic;
using CurrencyAlertApp.DataAccess;

using System;

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
    [Activity(Theme = "@style/MyTheme.Base", MainLauncher = true, Label = "CurrencyAlertApp")]
    //  MainLauncher = true,  
    // must have an appCompat theme         

    public class MainActivity : AppCompatActivity  
    {
        Button MyButton;
        ListView listView1;
        List<string> myList = new List<string>();
        ArrayAdapter adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Button to call next activity
            MyButton = FindViewById<Button>(Resource.Id.MyButton);
            MyButton.Click += delegate
            {
                Intent intent = new Intent(this, typeof(GordTestActivity));
                StartActivity(intent);
            };


            // select xml file to read
            XDocument xmlFile = XDocument.Load(Assets.Open("ff_calendar_thisweek.xml"));

            // populate list with default data (numbers) - list<string>
            myList = SetUpData.DefaultDataSetUp_ListOfNumbers();

            // wireup listView
            listView1 = FindViewById<ListView>(Resource.Id.listView1);

            // set up adapter for listView
            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, myList);
            listView1.Adapter = adapter;

            // ItemClick for listView
            listView1.ItemClick += (sender, e) =>
            {
                Toast.MakeText(this, $"You selected item no: {e.Position}:\n{myList[e.Position]}", ToastLength.Short).Show();

                // alert dialog for ItenClick event
                Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
               // builder.SetMessage("Hi there!");  // usisng this disable array of menu options - good for Ok/Cancel version
                builder.SetTitle("Choose one:");

                //var items = new string[] { "piza", "pasta", "diet coke" };
                //builder.SetItems(items, (sender2, e2) =>

                
                builder.SetItems(Resource.Array.itemSelect_AddToWatchList, (sender2, e2) =>
                {
                    var index = e2.Which;
                    Log.Debug("DEBUG", index.ToString());
                    Log.Debug("DEBUG", e2.Which.ToString());

                    switch (e2.Which)
                    {
                        case 0:
                            break;
                        case 1:
                            // call method in next activity to pass data across via a string - this will be an object at a later date
                            GordTestActivity.MethodToGetString(myList[e.Position]);
                            //GordTestActivity.MethodToGetString("latest test");

                            // call intent to start next activity
                            Intent intent = new Intent(this, typeof(GordTestActivity));
                            StartActivity(intent);
                            break;
                        case 2:
                            break;
                        default:
                            break;
                    };


                    //Log.Debug("DEBUG", items[index]);
                });


                //// positive 1st - so it is on the left of the screen, where user is used to seeing it!
                //builder.SetPositiveButton("OK", (sender2, e2) => {
                //    Log.Debug("dbg", "OK clicked");
                //});

                builder.SetNegativeButton("Cancel", (sender2, e2) =>
                {
                    Log.Debug("dbg", "Cancel clicked");
                });

                // builder.SetNeutralButton.........

                var alert = builder.Create();
                alert.Show();
            };


            // ToolBar - Top of Screen
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "My AppCompat Toolbar";

            // Toolbar - Bottom of Screen
            var editToolbar = FindViewById<Toolbar>(Resource.Id.edit_toolbar);
            editToolbar.Title = "Editing";
            editToolbar.InflateMenu(Resource.Menu.edit_menus);

            // bottom ToolBar Menu Selection
            editToolbar.MenuItemClick += (sender, e) =>
            {
                switch (e.Item.ItemId)         
                {
                    case Resource.Id.menu_data_default:
                        // display user selection info
                        Toast.MakeText(this, "Bottom toolbar / Default:\nID: " + e.Item.ItemId + "\nTitle: " + e.Item.TitleFormatted, ToastLength.Short).Show();

                        // clear adapter & clear list
                        ClearListAndAdapter();

                        // populate list by calling method with XDocument xmlFile - declared above
                        myList = SetUpData.DefaultDataSetUp_ListOfNumbers();

                        // re-populate adapter by running a 'forEach' through the list
                        RepopulateAdapter();
                        //adapter.NotifyDataSetChanged();
                        break;

                    case Resource.Id.menu_data_raw:
                        // display user selection info
                        Toast.MakeText(this, "Bottom toolbar / RAW:\nID: " + e.Item.ItemId + "\nTitle: " + e.Item.TitleFormatted, ToastLength.Short).Show();

                        // clear adapter & clear list
                        ClearListAndAdapter();                       

                        // populate list with ALL raw data - by calling method with XDocument xmlFile - declared above
                        myList = SetUpData.GetAllRawData(xmlFile);

                        // re-populate adapter by running a 'forEach' through the list
                        RepopulateAdapter();
                        //adapter.NotifyDataSetChanged();
                        break;       
                        
                        
                    case Resource.Id.menu_data_formatted:
                        // display user selection info
                        Toast.MakeText(this, "Bottom toolbar / RAW:\nID: " + e.Item.ItemId + "\nTitle: " + e.Item.TitleFormatted, ToastLength.Short).Show();

                        // clear adapter & clear list
                        ClearListAndAdapter();

                        // populate list with ALL data - formatted - by calling method with XDocument xmlFile - declared above
                        myList = SetUpData.GetAllDataFormatted(xmlFile);

                        // re-populate adapter by running a 'forEach' through the list
                        RepopulateAdapter();
                        //adapter.NotifyDataSetChanged();
                        break;


                    case Resource.Id.menu_data_LINQ_query:
                        // display user selection info
                        Toast.MakeText(this, "Bottom toolbar / LINQ Query:\nID: " + e.Item.ItemId + "\nTitle:" + e.Item.TitleFormatted, ToastLength.Short).Show();

                        // clear adapter & clear list
                        ClearListAndAdapter();


                        // populate list with LINQ query result - by calling method with XDocument xmlFile - declared above
                        myList = SetUpData.GetLINQResultData(xmlFile);

                        // re-populate adapter by running a 'forEach' through the list
                        RepopulateAdapter();
                        //adapter.NotifyDataSetChanged();
                        break;

                    default:
                        break;
                }
            };                    
        }// end onCreate


        public override bool OnCreateOptionsMenu(IMenu menu)  // for top toolbar
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }


        public override bool OnOptionsItemSelected(IMenuItem item) // for top toolbar
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted, ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
        }

       
        void ClearListAndAdapter()
        {
            adapter.Clear();
            myList.Clear();
        }

        void RepopulateAdapter()
        {
            // re-populate adapter by running a 'forEach' through the list
            foreach (var item in myList)
            {
                adapter.Add(item);
            }
            //adapter.NotifyDataSetChanged();
        }        
    }
}

