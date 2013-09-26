using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using GalaSoft.MvvmLight.Messaging;

using LinqToVisualTree;

using Newtonsoft.Json;

using Please2.Models;
using Please2.Util;
using Please2.ViewModels;

namespace Please2.Views
{
    public partial class Conversation : ViewBase
    {
        ConversationViewModel vm;

        public Conversation()
        {
            InitializeComponent();            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            vm = (ConversationViewModel)DataContext;
            
            if (vm.DialogList == null || vm.DialogList.Count == 0)
            {
                vm.AddDialog("please", "how may I help you?");
                //Speak("please", "how may I help you?");
            }
            
            base.AddDebugTextBox();
        }

        protected void ContextMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = (sender as MenuItem).DataContext as DialogModel;

            Debug.WriteLine(dialog.message);

            Clipboard.SetText((string)dialog.message);
        }

        protected override void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
 	        base.OnKeyDown(sender, e);
        }

        /*
        private void ScrollTo()
        {
            // get last item
            var index = vm.DialogList.Count - 1;

            var enumerable = DialogList.Descendants<ScrollViewer>().Cast<ScrollViewer>();

            if (enumerable.Count() > 0)
            {
                var scrollViewer = enumerable.Single();

                scrollViewer.ScrollToVerticalOffset(index);
                scrollViewer.UpdateLayout();
            }
        }
         */
    }
}