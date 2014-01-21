using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using TimeZones;

using Please2.ViewModels;

namespace Please2.Views
{
    public partial class Clock : ViewBase
    {
        private DispatcherTimer timer;

        private ClockViewModel vm;

        public Clock()
        {
            InitializeComponent();

            vm = DataContext as ClockViewModel;

            Loaded += Page_Loaded;

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
        }

        protected void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SecondHand.Begin();
                LongHand.Begin();
                HourHand.Begin();

                int second = vm.Time.Second;
                SecondHand.Seek(new TimeSpan(0, 0, second));
                
                int minute = vm.Time.Minute;
                LongHand.Seek(new TimeSpan(0, minute, second));

                int hour = vm.Time.Hour;
                HourHand.Seek(new TimeSpan(hour, minute, second));

                timer.Interval = TimeSpan.FromSeconds(60 - second);
                timer.Start();
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        protected void Timer_Tick(object sender, EventArgs e)
        {
            vm.Time = vm.Time.AddMinutes(1);

            timer.Interval = TimeSpan.FromMinutes(1);
        }
    }
}