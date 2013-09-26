using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

using Please2.Models;

namespace Please2.Views
{
    public partial class ShoppingPage : PhoneApplicationPage
    {
        public ShoppingPage()
        {
            InitializeComponent();
        }

        protected void ShoppingListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // go to shopping item's detail page or navigate to link
            string link = (ShoppingListBox.SelectedItem as ShoppingModel).url;

            WebBrowserTask web = new WebBrowserTask();

            web.Uri = new Uri(link);

            web.Show();
        }
    }
}