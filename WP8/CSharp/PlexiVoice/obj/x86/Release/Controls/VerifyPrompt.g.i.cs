﻿#pragma checksum "D:\GitHub\VoiceAssitant-Client\WP8\CSharp\PlexiVoice\Controls\VerifyPrompt.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "608B911AD4D1381310250B5CC14718C4"
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
    
    
    public partial class VerifyPrompt : System.Windows.Controls.UserControl {
        
        internal System.Windows.Media.Animation.Storyboard ShowPrompt;
        
        internal System.Windows.Media.Animation.Storyboard HidePrompt;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Border MessageBorder;
        
        internal System.Windows.Media.PlaneProjection xProjection;
        
        internal System.Windows.Controls.TextBox VerifyEmail;
        
        internal System.Windows.Controls.TextBox VerifyCode;
        
        internal System.Windows.Controls.Button OkButton;
        
        internal System.Windows.Controls.Button CancelButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/PlexiVoice;component/Controls/VerifyPrompt.xaml", System.UriKind.Relative));
            this.ShowPrompt = ((System.Windows.Media.Animation.Storyboard)(this.FindName("ShowPrompt")));
            this.HidePrompt = ((System.Windows.Media.Animation.Storyboard)(this.FindName("HidePrompt")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.MessageBorder = ((System.Windows.Controls.Border)(this.FindName("MessageBorder")));
            this.xProjection = ((System.Windows.Media.PlaneProjection)(this.FindName("xProjection")));
            this.VerifyEmail = ((System.Windows.Controls.TextBox)(this.FindName("VerifyEmail")));
            this.VerifyCode = ((System.Windows.Controls.TextBox)(this.FindName("VerifyCode")));
            this.OkButton = ((System.Windows.Controls.Button)(this.FindName("OkButton")));
            this.CancelButton = ((System.Windows.Controls.Button)(this.FindName("CancelButton")));
        }
    }
}

