using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using GalaSoft.MvvmLight.Ioc;

using Please2.ViewModels;

namespace Please2.Views
{
    public partial class Stock : ViewBase
    {
        public Stock()
        {
            InitializeComponent();

            DataContext = base.GetViewModelInstance<StockViewModel>();
        }
    }
}