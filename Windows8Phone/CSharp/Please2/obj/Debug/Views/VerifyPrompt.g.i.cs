﻿#pragma checksum "C:\Users\Jeff Schifano\Documents\Visual Studio 2012\Projects\Please2\Please2\Views\VerifyPrompt.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C93F6045064A701E0BEE0CA334AEEAD8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
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


namespace Please2.Views {
    
    
    public partial class VerifyPrompt : System.Windows.Controls.UserControl {
        
        internal System.Windows.Media.Animation.Storyboard ShowBoxAnimation;
        
        internal System.Windows.Media.Animation.Storyboard HideBoxAnimation;
        
        internal System.Windows.Controls.Border OverlayBorder;
        
        internal System.Windows.Controls.Border MessageBorder;
        
        internal System.Windows.Controls.TextBox VerifyEmail;
        
        internal System.Windows.Controls.TextBox VerifyCode;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Please2;component/Views/VerifyPrompt.xaml", System.UriKind.Relative));
            this.ShowBoxAnimation = ((System.Windows.Media.Animation.Storyboard)(this.FindName("ShowBoxAnimation")));
            this.HideBoxAnimation = ((System.Windows.Media.Animation.Storyboard)(this.FindName("HideBoxAnimation")));
            this.OverlayBorder = ((System.Windows.Controls.Border)(this.FindName("OverlayBorder")));
            this.MessageBorder = ((System.Windows.Controls.Border)(this.FindName("MessageBorder")));
            this.VerifyEmail = ((System.Windows.Controls.TextBox)(this.FindName("VerifyEmail")));
            this.VerifyCode = ((System.Windows.Controls.TextBox)(this.FindName("VerifyCode")));
        }
    }
}

