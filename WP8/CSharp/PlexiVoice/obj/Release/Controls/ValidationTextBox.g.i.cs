﻿#pragma checksum "D:\GitHub\VoiceAssitant-Client\WP8\CSharp\PlexiVoice\Controls\ValidationTextBox.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6FFABF0F1944553B01AD83BC1812969A"
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
    
    
    public partial class ValidationTextBox : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Border ValidationBorder;
        
        internal System.Windows.Controls.TextBox ValTextBox;
        
        internal System.Windows.Controls.PasswordBox ValPasswordBox;
        
        internal System.Windows.Controls.Grid ErrorBubble;
        
        internal System.Windows.Shapes.Polygon Point;
        
        internal System.Windows.Controls.TextBlock ErrorBubbleTextBlock;
        
        internal System.Windows.Media.Animation.Storyboard ErrorFadeOut;
        
        internal System.Windows.Media.Animation.Storyboard ErrorFadeIn;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/PlexiVoice;component/Controls/ValidationTextBox.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ValidationBorder = ((System.Windows.Controls.Border)(this.FindName("ValidationBorder")));
            this.ValTextBox = ((System.Windows.Controls.TextBox)(this.FindName("ValTextBox")));
            this.ValPasswordBox = ((System.Windows.Controls.PasswordBox)(this.FindName("ValPasswordBox")));
            this.ErrorBubble = ((System.Windows.Controls.Grid)(this.FindName("ErrorBubble")));
            this.Point = ((System.Windows.Shapes.Polygon)(this.FindName("Point")));
            this.ErrorBubbleTextBlock = ((System.Windows.Controls.TextBlock)(this.FindName("ErrorBubbleTextBlock")));
            this.ErrorFadeOut = ((System.Windows.Media.Animation.Storyboard)(this.FindName("ErrorFadeOut")));
            this.ErrorFadeIn = ((System.Windows.Media.Animation.Storyboard)(this.FindName("ErrorFadeIn")));
        }
    }
}
