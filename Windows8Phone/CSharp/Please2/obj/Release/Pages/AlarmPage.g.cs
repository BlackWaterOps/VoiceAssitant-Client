﻿#pragma checksum "C:\Users\Jeff Schifano\Documents\Visual Studio 2012\Projects\Please2\Please2\Pages\AlarmPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6A0B684E92D7FA55258EC245BA4798B1"
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


namespace Please2.Pages {
    
    
    public partial class AlarmPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid NewAlarmGrid;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Microsoft.Phone.Controls.TimePicker AlarmTime;
        
        internal Microsoft.Phone.Controls.RecurringDaysPicker AlarmRecurringDays;
        
        internal System.Windows.Controls.StackPanel SavePanel;
        
        internal System.Windows.Controls.StackPanel DeleteSavePanel;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Please2;component/Pages/AlarmPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.NewAlarmGrid = ((System.Windows.Controls.Grid)(this.FindName("NewAlarmGrid")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.AlarmTime = ((Microsoft.Phone.Controls.TimePicker)(this.FindName("AlarmTime")));
            this.AlarmRecurringDays = ((Microsoft.Phone.Controls.RecurringDaysPicker)(this.FindName("AlarmRecurringDays")));
            this.SavePanel = ((System.Windows.Controls.StackPanel)(this.FindName("SavePanel")));
            this.DeleteSavePanel = ((System.Windows.Controls.StackPanel)(this.FindName("DeleteSavePanel")));
        }
    }
}

