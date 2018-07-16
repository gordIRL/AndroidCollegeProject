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

namespace CurrencyAlertApp.DataAccess
{
    public class SetUpData
    {
        // list to store newsObjects retrieved from database
        static List<NewsObject> newsObjectsList = new List<NewsObject>();       


        // location of database
        static string DBLocation = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "CurrencyAlertApp.db3");

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
            using(SQLiteConnection conn = new SQLiteConnection(DBLocation))
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
                string url = conn.Get<URLObject>(1).URLAddress;  // 1st Database is at '1' - ie not zero-based like arrays !!                
                Log.Debug("DEBUG", "URL in DB: " + url);
                return url;
            }
        }




        public static DateTime ConvertString_s_ToDateTimeObject(string dateString, string timeString)
        {
            // Returns a DateTime object from a date(string) and a time(string)

            string dateAndTimeString = dateString + " " + timeString;
            DateTime dateAndTimeObject = DateTime.Parse(dateAndTimeString);
            return dateAndTimeObject;
        }



        public static bool DownloadNewXMLAndStoreInDatabase()
        {
            // download external XML file hosted on public website 

            bool dataUpdateSuccessful = false;

            try
            {
                //XDocument xmlFile = XDocument.Load("https://cdn-nfs.forexfactory.net/ff_calendar_thisweek.xml");
                XDocument xmlFile = XDocument.Load(GetURLForXMLDownloadFromDatabase());
                dataUpdateSuccessful = true;
                Log.Debug("DEBUG", "XML data downloaded - SUCCESS");

                // convert XML and store in database
                ConvertXmlAndStoreInDatabase(xmlFile);
                Log.Debug("DEBUG", "XML data stored in database - SUCCESS");
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
                    // create a newsObject for every 'event' in xml file
                    NewsObject newsObject = new NewsObject
                    {
                        Name = item.Element("title").Value.TrimEnd(),          
                        // .Value - removes surrounding tags
                        CountryChar = item.Element("country").Value.TrimEnd(), 
                        MarketImpact = item.Element("impact").Value.TrimEnd(), 
                        DateOnly = item.Element("date").Value.TrimEnd(),       
                        TimeOnly = item.Element("time").Value.TrimEnd()                             
                    };  
                    // insert newsObject into database
                    conn.Insert(newsObject);
                }; 
            }
        }     
        

        public static List<NewsObject> GetAllRawDataFromDatabase()
        {
            newsObjectsList.Clear();

            using (SQLiteConnection conn = new SQLiteConnection(DBLocation))
            {
                // retrieve all data from database & store in list
                var retrievedDataList = conn.Table<NewsObject>();

                // store each item in list into a returnable list
                foreach (var item in retrievedDataList)
                {
                    newsObjectsList.Add(item);
                }                
            }// end USING          
            return newsObjectsList;
        }
                                
       
        public static List<string> GetAllDataFormattedIntoSingleString()
        {
            newsObjectsList.Clear();

            // call to database & populate newsObjectList with the result
            GetAllRawDataFromDatabase();

            List<string> allDataFormattedIntoString = new List<string>();

            foreach (var item in newsObjectsList) 
            {
                allDataFormattedIntoString.Add(string.Format(
                        "Date: {0} {1} \n{2} {3}\n{4}",
                        item.DateOnly.TrimEnd(),
                        item.TimeOnly.TrimEnd(),
                        item.CountryChar.TrimEnd(),
                        item.MarketImpact.TrimEnd(),
                        item.Name.TrimEnd()
                        ));               
            }
            return allDataFormattedIntoString;
        }
             
                
       
       
        public static List<string> LINQ_SortAllByUserSelection(List<string> marketImpact_selectedList, List<string> currencies_selectedList)
        {
            // LINQ queries (direct from database data)
            newsObjectsList.Clear();

            // call to database & populate newsObjectList with the result
            GetAllRawDataFromDatabase();

            List<string> linqQueryResultsList = new List<string>();


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

                    // Store result of LINQ query in List
                    foreach (var linqResultItem in tempLinqQueryList)
                    {
                        linqQueryResultsList.Add(
                               linqResultItem.CountryChar + ":    " +
                               linqResultItem.MarketImpact + "\n" +
                               linqResultItem.DateOnly + ":    " +
                               linqResultItem.TimeOnly + "\n" +
                               linqResultItem.Name);
                    }// end inner foreach
                }// end outer foreach         
            }
            //return list
            return linqQueryResultsList;   // returning a List<String> ..... eventually will be List<newsObject> !!!!
        }
        

        public static List<string> NoDataToDisplay()
        {   
            // display 'no data available' in Main Activity
            List<string> listToReturn = new List<string>();
            listToReturn.Add("No data available to display");
            return listToReturn;
        }


        public static List<string> TestXMLDataFromAssetsFile(XDocument xmlTestFile)
        {
            List<string> listToReturn = new List<string>();
            // next line won't unless - to pass an XDocument add reference:  System.Xml.Linq   !!!!
            //  declare Xdocument in Main Activity!!!!  XDocument xmlTestFile = XDocument.Load(Assets.Open("ff_calendar_thisweek.xml"));

            // all raw unformatted data in xml file
            listToReturn.Add("Unformatted Test Data");
            foreach (var item in xmlTestFile.Root.Elements())
            {
                listToReturn.Add(item.Value.Trim() + "\n");
            }
            // data retrieved indivudually from xml file
            listToReturn.Add("Individually Retrieved XML Test Data");
            foreach (var item in xmlTestFile.Descendants("event"))
            // 'event' is the surrounding xml <tag>
            {
                listToReturn.Add(
                    // uses xml <tag> names from xml file
                    item.Element("country").Value + "\n" +
                    item.Element("impact").Value + "\n" +
                    item.Element("date").Value + "\n" +
                    item.Element("time").Value + "\n" +
                    item.Element("title").Value + "\n"
                    ); // .Value - removes surrounding tags - giving only the value
            }
            return listToReturn;
        }


        public static List<string> TestLINQQueryUsingXML(XDocument xmlTestFile)
        {
            // LINQ queries (using xml file in Assets)           
            List<string> linqQueryResultsList = new List<string>();

            // sample selection using LINQ - GBP & USD currencies with 'High' impact status
            var highestImpact = from myVar in xmlTestFile.Descendants("event")
                                where myVar.Element("impact").Value == "High" &&
                                (myVar.Element("country").Value == "GBP" || myVar.Element("country").Value == "USD")
                                select myVar;

            // Store result of query in List and Return it
            linqQueryResultsList.Add("USD & GBP Result - HIGH");
            foreach (var item in highestImpact)
            {
                linqQueryResultsList.Add(
                    // uses xml <tag> names from xml file
                    item.Element("country").Value + "\n" +
                    item.Element("impact").Value + "\n" +
                    item.Element("date").Value + "\n" +
                    item.Element("time").Value + "\n" +
                    item.Element("title").Value + "\n"
                    );

                // ?? Not sure this is working properly - get dateTime object by calling method combing date(string) and time(string)
                //DateTime dateAndTimeObject = ConvertString_s_ToDateTimeObject(item.DateOnly.ToString(), item.TimeOnly.ToString());
            }
            return linqQueryResultsList;   
        }


    }//
}//