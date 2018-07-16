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
    [Table("URLObject")]
    public class URLObject
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [MaxLength(200)]
        public string URLAddress { get; set; }
    }
}