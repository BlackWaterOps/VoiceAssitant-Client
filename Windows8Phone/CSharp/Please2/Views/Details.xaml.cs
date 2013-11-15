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
    public partial class Details : ViewBase
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

        // TODO: add pin to start button to appbar
        private void AddMenuButtons()
        {
            var pin = new ApplicationBarIconButton();

            pin.IconUri = new Uri("/Assets/pin.png", UriKind.Relative);
            pin.Text = "pin to start";
            pin.Click += (s, e) => { vm.PinToStartCommand.Execute(null); };

            base.applicationBar.Buttons.Add(pin);
        }
    }
}