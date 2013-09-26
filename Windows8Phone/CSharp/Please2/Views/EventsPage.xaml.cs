using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Please2.Models;

namespace Please2.Views
{
    public partial class EventsPage : PhoneApplicationPage
    {
        public EventsPage()
        {
            InitializeComponent();
        }

        protected void EventsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (EventListBox.SelectedItem as EventModel);

            var id = current.id;

            NavigationService.Navigate(new Uri("/EventDetailsPage.xaml?id=" + id, UriKind.Relative));
        }
    }
}