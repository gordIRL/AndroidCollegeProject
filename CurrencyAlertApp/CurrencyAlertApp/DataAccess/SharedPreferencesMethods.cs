using Android.Content;


namespace CurrencyAlertApp.DataAccess
{
    public class SharedPreferencesMethods
    {
        Context context;

        public SharedPreferencesMethods(Context context)
        {
            this.context = context;
        } 
               

        public bool StoreToSharedPrefs(string input)
        {
            bool storedSuccessful = false;
            try
            {
                ISharedPreferences prefs = context.GetSharedPreferences("MyPreferences", FileCreationMode.Private);
                ISharedPreferencesEditor editor = prefs.Edit();

                editor.PutString("DateUpdated", input);
                editor.Commit();
                storedSuccessful = true;
            }
            catch
            {
                storedSuccessful = false;
            }           
            return storedSuccessful;
        }


        public string GetDataFromSharedPrefs()
        {
            string stringToReturn = "no data found";

            ISharedPreferences prefs = context.GetSharedPreferences("MyPreferences", FileCreationMode.Private);
            if (prefs.Contains("DateUpdated"))
            {
                stringToReturn = prefs.GetString("DateUpdated", "No data here !!");                
            }            
            return stringToReturn;
        }
    }
}