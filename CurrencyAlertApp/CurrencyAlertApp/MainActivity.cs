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


        List<string> marketImpact_selectedList = new List<string>();       
        List<string> currencies_selectedList = new List<string>();



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



            // variables - bottom toolbar - alert dialog - market impact
            string[] marketImpact_titlesArray = Resources.GetStringArray(Resource.Array.MarketImpactArray);
            //List<string> marketImpact_selectedList = new List<string>();
            bool[] marketImpact_selectedBoolArray = new bool[marketImpact_titlesArray.Length];  // for selected checkboxes in MultiItemSelect 

            // variables - bottom toolbar - alert dialog - currencies 
            string[] currencies_titlesArray = Resources.GetStringArray(Resource.Array.CurrenciesArray);
            //List<string> currencies_selectedList = new List<string>();
            bool[] currencies_selectedBoolArray = new bool[currencies_titlesArray.Length];  // for selected checkboxes in MultiItemSelect 


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

                                // !!!  need to clear the screen of whatever data is already being displayed  !!
                                // !!!  Market Impact selection only seems to work 2nd time around - currency selection works 1st time around !!!!

                                // populate list with LINQ query result 
                                myList = SetUpData.GetLINQResultData2(marketImpact_selectedList, currencies_selectedList);
                                // re-populate adapter by running a 'forEach' through the list
                                RepopulateAdapter();
                                //adapter.NotifyDataSetChanged();
                            });




                            // Set Multichoice Items
                            dialog.SetMultiChoiceItems(marketImpact_titlesArray, marketImpact_selectedBoolArray,
                               (sender2, event2) => {
                                   int index = event2.Which;
                                   bool isChecked = event2.IsChecked;
                                   marketImpact_selectedBoolArray[index] = isChecked;
                                   //Toast.MakeText(this, "You clicked: " + marketImpactTitlesArray[index]
                                   //   + "\nChecked: " + event2.IsChecked, ToastLength.Short).Show();

                                    // add item to list if now selected - ie isChecked is now TRUE  
                                    if (isChecked)
                                       marketImpact_selectedList.Add(marketImpact_titlesArray[index]);
                                   else
                                       marketImpact_selectedList.Remove(marketImpact_titlesArray[index]);
                               });
                           
                            dialog.Show();
                        }// end Using                                              
                        break;

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
                            dialog.SetMultiChoiceItems(currencies_titlesArray, currencies_selectedBoolArray,
                               (s, eEXtra) =>
                               {
                                   int index = eEXtra.Which;
                                   bool isChecked = eEXtra.IsChecked;
                                   currencies_selectedBoolArray[index] = isChecked;
                                   //Toast.MakeText(this, "You clicked: " + currencyTitlesArray[index]
                                   //    + "\nChecked: " + eEXtra.IsChecked, ToastLength.Short).Show();

                                   // add item to list if now selected - ie isChecked is now TRUE  
                                   if (isChecked)
                                   {
                                       currencies_selectedList.Add(currencies_titlesArray[index]);
                                       currencies_selectedBoolArray[index] = true;
                                   }
                                   else
                                   {
                                       currencies_selectedList.Remove(currencies_titlesArray[index]);
                                       currencies_selectedBoolArray[index] = false;
                                   }                                       
                               });

                            // check all boxes and add all items to list(s)
                            dialog.SetNeutralButton("ALL", delegate
                            {
                                // clear list 1st to avoid getting duplicate entries
                                currencies_selectedList.Clear();

                                // set all items in bool[] selected to TRUE
                                for (int i = 0; i < currencies_selectedBoolArray.Length; i++)
                                {
                                    currencies_selectedBoolArray[i] = true;
                                    currencies_selectedList.Add(currencies_titlesArray[i]);
                                }
                                DebugDisplayCurrencies();
                            });

                            // deselect all boxes & clear list
                            dialog.SetNegativeButton("Clear", delegate
                            {
                                currencies_selectedList.Clear();
                                //  clear bool[] set all items to FALSE
                                for (int i = 0; i < currencies_selectedBoolArray.Length; i++)
                                {
                                    currencies_selectedBoolArray[i] = false;
                                    currencies_selectedList.Remove(currencies_titlesArray[i]);
                                }
                                DebugDisplayCurrencies();
                            });

                            dialog.Show();
                        }// end Using
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

                        // populate list with LINQ query result 
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
                        foreach (var item in currencies_selectedList)
                        {
                            Log.Debug("DEBUG", item);
                        }
                        foreach (var item in marketImpact_selectedList)
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
                // displays contentents to debug output
                Log.Debug("DEBUG", ": Currencies Selected Display - Starts Here");
                foreach (var item in currencies_selectedList)
                {
                    Log.Debug("DEBUG", item);
                }               
            }
            void DebugDisplayMarketImpacts()
            {
                // displays contentents to debug output
                Log.Debug("DEBUG", ": Market Impacts Selected Display - Starts Here");
                foreach (var item in marketImpact_selectedList)
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

              
    }//
}//

