using Android.App;
using Android.OS;
using Android.Content;
using Android.Views;
using System.Collections.Generic;
using System;
using System.Xml.Linq;
// to pass a XDocument add reference:    System.Xml.Linq.
using Android.Content.Res;
using Android.Util;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using CurrencyAlertApp.DataAccess;
using System.Threading.Tasks;

namespace CurrencyAlertApp
{
    [Activity(Theme = "@style/MyTheme.Base",   Label = "CurrencyAlertApp",  Icon = "@drawable/icon")]
    //  MainLauncher = true,      // must have an appCompat theme  

    public class MainActivity : AppCompatActivity
    {
        // declare controls
        TextView txtDataLastUpdated;

        // list(s) used to populate adapter
        List<NewsObject> newsObjectDisplayList = new List<NewsObject>();
        List<NewsObject> tempNewsObjectDisplayList = new List<NewsObject>();

        List<string> marketImpact_selectedList = new List<string>();
        List<string> currencies_selectedList = new List<string>();

        public static bool TimeOffsetUpdated { get; set; }

        // RecyclerView instance that displays the newsObject List:
        public RecyclerView mRecyclerView;

        // Layout manager that lays out each card in the RecyclerView:
        public RecyclerView.LayoutManager mLayoutManager;

        // Adapter that accesses the data set (List<newsObject>):
        public NewsObject_RecycleAdapter mAdapter;
               

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource:
            SetContentView(Resource.Layout.Main);

            // Get our RecyclerView layout:
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView_MainActivity);

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
            mAdapter = new NewsObject_RecycleAdapter(newsObjectDisplayList);

            //Register the item click handler(below) with the adapter:           
            mAdapter.ItemClick += MAdapter_ItemClick1;

            // Plug the adapter into the RecyclerView:
            mRecyclerView.SetAdapter(mAdapter);

            //-----------------------------------
            //-----------------------------------

            // ToolBar - Top of Screen  (method 1)
            var toolbar = FindViewById<Toolbar>(Resource.Id.mainActivity_top_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = GetString(Resource.String.mainActivity_top_toolbar_title);

            // Toolbar - Bottom of Screen  (method 2)
            var toolbar_bottom = FindViewById<Toolbar>(Resource.Id.mainActivity_bottom_toolbar);
            toolbar_bottom.Title = GetString(Resource.String.mainActivity_bottom_toolbar_title);
            toolbar_bottom.InflateMenu(Resource.Menu.mainActivity_bottomMenu);

            // variables - bottom toolbar - alert dialog - market impact
            string[] marketImpact_titlesArray = Resources.GetStringArray(Resource.Array.MarketImpactArray);
            bool[] marketImpact_selectedBoolArray = new bool[marketImpact_titlesArray.Length];  // for selected checkboxes in MultiItemSelect 

            // variables - bottom toolbar - alert dialog - currencies 
            string[] currencies_titlesArray = Resources.GetStringArray(Resource.Array.CurrenciesArray);
            bool[] currencies_selectedBoolArray = new bool[currencies_titlesArray.Length];  // for selected checkboxes in MultiItemSelect 
                       
            // SQL only creates a new table if one doesn't already exist - it won't overwrite an existing table (?)
            DataAccessHelpers.CreateEmptyTable();

            // set up table to store url for xml data download
            DataAccessHelpers.CreateTableForURLDownload();

            // Display for last time data was updated - retrieved from Shared Preferences
            txtDataLastUpdated = FindViewById<TextView>(Resource.Id.mainActivity_txt_dataLastUpdated);

            RefreshTxtDataLastUpdated();
            txtDataLastUpdated.Text += "\n " + GetString(Resource.String.mainActivity_txt_dataLastUpdated_pleaseSelectData);

            // Display all currency data (if any) from database.
            // This is needed here because onCreate() is called when MainActivity 
            // is selected from a menu option (intent) in UserAlertsActivity. 
            // - Not needed or called if the user selects onBackPress() 
            GetAndDisplayDefaultData();

            //  for test data only------------------------------------------------------------
            // Get testdata from xml file in Assets folder             
            XDocument xmlTestDataFile = XDocument.Load(Assets.Open("ff_calendar_thisweek.xml"));           

            // assign xml data file from Asset directory TO SetupData Property  
            DataAccessHelpers.XmlTestDataFile = xmlTestDataFile;
            //--------------------------------------------------------------------------------


            if (TimeOffsetUpdated == false)
            {
                Log.Debug("DEBUG_MainActivity_OnCreate", "\n\n\nTime offset has NOT been updated\n\n\n");
            }

            if (TimeOffsetUpdated == true)
            {
                Log.Debug("DEBUG_MainActivity_OnCreate", "\n\n\nSUCCESS - Time offset HAS been updated\n\n\n");
                //Toast.MakeText(this, GetString(Resource.String.mainActivity_txt_timeOffset_successMessage), ToastLength.Long).Show();
                UpdateXML_Option();

                // reset property for reuse on next occasion
                TimeOffsetUpdated = false;
            }






            //----------------------------------------------------
            // bottom ToolBar Menu Selection
            toolbar_bottom.MenuItemClick += (sender, e) =>
            {
                switch (e.Item.ItemId)
                {
                    case Resource.Id.mainActivity_bottom_toolbar_option_ClearCurrencyData:

                        // clear MarketImpacts & Currencies checkboxes -  List<bool>'s, List<string>'s  & adapter
                        Array.Clear(marketImpact_selectedBoolArray, 0, marketImpact_selectedBoolArray.Length);
                        Array.Clear(currencies_selectedBoolArray, 0, currencies_selectedBoolArray.Length);
                        marketImpact_selectedList.Clear();
                        currencies_selectedList.Clear();
                        RefreshTxtDataLastUpdated();
                        txtDataLastUpdated.Text += "\n " + GetString(Resource.String.mainActivity_txt_dataLastUpdated_clearOptionSelected);
                        tempNewsObjectDisplayList.Clear();
                        PopulateNewsObjectAdapter();
                        break;

                    case Resource.Id.mainActivity_bottom_toolbar_option_displayAllCurrencyData:

                        GetAndDisplayDefaultData();
                        break;
                        
                    case Resource.Id.mainActivity_bottom_toolbar_option_selectCurrencies:

                        using (var dialog = new Android.Support.V7.App.AlertDialog.Builder(this))
                        {
                            dialog.SetTitle(GetString(Resource.String.mainActivity_dialogOption_selectCurrencies));
                            dialog.SetPositiveButton(GetString(Resource.String.mainActivity_dialogOption_selectCurrencies_positiveButton), delegate
                            {
                                // clear list - get LINQ query result - populate list 
                                tempNewsObjectDisplayList.Clear();
                                tempNewsObjectDisplayList = DataAccessHelpers.LINQ_SortAllByUserSelection(marketImpact_selectedList, currencies_selectedList);

                                // call populate adapter
                                PopulateNewsObjectAdapter();
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
                            dialog.SetNeutralButton(GetString(Resource.String.mainActivity_dialogOption_selectCurrencies_neutralButton), delegate
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
                                tempNewsObjectDisplayList.Clear();
                                tempNewsObjectDisplayList 
                                    = DataAccessHelpers.LINQ_SortAllByUserSelection(marketImpact_selectedList, currencies_selectedList);

                                // call populate adapter
                                PopulateNewsObjectAdapter();
                                DebugDisplayCurrencies();
                            });

                            // deselect all boxes & clear list
                            dialog.SetNegativeButton(GetString(Resource.String.mainActivity_dialogOption_selectCurrencies_negativeButton), delegate
                            {
                                currencies_selectedList.Clear();
                                //  clear bool[] set all items to FALSE
                                for (int i = 0; i < currencies_selectedBoolArray.Length; i++)
                                {
                                    currencies_selectedBoolArray[i] = false;
                                    currencies_selectedList.Remove(currencies_titlesArray[i]);
                                }
                                // clear list & adapter  
                                tempNewsObjectDisplayList.Clear();
                                PopulateNewsObjectAdapter();
                                DebugDisplayCurrencies();
                            });
                            dialog.Show();
                        }
                        break;

                    case Resource.Id.mainActivity_bottom_toolbar_option_debugDisplay:
                        // displays contents of currency & marketImpact list in the Debug Output window
                        Log.Debug("DEBUG_MainActivity", ": Currency & Market Impact Selected Display - Starts Here");
                        foreach (var item in currencies_selectedList)
                        {
                            Log.Debug("DEBUG", item);
                        }
                        foreach (var item in marketImpact_selectedList)
                        {
                            Log.Debug("DEBUG", item);
                        }
                        break;


                    case Resource.Id.mainActivity_bottom_toolbar_option_selectMarketImpacts:

                        using (var dialog = new Android.Support.V7.App.AlertDialog.Builder(this))
                        {
                            dialog.SetTitle(GetString(Resource.String.mainActivity_dialogOption_selectmarketimpacts));
                            dialog.SetPositiveButton(GetString(Resource.String.mainActivity_dialogOption_selectmarketimpacts_positiveButton), delegate
                            {
                                // clear list - get LINQ query result - populate list                                                              
                                tempNewsObjectDisplayList.Clear();
                                tempNewsObjectDisplayList = DataAccessHelpers.LINQ_SortAllByUserSelection(marketImpact_selectedList, currencies_selectedList);

                                // call populate adapter
                                PopulateNewsObjectAdapter();
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


                    //// old - not in use anymore - for demonstration purposes
                    //case Resource.Id.mainActivity_bottom_toolbar_option_sampleData:

                    //    // get sample data (from xml file in assets folder) & pass to method
                    //    XDocument xmlTestFile2 = XDocument.Load(Assets.Open("ff_calendar_thisweek.xml"));

                    //    tempNewsObjectDisplayList.Clear();
                    //    tempNewsObjectDisplayList = DataAccessHelpers.TestXMLDataFromAssetsFile(xmlTestFile2);

                    //    // call populate adapter
                    //    PopulateNewsObjectAdapter();
                    //    txtDataLastUpdated.Text = GetString(Resource.String.mainActivity_txt_dataLastUpdated_warningTestDataOnlyAll);
                    //    break;

                    //// old - not in use anymore - for demonstration purposes
                    //case Resource.Id.mainActivity_bottom_toolbar_option_sampleLinqQuery:

                    //    // get sample data (from xml file in assets folder) & pass to method & pass to LINQ query method
                    //    XDocument xmlTestFile1 = XDocument.Load(Assets.Open("ff_calendar_thisweek.xml"));
                    //    tempNewsObjectDisplayList.Clear();
                    //    tempNewsObjectDisplayList = DataAccessHelpers.TestLINQQueryUsingXML(xmlTestFile1);

                    //    // call populate adapter
                    //    PopulateNewsObjectAdapter();
                    //    txtDataLastUpdated.Text = GetString(Resource.String.mainActivity_txt_dataLastUpdated_WarningTestDataOnlyQuery);
                    //    break;                    

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
                    Log.Debug("DEBUG_MainActivity_DisplayCurrencies", item);
                }
            }


            void DebugDisplayMarketImpacts()
            {
                // displays marketImpacts (selected) to debug output
                Log.Debug("DEBUG", ": Market Impacts Selected Display - Starts Here");
                foreach (var item in marketImpact_selectedList)
                {
                    Log.Debug("DEBUG_MainActivity_DisplayMarketImpacts", item);
                }
            }


           
        }// end onCreate 



        public override void OnBackPressed()
        {
            GetAndDisplayDefaultData();
            base.OnBackPressed();
            GetAndDisplayDefaultData();
        }

       

        protected override void OnResume()
        {   // this seems to work when OnBackPressed() doesn't !!
            GetAndDisplayDefaultData();
            base.OnResume();
            GetAndDisplayDefaultData();
        }



      





        private void MAdapter_ItemClick1(object sender, int e)
        {
            // alert dialog for ItemClick event
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            // using this disable array of menu options - good for Ok/Cancel version
            builder.SetMessage(GetString(Resource.String.mainActivity_mAdapterItemClick_message));

            builder.SetPositiveButton(GetString(Resource.String.mainActivity_mAdapterItemClick_positiveButton), (sender2, e2) =>
            {
                // call Property in UserAlertActivity to pass data across (newsObject)
                UserAlertsActivity.SelectedNewsObject_PassedFrom_MainActivity = (newsObjectDisplayList[e]);

                // call intent to start next activity
                Intent intent = new Intent(this, typeof(UserAlertsActivity));
                StartActivity(intent);
            });

            builder.SetNegativeButton(GetString(Resource.String.mainActivity_mAdapterItemClick_negativeButton), (sender2, e2) =>
            {
                Log.Debug("DEBUG", "Cancel clicked");
            });

            var alert = builder.Create();
            alert.Show();
        }



        // TOP Toolbar
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.mainActivity_topMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }


        public void UpdateXML_Option()
        {
            DataAccess.DataAccessHelpers.DropNewsObjectTable();

            bool dataUpdateSuccessful = DataAccess.DataAccessHelpers.DownloadNewXMLAndStoreInDatabase();
            if (dataUpdateSuccessful)
            {
                Log.Debug("DEBUG", "\n\n\n" 
                    + GetString(Resource.String.mainActivity_top_toolbar_dataUpdate)
                    + dataUpdateSuccessful + "\n\n\n");

                // store date & time of xml download in Shared Preferences
                DateTime dateTime = DateTime.Now;
                SharedPreferencesMethods mySharedPreferencesMethods = new SharedPreferencesMethods(this);
                string dateInPreferedFormat = dateTime.ToShortDateString();
                mySharedPreferencesMethods.StoreToSharedPrefs(dateInPreferedFormat);
                txtDataLastUpdated.Text = "Data Updated: " + dateInPreferedFormat;
            }
            else
                txtDataLastUpdated.Text = GetString(Resource.String.mainActivity_txt_dataNotUpdated);

            // clear List & get raw newsObject data from database  
            tempNewsObjectDisplayList.Clear();
            tempNewsObjectDisplayList = DataAccessHelpers.GetAllNewsObjectDataFromDatabase();

            // call populate adapter
            PopulateNewsObjectAdapter();
            RefreshTxtDataLastUpdated();
        }


        // TOP Toolbar ('menu options')
        public override bool OnOptionsItemSelected(IMenuItem item) 
        {
            switch (item.ItemId)
            {
                case Resource.Id.mainActivity_top_toolbar_option_updateXML:

                    UpdateXML_Option();
                    break;

                case Resource.Id.mainActivity_top_toolbar_option_userAlertsActivity:
                    Intent intent = new Intent(this, typeof(UserAlertsActivity));
                    StartActivity(intent);
                    break;

                case Resource.Id.mainActivity_top_toolbar_option_preferences:
                    intent = new Intent(this, typeof(PreferencesActivity));
                    StartActivity(intent);                    
                    break;

                case Resource.Id.mainActivity_top_toolbar_option_reports:
                    Toast.MakeText(this, GetString(Resource.String.generalMessage_forFutureDevelopment), ToastLength.Long).Show();
                    break;

                //// old - not in use anymore - for demonstration purposes
                //case Resource.Id.mainActivity_top_toolbar_option_alertsOldVersion:
                //    intent = new Intent(this, typeof(PersonalAlarmsActivity_OldVersion));
                //    StartActivity(intent);
                //    break;

                //// old - not in use anymore - for demonstration purposes
                //case Resource.Id.mainActivity_top_toolbar_option_customAdapter:                    
                //    intent = new Intent(this, typeof(NewsObject_CustomAdapter_Test_Activity));
                //    StartActivity(intent);
                //    break;              

                default:
                    break;
            };
            return base.OnOptionsItemSelected(item);
        }


        void PopulateNewsObjectAdapter()
        {
            // Refresh adapter by running a 'forEach' through tempList to 
            // repopulate the same DisplayListObect that adapter has memory reference to  
            newsObjectDisplayList.Clear();
            foreach (var item in tempNewsObjectDisplayList)
            {
                newsObjectDisplayList.Add(item);
            }
            mAdapter.NotifyDataSetChanged();
        }

        void RefreshTxtDataLastUpdated()
        {
            SharedPreferencesMethods mySharedPreferencesMethods = new SharedPreferencesMethods(this);
            string dateXmlUpdated = mySharedPreferencesMethods.GetDataFromSharedPrefs();

            txtDataLastUpdated.Text = GetString(Resource.String.mainActivity_txt_dataLastUpdated) + " " + dateXmlUpdated
                + "\n" + GetString(Resource.String.mainActivity_txt_timeOffsetMessage)  + " "
                +  DataAccessHelpers.TimeToGoOffBeforeMarketAnnouncement + "  minutes";
        }


        void GetAndDisplayDefaultData()
        {
            // clear List & get raw newsObject data from database  
            tempNewsObjectDisplayList.Clear();
            tempNewsObjectDisplayList = DataAccessHelpers.GetAllNewsObjectDataFromDatabase();

            // call populate adapter
            PopulateNewsObjectAdapter();
            RefreshTxtDataLastUpdated();
        }
    }
}