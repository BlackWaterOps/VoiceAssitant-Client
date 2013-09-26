﻿using System;
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
using GalaSoft.MvvmLight.Messaging;

using Please2.Models;
using Please2.Util;
using Please2.Views;
using Please2.ViewModels;

namespace Please2
{
    public partial class MainPage2 : ViewBase
    {
        MainMenuViewModel vm;

        public MainPage2()
        {
            InitializeComponent();

            vm = (MainMenuViewModel)DataContext;

            //base.applicationBar.IsVisible = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            /*
            Debug.WriteLine("&#xf130");

            var uni = new System.Text.UnicodeEncoding();

            Byte[] encoded = uni.GetBytes("&#xf130");

            Debug.WriteLine();
            */

            Debug.WriteLine(typeof(ShoppingModel));

            // kick off geolocation listener
            // this should be done somewhere else!!
            Please2.Util.Location.StartTrackingGeolocation();

            NavigationService.NavigationFailed += OnNavigationFailed;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            NavigationService.NavigationFailed -= OnNavigationFailed;

            base.OnNavigatingFrom(e);
        }

        protected void MenuItem_Tapped(object sender, EventArgs e)
        {
            var item = sender as FrameworkElement;

            var navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
             
            navigationService.NavigateTo(new Uri((string)item.Tag, UriKind.Relative));
        }

        protected void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            MessageBox.Show("Sorry, This page hasn't been implemented yet.");
        }
    }
}