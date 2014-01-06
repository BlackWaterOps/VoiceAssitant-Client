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
    public partial class RealEstateDetails : ViewBase
    {
        private RealEstateDetailsViewModel vm;

        public RealEstateDetails()
        {
            InitializeComponent();

            vm = DataContext as RealEstateDetailsViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string id;

            if (NavigationContext.QueryString.TryGetValue("id", out id))
            {
                LoadListing(id);
            }
        }

        private void DirectionsPivotPanel_Loaded(object sender, RoutedEventArgs e)
        {
            vm.AddDirectionsMap();
        }

        private void LoadListing(string id)
        {
            // handle navigation from pinned tile
            // need endpoint to retrieve specific listing
        }
    }
}