﻿#pragma checksum "D:\GitHub\stremor-please\Windows8Phone\CSharp\PlexiVoice\Controls\Clock.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C176068124ED1179B1559C5AD3595DCB"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace PlexiVoice.Controls {
    
    
    public partial class Clock : System.Windows.Controls.UserControl {
        
        internal System.Windows.Media.Animation.Storyboard ClockSecondHand;
        
        internal System.Windows.Media.Animation.Storyboard ClockLongHand;
        
        internal System.Windows.Media.Animation.Storyboard ClockHourHand;
        
        internal System.Windows.Controls.Grid ClockLayoutRoot;
        
        internal System.Windows.Shapes.Polygon Min;
        
        internal System.Windows.Media.RotateTransform MinTransform;
        
        internal System.Windows.Shapes.Polygon Hour;
        
        internal System.Windows.Media.RotateTransform HourTransform;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/PlexiVoice;component/Controls/Clock.xaml", System.UriKind.Relative));
            this.ClockSecondHand = ((System.Windows.Media.Animation.Storyboard)(this.FindName("ClockSecondHand")));
            this.ClockLongHand = ((System.Windows.Media.Animation.Storyboard)(this.FindName("ClockLongHand")));
            this.ClockHourHand = ((System.Windows.Media.Animation.Storyboard)(this.FindName("ClockHourHand")));
            this.ClockLayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("ClockLayoutRoot")));
            this.Min = ((System.Windows.Shapes.Polygon)(this.FindName("Min")));
            this.MinTransform = ((System.Windows.Media.RotateTransform)(this.FindName("MinTransform")));
            this.Hour = ((System.Windows.Shapes.Polygon)(this.FindName("Hour")));
            this.HourTransform = ((System.Windows.Media.RotateTransform)(this.FindName("HourTransform")));
        }
    }
}

