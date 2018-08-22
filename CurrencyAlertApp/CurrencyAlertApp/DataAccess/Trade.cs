using System;

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