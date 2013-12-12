using System;
using System.Collections.Generic;
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
    public partial class Notes : ViewBase
    {
        public Notes()
        {
            InitializeComponent();

            AddMenu();
        }

        private void AddMenu()
        {
            ApplicationBarMenuItem addButton = new ApplicationBarMenuItem("Add");

            addButton.Click += AddButton_Click;

            ApplicationBar.MenuItems.Add(addButton);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Note.xaml", UriKind.Relative));
        }
    }
}