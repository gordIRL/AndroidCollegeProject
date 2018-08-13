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
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Util;
using CurrencyAlertApp.DataAccess;

namespace CurrencyAlertApp
{
    [Activity(Label = "PreferencesActivity" ,   Theme = "@style/MyTheme.Base")]
    // MainLauncher = true,
    public class PreferencesActivity : AppCompatActivity
    {
        EditText edtTimeBeforeAlert;
        TextView lbl_timesIsSetTo;
        Button btnCancelOffset, btnSetOffset, btnClear;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Preferences);

            // ToolBar - Top of Screen  (method 1)
            var toolbar = FindViewById<Toolbar>(Resource.Id.preferencesActivity_top_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = GetString(Resource.String.preferencesActivity_top_toolbar_title);

            edtTimeBeforeAlert = FindViewById<EditText>(Resource.Id.preferencesActivity_edt_enterTimeBeforeAlert);
            lbl_timesIsSetTo = FindViewById<TextView>(Resource.Id.preferencesActivity_lbl_timesIsSetTo);
            btnCancelOffset = FindViewById<Button>(Resource.Id.preferencesActivity_btn_cancelOffset);
            btnSetOffset = FindViewById<Button>(Resource.Id.preferencesActivity_btn_setOffset);
            btnClear = FindViewById<Button>(Resource.Id.preferencesActivity_btn_clear);
            

            btnCancelOffset.Click += BtnCancelOffset_Click;
            btnSetOffset.Click += BtnSetOffset_Click;
            btnClear.Click += BtnClear_Click;

            edtTimeBeforeAlert.RequestFocus();


        }// end OnCreate()

       

        private void BtnSetOffset_Click(object sender, EventArgs e)
        {
            bool validNumber = double.TryParse(edtTimeBeforeAlert.Text, out double myDouble);

            Log.Debug("DEBUG", "\n\n\nSET Selected\n\n\n");

            if (edtTimeBeforeAlert.Text == string.Empty)
            {
                edtTimeBeforeAlert.Text = "";
                edtTimeBeforeAlert.Hint = ("Can't be blank");
            }
            else if( validNumber == false)
            {
                edtTimeBeforeAlert.Text = "";
                edtTimeBeforeAlert.Hint = ("valid number needed");
            }
            else if ( myDouble < -59 || myDouble > 59)
            {
                edtTimeBeforeAlert.Text = "";
                edtTimeBeforeAlert.Hint = ("number between -59 & +59");
            }
            else
            {
                //lbl_timesIsSetTo.Text = "  " + myDouble.ToString();
                //edtTimeBeforeAlert.Text = "Success - number accepted";

                // UPDATE PROPERTIES !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                SetUpData.TimeToGoOffBeforeMarketAnnouncement = myDouble;
                MainActivity.TimeOffsetUpdated = true;

                Toast.MakeText(this, "Success - number accepted", ToastLength.Long).Show();
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }           
        }



        private void BtnCancelOffset_Click(object sender, EventArgs e)
        {
            Log.Debug("DEBUG", "\n\n\nCancel Selected\n\n\n");
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            edtTimeBeforeAlert.Text = "";
            lbl_timesIsSetTo.Text = "";            
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.preferencesActivity_topMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.preferencesActivity_top_toolbar_option_marketData:
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                    break;
                default:
                    break;
            };
            return base.OnOptionsItemSelected(item);
        }

    }//
}//