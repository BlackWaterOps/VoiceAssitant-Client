﻿#pragma checksum "C:\Users\Jeff Schifano\Documents\Visual Studio 2012\Projects\Please2\Please2\Views\PreferencesPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2E9CF8439C911716ACD0435466D699B3"
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


namespace Please2.Views {
    
    
    public partial class PreferencesPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBlock EmptyTextBlock;
        
        internal System.Windows.Controls.ListBox CategoryLongListSelector;
        
        internal Microsoft.Phone.Shell.ApplicationBar AppControl;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton AddButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Please2;component/Views/PreferencesPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.EmptyTextBlock = ((System.Windows.Controls.TextBlock)(this.FindName("EmptyTextBlock")));
            this.CategoryLongListSelector = ((System.Windows.Controls.ListBox)(this.FindName("CategoryLongListSelector")));
            this.AppControl = ((Microsoft.Phone.Shell.ApplicationBar)(this.FindName("AppControl")));
            this.AddButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("AddButton")));
        }
    }
}

