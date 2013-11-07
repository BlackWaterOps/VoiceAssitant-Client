using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

using Please2.Models;
using Please2.Util;
using Please2.Views;
using Please2.ViewModels;

using Plexi.Util;
namespace Please2
{
    public partial class MainMenu : ViewBase
    {
        MainMenuViewModel vm;

        public MainMenu()
        {
            InitializeComponent();

            vm = (MainMenuViewModel)DataContext;

            //base.applicationBar.IsVisible = false;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // kick off geolocation listener
            // this should be done somewhere else!!
            LocationTracker.StartTrackingGeolocation();

            NavigationService.NavigationFailed += OnNavigationFailed;
           
            //base.AddDebugTextBox();

            var navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            NavigationService.NavigationFailed -= OnNavigationFailed;

            base.OnNavigatingFrom(e);
        }
 
        protected void MenuItem_Tapped(object sender, EventArgs e)
        {
            try
            {
                var item = sender as FrameworkElement;

                var model = item.DataContext as MainMenuModel;

                vm.LoadDefaultTemplate(model);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        protected void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            MessageBox.Show("Sorry, This page hasn't been implemented yet.");
        }
    }
}