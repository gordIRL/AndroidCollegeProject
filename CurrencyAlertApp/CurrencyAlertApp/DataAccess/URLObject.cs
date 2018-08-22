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