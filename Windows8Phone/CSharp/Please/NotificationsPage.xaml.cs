using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Please.ViewModels;

namespace Please
{
    public partial class NotificationsPage : PhoneApplicationPage
    {
        private NotificationsViewModel viewModel;

        public NotificationsPage()
        {
            InitializeComponent();

            viewModel = new NotificationsViewModel();

            DataContext = viewModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            // The scheduled action name is stored in the Tag property
            // of the delete button for each reminder.
            string name = (string)((Button)sender).Tag;

            viewModel.deleteNotification(name);
           
            // Reset the ReminderListBox items
            HaveNotifications();
        }

        private void HaveNotifications()
        {
            var notifications = viewModel.getNotifications();

            if (notifications.Count() > 0)
            {
                EmptyTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmptyTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}