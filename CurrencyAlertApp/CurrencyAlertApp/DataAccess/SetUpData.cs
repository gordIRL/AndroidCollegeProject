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
using Android.Views;
using Android.Widget;

namespace CurrencyAlertApp.DataAccess
{
    public static class SetUpData
    {
        public static List<string> DefaultDataSetUp_ListOfNumbers()
        {
            List<string> listToReturn = new List<string>();

            // set up tempory list for listView            
            for (int i = 0; i < 20; i++)
            {
                listToReturn.Add("Item no: " + i.ToString());
            }
            return listToReturn;
        }



        public static List<string> GetAllRawData(XDocument xmlFile)
        {
            List<string> listToReturn = new List<string>();

            //// select xml file to read  - NOT WORKING !!            
            //XDocument xmlFile = XDocument.Load(Assets.Open("ff_calendar_thisweek.xml"));

            //// copied code:   Read the contents of our asset
            //string content;
            //AssetManager assets = this.Assets;
            //using (StreamReader sr = new StreamReader(assets.Open("read_asset.txt")))
            //{
            //   // content = sr.ReadToEnd();
            //}

            //displays all raw data in XML file            
            foreach (var item in xmlFile.Root.Elements())
            {
                listToReturn.Add(item.Value.Trim() + "");
            }
            return listToReturn;
        }

        public static List<string> GetAllDataFormatted(XDocument xmlFile)
        {
            List<string> listToReturn = new List<string>();
           
            foreach (var item in xmlFile.Descendants("event"))
            {
                listToReturn.Add(
                            item.Element("country").Value + "\n" +
                            item.Element("impact").Value + "\n" +
                            item.Element("date").Value + "\n" +
                            item.Element("time").Value + "\n" +
                            item.Element("title").Value
                            ); // .Value - removes surrounding tags               
            }
            return listToReturn;
        }


        public static List<string> GetLINQResultData(XDocument xmlFile)
        {
            List<string> listToReturn = new List<string>();

            // sample selection using LINQ - GBP & USD currencies with 'High' impact status
            var highestImpact = from myVar in xmlFile.Descendants("event")
                                where myVar.Element("impact").Value == "High" &&
                                (myVar.Element("country").Value == "GBP"
                                || myVar.Element("country").Value == "USD")
                                select myVar;

            foreach (var item in highestImpact)
            {
                listToReturn.Add(
                           item.Element("country").Value + "\n" +
                           item.Element("impact").Value + "\n" +
                           item.Element("date").Value + "\n" +
                           item.Element("time").Value + "\n" +
                           item.Element("title").Value);             
            }
            return listToReturn;
        }        
    }
}