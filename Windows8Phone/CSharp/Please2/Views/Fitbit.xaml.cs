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

using GalaSoft.MvvmLight.Ioc;

using Newtonsoft.Json;

using Please2.Models;
using Please2.ViewModels;

namespace Please2.Views
{
    public partial class Fitbit : ViewBase
    {
        FitbitViewModel fitbit;

        public Fitbit()
        {
            InitializeComponent();

            if (!SimpleIoc.Default.IsRegistered<FitbitViewModel>())
            {
                SimpleIoc.Default.Register<FitbitViewModel>();
            }

            fitbit = SimpleIoc.Default.GetInstance<FitbitViewModel>();

            DataContext = fitbit;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
    }
}