﻿#pragma checksum "C:\Users\Jeff Schifano\Documents\Visual Studio 2012\Projects\Please2\Please2\SettingsPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C6E74074504A044A83FBD9D9C9E8AFB3"
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


namespace Please2 {
    
    
    public partial class SettingsPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.Pivot EventDetailsPivotView;
        
        internal Microsoft.Phone.Controls.PivotItem PreferencesPivotPanel;
        
        internal Microsoft.Phone.Controls.PivotItem AboutPivotPanel;
        
        internal Microsoft.Phone.Controls.PivotItem FeedbackPivotPanel;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Please2;component/SettingsPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.EventDetailsPivotView = ((Microsoft.Phone.Controls.Pivot)(this.FindName("EventDetailsPivotView")));
            this.PreferencesPivotPanel = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("PreferencesPivotPanel")));
            this.AboutPivotPanel = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("AboutPivotPanel")));
            this.FeedbackPivotPanel = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("FeedbackPivotPanel")));
        }
    }
}

