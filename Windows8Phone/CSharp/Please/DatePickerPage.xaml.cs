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
    public partial class DatePickerPage : PhoneApplicationPage
    {
        public DatePickerPage()
        {
            InitializeComponent();

            DataContext = App.DatePickerViewModel;
        }

        protected void DatePickerGotFocus(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("datepicker got focus");
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
            }
        }

        protected void DateSelected(object sender, EventArgs e)
        {
            try
            {
                var picker = sender as DatePicker;

                Debug.WriteLine(picker.Value.Value.ToString("MMMM d yyyy"));


                App.userChoice = picker.Value.Value.ToString("MMMM d yyyy");

                NavigationService.GoBack();
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
        }
    }
}