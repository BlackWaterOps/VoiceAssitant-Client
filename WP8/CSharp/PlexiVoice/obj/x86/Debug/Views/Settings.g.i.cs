﻿#pragma checksum "D:\GitHub\stremor-please\WP8\CSharp\PlexiVoice\Views\Settings.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1332C0F6A92F2D04014769349CE59401"
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
    
    
    public partial class Settings : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.Pivot SettingsPivotView;
        
        internal Microsoft.Phone.Controls.PivotItem PreferencesPivotPanel;
        
        internal System.Windows.Controls.StackPanel GeneralStackPanel;
        
        internal System.Windows.Controls.StackPanel AccountsStackPanel;
        
        internal Microsoft.Phone.Controls.PivotItem AboutPivotPanel;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/PlexiVoice;component/Views/Settings.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.SettingsPivotView = ((Microsoft.Phone.Controls.Pivot)(this.FindName("SettingsPivotView")));
            this.PreferencesPivotPanel = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("PreferencesPivotPanel")));
            this.GeneralStackPanel = ((System.Windows.Controls.StackPanel)(this.FindName("GeneralStackPanel")));
            this.AccountsStackPanel = ((System.Windows.Controls.StackPanel)(this.FindName("AccountsStackPanel")));
            this.AboutPivotPanel = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("AboutPivotPanel")));
        }
    }
}

