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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string offset;

            if (NavigationContext.QueryString.TryGetValue("offset", out offset))
            {
                try
                {
                    //TODO: handle dst   
                    DateTime utc = DateTime.UtcNow;

                    DateTime locale = utc.AddHours(Convert.ToDouble(offset));

                    vm.Time = locale;
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                    vm.Time = DateTime.Now;
                }
            }
            else
            {
                vm.Time = DateTime.Now;
            }

            timer.Start();
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
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        protected void Timer_Tick(object sender, EventArgs e)
        {
            var timezone = TimeZoneInfo.Local;

            var name = timezone.DisplayName.Substring(timezone.DisplayName.IndexOf(")")+1).Trim();

            if (name != null && name != "")
            {
                name += " Time";
            }

            TimeTextBlock.Text = String.Format("{0} {1}", vm.Time.ToString("h:mm tt "), name);
        }
    }
}