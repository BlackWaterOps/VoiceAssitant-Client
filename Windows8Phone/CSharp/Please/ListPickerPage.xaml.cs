using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Please
{
    public partial class ListPickerPage : PhoneApplicationPage
    {
        public ListPickerPage()
        {
            InitializeComponent();

            DataContext = App.ListPickerViewModel;
        }

        protected void ListItemSelected(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("list item selected");
                Debug.WriteLine(ListPickerList.SelectedIndex);
                if (ListPickerList.SelectedIndex >= 0)
                {
                    App.userChoice = (string)App.ListPickerViewModel.TheList[ListPickerList.SelectedIndex];

                    NavigationService.GoBack();
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
            }
        }
    }
}