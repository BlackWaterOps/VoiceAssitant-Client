﻿#pragma checksum "D:\GitHub\VoiceAssitant-Client\WP8\CSharp\PlexiVoice\Views\Alarm.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F4FE48C09E71B81890D1786912449103"
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
    
    
    public partial class Alarm : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBox AlarmLabel;
        
        internal Microsoft.Phone.Controls.TimePicker AlarmTime;
        
        internal Microsoft.Phone.Controls.RecurringDaysPicker AlarmRecurringDays;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/PlexiVoice;component/Views/Alarm.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.AlarmLabel = ((System.Windows.Controls.TextBox)(this.FindName("AlarmLabel")));
            this.AlarmTime = ((Microsoft.Phone.Controls.TimePicker)(this.FindName("AlarmTime")));
            this.AlarmRecurringDays = ((Microsoft.Phone.Controls.RecurringDaysPicker)(this.FindName("AlarmRecurringDays")));
        }
    }
}

