using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Please2.Views
{
    public partial class Clock : ViewBase
    {
        public string SubTitle
        {
            get { return DateTime.Now.ToString("dddd, MMMM d, yyyy"); }    
        }
        
        public Clock()
        {
            InitializeComponent();

            Loaded += Page_Loaded;

            //var phoneAppService = PhoneApplicationService.Current;
            //phoneAppService.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            var timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        protected void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                PageSubTitle.Text = DateTime.Now.ToString("dddd, MMMM d, yyyy");

                SecondHand.Begin();
                LongHand.Begin();
                HourHand.Begin();

                int second = DateTime.Now.Second;
                SecondHand.Seek(new TimeSpan(0, 0, second));

                int minute = DateTime.Now.Minute;
                LongHand.Seek(new TimeSpan(0, minute, second));

                int hour = DateTime.Now.Hour;
                HourHand.Seek(new TimeSpan(hour, minute, second));
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        protected void Timer_Tick(object sender, EventArgs e)
        {
            var dt = DateTime.Now;

            var timezone = TimeZoneInfo.Local;

            var name = timezone.DisplayName.Substring(timezone.DisplayName.IndexOf(")")+1).Trim();

            if (name != null && name != "")
            {
                name += " Time";
            }

            TimeTextBlock.Text = dt.ToString("h:mm tt ") + name;
        }
    }
}