﻿#pragma checksum "D:\GitHub\stremor-please\WP8\CSharp\PlexiVoice\Views\RealEstateDetails.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B822B8B3BAD1C5C5F84F6EF763676718"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using PlexiVoice.Views;
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


namespace PlexiVoice.Views {
    
    
    public partial class RealEstateDetails : PlexiVoice.Views.ViewBase {
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/PlexiVoice;component/Views/RealEstateDetails.xaml", System.UriKind.Relative));
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

