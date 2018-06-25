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
    public class Trade
    {
        public DateTime TradeOpened { get; set; }
        public DateTime TradeClosed { get; set; }
        public bool TradeDirectionLong { get; set; }
        public int DurationOfTrade { get; set; }
        public decimal AmountWonOrLost { get; set; }
        public bool TradeWasProfitable { get; set; }
    }
}