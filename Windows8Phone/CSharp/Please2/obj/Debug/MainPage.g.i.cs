﻿#pragma checksum "C:\Users\Jeff Schifano\Documents\Visual Studio 2012\Projects\Please2\Please2\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3B9A09A262B0135C31D6617ADF869931"
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


namespace Please2 {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBox ManualInput;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.Grid InputGrid;
        
        internal System.Windows.Controls.TextBox SpeakTextBox;
        
        internal System.Windows.Controls.Grid DialogGrid;
        
        internal System.Windows.Controls.ListBox DialogList;
        
        internal Microsoft.Phone.Shell.ApplicationBar AppControl;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton SpeakButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarMenuItem InputToggle;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Please2;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ManualInput = ((System.Windows.Controls.TextBox)(this.FindName("ManualInput")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.InputGrid = ((System.Windows.Controls.Grid)(this.FindName("InputGrid")));
            this.SpeakTextBox = ((System.Windows.Controls.TextBox)(this.FindName("SpeakTextBox")));
            this.DialogGrid = ((System.Windows.Controls.Grid)(this.FindName("DialogGrid")));
            this.DialogList = ((System.Windows.Controls.ListBox)(this.FindName("DialogList")));
            this.AppControl = ((Microsoft.Phone.Shell.ApplicationBar)(this.FindName("AppControl")));
            this.SpeakButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("SpeakButton")));
            this.InputToggle = ((Microsoft.Phone.Shell.ApplicationBarMenuItem)(this.FindName("InputToggle")));
        }
    }
}

