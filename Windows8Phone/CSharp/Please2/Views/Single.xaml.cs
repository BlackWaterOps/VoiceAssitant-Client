using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

using Please2.ViewModels;

namespace Please2.Views
{
    public partial class Single : ViewBase
    {
        private SingleViewModel vm;

        public Single()
        {
            InitializeComponent();

            vm = (SingleViewModel)DataContext;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

#if DEBUG
            string test = String.Empty;

            NavigationContext.QueryString.TryGetValue("test", out test);

            if (!String.IsNullOrEmpty(test))
            {
                vm.RunTest(test);
            }
#endif
        }
    }
}