﻿#pragma checksum "C:\Users\Jeff Schifano\Documents\Visual Studio 2012\Projects\VoiceAssistant\VoiceAssistant\Views\RealEstateDetails.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4E764B7B494B95094D3F55A12393EB1F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
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
using VoiceAssistant.Views;


namespace VoiceAssistant.Views {
    
    
    public partial class RealEstateDetails : VoiceAssistant.Views.ViewBase {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Microsoft.Phone.Controls.Pivot ListingDetailsPivotView;
        
        internal Microsoft.Phone.Controls.PivotItem DetailsPivotPanel;
        
        internal System.Windows.Controls.TextBlock ListingDescription;
        
        internal Microsoft.Phone.Controls.PivotItem DirectionsPivotPanel;
        
        internal Microsoft.Phone.Maps.Controls.Map EventMap;
        
        internal System.Windows.Controls.Button EventMapButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/VoiceAssistant;component/Views/RealEstateDetails.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.ListingDetailsPivotView = ((Microsoft.Phone.Controls.Pivot)(this.FindName("ListingDetailsPivotView")));
            this.DetailsPivotPanel = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("DetailsPivotPanel")));
            this.ListingDescription = ((System.Windows.Controls.TextBlock)(this.FindName("ListingDescription")));
            this.DirectionsPivotPanel = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("DirectionsPivotPanel")));
            this.EventMap = ((Microsoft.Phone.Maps.Controls.Map)(this.FindName("EventMap")));
            this.EventMapButton = ((System.Windows.Controls.Button)(this.FindName("EventMapButton")));
        }
    }
}

