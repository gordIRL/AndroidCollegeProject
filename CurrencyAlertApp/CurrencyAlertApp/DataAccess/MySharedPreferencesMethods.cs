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
    public class MySharedPreferencesMethods
    {
        Context context;

        public MySharedPreferencesMethods(Context context)
        {
            this.context = context;
        } 

        public string FirstMethod()
        {
            return "String returned";
        }


        public bool StoreToSharedPrefs(string input)
        {
            bool storedSuccessful = false;
            try
            {
                ISharedPreferences prefs = context.GetSharedPreferences("MyPreferences", FileCreationMode.Private);
                ISharedPreferencesEditor editor = prefs.Edit();

                editor.PutString("MyKey", input);
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
            if (prefs.Contains("MyKey"))
            {
                stringToReturn = prefs.GetString("MyKey", "No data here !!");                
            }            
            return stringToReturn;
        }



    }
}