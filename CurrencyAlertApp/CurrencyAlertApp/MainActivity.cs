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
        List<string> DisplayListForMainActivity = new List<string>();
        List<NewsObject> newsObjectsList = new List<NewsObject>();
        ArrayAdapter adapter;
        List<string> marketImpact_selectedList = new List<string>();       
        List<string> currencies_selectedList = new List<string>();


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // set up blank database table - should if check for null elsewhere ??
            // SQL only creates a new table if one doesn't already exist - it won't overwrite an existing table (?)
            SetUpData.CreateEmptyTable();

            // set up table to store url for xml data download
            SetUpData.CreateTableForURLDownload();
            //SetUpData.GetURLForXMLDownloadFromDatabase();      // delete !!!


            // Display for last time data was updated - to be linked to XML download date.....
            txtDataLastUpdated = FindViewById<TextView>(Resource.Id.txtDataLastUpdated);
            txtDataLastUpdated.Text += "  "  + DateTime.Now.ToShortDateString();







            // Shared Preferences Stuff        

            // instantiate object
            MySharedPreferencesMethods mySharedPreferencesMethods = new MySharedPreferencesMethods(this);

            // test
            string testData = mySharedPreferencesMethods.FirstMethod();
            Log.Debug("DEBUG", "Return String: " + testData);

            // store data
            bool testData2 = mySharedPreferencesMethods.StoreToSharedPrefs("Todays date is 17th July 2018!!!!!!!!!");
            Log.Debug("DEBUG", "Return bool: " + testData2);

            // retun data
            string myDateString = mySharedPreferencesMethods.GetDataFromSharedPrefs();
            Log.Debug("DEBUG", "Returned Data " + myDateString);

            Log.Debug("DEBUG", "Pause for Break Point");














            // Button to call next activity
            MyButton = FindViewById<Button>(Resource.Id.MyButton);
            //MyButton.Text =  GetString(Resource.String.Button_Activity2Test);  // not in use
            MyButton.Click += delegate
            {
                Intent intent = new Intent(this, typeof(GordTestActivity));
                StartActivity(intent);
            };



            // populate list with default data (numbers) - list<string>
            DisplayListForMainActivity = SetUpData.NoDataToDisplay();

            // wireup listView
            listView1 = FindViewById<ListView>(Resource.Id.listView1);

            // set up adapter for listView
            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, DisplayListForMainActivity);
            listView1.Adapter = adapter;

            // ItemClick for listView
            listView1.ItemClick += (sender, e) =>
            {
                Toast.MakeText(this, $"You selected item no: {e.Position}:\n{DisplayListForMainActivity[e.Position]}", ToastLength.Short).Show();

                // alert dialog for ItenClick event
                Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
               // builder.SetMessage("Hi there!");  // usisng this disable array of menu options - good for Ok/Cancel version
                builder.SetTitle("Choose one:");
                
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
                            GordTestActivity.MethodToGetString(DisplayListForMainActivity[e.Position]);
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
            bool[] marketImpact_selectedBoolArray = new bool[marketImpact_titlesArray.Length];  // for selected checkboxes in MultiItemSelect 

            // variables - bottom toolbar - alert dialog - currencies 
            string[] currencies_titlesArray = Resources.GetStringArray(Resource.Array.CurrenciesArray);
            bool[] currencies_selectedBoolArray = new bool[currencies_titlesArray.Length];  // for selected checkboxes in MultiItemSelect 


            // bottom ToolBar Menu Selection
            editToolbar.MenuItemClick += (sender, e) =>
            {
                switch (e.Item.ItemId)         
                {
                    case Resource.Id.menu_data_clear:
                        // clear checkboxes for MarketImpacts & Currencies via their bool arrays & clear respective lists
                        Array.Clear(marketImpact_selectedBoolArray, 0, marketImpact_selectedBoolArray.Length);
                        Array.Clear(currencies_selectedBoolArray, 0, currencies_selectedBoolArray.Length);
                        marketImpact_selectedList.Clear();
                        currencies_selectedList.Clear();

                        ClearListAndAdapter();
                        DisplayListForMainActivity = SetUpData.NoDataToDisplay();
                        RepopulateAdapter();                        
                        //adapter.NotifyDataSetChanged();
                        break;

                    case Resource.Id.menu_data_selectMarketImpact:                       
                        using (var dialog = new Android.Support.V7.App.AlertDialog.Builder(this))
                        {
                            dialog.SetTitle("Select Market Impact(s)");
                            dialog.SetPositiveButton("Close", delegate {                                
                                // clear adapter & list & populate list with LINQ query result
                                ClearListsAndRepopulateAdapter();
                                DebugDisplayMarketImpacts();
                            });

                            // Set Multichoice Items
                            dialog.SetMultiChoiceItems(marketImpact_titlesArray, marketImpact_selectedBoolArray,
                               (sender2, event2) => {
                                   int index = event2.Which;
                                   bool isChecked = event2.IsChecked;
                                   marketImpact_selectedBoolArray[index] = isChecked;                                  

                                    // add item to list if now selected - ie isChecked is now TRUE  
                                    if (isChecked)
                                       marketImpact_selectedList.Add(marketImpact_titlesArray[index]);
                                   else
                                       marketImpact_selectedList.Remove(marketImpact_titlesArray[index]);
                               });                           
                            dialog.Show();
                        }                                             
                        break;

                    case Resource.Id.menu_selectCurrencies:                       
                        using (var dialog = new Android.Support.V7.App.AlertDialog.Builder(this))
                        {
                            dialog.SetTitle("Select Currencies");
                            dialog.SetPositiveButton("Close", delegate{
                                // clear adapter & list & populate list with LINQ query result
                                ClearListsAndRepopulateAdapter();
                                DebugDisplayCurrencies();
                            });

                            // Set Multichoice Items
                            dialog.SetMultiChoiceItems(currencies_titlesArray, currencies_selectedBoolArray,
                               (s, eEXtra) =>  {
                                   int index = eEXtra.Which;
                                   bool isChecked = eEXtra.IsChecked;
                                   currencies_selectedBoolArray[index] = isChecked;

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
                            dialog.SetNeutralButton("ALL", delegate {
                                // clear list 1st to avoid getting duplicate entries
                                currencies_selectedList.Clear();

                                // set all items in bool[] selected to TRUE
                                for (int i = 0; i < currencies_selectedBoolArray.Length; i++)
                                {
                                    currencies_selectedBoolArray[i] = true;
                                    currencies_selectedList.Add(currencies_titlesArray[i]);
                                }                               
                                // clear adapter & list & populate list with LINQ query result
                                ClearListsAndRepopulateAdapter();
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
                                // clear adapter & list & populate list with LINQ query result
                                ClearListsAndRepopulateAdapter();
                                DebugDisplayCurrencies();
                            });
                            dialog.Show();
                        }
                        break;


                    case Resource.Id.menu_data_all_data:
                        // clear adapter & list &  populate list with ALL data (formatted)
                        ClearListAndAdapter();
                        DisplayListForMainActivity = SetUpData.GetAllDataFormattedIntoSingleString();
                        RepopulateAdapter();  // runs a 'forEach' through the list
                        //adapter.NotifyDataSetChanged();
                        break;

                    case Resource.Id.menu_data_sample_LINQ_query:
                        // clear adapter & list & populate list with LINQ query result 
                        ClearListAndAdapter();
                        XDocument xmlTestFile1 = XDocument.Load(Assets.Open("ff_calendar_thisweek.xml"));
                        DisplayListForMainActivity = SetUpData.TestLINQQueryUsingXML(xmlTestFile1); 
                        RepopulateAdapter();
                        //adapter.NotifyDataSetChanged();
                        break;

                    case Resource.Id.menu_sample_data:
                        // clear adapter & list & populate list with a 'for' loop of sample text
                        ClearListAndAdapter();

                        XDocument xmlTestFile2= XDocument.Load(Assets.Open("ff_calendar_thisweek.xml"));
                        DisplayListForMainActivity = SetUpData.TestXMLDataFromAssetsFile(xmlTestFile2);
                        RepopulateAdapter();
                        //adapter.NotifyDataSetChanged();
                        break;

                    case Resource.Id.menu_debugDisplay:
                        // displays contents of currency & marketImpact list in the Debug Output window
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
                    DisplayListForMainActivity = SetUpData.GetAllDataFormattedIntoSingleString();
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

       
        void ClearListsAndRepopulateAdapter()
        {
            ClearListAndAdapter();
            DisplayListForMainActivity = SetUpData.LINQ_SortAllByUserSelection(marketImpact_selectedList, currencies_selectedList);
            RepopulateAdapter();
        }

        void ClearListAndAdapter()
        {
            adapter.Clear();
            DisplayListForMainActivity.Clear();
        }

        void RepopulateAdapter()
        {
            // re-populate adapter by running a 'forEach' through the list
            foreach (var item in DisplayListForMainActivity)
            {
                adapter.Add(item);
            }
            //adapter.NotifyDataSetChanged();
        }
    }//
}//

