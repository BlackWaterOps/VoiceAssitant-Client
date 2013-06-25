using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using WPControls;

namespace Please
{
    public partial class CalendarPage : PhoneApplicationPage
    {
        public CalendarPage()
        {
            InitializeComponent();

            // test data binding
            SelectedDate = DateTime.Now;
        }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
            }
        }

        protected void OnDateClicked(object sender, EventArgs e)
        {
            DateTime selection = Cal.SelectedDate;

            App.userChoice = selection.ToString("MMMM d yyyy");

            NavigationService.GoBack();
        }
    }

    /*
    public class BackgroundConverter : IDateToBrushConverter
    {
        public Brush Convert(DateTime dateTime, bool isSelected, Brush defaultValue)
        {
            if (dateTime == new DateTime(DateTime.Today.Year, DateTime.Today.Month, 5))
            {
                return new SolidColorBrush(Colors.Yellow);
            }
            else
            {
                return defaultValue;
            }
        }
    }

    public class DayNumberConverter : IDateToBrushConverter
    {

        public Brush Convert(DateTime dateTime, bool isSelected, Brush defaultValue)
        {
            if (dateTime == new DateTime(DateTime.Today.Year, DateTime.Today.Month, 7))
            {
                return new SolidColorBrush(Colors.Orange);
            }
            else if (dateTime == new DateTime(DateTime.Today.Year, DateTime.Today.Month, 5))
            {
                return new SolidColorBrush(Colors.Purple);
            }
            else
            {
                return defaultValue;
            }
        }
    }
    */
}