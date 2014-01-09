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

using Please2.ViewModels;

namespace Please2.Views
{
    public partial class FuelDetails : PhoneApplicationPage
    {
        FuelDetailsViewModel vm;

        bool mapAdded = false;

        public FuelDetails()
        {
            InitializeComponent();

            vm = DataContext as FuelDetailsViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(vm.CurrentItem));
        }

        private void DirectionsPivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mapAdded)
            {
                vm.AddDirectionsMap();

                mapAdded = true;
            }
        }
    }
}