using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;

//  ??
using Android.Support.V7.Widget;




namespace CurrencyAlertApp
{
    [Activity(Theme = "@style/MyTheme.Base", Label = "CurrencyAlertApp")]  // removed (MainLauncher = true)  // must have an appCompat theme
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
        }
    }
}

