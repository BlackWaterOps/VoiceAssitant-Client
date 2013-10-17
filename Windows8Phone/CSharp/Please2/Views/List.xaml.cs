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
    public partial class List : ViewBase
    {
        private ListViewModel vm;

        public List()
        {
            InitializeComponent();

            vm = (ListViewModel)DataContext;
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