﻿#pragma checksum "C:\Users\Jeff Schifano\Documents\Visual Studio 2012\Projects\Please2\Please2\Pages\ImagesPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DA17937AE17E6B0F6285FC220E8C8182"
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
    
    
    public partial class ImagesPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Microsoft.Phone.Controls.PhoneApplicationPage PageRoot;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Microsoft.Phone.Controls.LongListSelector ImagesThumbnail;
        
        internal System.Windows.Controls.ListBox ImagesFull;
        
        internal System.Windows.Media.CompositeTransform ListTransform;
        
        internal System.Windows.Controls.Primitives.ViewportControl viewport;
        
        internal System.Windows.Controls.Canvas canvas;
        
        internal System.Windows.Controls.Image ImageSingle;
        
        internal System.Windows.Media.ScaleTransform xform;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Please2;component/Pages/ImagesPage.xaml", System.UriKind.Relative));
            this.PageRoot = ((Microsoft.Phone.Controls.PhoneApplicationPage)(this.FindName("PageRoot")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.ImagesThumbnail = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("ImagesThumbnail")));
            this.ImagesFull = ((System.Windows.Controls.ListBox)(this.FindName("ImagesFull")));
            this.ListTransform = ((System.Windows.Media.CompositeTransform)(this.FindName("ListTransform")));
            this.viewport = ((System.Windows.Controls.Primitives.ViewportControl)(this.FindName("viewport")));
            this.canvas = ((System.Windows.Controls.Canvas)(this.FindName("canvas")));
            this.ImageSingle = ((System.Windows.Controls.Image)(this.FindName("ImageSingle")));
            this.xform = ((System.Windows.Media.ScaleTransform)(this.FindName("xform")));
        }
    }
}

