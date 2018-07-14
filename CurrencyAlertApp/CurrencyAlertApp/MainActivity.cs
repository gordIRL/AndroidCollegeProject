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
    [Activity(Theme = "@style/MyTheme.Base", MainLauncher = true,   Label = "CurrencyAlertApp")]
    //  MainLauncher = true,  
    // must have an appCompat theme         

    public class MainActivity : AppCompatActivity  
    {
        Button MyButton;
        ListView listView1;
        TextView txtDataLastUpdated;
        List<string> myList = new List<string>();
        List<NewsObject> newsObjectsList = new List<NewsObject>();
        ArrayAdapter adapter;

       

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Display for last time data was updated - to be linked to XML download date.....
            txtDataLastUpdated = FindViewById<TextView>(Resource.Id.txtDataLastUpdated);
            txtDataLastUpdated.Text += "  "  + DateTime.Now.ToShortDateString();
            

            // Button to call next activity
            MyButton = FindViewById<Button>(Resource.Id.MyButton);
            //MyButton.Text =  GetString(Resource.String.Button_Activity2Test);  // not in use
            MyButton.Click += delegate
            {
                Intent intent = new Intent(this, typeof(GordTestActivity));
                StartActivity(intent);
            };





            // populate list with default data (numbers) - list<string>
            myList = SetUpData.NoDataToDisplay();

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
                });               

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
            SupportActionBar.Title = GetString(Resource.String.ToolbarTopTitle);

            // Toolbar - Bottom of Screen
            var editToolbar = FindViewById<Toolbar>(Resource.Id.edit_toolbar);
            editToolbar.Title = GetString(Resource.String.ToolbarBottomTitle);
            editToolbar.InflateMenu(Resource.Menu.edit_menus);

            // bottom ToolBar Menu Selection
            editToolbar.MenuItemClick += (sender, e) =>
            {
                switch (e.Item.ItemId)         
                {
                    case Resource.Id.menu_data_default:
                        // display user selection info
                        Toast.MakeText(this, "Bottom toolbar / Default!!!!!!:\nID: " + e.Item.ItemId + "\nTitle: " + e.Item.TitleFormatted, ToastLength.Short).Show();

                        List<string> selectedCurrencyList = new List<string>();

                        // clear user's selection to begin
                        selectedCurrencyList.Clear();

                        // list of items for user to select from
                        string[] items = Resources.GetStringArray(Resource.Array.CurrenciesAndMarketImpactArray);

                        // bool array for selected checkboxes in MultiItemSelect 
                        bool[] selected = new bool[items.Length];



                        using (var dialog = new Android.Support.V7.App.AlertDialog.Builder(this))
                        {
                            dialog.SetTitle("Alert Title");
                            dialog.SetPositiveButton("Close", delegate {
                            });

                            // Set Multichoice Items
                            dialog.SetMultiChoiceItems(items, selected,
                               (s, eEXtra) => {
                                   int index = eEXtra.Which;
                                   bool isChecked = eEXtra.IsChecked;
                                   selected[index] = isChecked;
                                   Toast.MakeText(this, "You clicked: " + items[index]
                                       + "\nChecked: " + eEXtra.IsChecked, ToastLength.Short).Show();

                                    // add item to list if now selected - ie isChecked is now TRUE  
                                    if (isChecked)
                                       selectedCurrencyList.Add(items[index]);
                                   else
                                       selectedCurrencyList.Remove(items[index]);
                               });



                            // check all boxes and add all items to list(s)
                            dialog.SetNeutralButton("ALL", delegate
                            {
                                // clear list 1st to avoid getting duplicate entries
                                selectedCurrencyList.Clear();

                                // set all items in bool[] selected to TRUE
                                for (int i = 0; i < selected.Length; i++)
                                {
                                    selected[i] = true;
                                    selectedCurrencyList.Add(items[i]);
                                }
                            });

                            // deselect all boxes & clear list
                            dialog.SetNegativeButton("Clear", delegate
                            {
                                selectedCurrencyList.Clear();
                            });
                            
                            dialog.Show();
                        }// end Using
                        break;









                    case Resource.Id.menu_test_data:
                        // display user selection info
                        Toast.MakeText(this, "Bottom toolbar / Default:\nID: " + e.Item.ItemId + "\nTitle: " + e.Item.TitleFormatted, ToastLength.Short).Show();

                        // clear adapter & clear list
                        ClearListAndAdapter();

                        // populate list by calling method with XDocument xmlFile - declared above
                        myList = SetUpData.NoDataToDisplay();

                        // re-populate adapter by running a 'forEach' through the list
                        RepopulateAdapter();
                        //adapter.NotifyDataSetChanged();
                        break;


                    case Resource.Id.menu_data_raw:
                        // display user selection info
                        Toast.MakeText(this, "Bottom toolbar / RAW:\nID: " + e.Item.ItemId + "\nTitle: " + e.Item.TitleFormatted, ToastLength.Short).Show();

                        // clear adapter & clear list
                        //ClearListAndAdapter();
                        adapter.Clear();
                        newsObjectsList.Clear();

                        // populate list with ALL raw data
                        newsObjectsList = SetUpData.GetAllRawDataFromDatabase();

                        // re-populate adapter by running a 'forEach' through the list
                        //RepopulateAdapter();
                        foreach (var item in newsObjectsList)
                        {
                            // uses the ToString() method in class NewsObject
                            adapter.Add(item.ToString());
                        }
                        //adapter.NotifyDataSetChanged();
                        break;
                    
                    case Resource.Id.menu_data_formatted:
                        // display user selection info
                        Toast.MakeText(this, "Bottom toolbar / RAW:\nID: " + e.Item.ItemId + "\nTitle: " + e.Item.TitleFormatted, ToastLength.Short).Show();

                        // clear adapter & clear list
                        ClearListAndAdapter();

                        // populate list with ALL data - formatted 
                        myList = SetUpData.GetAllDataFormattedIntoSingleString();

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
                        myList = SetUpData.GetLINQResultData();  // GetLINQResultData2

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
            switch (item.ItemId)
            {
                case Resource.Id.menu_top_marketData:
                    Toast.MakeText(this, "Action selected: \nMarket Data", ToastLength.Short).Show();
                    DataAccess.SetUpData.DropNewsObjectTable();

                    bool dataUpdateSuccessful = DataAccess.SetUpData.DownloadNewXMLAndStoreInDatabase();
                    Toast.MakeText(this, "data update: " + dataUpdateSuccessful, ToastLength.Short).Show();

                    ///////// REFACTOR THIS ????
                    // clear adapter & clear list
                    ClearListAndAdapter();
                    // populate list with ALL data - formatted 
                    myList = SetUpData.GetAllDataFormattedIntoSingleString();
                    // re-populate adapter by running a 'forEach' through the list
                    RepopulateAdapter();
                    //adapter.NotifyDataSetChanged();

                    break;
                case Resource.Id.menu_top_alerts:
                    Toast.MakeText(this, "Action selected: \nSet Alert", ToastLength.Short).Show();
                    Intent intent = new Intent(this, typeof(PersonalAlarmsActivity));
                    StartActivity(intent);
                    break;
                case Resource.Id.menu_top_preferences:                    
                    Toast.MakeText(this, "Action selected: \nPreferences - test Activity", ToastLength.Short).Show();
                    intent = new Intent(this, typeof(GordTestActivity));
                    StartActivity(intent);
                    break;
                default:
                    break;
            };
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

