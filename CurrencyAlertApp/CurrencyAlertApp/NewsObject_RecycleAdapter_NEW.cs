using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using CurrencyAlertApp.DataAccess;

using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// to pass a XDocument add reference:    System.Xml.Linq.
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;
using System.Reflection;
using Android.Content.Res;
using Android.Util;
using System.Collections.Generic;

namespace CurrencyAlertApp
{
    // Adapter to connect the data set (photo album) to the RecyclerView: 
    public class NewsObject_RecycleAdapter_NEW : RecyclerView.Adapter
    {
        public event EventHandler<NewsObject_RecycleAdapter_NEWClickEventArgs> ItemClick;
        public event EventHandler<NewsObject_RecycleAdapter_NEWClickEventArgs> ItemLongClick;

        // Underlying data set (a List<newsObject>):
        public List<NewsObject> mNewsObjectList;

        // Load the adapter with the data set (List<newsObject>) at construction time:
        public NewsObject_RecycleAdapter_NEW(List<NewsObject> newsObjectList)
        {
            mNewsObjectList = newsObjectList;
        }





        // Create a new views / photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.PhotoCardView, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            //NewsObject_ViewHolder vh = new NewsObject_ViewHolder(itemView, OnClick); (original)
            var vh = new NewsObject_RecycleAdapter_NEW_ViewHolder(itemView, OnClick, OnLongClick);       
            return vh;
        }




        // Fill in the contents of a view / the photo card (invoked by the layout manager):
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            // Replace the contents of the view with that element          
            NewsObject_RecycleAdapter_NEW_ViewHolder vh = viewHolder as NewsObject_RecycleAdapter_NEW_ViewHolder;
            
            // Set the ImageView and TextView in this ViewHolder's CardView 
            // from this position in the List<newsObject> (photo album)

            //  Assign content (get currency icon 1st)
            string countryChar = mNewsObjectList[position].CountryChar.ToString().ToUpper();
            int imageID = GetImageForCurrency(countryChar);
            vh.Icon.SetImageResource(imageID);            

            //  Assign content - continued
            vh.Caption1.Text = mNewsObjectList[position].CountryChar;
            vh.Caption2.Text = mNewsObjectList[position].MarketImpact;
        }


        // Return the number of photos available in the photo album:
        public override int ItemCount =>  mNewsObjectList.Count;

        void OnClick(NewsObject_RecycleAdapter_NEWClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(NewsObject_RecycleAdapter_NEWClickEventArgs args) => ItemLongClick?.Invoke(this, args);



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


    }




    public class NewsObject_RecycleAdapter_NEW_ViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public ImageView Icon { get; private set; }
        public TextView Caption1 { get; private set; }
        public TextView Caption2 { get; private set; }

        // Get references to the views defined in the CardView layout.
        public NewsObject_RecycleAdapter_NEW_ViewHolder(View itemView, 
                        Action<NewsObject_RecycleAdapter_NEWClickEventArgs> clickListener,
                        Action<NewsObject_RecycleAdapter_NEWClickEventArgs> longClickListener) : base(itemView)

        ////public NewsObject_RecycleAdapter_NEWViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            //TextView = v;
            // Locate and cache view references:
            Icon = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            Caption1 = itemView.FindViewById<TextView>(Resource.Id.textView);
            Caption2 = itemView.FindViewById<TextView>(Resource.Id.textView222);

            // Detect user clicks on the item view and report which item was clicked 
            itemView.Click += (sender, e) => clickListener(new NewsObject_RecycleAdapter_NEWClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new NewsObject_RecycleAdapter_NEWClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class NewsObject_RecycleAdapter_NEWClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}