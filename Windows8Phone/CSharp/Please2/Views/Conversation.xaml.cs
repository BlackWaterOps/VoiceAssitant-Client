using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

            // add opening dialog before binding listener
            vm.AddOpeningDialog();

            vm.DialogList.CollectionChanged += DialogCollectionChanged;

            if (vm.DialogList.Count > 1)
            {
                base.AddCancelButton();
            }

            ScrollTo();            
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            vm.DialogList.CollectionChanged -= DialogCollectionChanged;

            vm.RemoveOpeningDialog();

            base.OnNavigatingFrom(e);
        }

        private void DialogCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine("collection changed");
            Debug.WriteLine(e.Action);
            Debug.WriteLine(e.NewStartingIndex);

            if (e.NewStartingIndex > 0)
            {
                base.AddCancelButton();
            }
        }

        protected void ContextMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = (sender as MenuItem).DataContext as DialogModel;

            Debug.WriteLine(dialog.message);

            Clipboard.SetText((string)dialog.message);
        }

        private void ScrollTo()
        {
            // get last item
            var index = vm.DialogList.Count - 1;

            var enumerable = DialogList.Descendants<ScrollViewer>().Cast<ScrollViewer>();

            if (enumerable.Count() > 0 && index > 0)
            {
                Debug.WriteLine("scroll to");

                var scrollViewer = enumerable.Single();

                scrollViewer.ScrollToVerticalOffset(index);
                scrollViewer.UpdateLayout();
            }
        }
    }
}