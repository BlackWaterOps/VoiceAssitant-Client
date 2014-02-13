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

namespace PlexiVoice.Controls
{
    public partial class Clock : UserControl
    {
        private DispatcherTimer timer;

        public event EventHandler Clock_Tick;

        public Clock()
        {
            InitializeComponent();

            ClockLayoutRoot.DataContext = this;

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
        }

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(
            "Time",
            typeof(DateTime),
            typeof(Clock),
            new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(OnTimePropertyChanged)));

        public DateTime Time
        {
            get { return (DateTime)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        private static void OnTimePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            Clock val = (Clock)dependencyObject;

            DateTime oldValue = (DateTime)eventArgs.OldValue;
            DateTime newValue = (DateTime)eventArgs.NewValue;

            if (oldValue != newValue)
            {
                val.StartClock();
            }
        }

        private void StartClock()
        {
            //ClockSecondHand.Begin();
            //ClockLongHand.Begin();
            //ClockHourHand.Begin();

            int second = Time.Second;
            //ClockSecondHand.Seek(new TimeSpan(0, 0, second));

            int minute = Time.Minute;
            //ClockLongHand.Seek(new TimeSpan(0, minute, second));

            int hour = Time.Hour;
            //ClockHourHand.Seek(new TimeSpan(hour, minute, second));

            //timer.Interval = TimeSpan.FromSeconds(60 - second);
            //timer.Start();

            HourTransform.Angle = ((hour > 12) ? (hour - 12) : hour) * 30;
            MinTransform.Angle = (minute * 6);
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Interval = TimeSpan.FromMinutes(1);

            EventHandler handler = Clock_Tick;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        } 
    }
}
