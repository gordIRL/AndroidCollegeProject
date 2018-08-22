using System;
using SQLite;

namespace CurrencyAlertApp.DataAccess
{
    [Table("UserAlert")]
    public class UserAlert
    {
        [PrimaryKey, AutoIncrement]
        public int UserAlertID { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(10)]
        public string CountryChar { get; set; }

        [MaxLength(10)]
        public string MarketImpact { get; set; }

        [Ignore] 
        // DateTime object not supported by SQLite
        public DateTime DateAndTime { get; set; }

        public long DateInTicks { get; set; }

        [MaxLength(200)]
        public string DescriptionOfPersonalEvent { get; set; }

        public bool IsPersonalAlert { get; set; }


        public override string ToString()
        {
            // this ToString is different from NewsObject.ToString - as it will receive proper C# DateTime object
            // because its data is coming from an existing newsObject - NOT from A DB call which only has Ticks(long)


            // convert DateInTicks to DateTimeObject - for display purposes only (ToString())
            DateAndTime = new DateTime(DateInTicks);

            return string.Format("ID:{0} {1} {2} {3}\nConvert from Ticks!!!!  Date: {4}  Time: {5}" +
                "\nIsPersonalAlert: {6}\nDescription: \n{7}\nTicks: {8}",
                UserAlertID, Title, CountryChar, MarketImpact,
                DateAndTime.ToString("dd/MM/yyyy"), DateAndTime.ToString("HH:mmtt"),
                IsPersonalAlert.ToString(), DescriptionOfPersonalEvent,
               DateInTicks);  
        }
    }
}