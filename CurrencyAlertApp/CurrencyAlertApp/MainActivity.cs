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
using Android.Support.V7.Widget;

namespace CurrencyAlertApp
{
    [Activity(Theme = "@style/MyTheme.Base", Label = "CurrencyAlertApp", MainLauncher = true, Icon = "@drawable/icon")]
    //  MainLauncher = true,      // must have an appCompat theme  

    public class MainActivity : AppCompatActivity
    {
        List<string> DisplayListSTRING = new List<string>();
        List<NewsObject> DisplayListOBJECT = new List<NewsObject>();
        List<NewsObject> tempDisplayListOBJECT = new List<NewsObject>();

        TextView txtDataLastUpdated;
        List<string> marketImpact_selectedList = new List<string>();
        List<string> currencies_selectedList = new List<string>();


        // RecyclerView instance that displays the newsObject List:
        public RecyclerView mRecyclerView;

        // Layout manager that lays out each card in the RecyclerView:
        public RecyclerView.LayoutManager mLayoutManager;

        // Adapter that accesses the data set (List<newsObject>):
        public NewsObject_RecycleAdapter_NEW mAdapter;




        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource:
            SetContentView(Resource.Layout.Main);

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

            //Register the item click handler(below) with the adapter:           
            mAdapter.ItemClick += MAdapter_ItemClick1;

            // Plug the adapter into the RecyclerView:
            mRecyclerView.SetAdapter(mAdapter);

            //-----------------------------------------------------------------------------------
            //------------------------------------------------------------------------------------

            // ToolBar - Top of Screen  (method 1)
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = GetString(Resource.String.ToolbarTopTitle);

            // Toolbar - Bottom of Screen  (method 2)
            var toolbar_bottom = FindViewById<Toolbar>(Resource.Id.toolbar_bottom);
            toolbar_bottom.Title = GetString(Resource.String.ToolbarBottomTitle);
            toolbar_bottom.InflateMenu(Resource.Menu.menus_Main_bottom);

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

            //// Display all currency data (if any) from database - NOT using until custom view is implemented (???)
            //DefaultDisplayAllData();


            // bottom ToolBar Menu Selection
            toolbar_bottom.MenuItemClick += (sender, e) =>
            {
                switch (e.Item.ItemId)
                {
                    case Resource.Id.menu_data_clear:

                        // clear MarketImpacts & Currencies checkboxes -  List<bool>'s, List<string>'s  & adapter
                        Array.Clear(marketImpact_selectedBoolArray, 0, marketImpact_selectedBoolArray.Length);
                        Array.Clear(currencies_selectedBoolArray, 0, currencies_selectedBoolArray.Length);
                        marketImpact_selectedList.Clear();
                        currencies_selectedList.Clear();

                        RefreshTxtDataLastUpdated();
                        txtDataLastUpdated.Text += "\n - Clear Option Selected";

                        tempDisplayListOBJECT.Clear();
                        PopulateAdapter();
                        break;


                    case Resource.Id.menu_data_getAllData:

                        // clear List & get raw newsObject data from database  
                        tempDisplayListOBJECT.Clear();
                        tempDisplayListOBJECT = SetUpData.GetAllRawDataFromDatabase();

                        // call populate adapter
                        PopulateAdapter();
                        RefreshTxtDataLastUpdated();
                        break;


                    case Resource.Id.menu_data_selectMarketImpacts:

                        using (var dialog = new Android.Support.V7.App.AlertDialog.Builder(this))
                        {
                            dialog.SetTitle("Select Market Impact(s)");
                            dialog.SetPositiveButton("Close", delegate
                            {
                                // clear list - get LINQ query result - populate list                                                              
                                tempDisplayListOBJECT.Clear();
                                tempDisplayListOBJECT = SetUpData.LINQ_SortAllByUserSelection(marketImpact_selectedList, currencies_selectedList);

                                // call populate adapter
                                PopulateAdapter();
                                RefreshTxtDataLastUpdated();
                                DebugDisplayMarketImpacts();
                            });

                            // Set Multichoice Items
                            dialog.SetMultiChoiceItems(marketImpact_titlesArray, marketImpact_selectedBoolArray,
                               (sender2, event2) =>
                               {
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

                        using (var dialog = new Android.Support.V7.App.AlertDialog.Builder(this))
                        {
                            dialog.SetTitle("Select Currencies");
                            dialog.SetPositiveButton("Close", delegate
                            {
                                // clear list - get LINQ query result - populate list 
                                tempDisplayListOBJECT.Clear();
                                tempDisplayListOBJECT = SetUpData.LINQ_SortAllByUserSelection(marketImpact_selectedList, currencies_selectedList);

                                // call populate adapter
                                PopulateAdapter();
                                RefreshTxtDataLastUpdated();
                                DebugDisplayCurrencies();
                            });

                            // Set Multichoice Items
                            dialog.SetMultiChoiceItems(currencies_titlesArray, currencies_selectedBoolArray,
                               (s, eEXtra) =>
                               {
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

                                // clear list & adapter - get LINQ query result - populate list 
                                tempDisplayListOBJECT.Clear();
                                tempDisplayListOBJECT = SetUpData.LINQ_SortAllByUserSelection(marketImpact_selectedList, currencies_selectedList);

                                // call populate adapter
                                PopulateAdapter();
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
                                // clear list & adapter  
                                tempDisplayListOBJECT.Clear();
                                PopulateAdapter();
                                DebugDisplayCurrencies();
                            });
                            dialog.Show();
                        }
                        break;


                    case Resource.Id.menu_data_sampleData:

                        // get sample data (from xml file in assets folder) & pass to method
                        XDocument xmlTestFile2 = XDocument.Load(Assets.Open("ff_calendar_thisweek.xml"));

                        tempDisplayListOBJECT.Clear();
                        tempDisplayListOBJECT = SetUpData.TestXMLDataFromAssetsFile(xmlTestFile2);

                        // call populate adapter
                        PopulateAdapter();
                        txtDataLastUpdated.Text = "Warning - Test Data Only (all)";
                        break;

                    case Resource.Id.menu_data_sampleLINQQuery:

                        // get sample data (from xml file in assets folder) & pass to method & pass to LINQ query method
                        XDocument xmlTestFile1 = XDocument.Load(Assets.Open("ff_calendar_thisweek.xml"));
                        tempDisplayListOBJECT.Clear();
                        tempDisplayListOBJECT = SetUpData.TestLINQQueryUsingXML(xmlTestFile1);

                        // call populate adapter
                        PopulateAdapter();
                        txtDataLastUpdated.Text = "Warning - Test Data Only (query)";
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

        }// end onCreate  ------------------------------------------------------------------------





        private void MAdapter_ItemClick1(object sender, int e)
        {
            // alert dialog for ItenClick event
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            // builder.SetMessage("Hi there!");  // usisng this disable array of menu options - good for Ok/Cancel version
            builder.SetTitle("Choose one:");

            builder.SetItems(Resource.Array.itemSelect_AddToWatchList, (sender2, e2) =>
            {
                var index = e2.Which;
                Log.Debug("DEBUG", index.ToString());
                Log.Debug("DEBUG", e2.Which.ToString());
                //Toast.MakeText(this, $"You selected item no: {e}:\n" + DisplayListOBJECT[e].ToString(), ToastLength.Long).Show();

                switch (e2.Which)
                {
                    case 0:
                        break;
                    case 1:
                        // call method in next activity to pass data across (newsObject)
                        CustomAdapter_Test_Activity.MethodToPassObject(DisplayListOBJECT[e]);
                        // call intent to start next activity
                        Intent intent = new Intent(this, typeof(CustomAdapter_Test_Activity));
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
        }// end  MAdapter_ItemClick1
         //-----------------------------------------------------------------------------------




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
                    tempDisplayListOBJECT.Clear();
                    tempDisplayListOBJECT = SetUpData.GetAllRawDataFromDatabase();

                    // call populate adapter
                    PopulateAdapter();
                    RefreshTxtDataLastUpdated();
                    break;

                case Resource.Id.menu_top_alerts:
                    Toast.MakeText(this, "Action selected: \nSet Alert", ToastLength.Short).Show();
                    Intent intent = new Intent(this, typeof(PersonalAlarmsActivity));
                    StartActivity(intent);
                    break;

                case Resource.Id.menu_top_custom_adapter:
                    Toast.MakeText(this, "Action selected: \nPreferences - test Activity", ToastLength.Short).Show();
                    intent = new Intent(this, typeof(CustomAdapter_Test_Activity));
                    StartActivity(intent);
                    break;

                case Resource.Id.menu_top_preferences:
                    Toast.MakeText(this, "Action selected: \nPreferences - test Activity", ToastLength.Short).Show();
                    intent = new Intent(this, typeof(CustomAdapter_Test_Activity));
                    StartActivity(intent);
                    break;

                default:
                    break;
            };
            return base.OnOptionsItemSelected(item);
        }
        //-----------------------------------------------------------------------------------



        void PopulateAdapter()
        {
            // Refresh adapter by running a 'forEach' through tempList to 
            // repopulate the same DisplayListObect that adapter has memory reference to  
            DisplayListOBJECT.Clear();
            foreach (var item in tempDisplayListOBJECT)
            {
                DisplayListOBJECT.Add(item);
            }
            mAdapter.NotifyDataSetChanged();
        }

        void RefreshTxtDataLastUpdated()
        {
            MySharedPreferencesMethods mySharedPreferencesMethods = new MySharedPreferencesMethods(this);
            string dateXmlUpdated = mySharedPreferencesMethods.GetDataFromSharedPrefs();
            txtDataLastUpdated.Text = "Data Updated: " + dateXmlUpdated;
        }
    }// end MainActivity
}// end Namespace