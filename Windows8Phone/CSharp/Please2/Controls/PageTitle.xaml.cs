using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Please2.Controls
{
    public enum TitleType { AppTitle, PageTitle, PageSubTitle };

    public partial class PageTitle : UserControl
    {
        public PageTitle()
        {
            InitializeComponent();

            DataContext = this;
        }

        public static readonly DependencyProperty SchemeProperty = DependencyProperty.Register(
            "Scheme",
            typeof(string),
            typeof(PageTitle),
            null);

        public string Scheme
        {
            get { return (string)GetValue(SchemeProperty); }
            set { SetValue(SchemeProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(PageTitle),
            new PropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged)));

        private static void OnTitleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            PageTitle pt = dependencyObject as PageTitle;

            pt.ShowTitle(TitleType.PageTitle);
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register(
            "SubTitle",
            typeof(string),
            typeof(PageTitle),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSubTitleChanged)));

        public string SubTitle
        {
            get { return (string)GetValue(SubTitleProperty); }
            set { SetValue(SubTitleProperty, value); }
        }

        private static void OnSubTitleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            PageTitle pt = dependencyObject as PageTitle;

            pt.ShowTitle(TitleType.PageSubTitle);
        }

        private void ShowTitle(TitleType type)
        {
            switch (type)
            {
                case TitleType.PageTitle:
                    PageTitleBorder.Visibility = Visibility.Visible;
                    break;

                case TitleType.PageSubTitle:
                    PageSubTitleBorder.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
