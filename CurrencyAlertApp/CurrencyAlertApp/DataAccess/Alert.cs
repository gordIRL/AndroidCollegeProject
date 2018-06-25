using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CurrencyAlertApp.DataAccess
{
    public class Alert
    {       
        public string NameOfNewsEvent { get; set; }     // e.g. Non Farm Payrolls (USD)
        public DateTime DateAndTimeOfNewsEvent { get; set; }
        public string Currency { get; set; }            // e.g. Euro
        public string Country { get; set; }             // e.g. France or Ireland
        public string MarketImpact { get; set; }        // e.g. High, Medium, Low  (use enum ?)
        public string Timezone { get; set; }
       
        public bool PersonalAlert { get; set; }
    }
}