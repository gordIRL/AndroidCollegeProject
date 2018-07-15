using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
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
        static string DBLocation = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "NewsObjects.db3");


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
                XDocument xmlFile = XDocument.Load("https://cdn-nfs.forexfactory.net/ff_calendar_thisweek.xml");
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
             
                
        public static List<string> GetLINQResultData()
        {
            // LINQ queries (direct from database data)
            newsObjectsList.Clear();

            // call to database & populate newsObjectList with the result
            GetAllRawDataFromDatabase();

            List<string> linqQueryResultsList = new List<string>();

            // sample selection using LINQ - GBP & USD currencies with 'High' impact status
            var highestImpact = from myVar in newsObjectsList
                                where myVar.MarketImpact == "High" &&
                                (myVar.CountryChar == "GBP")    // || myVar.CountryChar == "USD")
                                select myVar;

            // Store result of query in List and Return it
            foreach (var item in highestImpact)
            {
                linqQueryResultsList.Add(
                       item.CountryChar + ":    " +
                       item.MarketImpact + "\n" +
                       item.DateOnly + ":    " +
                       item.TimeOnly + "\n" +
                       item.Name);

                // get dateTime object by calling method combing date(string) and time(string)
                DateTime dateAndTimeObject = ConvertString_s_ToDateTimeObject(item.DateOnly.ToString(), item.TimeOnly.ToString());
            }
            return linqQueryResultsList;   // returning a List<String> ..... eventually will be List<newsObject> !!!!
        }





        //public static List<string> GetLINQResultData2(string[] marketImpact_selectedList, string[] currencies_selectedList)
        public static List<string> GetLINQResultData2(List<string> marketImpact_selectedList, List<string> currencies_selectedList)
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
                            "Some text!!!!" + 
                               linqResultItem.CountryChar + ":    " +
                               linqResultItem.MarketImpact + "\n" +
                               linqResultItem.DateOnly + ":    " +
                               linqResultItem.TimeOnly + "\n" +
                               linqResultItem.Name);

                        // get dateTime object by calling method combing date(string) and time(string)
                        DateTime dateAndTimeObject = ConvertString_s_ToDateTimeObject(linqResultItem.DateOnly.ToString(), linqResultItem.TimeOnly.ToString());

                    }// end inner foreach
                }// end outer foreach         
            }
            //return list
            linqQueryResultsList.Add("Some text!!!!");
            return linqQueryResultsList;   // returning a List<String> ..... eventually will be List<newsObject> !!!!
        }














        // creates a list of numbers to display in Main Activity
        public static List<string> NoDataToDisplay()
        {
            List<string> listToReturn = new List<string>();

            // set up tempory list for listView            
            for (int i = 0; i < 20; i++)
            {
                listToReturn.Add("Item no: " + i.ToString() + " - no data");
            }
            return listToReturn;
        }
    }//
}//