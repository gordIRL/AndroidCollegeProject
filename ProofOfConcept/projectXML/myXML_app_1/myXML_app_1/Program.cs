using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace myXML_app_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello world");

            XDocument xmlFile = XDocument.Load("ff_calendar_thisweek.xml");

            //Console.WriteLine(xmlFile.Root.ToString()); 

            // displays everything
            //foreach (var item in xmlFile.Root.Elements())
            //{
            //    Console.WriteLine(item.Value.Trim() + "  ");
            //}


            //// display all 
            ////foreach (var item in xmlFile.Descendants("event").Elements("title"))
            //foreach (var item in xmlFile.Descendants("event"))
            //{
            //    Console.WriteLine(
            //                        item.Element("country").Value  + ":  \t"  +
            //                        item.Element("impact").Value   + ":    \t"  +
            //                        item.Element("date").Value     + ":    \t" +
            //                        item.Element("time").Value     + ":    \t" +
            //                        item.Element("title").Value    
            //                      );                         // .Value - removes surrounding tags
            //}



            var highestImpact = from myVar in xmlFile.Descendants("event")
                                where myVar.Element("impact").Value == "High"     && 
                                (myVar.Element("country").Value == "GBP"
                                || myVar.Element("country").Value == "USD")
                                select myVar;

            foreach (var item in highestImpact)
            {
                Console.WriteLine(
                                    item.Element("country").Value + ":  \t" +
                                    item.Element("impact").Value + ":    \t" +
                                    item.Element("date").Value + ":    \t" +
                                    item.Element("time").Value + ":    \t" +
                                    item.Element("title").Value
                                  );                         // .Value - removes surrounding tags
            }




            Console.ReadLine();
        }
    }
}
