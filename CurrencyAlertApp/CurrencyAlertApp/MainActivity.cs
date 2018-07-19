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
        TextView txtDataLastUpdated;
        List<string> DisplayListSTRING = new List<string>();
        List<NewsObject> DisplayListOBJECT = new List<NewsObject>();
        ArrayAdapter adapter;
        List<string> marketImpact_selectedList = new List<string>();
        List<string> currencies_selectedList = new List<string>();


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

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

            // set up blank database table - should if check for null elsewhere ??
            // SQL only creates a new table if one doesn't already exist - it won't overwrite an existing table (?)
            SetUpData.CreateEmptyTable();

            // set up table to store url for xml data download
            SetUpData.CreateTableForURLDownload();

            // Display for last time data was updated - retrieved from Shared Preferences
            txtDataLastUpdated = FindViewById<TextView>(Resource.Id.txtDataLastUpdated);

            RefreshTxtDataLastUpdated();            
            txtDataLastUpdated.Text += "\n - Please select data to display";

            // wireup listView
            listView1 = FindViewById<ListView>(Resource.Id.listView1);

            // set up adapter for listView
            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, DisplayListSTRING);
            listView1.Adapter = adapter;

            //// Display all currency data (if any) from database - NOT using until custom view is implemented (???)
            //DefaultDisplayAllData();




            // Button to call next activity
            MyButton = FindViewById<Button>(Resource.Id.MyButton);
            //MyButton.Text =  GetString(Resource.String.Button_Activity2Test);  // not in use
            MyButton.Click += delegate
            {
                Intent intent = new Intent(this, typeof(GordTestActivity));
                StartActivity(intent);
            };



            // ItemClick for listView
            listView1.ItemClick += (sender, e) =>
            {
                Toast.MakeText(this, $"You selected item no: {e.Position}:\n{DisplayListSTRING[e.Position]}", ToastLength.Short).Show();

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
                            GordTestActivity.MethodToGetString(DisplayListSTRING[e.Position]);

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





            // bottom ToolBar Menu Selection
            editToolbar.MenuItemClick += (sender, e) =>
            {
                switch (e.Item.ItemId)
                {
                    case Resource.Id.menu_data_clear:

                        // clear MarketImpacts & Currencies checkboxes -  List<bool>'s, List<string>'s  & adapter
                        Array.Clear(marketImpact_selectedBoolArray, 0, marketImpact_selectedBoolArray.Length);
                        Array.Clear(currencies_selectedBoolArray, 0, currencies_selectedBoolArray.Length);
                        marketImpact_selectedList.Clear();
                        currencies_selectedList.Clear();

                        ClearDisplayListAndAdapter();
                        RefreshTxtDataLastUpdated();
                        txtDataLastUpdated.Text += "\n - Clear Option Selected";
                        PopulateAdapter();
                        //adapter.NotifyDataSetChanged();
                        break;


                    case Resource.Id.menu_data_getAllData:

                        // clear List & get raw newsObject data from database
                        ClearDisplayListAndAdapter();
                        RefreshTxtDataLastUpdated();
                        DisplayListOBJECT.Clear();
                        DisplayListOBJECT = SetUpData.GetAllRawDataFromDatabase();

                        // convert this newsObject data to a List<string>
                        DisplayListSTRING = ConvertObjectsListToStringList(DisplayListOBJECT);

                        // call populate adapter
                        PopulateAdapter();
                        break;


                    case Resource.Id.menu_data_selectMarketImpacts:

                        RefreshTxtDataLastUpdated();
                        using (var dialog = new Android.Support.V7.App.AlertDialog.Builder(this))
                        {
                            dialog.SetTitle("Select Market Impact(s)");
                            dialog.SetPositiveButton("Close", delegate {

                                // clear list - get LINQ query result - populate list                              
                                ClearDisplayListAndAdapter();
                                DisplayListOBJECT = SetUpData.LINQ_SortAllByUserSelection(marketImpact_selectedList, currencies_selectedList);

                                // convert this newsObject data to a List<string>
                                DisplayListSTRING = ConvertObjectsListToStringList(DisplayListOBJECT);

                                // call populate adapter
                                PopulateAdapter();
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


                    case Resource.Id.menu_data_selectCurrencies:

                        RefreshTxtDataLastUpdated();
                        using (var dialog = new Android.Support.V7.App.AlertDialog.Builder(this))
                        {
                            dialog.SetTitle("Select Currencies");
                            dialog.SetPositiveButton("Close", delegate {

                                // clear list - get LINQ query result - populate list                              
                                ClearDisplayListAndAdapter();
                                DisplayListOBJECT = SetUpData.LINQ_SortAllByUserSelection(marketImpact_selectedList, currencies_selectedList);

                                // convert this newsObject data to a List<string>
                                DisplayListSTRING = ConvertObjectsListToStringList(DisplayListOBJECT);

                                // call populate adapter
                                PopulateAdapter();
                                DebugDisplayMarketImpacts();
                            });

                            // Set Multichoice Items
                            dialog.SetMultiChoiceItems(currencies_titlesArray, currencies_selectedBoolArray,
                               (s, eEXtra) => {
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
                        }
                        break;


                    case Resource.Id.menu_data_sampleData:

                        // clear List & get sample data (from xml file in assets folder)
                        ClearDisplayListAndAdapter();
                        DisplayListOBJECT.Clear();
                        txtDataLastUpdated.Text = "Warning - Test Data Only (all)";

                        // get xml file & pass to method
                        XDocument xmlTestFile2 = XDocument.Load(Assets.Open("ff_calendar_thisweek.xml"));
                        DisplayListOBJECT = SetUpData.TestXMLDataFromAssetsFile(xmlTestFile2);

                        // convert this newsObjectList to a List<string>
                        DisplayListSTRING = ConvertObjectsListToStringList(DisplayListOBJECT);

                        // call populate adapter
                        PopulateAdapter();
                        break;


                    case Resource.Id.menu_data_sampleLINQQuery:

                        // clear List & get sample data (from xml file in assets folder)
                        ClearDisplayListAndAdapter();
                        DisplayListOBJECT.Clear();
                        txtDataLastUpdated.Text = "Warning - Test Data Only (query)";

                        // get xml file & pass to method
                        XDocument xmlTestFile1 = XDocument.Load(Assets.Open("ff_calendar_thisweek.xml"));
                        DisplayListOBJECT = SetUpData.TestLINQQueryUsingXML(xmlTestFile1);

                        // convert this newsObjectList to a List<string>
                        DisplayListSTRING = ConvertObjectsListToStringList(DisplayListOBJECT);

                        // call populate adapter
                        PopulateAdapter();
                        break;



                    case Resource.Id.menu_data_debugDisplay:
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
                // displays currency (selected) to debug output
                Log.Debug("DEBUG", ": Currencies Selected Display - Starts Here");
                foreach (var item in currencies_selectedList)
                {
                    Log.Debug("DEBUG", item);
                }
            }
            void DebugDisplayMarketImpacts()
            {
                // displays marketImpacts (selected) to debug output
                Log.Debug("DEBUG", ": Market Impacts Selected Display - Starts Here");
                foreach (var item in marketImpact_selectedList)
                {
                    Log.Debug("DEBUG", item);
                }
            }
        }// end onCreate



        // TOP Toolbar
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }


        // TOP Toolbar ('menu options')
        public override bool OnOptionsItemSelected(IMenuItem item) // 
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_top_marketData:
                    Toast.MakeText(this, "Action selected: \nMarket Data", ToastLength.Short).Show();
                    DataAccess.SetUpData.DropNewsObjectTable();

                    bool dataUpdateSuccessful = DataAccess.SetUpData.DownloadNewXMLAndStoreInDatabase();
                    if (dataUpdateSuccessful)
                    {
                        Toast.MakeText(this, "data update: " + dataUpdateSuccessful, ToastLength.Short).Show();

                        // store date & time of xml download in Shared Preferences
                        DateTime dateTime = DateTime.Now;
                        MySharedPreferencesMethods mySharedPreferencesMethods = new MySharedPreferencesMethods(this);
                        string dateInPreferedFormat = dateTime.ToShortDateString();
                        mySharedPreferencesMethods.StoreToSharedPrefs(dateInPreferedFormat);
                        txtDataLastUpdated.Text = "Data Updated: " + dateInPreferedFormat;
                    }
                    else
                        txtDataLastUpdated.Text = "Data Not Updated";

                    // Copied from display all raw data (bottom menu - selection 2)  !!!!!!!!!!!!!!!!!
                    // clear List & get raw newsObject data from database
                    ClearDisplayListAndAdapter();
                    DisplayListOBJECT.Clear();
                    DisplayListOBJECT = SetUpData.GetAllRawDataFromDatabase();

                    // convert this newsObject data to a List<string>
                    DisplayListSTRING = ConvertObjectsListToStringList(DisplayListOBJECT);

                    // call populate adapter
                    PopulateAdapter();
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

        //-----------------------------------------------------------------------------------




        void ClearDisplayListAndAdapter()
        {
            adapter.Clear();
            DisplayListSTRING.Clear();
        }


        void PopulateAdapter()  // original
        {
            // re-populate adapter by running a 'forEach' through the list
            foreach (var item in DisplayListSTRING)
            {
                adapter.Add(item);
            }
            //adapter.NotifyDataSetChanged();
        }


        public List<string> ConvertObjectsListToStringList(List<NewsObject> newsObjects)
        {
            List<string> stringList = new List<string>();

            foreach (var tempNewsObject in newsObjects)
            {
                // convert individual newsObject to a single string
                string tempStringItem = (string.Format(
                        "Date: {0} {1} \n{2} {3}\n{4}",
                        tempNewsObject.DateOnly.TrimEnd(),
                        tempNewsObject.TimeOnly.TrimEnd(),
                        tempNewsObject.CountryChar.TrimEnd(),
                        tempNewsObject.MarketImpact.TrimEnd(),
                        tempNewsObject.Name.TrimEnd()));

                // add string to string list
                stringList.Add(tempStringItem);
            }
            return stringList;
        }



        void RefreshTxtDataLastUpdated()
        {
            MySharedPreferencesMethods mySharedPreferencesMethods = new MySharedPreferencesMethods(this);
            string dateXmlUpdated = mySharedPreferencesMethods.GetDataFromSharedPrefs();
            txtDataLastUpdated.Text = "Data Updated: " + dateXmlUpdated;
        }








        // string version
        //void ClearListsAndRepopulateAdapter()
        //{
        //    ClearListAndAdapter();
        //    DisplayListSTRING = SetUpData.LINQ_SortAllByUserSelection(marketImpact_selectedList, currencies_selectedList);
        //    PopulateAdapter();
        //}







        //void DefaultDisplayAllData()
        //{
        //    ClearListAndAdapter();
        //    DisplayListSTRING = SetUpData.GetAllDataFormattedIntoSingleString();
        //    RepopulateAdapter();  // runs a 'forEach' through the list
        //    //adapter.NotifyDataSetChanged();
        //}



    }//
}//

