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
using SQLite;

namespace CurrencyAlertApp.DataAccess
{
    [Table("NewsObject")]
    public class NewsObject
    {
        [PrimaryKey, AutoIncrement]
        public int NewsObjectID{ get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(10)]
        public string CountryChar { get; set; }

        [MaxLength(10)]
        public string MarketImpact { get; set; }       

        [Ignore]
        // DateTime object not supported by SQLite
        public DateTime DateAndTime { get; set; }        

        public long DateInTicks { get; set; }


        public override string ToString()
        {
            DateTime tempDateTime = new DateTime(DateInTicks);
            string tempDate = tempDateTime.ToShortDateString();
            string tempTime = tempDateTime.ToShortTimeString();

            return string.Format("ID:{0} {1} {2} {3}\nDate: {4}  Time: {5}",
                NewsObjectID, Name, CountryChar, MarketImpact,
                tempDate, tempTime);   // DateInTicks, DateInTicks);
        }
    }
}