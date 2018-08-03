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
using CurrencyAlertApp;
using CurrencyAlertApp.DataAccess;

namespace CurrencyAlertApp
{
    class NewsObject_CustomAdapter : BaseAdapter<NewsObject>
    {
        Context context;
        public List<NewsObject> NewsObjectList { get; }


        public NewsObject_CustomAdapter(Context context, List<NewsObject> newsObjects)
        {
            this.context = context;
            NewsObjectList = newsObjects;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var newsObjectInfoRowView = convertView;
            NewsObjectAdapterViewHolder holder = null;

            if (newsObjectInfoRowView == null)
            {
                //  inflate
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                newsObjectInfoRowView = inflater.Inflate(Resource.Layout.NewsObject_CustomAdapter_InfoRow, parent, false);

                //  findID's
                var currencyIconView = newsObjectInfoRowView.FindViewById<ImageView>(Resource.Id.imageView_NewsObjectInfoRow_Icon);
                var txtLine_1_view = newsObjectInfoRowView.FindViewById<TextView>(Resource.Id.txt_NewsObjectInfoRow_line1);
                var txtLine_2_view = newsObjectInfoRowView.FindViewById<TextView>(Resource.Id.txt_NewsObjectInfoRow_line2);

                //  Assign content (get currency icon 1st)
                string countryChar = NewsObjectList[position].CountryChar.ToString().ToUpper();
                int imageID = GetImageForCurrency(countryChar);
                currencyIconView.SetImageResource(imageID);

                //  Assign content - continued
                txtLine_1_view.Text = NewsObjectList[position].CountryChar + ": " + NewsObjectList[position].MarketImpact;
                txtLine_2_view.Text = NewsObjectList[position].DateAndTime.ToString("dd/MM/yyyy") + ":  " + NewsObjectList[position].DateAndTime.ToString("HH:mmtt") + "\n" +
                    NewsObjectList[position].Name; 

                // holder in Tag
                holder = new NewsObjectAdapterViewHolder(currencyIconView, txtLine_1_view, txtLine_2_view);
                newsObjectInfoRowView.Tag = holder;
            }
            else
            {
                //  pull out cached view from object tag
                var cachedNewsObjectAdapterHolder = newsObjectInfoRowView.Tag as NewsObjectAdapterViewHolder;

                // Assign values to child views (get currency icon 1st)     
                string countryChar = NewsObjectList[position].CountryChar.ToString().ToUpper();
                int imageID = GetImageForCurrency(countryChar);
                cachedNewsObjectAdapterHolder.VH_CurrencyIcon.SetImageResource(imageID);

                //  Assign content - continued
                cachedNewsObjectAdapterHolder.VH_txtLine1.Text = NewsObjectList[position].CountryChar + ": " + NewsObjectList[position].MarketImpact;
                cachedNewsObjectAdapterHolder.VH_txtLine2.Text = NewsObjectList[position].DateAndTime.ToString("dd/MM/yyyy") + ":  " + NewsObjectList[position].DateAndTime.ToString("HH:mmtt") + "\n" +
                    NewsObjectList[position].Name;
            }
            return newsObjectInfoRowView;
        }


        public int GetImageForCurrency(string currentCurrency)
        {
            int imageID = 0;

            switch (currentCurrency)
            {
                case "USD":
                    imageID = Resource.Mipmap.united_states;
                    break;
                case "GBP":
                    imageID = Resource.Mipmap.united_kingdom;
                    break;
                case "CAD":
                    imageID = Resource.Mipmap.canada;
                    break;
                case "JPY":
                    imageID = Resource.Mipmap.japan;
                    break;
                case "CNY":
                    imageID = Resource.Mipmap.china;
                    break;
                case "NZD":
                    imageID = Resource.Mipmap.new_zealand;
                    break;
                case "AUD":
                    imageID = Resource.Mipmap.australia;
                    break;
                case "CHF":
                    imageID = Resource.Mipmap.switzerland;
                    break;
                case "EUR":
                    imageID = Resource.Mipmap.european_union;
                    break;
                default:
                    imageID = Resource.Mipmap.globe;
                    break;
            }
            return imageID;
        }

        public override int Count
        {
            get
            {
                return NewsObjectList.Count;
            }
        }

        public override NewsObject this[int position]
        {
            get
            {
                return NewsObjectList[position];
            }
        }
    }

    class NewsObjectAdapterViewHolder : Java.Lang.Object
    {
        // adapter views to re-use
        public ImageView VH_CurrencyIcon { get; }
        public TextView VH_txtLine1 { get; }
        public TextView VH_txtLine2 { get; }

        public NewsObjectAdapterViewHolder(ImageView currencyIcon, TextView txtLine1, TextView txtLine2)
        {
            VH_CurrencyIcon = currencyIcon;
            VH_txtLine1 = txtLine1;
            VH_txtLine2 = txtLine2;
        }
    }
}