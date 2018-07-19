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

namespace CurrencyAlertApp
{
    class Animal
    {
        public string Name { get; }
        public string ShortDescription { get; }
        public int ImageDrawableID { get; }

        public Animal(string name, string shortDescription, int imageDrawableID)
        {
            Name = name;
            ShortDescription = shortDescription;
            ImageDrawableID = imageDrawableID;
        }
    }
}