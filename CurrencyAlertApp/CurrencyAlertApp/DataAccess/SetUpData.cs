using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
// to pass a XDocument add reference:    System.Xml.Linq.
using System.Xml;
using System.Xml.XPath;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using SQLite;
using System.Globalization;
// to pass a XDocument add reference:    System.Xml.Linq.



namespace CurrencyAlertApp.DataAccess
{
    public class SetUpData
    {
        // General Declarations:
        // Create a single CultureInfo object (once so it can be reused) for correct Parsing of strings to DateTime object
        static CultureInfo cultureInfo = new CultureInfo("en-US");

        // location of database
        static string DBLocation = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "CurrencyAlertApp.db3");

        // To use test data from xml file in asset - set 'testMode = true'
        static bool testMode = false;
        static XDocument xmlTestDataFile;

        //------------------------------------------

        // NewsObject Declarations:
        // list to store newsObjects retrieved from database
        static List<NewsObject> newsObjectsList = new List<NewsObject>();

        //-------------------------------------------------------------------


        // UserAlert Declarations:
        // list to store userAlert retrieved from database
        static List<UserAlert> userAlertList = new List<UserAlert>();


        //-------------------------------------------------------------------

        // UserAlert Methods

        // create empty table - for program load
        public static void CreateEmptyUserAlertTable()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DBLocation))
            {
                if (conn.Table<UserAlert>() == null)
                {                    
                    Log.Debug("DEBUG", "Created USER ALERT table - null instance detected");
                }
                else
                {
                    Log.Debug("DEBUG", "USER ALERT table already created");
                }
                conn.CreateTable<UserAlert>();
            }
        }



        // method to convert newsObject to userAlert object
        public static UserAlert ConvertNewsObjectToUserAlert(NewsObject newsObject)
        {
            UserAlert userAlert = new UserAlert
            {
                // don't assign ID - SQLite will do this automatically when object is inserted into DB
                Title = newsObject.Title,
                CountryChar = newsObject.CountryChar,
                MarketImpact = newsObject.MarketImpact,
                DateAndTime = newsObject.DateAndTime,
                DateInTicks = newsObject.DateInTicks,

                IsPersonalAlert = false,  // because this is converting a market event
                DescriptionOfPersonalEvent = string.Empty   
            };
            return userAlert;
        }

        //// method to convert personalAlert to userAlert object
        //public static UserAlert ConvertPersonalAlertToUserAlert(NewsObject newsObject)
        //{     //}





        /* 
         * method to add a single UserAlert to UserAlert to  
         * database - passed from Main Activity
         * -- ?? don't use to repopulate adapter from database ??  TICKS ??
        */
        public static bool AddNewUserAlertToDatabase(UserAlert userAlert)  
            {
            bool dataAddedToDatabase = false;
            using (SQLiteConnection conn = new SQLiteConnection(DBLocation))
            {  
                try
                {
                    // insert new UserAlert into database
                    conn.Insert(userAlert);  // this won't insert the 'ignored' C# DateTime into the database !! (only TICKS stored)
                    dataAddedToDatabase = true;
                    Log.Debug("DEBUG", "INSERTED Single User Alert:\n" + userAlert.ToString());
                }
                catch
                {
                    dataAddedToDatabase = false;
                }
                // set breakpoint here when required
                Log.Debug("DEBUG", "No of items in database: " + conn.Table<UserAlert>().Count().ToString());
                Log.Debug("DEBUG", "FINISHED\n\n\n");
            }// end using   
            return dataAddedToDatabase;
        }// end  AddNewUserAlertToDatabase()

        

        public static List<UserAlert> GetAllUserAlertDataFromDatabase()
        {
            // this populates the userAlertList &&&& returns it
            List<UserAlert> tempUserAlertsList = new List<UserAlert>();

            userAlertList.Clear();

            using (SQLiteConnection conn = new SQLiteConnection(DBLocation))
            {
                // retrieve all data from database & store in list
                var retrievedDataList = conn.Table<UserAlert>();

                // store each item in list into a returnable list
                foreach (var item in retrievedDataList)
                {
                    // convert DateInTicks to DateTimeObject 
                    item.DateAndTime = new DateTime(item.DateInTicks);

                    //userAlertList.Add(item);
                    tempUserAlertsList.Add(item);
                }
            }// end USING 

            // sort list by DateTime object



            // sort list by long DateInTicks
            var sortedUserAlertList = from myvar in tempUserAlertsList
                                       orderby myvar.DateInTicks
                                       select myvar;


            // convert 'var' List into 'real' List
            //List<NewsObject> finalNewsObjectList = new List<NewsObject>();
            foreach (var item in sortedUserAlertList)
            {
                userAlertList.Add(item);
            }
            return userAlertList;
        }





        public static int DeleteSelectedUserAlert(int userAlertID)
        {
            int rowCount = 0;

            using (SQLiteConnection conn = new SQLiteConnection(DBLocation))
            {
                rowCount = conn.Delete<UserAlert>(userAlertID);
            }
            return rowCount;
        }



        //---------------------------------------------------------------------------------------------------------------

        // method to populate Table<UserAlert> with dummy data  -- use to add data passed from Main Activity
        //-- ?? don't use to repopulate adapter from database ??  TICKS ??
        public static void PopulateUserAlertTableWithDummyData()
        {
            List<UserAlert> userAlertsList = new List<UserAlert>();
            userAlertsList = SetUpData.DummyDataForUserAlert();

            using (SQLiteConnection conn = new SQLiteConnection(DBLocation))
            {
                conn.DropTable<UserAlert>();
                conn.CreateTable<UserAlert>();

                foreach (var tempUserAlert in userAlertsList)
                {
                    // insert current UserAlert into database
                    conn.Insert(tempUserAlert);  // this won't insert the 'ignored' C# DateTime into the database !! (only TICKS stored)

                    Log.Debug("DEBUG", "INSERTED:\n" + tempUserAlert.ToString());
                }// end foreach

                // list of items currently in the database 
                var retrievedDataList = conn.Table<UserAlert>();

                // set breakpoint here when required
                Log.Debug("DEBUG", "FINISHED\n\n\n");


                // Display what's currently in the table
                foreach (var item in retrievedDataList)
                {
                    Log.Debug("DEBUG", item.ToString());
                }
            }// end using          
        }// end  PopulateUserAlertTableWithDummyData()


        public static List<UserAlert> DummyDataForUserAlert()
        {
            UserAlert userAlert1 = new UserAlert()
            {
                UserAlertID = 1,
                IsPersonalAlert = false,
                DateAndTime = new DateTime(2018, 1, 15, 10, 15, 0),
                DateInTicks = 636516081000000000,
                CountryChar = "USD",
                MarketImpact = "High",
                Title = "Non Farm Payroll"
            };
            UserAlert userAlert2 = new UserAlert()
            {
                UserAlertID = 2,
                IsPersonalAlert = false,
                DateAndTime = new DateTime(2018, 2, 20, 13, 20, 0),
                DateInTicks = 636547296000000000,
                CountryChar = "GBP",
                MarketImpact = "Low",
                Title = "Meeting 1"
            };
            UserAlert userAlert3 = new UserAlert()
            {
                UserAlertID = 3,
                IsPersonalAlert = false,
                DateAndTime = new DateTime(2018, 3, 3, 14, 30, 0),
                DateInTicks = 636556842000000000,
                CountryChar = "EUR",
                MarketImpact = "Medium",
                Title = "Interest Rate"
            };
            UserAlert userAlert4 = new UserAlert()
            {
                UserAlertID = 4,
                IsPersonalAlert = false,
                DateAndTime = new DateTime(2018, 8, 6, 11, 00, 0),
                DateInTicks = 636691500000000000,
                CountryChar = "AUD",
                MarketImpact = "Low",
                Title = "Car Sales monthly"
            };
            // Create UserAlert List
            List<UserAlert> userAlertsList = new List<UserAlert>();

            // Add dummy data to User Alert List
            userAlertsList.Add(userAlert1);
            userAlertsList.Add(userAlert2);
            userAlertsList.Add(userAlert3);
            userAlertsList.Add(userAlert4);
            
            // Return List
            return userAlertsList;
        }// end DummyDataForUserAlert
         //-----------------------------------------------------------------------------------------------

















        //-------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Everything below is the original SetUp Data before work started on UserAlert Section  !!!!
        /// </summary>
        /// 

        // create empty table - for program load
        public static void CreateEmptyTable()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DBLocation))
            {
                if (conn.Table<NewsObject>() == null)
                {
                    Log.Debug("DEBUG", "Created new table - null instance detected");
                }
                conn.CreateTable<NewsObject>();
            }
        }


        public static void CreateTableForURLDownload()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DBLocation))
            {
                conn.DropTable<URLObject>();
                conn.CreateTable<URLObject>();
                URLObject urlForexFactoryXmlDownload = new URLObject { URLAddress = "https://cdn-nfs.forexfactory.net/ff_calendar_thisweek.xml" };
                conn.Insert(urlForexFactoryXmlDownload);
            }
        }

        public static string GetURLForXMLDownloadFromDatabase()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DBLocation))
            {
                string url = conn.Get<URLObject>(1).URLAddress;  // 1st Database item is at '1' - ie not zero-based like arrays !!                
                Log.Debug("DEBUG", "URL in DB: " + url);
                return url;
            }
        }

        public static DateTime ConvertString_s_ToDateTimeObject(string dateString, string timeString, CultureInfo cultureInfo)
        {
            // Returns a DateTime object from combining a date(string) and a time(string)
            // - used to convert the xml format date (string) and time (string) into a C# DateTime object

            string dateAndTimeString = dateString +" " +  timeString;

            // original version - working - but would create a new CultureInfo object on each instance
            //DateTime dateAndTimeObject = DateTime.Parse(dateAndTimeString, new CultureInfo("en-US"));

            // final version - working - uses cultureInfo instantiated at class level (line 28/29 above)
            DateTime dateAndTimeObject = DateTime.Parse(dateAndTimeString, cultureInfo);

            return dateAndTimeObject;
        }


        public static void GetTestXmlFileFromMainActivity(XDocument xmlTestDataFileInput)
        {
            xmlTestDataFile = xmlTestDataFileInput;            
        }


        public static bool DownloadNewXMLAndStoreInDatabase()
        {
            // download external XML file hosted on public website 
            bool dataUpdateSuccessful = false;

            try
            {
                // bool testMode = true;       
           
                if(testMode == false)
                {
                    // convert XML and store in database
                    XDocument xmlFile = XDocument.Load(GetURLForXMLDownloadFromDatabase());
                    Log.Debug("DEBUG", "XML data downloaded - SUCCESS");

                    ConvertXmlAndStoreInDatabase(xmlFile);
                    Log.Debug("DEBUG", "XML data stored in database - SUCCESS");
                    dataUpdateSuccessful = true;
                }
                if (testMode == true)
                {
                    // get sample data (from xml file in assets folder) & pass to method
                    //XDocument  xmlTestDataFile_notWorkingVersion = XDocument.Load(Assets.Open("ff_calendar_thisweek.xml")); 

                    ConvertXmlAndStoreInDatabase(xmlTestDataFile);
                    Log.Debug("DEBUG", "!!!!! XML  TEST-Data     stored in database - SUCCESS");
                    dataUpdateSuccessful = true;
                }
            }
            catch
            {
                dataUpdateSuccessful = false;
                Log.Debug("DEBUG", "FAIL - XML data not downloaded");
            }
            return dataUpdateSuccessful;
        }

        public static void DropNewsObjectTable()
        {
            newsObjectsList.Clear();

            using (SQLiteConnection conn = new SQLiteConnection(DBLocation))
            {
                conn.DropTable<NewsObject>();
                conn.CreateTable<NewsObject>();
            }
        }


        public static void ConvertXmlAndStoreInDatabase(XDocument xmlFile)
        {
            // converts downloaded xml data & add to database
            newsObjectsList.Clear();

            using (SQLiteConnection conn = new SQLiteConnection(DBLocation))
            {
                conn.DropTable<NewsObject>();
                conn.CreateTable<NewsObject>();

                foreach (var item in xmlFile.Descendants("event"))
                {
                    // get date & time values from XML (string)
                    string dateOnly = item.Element("date").Value.TrimEnd();
                    string timeOnly = item.Element("time").Value.TrimEnd();

                    // convert date & time strings to DateTime object
                    DateTime tempDateTime = ConvertString_s_ToDateTimeObject(dateOnly, timeOnly, cultureInfo);

                    // convert DateTime object to a Long of ticks
                    long dateTimeInTicks = tempDateTime.Ticks;

                    // create a newsObject for every 'event' in xml file
                    NewsObject newsObject = new NewsObject
                    {
                        Title = item.Element("title").Value.TrimEnd(),
                        // .Value - removes surrounding tags
                        CountryChar = item.Element("country").Value.TrimEnd(),
                        MarketImpact = item.Element("impact").Value.TrimEnd(),
                        DateInTicks = dateTimeInTicks
                    };
                    // insert newsObject into database
                    conn.Insert(newsObject);
                };
            }
        }


        public static List<NewsObject> GetAllNewsObjectDataFromDatabase()
        {
            // this populates the newsObjectList &&&& returns it
            newsObjectsList.Clear();

            using (SQLiteConnection conn = new SQLiteConnection(DBLocation))
            {
                // retrieve all data from database & store in list
                var retrievedDataList = conn.Table<NewsObject>();

                // store each item in list into a returnable list
                foreach (var item in retrievedDataList)
                {
                    // convert DateInTicks to DateTimeObject 
                    item.DateAndTime = new DateTime(item.DateInTicks);

                    newsObjectsList.Add(item);
                }
            }// end USING          
            return newsObjectsList;
        }



        public static List<NewsObject> LINQ_SortAllByUserSelection(List<string> marketImpact_selectedList, List<string> currencies_selectedList)
        {
            List<NewsObject> tempNewsObjectsList = new List<NewsObject>();
            
            // call to database & populate newsObjectList with the result 
            newsObjectsList.Clear();
            GetAllNewsObjectDataFromDatabase();  // returns List<NewsObject>  &&&& populates newsObjectList declared above (YES)            

            // loop through MarketImpact List (ie. act on each - all HIGH, all Medium, all Low events)
            foreach (var marketImpactSelectedItem in marketImpact_selectedList)
            {
                foreach (var currencySelectedItem in currencies_selectedList)
                {
                    // use LINQ to get all the selected currencies within the current MarketImpact
                    var tempLinqQueryList = from myVar in newsObjectsList
                                            where myVar.MarketImpact == marketImpactSelectedItem.ToString() &&
                                            myVar.CountryChar == currencySelectedItem.ToString()
                                            select myVar;

                    // add each newsobject from linq query to object list
                    foreach (var linqResultItem in tempLinqQueryList)
                    {
                        tempNewsObjectsList.Add(linqResultItem);
                    }
                }// end inner foreach
            } // end outer foreach 

            // sort list by long DateInTicks
            var sortedNewsObjectList = from myvar in tempNewsObjectsList
                                       orderby  myvar.DateInTicks
                                       select myvar;

            // convert 'var' List into 'real' List
            List<NewsObject> finalNewsObjectList = new List<NewsObject>();
            foreach (var item in sortedNewsObjectList)
            {
                finalNewsObjectList.Add(item);
            }
            return finalNewsObjectList;
        }



        public static List<NewsObject> TestXMLDataFromAssetsFile(XDocument xmlTestFile)
        {
            List<NewsObject> listToReturn = new List<NewsObject>();
            // next line won't unless - to pass an XDocument add reference:  System.Xml.Linq   !!!!
            //  declare Xdocument in Main Activity!!!!  XDocument xmlTestFile = XDocument.Load(Assets.Open("ff_calendar_thisweek.xml"));

            //// 'Root.Elements' version - (both versions work) - all raw unformatted data in xml file            
            //foreach (var item in xmlTestFile.Root.Elements())
            //{    OR.................  (other version)


            // 'Descendants' version (both versions work) - data retrieved indivudually from xml file            
            foreach (var item in xmlTestFile.Descendants("event"))
            // 'event' is the surrounding xml <tag>
            {
                // get date & time values from XML (string)
                string dateOnly = item.Element("date").Value.TrimEnd();
                string timeOnly = item.Element("time").Value.TrimEnd();

                // convert date & time strings to DateTime object
                DateTime tempDateTime = ConvertString_s_ToDateTimeObject(dateOnly, timeOnly, cultureInfo);

                // convert DateTime object to a Long of ticks
                long dateTimeInTicks = tempDateTime.Ticks;

                NewsObject tempNewsObject = new NewsObject();
                // assign xml values to newsObject - uses xml <tag> names from xml file
                tempNewsObject.Title = item.Element("title").Value;
                tempNewsObject.CountryChar = item.Element("country").Value;
                tempNewsObject.MarketImpact = item.Element("impact").Value;  // .Value - removes surrounding tags - giving only the value 
                tempNewsObject.DateAndTime = tempDateTime;  
                tempNewsObject.DateInTicks = dateTimeInTicks;  // ticks aren't really needed here as these objects won't be stored in the database

                // add the tempNewsObject to list to return
                listToReturn.Add(tempNewsObject);
            }
            return listToReturn;
        }



        public static List<NewsObject> TestLINQQueryUsingXML(XDocument xmlTestFile)
        {
            // LINQ queries (using xml file in Assets)           
            List<NewsObject> linqQueryResultsList = new List<NewsObject>();

            // sample selection using LINQ - GBP & USD currencies with 'High' impact status
            var highestImpact = from myVar in xmlTestFile.Descendants("event")
                                where myVar.Element("impact").Value == "High" &&
                                (myVar.Element("country").Value == "GBP" || myVar.Element("country").Value == "USD")
                                select myVar;

            foreach (var item in highestImpact)
            {
                // get date & time values from XML (string)
                string dateOnly = item.Element("date").Value.TrimEnd();
                string timeOnly = item.Element("time").Value.TrimEnd();

                // convert date & time strings to DateTime object
                DateTime tempDateTime = ConvertString_s_ToDateTimeObject(dateOnly, timeOnly, cultureInfo);

                // convert DateTime object to a Long of ticks
                long dateTimeInTicks = tempDateTime.Ticks;

                NewsObject tempNewsObject = new NewsObject();
                // assign xml values to newsObject - uses xml <tag> names from xml file
                tempNewsObject.Title = item.Element("title").Value;
                tempNewsObject.CountryChar = item.Element("country").Value;
                tempNewsObject.MarketImpact = item.Element("impact").Value;  // .Value - removes surrounding tags - giving only the value
                tempNewsObject.DateAndTime = tempDateTime;
                tempNewsObject.DateInTicks = dateTimeInTicks;  // ticks aren't really needed here as these objects won't be stored in the database

                // add the tempNewsObject to list to return
                linqQueryResultsList.Add(tempNewsObject);
            }
            return linqQueryResultsList;
        }
    }//
}//
   