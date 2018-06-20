using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Content;
using Android.Views;
using System.Collections.Generic;

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

namespace CurrencyAlertApp
{
    [Activity(Theme = "@style/MyTheme.Base", MainLauncher = true, Label = "CurrencyAlertApp")]
    //  MainLauncher = true,  
    // must have an appCompat theme         

    public class MainActivity : AppCompatActivity  
    {
        Button MyButton;
        ListView listView1;
        List<string> myList = new List<string>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "My AppCompat Toolbar";

            var editToolbar = FindViewById<Toolbar>(Resource.Id.edit_toolbar);
            editToolbar.Title = "Editing";
            editToolbar.InflateMenu(Resource.Menu.edit_menus);
            editToolbar.MenuItemClick += (sender, e) =>
            {
                Toast.MakeText(this, "Bottom toolbar tapped: " + e.Item.TitleFormatted, ToastLength.Short).Show();
            };


            MyButton = FindViewById <Button>(Resource.Id.MyButton);
            MyButton.Click += delegate
            {
                Intent intent = new Intent(this, typeof(GordTestActivity));
                StartActivity(intent);
            };



            listView1 = FindViewById<ListView>(Resource.Id.listView1);

            // set up list    
            SetUpData();
            //for (int i = 0; i < 20; i++)
            //{
            //    myList.Add("Item no: " + i.ToString());
            //}

            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, myList);
            listView1.Adapter = adapter;
           
            listView1.ItemClick += (sender, e) =>
            {
                Toast.MakeText(this, $"You selected item no: {e.Position}:\n{myList[e.Position]}", ToastLength.Short).Show();
            };
        }// end onCreate

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted, ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
        }

        void SetUpData()
        {
            // select xml file to read
            XDocument xmlFile = XDocument.Load(Assets.Open("ff_calendar_thisweek.xml"));

            ////displays all raw data in XML file
            //foreach (var item in xmlFile.Root.Elements())
            //{
            //    myList.Add(item.Value.Trim() + "");
            //}

            // display all XML data - formatted             
            foreach (var item in xmlFile.Descendants("event"))
            {
                myList.Add(
                            item.Element("country").Value + "\n" +
                            item.Element("impact").Value + "\n" +
                            item.Element("date").Value + "\n" +
                            item.Element("time").Value + "\n" +
                            item.Element("title").Value
                            ); // .Value - removes surrounding tags
            }




            ////// sample selection using LINQ - GBP & USD currencies with 'High' impact status
            ////var highestImpact = from myVar in xmlFile.Descendants("event")
            ////                    where myVar.Element("impact").Value == "High" &&
            ////                    (myVar.Element("country").Value == "GBP"
            ////                    || myVar.Element("country").Value == "USD")
            ////                    select myVar;


            ////// display the result of LINQ query
            ////foreach (var item in highestImpact)
            ////{
            ////    myList.Add(
            ////               item.Element("country").Value + "\n" +
            ////               item.Element("impact").Value + "\n" +
            ////               item.Element("date").Value + "\n" +
            ////               item.Element("time").Value + "\n" +
            ////               item.Element("title").Value
            ////               ); // .Value - removes surrounding tags
            ////}




        }// end SetUpData
    }
}

