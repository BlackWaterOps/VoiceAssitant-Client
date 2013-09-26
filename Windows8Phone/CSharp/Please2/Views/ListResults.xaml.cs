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
using Microsoft.Phone.UserData;

using GalaSoft.MvvmLight.Ioc;

using Please2.ViewModels;
using Please2.Util;

namespace Please2.Views
{
    public partial class ListResults : ViewBase
    {
        public ListResults()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext == null)
            {
                //var url = e.Uri.ToString();
                //var itemUrl = url.Substring(url.IndexOf("?") + 1);

                if (!SimpleIoc.Default.IsRegistered<ListViewModel>())
                {
                    MessageBox.Show("List not found");
                    return;
                }

                var vm = SimpleIoc.Default.GetInstance<ListViewModel>();

                DataContext = vm;


            }
        }

        // ALL THE DETAILS COULD BE STUFFED INTO ONE VIEW
        protected void ShoppingItem_Tap(object sender, EventArgs e)
        {
            // navigate to shopping item details page
        }

        protected void EventItem_Tap(object sender, EventArgs e)
        {
            // navigate to event item details page
        }

        protected void MovieItem_Tap(object sender, EventArgs e)
        {
            // navigate to movie item details page
        }

        protected void NewsItem_Tap(object sender, EventArgs e)
        {

        }

        protected void Contact_Tap(object sender, EventArgs e)
        {
            var contact = sender as Contact;

            /* it is assumed that, at this point, the task service has been registered */
            if (!SimpleIoc.Default.IsRegistered<ITaskService>())
            {
                Debug.WriteLine("uh oh, the task service has not been registered");
                return;
                //SimpleIoc.Default.Register<ITaskService, TaskService>();
            }

            var tasks = SimpleIoc.Default.GetInstance<ITaskService>();

            switch (tasks.MainContext.model)
            {
                case "email":
                case "email_denial":
                    tasks.ComposeEmail(contact);
                    break;

                case "sms":
                case "sms_denial":
                    tasks.ComposeSms(contact);
                    break;

                case "call":
                case "call_denial":
                    tasks.PhoneCall(contact);
                    break;
            }
        }
    }
}