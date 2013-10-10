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
    public partial class SingleResult : ViewBase
    {
        private SingleViewModel vm;

        public SingleResult()
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
                RunTest(test);
            }
#endif
        }

        private void RunTest(string name)
        {
            var templates = App.Current.Resources["SingleTemplateDictionary"] as ResourceDictionary;

            if (templates[name] == null)
            {
                Debug.WriteLine("cold not find template " + name);
            }
            else
            {
                vm.RunTest(name, templates[name] as DataTemplate);
            }
        }
    }
}