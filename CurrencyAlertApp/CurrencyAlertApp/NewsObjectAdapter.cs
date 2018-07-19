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
    class NewsObjectAdapter : BaseAdapter<NewsObject>
    {
        Context context;
        public List<NewsObject> NewsObjectList { get; }


        public NewsObjectAdapter(Context context, List<NewsObject> newsObjects)
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
                // ? inflate
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                newsObjectInfoRowView = inflater.Inflate(Resource.Layout.NewsObjectInfoRow, parent, false);

                // ? findID's
                var animalImageView = newsObjectInfoRowView.FindViewById<ImageView>(Resource.Id.imageViewAnimal);
                var lblAnimalNameView = newsObjectInfoRowView.FindViewById<TextView>(Resource.Id.lblAnimalName);
                var lblShortDescriptionView = newsObjectInfoRowView.FindViewById<TextView>(Resource.Id.lblAnimalComment);

                //  Assign content               
                //animalImageView.SetImageResource(NewsObjectList[position].ImageDrawableID);
                string useThisString = NewsObjectList[position].CountryChar.ToString().ToUpper();
                int imageID = GetImageForCurrency(useThisString);
                animalImageView.SetImageResource(imageID);

                lblAnimalNameView.Text = NewsObjectList[position].CountryChar + ": " + NewsObjectList[position].MarketImpact;
                lblShortDescriptionView.Text = NewsObjectList[position].DateOnly + ":  " + NewsObjectList[position].TimeOnly + "\n" +
                    NewsObjectList[position].Name; 

                // holder in Tag
                holder = new NewsObjectAdapterViewHolder(animalImageView, lblAnimalNameView, lblShortDescriptionView);
                newsObjectInfoRowView.Tag = holder;
            }
            else
            {
                // ?  pull out cached view from object tag
                var cachedNewsObjectAdapterHolder = newsObjectInfoRowView.Tag as NewsObjectAdapterViewHolder;

                // Assign values to child views                
                //cachedAnimalAdapterHolder.AnimalSnap.SetImageResource(NewsObjectList[position].ImageDrawableID);
                string useThisString = NewsObjectList[position].CountryChar.ToString().ToUpper();
                int imageID = GetImageForCurrency(useThisString);
                cachedNewsObjectAdapterHolder.AnimalSnap.SetImageResource(imageID);  
               
                cachedNewsObjectAdapterHolder.AnimalName.Text = NewsObjectList[position].CountryChar + ": " + NewsObjectList[position].MarketImpact;
                cachedNewsObjectAdapterHolder.AnimalShortDecription.Text = NewsObjectList[position].DateOnly + ":  " + NewsObjectList[position].TimeOnly + "\n" +
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
                    imageID = Resource.Drawable.bears;
                    break;
                case "GBP":
                    imageID = Resource.Drawable.dogbasket;
                    break;
                case "CAD":
                    imageID = Resource.Drawable.elephant;
                    break;
                case "JPY":
                    imageID = Resource.Drawable.gerbil;
                    break;
                case "CNY":
                    imageID = Resource.Drawable.Kangaroo;
                    break;
                case "NZD":
                    imageID = Resource.Drawable.kitten;
                    break;
                case "AUD":
                    imageID = Resource.Drawable.penguin;
                    break;
                case "CHF":
                    imageID = Resource.Drawable.snonleopard;
                    break;
                case "EUR":
                    imageID = Resource.Drawable.whaleshark;
                    break;
                case "Events that effect All":
                    imageID = Resource.Drawable.musicalnotesheadphoneColor;
                    break;
                default:
                    imageID = Resource.Drawable.musicalnotesheadphoneColor;
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
        //Your adapter views to re-use
        //public TextView Title { get; set; }
        public ImageView AnimalSnap { get; }
        public TextView AnimalName { get; }
        public TextView AnimalShortDecription { get; }

        public NewsObjectAdapterViewHolder(ImageView animalSnap, TextView animalName, TextView animalShortDescription)
        {
            AnimalSnap = animalSnap;
            AnimalName = animalName;
            AnimalShortDecription = animalShortDescription;
        }
    }
}