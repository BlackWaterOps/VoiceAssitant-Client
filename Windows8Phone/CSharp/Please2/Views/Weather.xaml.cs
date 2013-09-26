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
    public partial class Weather : ViewBase
    {
        WeatherViewModel vm;

        public Weather()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            vm = (WeatherViewModel)DataContext;

            base.AddDebugTextBox();
        }

        protected override void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(sender, e);
        }
    }
}