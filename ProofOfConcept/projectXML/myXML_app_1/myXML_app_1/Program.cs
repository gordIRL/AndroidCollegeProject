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
            // select xml file to read
            XDocument xmlFile = XDocument.Load("ff_calendar_thisweek.xml");


            Console.WriteLine("\n\nView original XML data - Press ENTER");
            Console.ReadLine();
            // display xml file - original format, includes tags etc
            Console.WriteLine(xmlFile.Root.ToString());
            

            Console.WriteLine("\n\nView raw XML data - Press ENTER");
            Console.ReadLine();
            //displays all raw data in XML file
            foreach (var item in xmlFile.Root.Elements())
            {
                Console.WriteLine(item.Value.Trim() + "  ");
            }



            Console.WriteLine("\n\nView XML data - formatted - Press ENTER");
            Console.ReadLine();          
            // display all XML data - formatted             
            foreach (var item in xmlFile.Descendants("event"))
            {
                Console.WriteLine(
                                    item.Element("country").Value + ":  \t" +
                                    item.Element("impact").Value + ":    \t" +
                                    item.Element("date").Value + ":    \t" +
                                    item.Element("time").Value + ":    \t" +
                                    item.Element("title").Value
                                  );                         // .Value - removes surrounding tags
            }



            Console.WriteLine("\n\nView LINQ queried XML data - Press ENTER");
            Console.ReadLine();
            // sample selection using LINQ - GBP & USD currencies with 'High' impact status
            var highestImpact = from myVar in xmlFile.Descendants("event")
                                where myVar.Element("impact").Value == "High" &&
                                (myVar.Element("country").Value == "GBP"
                                || myVar.Element("country").Value == "USD")
                                select myVar;


            // display the result of LINQ query
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
            // Pause before ending program
            Console.WriteLine("\n\nExit Program - Press ENTER");
            Console.ReadLine();
        }
    }
}
