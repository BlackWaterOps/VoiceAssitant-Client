﻿#pragma checksum "C:\Users\Jeff Schifano\Documents\Visual Studio 2012\Projects\VoiceAssistant\VoiceAssistant\Views\Reminder.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6B40193023A871ACFB450C2259A8B6B1"
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


namespace VoiceAssistant.Views {
    
    
    public partial class Reminder : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBox ReminderLabel;
        
        internal Microsoft.Phone.Controls.DatePicker ReminderDate;
        
        internal Microsoft.Phone.Controls.TimePicker ReminderTime;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/VoiceAssistant;component/Views/Reminder.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.ReminderLabel = ((System.Windows.Controls.TextBox)(this.FindName("ReminderLabel")));
            this.ReminderDate = ((Microsoft.Phone.Controls.DatePicker)(this.FindName("ReminderDate")));
            this.ReminderTime = ((Microsoft.Phone.Controls.TimePicker)(this.FindName("ReminderTime")));
        }
    }
}

