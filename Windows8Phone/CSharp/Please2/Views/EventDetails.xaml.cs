using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Please2.ViewModels;

namespace Please2.Views
{
    public partial class EventDetails : ViewBase
    {
        EventDetailsViewModel vm;

        bool mapAdded = false;

        public EventDetails()
        {
            InitializeComponent();

            vm = DataContext as EventDetailsViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string id;

            if (NavigationContext.QueryString.TryGetValue("id", out id))
            {
                LoadEvent(id);
            }
        }

        private void DirectionsPivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mapAdded)
            {
                vm.AddDirectionsMap();

                mapAdded = true;
            }
        }

        private void LoadEvent(string id)
        {
            // handle navigation from pinned tile
            // need an endpoint to retrieve a specific event
        }
    }
}