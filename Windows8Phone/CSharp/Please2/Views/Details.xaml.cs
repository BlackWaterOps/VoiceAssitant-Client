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

using Please2.ViewModels;

namespace Please2.Views
{
    public partial class Details : PhoneApplicationPage
    {
        DetailsViewModel vm;

        public Details()
        {
            InitializeComponent();

            vm = (DetailsViewModel)DataContext;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string template = null;

            NavigationContext.QueryString.TryGetValue("template", out template);

            if (template == null)
            {
                Debug.WriteLine("no details template supplied");
                return;
            }

            vm.SetDetailsTemplate(template);
        }
    }
}