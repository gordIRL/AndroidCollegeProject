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
using Xamarin.Essentials;

namespace Vibrate1
{
    [Activity(Label = "Screen2Activity")]
    public class Screen2Activity : Activity
    {
        Button btnVB_On;
        Button btnVB_Off;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Screen2);

            // Create your application here
            btnVB_On = FindViewById<Button>(Resource.Id.btnSCVB_On);
            btnVB_Off = FindViewById<Button>(Resource.Id.btnSC2VB_Off);

            btnVB_On.Click += BtnVB_On_Click;
            btnVB_Off.Click += BtnVB_Off_Click;
        }

        private void BtnVB_Off_Click(object sender, System.EventArgs e)
        {
            try
            {
                Vibration.Cancel();
                Toast.MakeText(this, $"Vibration - cancelled", ToastLength.Short).Show();
            }
            catch (FeatureNotSupportedException ex)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        private void BtnVB_On_Click(object sender, System.EventArgs e)
        {
            try
            {
                Toast.MakeText(this, $"Vibration - ON !!", ToastLength.Short).Show();
                //// Use default vibration length
                //Vibration.Vibrate();

                // Or use specified time
                var duration = TimeSpan.FromSeconds(5);
                Vibration.Vibrate(duration);
            }
            catch (FeatureNotSupportedException ex)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }



        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


    }//
}//