﻿#pragma checksum "C:\Users\Jeff Schifano\Documents\Visual Studio 2012\Projects\Please2\Please2\Pages\EventDetailsPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DDCEFA0FA26262894212990347845F34"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Please2.Pages {
    
    
    public partial class EventDetailsPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.Pivot EventDetailsPivotView;
        
        internal Microsoft.Phone.Controls.PivotItem DetailsPivotPanel;
        
        internal System.Windows.Controls.TextBlock EventLocation;
        
        internal System.Windows.Controls.TextBlock EventDescription;
        
        internal Microsoft.Phone.Controls.PivotItem DirectionsPivotPanel;
        
        internal Microsoft.Phone.Maps.Controls.Map EventMap;
        
        internal System.Windows.Controls.Button EventMapButton;
        
        internal Microsoft.Phone.Shell.ApplicationBar AppControl;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton PinToStartButton;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Please2;component/Pages/EventDetailsPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.EventDetailsPivotView = ((Microsoft.Phone.Controls.Pivot)(this.FindName("EventDetailsPivotView")));
            this.DetailsPivotPanel = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("DetailsPivotPanel")));
            this.EventLocation = ((System.Windows.Controls.TextBlock)(this.FindName("EventLocation")));
            this.EventDescription = ((System.Windows.Controls.TextBlock)(this.FindName("EventDescription")));
            this.DirectionsPivotPanel = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("DirectionsPivotPanel")));
            this.EventMap = ((Microsoft.Phone.Maps.Controls.Map)(this.FindName("EventMap")));
            this.EventMapButton = ((System.Windows.Controls.Button)(this.FindName("EventMapButton")));
            this.AppControl = ((Microsoft.Phone.Shell.ApplicationBar)(this.FindName("AppControl")));
            this.PinToStartButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("PinToStartButton")));
        }
    }
}

