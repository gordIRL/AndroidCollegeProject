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
    public class NewsObject_RecycleAdapter : RecyclerView.Adapter
    {       
        public event EventHandler<int> ItemClick;

        // Underlying data set (a List<newsObject>):
        public List<NewsObject> mNewsObjectList;

        // Load the adapter with the data set (List<newsObject>) at construction time:
        public NewsObject_RecycleAdapter(List<NewsObject> newsObjectList)
        {
            mNewsObjectList = newsObjectList;
        }


        // Create a new views / photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.NewsObject_CardView, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            //NewsObject_ViewHolder vh = new NewsObject_ViewHolder(itemView, OnClick); (original)
            var vh = new NewsObject_RecycleAdapter_NEW_ViewHolder(itemView, OnClick);       // , OnLongClick
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
            vh.Caption1.Text = mNewsObjectList[position].CountryChar + ": " + mNewsObjectList[position].MarketImpact; 
            vh.Caption2.Text = mNewsObjectList[position].DateAndTime.ToString("dd/MM/yyyy") + ":  " 
                    + mNewsObjectList[position].DateAndTime.ToString("HH:mmtt") + "\n" 
                    + mNewsObjectList[position].Name;
        }



        // Return the number of photos available in the photo album:
        //public override int ItemCount =>  mNewsObjectList.Count;
        public override int ItemCount
        {
            get {return mNewsObjectList.Count; }
        }
            
           


        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);    // ItemClick?.Invoke(this, position);  (simplified delegate)
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
    }
    

    public class NewsObject_RecycleAdapter_NEW_ViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public ImageView Icon { get; private set; }
        public TextView Caption1 { get; private set; }
        public TextView Caption2 { get; private set; }

        // Get references to the views defined in the CardView layout.        
        public NewsObject_RecycleAdapter_NEW_ViewHolder(View itemView, Action<int> listener) 
            : base(itemView)
        {
            // Locate and cache view references:
            Icon = itemView.FindViewById<ImageView>(Resource.Id.img_1_News_CardView);
            Caption1 = itemView.FindViewById<TextView>(Resource.Id.txt_1_News_CardView);
            Caption2 = itemView.FindViewById<TextView>(Resource.Id.txt_2_News_CardView);

            // Detect user clicks on the item view and report which item was clicked 
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }   
}