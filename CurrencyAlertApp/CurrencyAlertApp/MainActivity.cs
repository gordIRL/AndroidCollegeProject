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



            // variables for bottom toolbar - alert - market impact
            List<string> selectedMarketImpactList = new List<string>();
            // list of marketImpact(s) for user to select from
            string[] marketImpactTitlesArray = Resources.GetStringArray(Resource.Array.MarketImpactArray);
            // bool array for selected Market Impact checkboxes in MultiItemSelect 
            bool[] selectedMarketImpactBoolArray = new bool[marketImpactTitlesArray.Length];


            // variables for bottom toolbar - alert - currencies            
            List<string> selectedCurrencyList = new List<string>();
            // list of currencies for user to select from
            string[] currencyTitlesArray = Resources.GetStringArray(Resource.Array.CurrenciesArray);
            // bool array for selected Currency checkboxes in MultiItemSelect 
            bool[] selectedCurrenciesBoolArray = new bool[currencyTitlesArray.Length];


            // bottom ToolBar Menu Selection
            editToolbar.MenuItemClick += (sender, e) =>
            {
                switch (e.Item.ItemId)         
                {
                    case Resource.Id.menu_data_selectMarketImpact:
                        // display user selection info
                        //Toast.MakeText(this, "Bottom toolbar / Default!!!!!!:\nID: " + e.Item.ItemId + "\nTitle: " + e.Item.TitleFormatted, ToastLength.Short).Show();
                        
                        using (var dialog = new Android.Support.V7.App.AlertDialog.Builder(this))
                        {
                            dialog.SetTitle("Select Market Impact(s)");
                            dialog.SetPositiveButton("Close", delegate {
                                DebugDisplayMarketImpacts();
                            });

                            // Set Multichoice Items
                            dialog.SetMultiChoiceItems(marketImpactTitlesArray, selectedMarketImpactBoolArray,
                               (sender2, event2) => {
                                   int index = event2.Which;
                                   bool isChecked = event2.IsChecked;
                                   selectedMarketImpactBoolArray[index] = isChecked;
                                   //Toast.MakeText(this, "You clicked: " + marketImpactTitlesArray[index]
                                   //   + "\nChecked: " + event2.IsChecked, ToastLength.Short).Show();

                                    // add item to list if now selected - ie isChecked is now TRUE  
                                    if (isChecked)
                                       selectedMarketImpactList.Add(marketImpactTitlesArray[index]);
                                   else
                                       selectedMarketImpactList.Remove(marketImpactTitlesArray[index]);
                               });
                           
                            dialog.Show();
                        }// end Using
                                              
                        break;


                    //-----------------------------------------------------------------------------

                    case Resource.Id.menu_selectCurrencies:
                        // display user selection info
                        //Toast.MakeText(this, "Bottom toolbar / Default!!!!!!:\nID: " + e.Item.ItemId + "\nTitle: " + e.Item.TitleFormatted, ToastLength.Short).Show();

                        using (var dialog = new Android.Support.V7.App.AlertDialog.Builder(this))
                        {
                            dialog.SetTitle("Select Currencies");
                            dialog.SetPositiveButton("Close", delegate
                            {
                                DebugDisplayCurrencies();
                            });

                            // Set Multichoice Items
                            dialog.SetMultiChoiceItems(currencyTitlesArray, selectedCurrenciesBoolArray,
                               (s, eEXtra) =>
                               {
                                   int index = eEXtra.Which;
                                   bool isChecked = eEXtra.IsChecked;
                                   selectedCurrenciesBoolArray[index] = isChecked;
                                   //Toast.MakeText(this, "You clicked: " + currencyTitlesArray[index]
                                   //    + "\nChecked: " + eEXtra.IsChecked, ToastLength.Short).Show();

                                   // add item to list if now selected - ie isChecked is now TRUE  
                                   if (isChecked)
                                   {
                                       selectedCurrencyList.Add(currencyTitlesArray[index]);
                                       selectedCurrenciesBoolArray[index] = true;
                                   }
                                   else
                                   {
                                       selectedCurrencyList.Remove(currencyTitlesArray[index]);
                                       selectedCurrenciesBoolArray[index] = false;
                                   }                                       
                               });

                            // check all boxes and add all items to list(s)
                            dialog.SetNeutralButton("ALL", delegate
                            {
                                // clear list 1st to avoid getting duplicate entries
                                selectedCurrencyList.Clear();

                                // set all items in bool[] selected to TRUE
                                for (int i = 0; i < selectedCurrenciesBoolArray.Length; i++)
                                {
                                    selectedCurrenciesBoolArray[i] = true;
                                    selectedCurrencyList.Add(currencyTitlesArray[i]);
                                }
                                DebugDisplayCurrencies();
                            });

                            // deselect all boxes & clear list
                            dialog.SetNegativeButton("Clear", delegate
                            {
                                selectedCurrencyList.Clear();
                                //  clear bool[] set all items to FALSE
                                for (int i = 0; i < selectedCurrenciesBoolArray.Length; i++)
                                {
                                    selectedCurrenciesBoolArray[i] = false;
                                    selectedCurrencyList.Remove(currencyTitlesArray[i]);
                                }
                                DebugDisplayCurrencies();
                            });

                            dialog.Show();
                        }// end Using
                        break;


                    //-----------------------------------------------------------









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


                    case Resource.Id.menu_debugDisplay:
                        Log.Debug("DEBUG", ": Currency & Market Impact Selected Display - Starts Here");
                        foreach (var item in selectedCurrencyList)
                        {
                            Log.Debug("DEBUG", item);
                        }
                        foreach (var item in selectedMarketImpactList)
                        {
                            Log.Debug("DEBUG", item);
                        }
                        break;

                    default:
                        break;
                }
            };    
            
            void DebugDisplayCurrencies()
            {
                Log.Debug("DEBUG", ": Currencies Selected Display - Starts Here");
                foreach (var item in selectedCurrencyList)
                {
                    Log.Debug("DEBUG", item);
                }               
            }
            void DebugDisplayMarketImpacts()
            {
                Log.Debug("DEBUG", ": Market Impacts Selected Display - Starts Here");
                foreach (var item in selectedMarketImpactList)
                {
                    Log.Debug("DEBUG", item);
                }

            }
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

        //void DebugCurrencyList()
        //{}

       
    }//
}//

