using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Media.Animation;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TimeZones;

using PlexiVoice.Util;
namespace PlexiVoice.ViewModels
{
    public class ClockViewModel : ViewModelBase
    {
        private DateTime time;
        public DateTime Time
        {
            get { return time; }
            set
            {
                time = value;
                RaisePropertyChanged("Time");
            }
        }

        private string timeZoneName;
        public string TimeZoneName
        {
            get { return timeZoneName; }
            set
            {
                timeZoneName = value;
                RaisePropertyChanged("TimeZoneName");
            }
        }

        public bool ObservesDST { get; set; }

        public RelayCommand ClockTicked { get; set; }

        public ClockViewModel()
        {
            ClockTicked = new RelayCommand(Clock_Tick);
        }

        private void Clock_Tick()
        {
            Time = Time.AddMinutes(1);
        }

        public void SetDateTime(string offset)
        {
            SetDateTime(Convert.ToDouble(offset));
        }

        public void SetDateTime(double offset)
        {
            SetDateTime(TimeSpan.FromHours(offset));
        }

        public void SetDateTime(TimeSpan offset)
        {
            var zones = TimeZoneService.AllTimeZones;

            ITimeZoneEx zone = zones.Where(x => x.BaseUtcOffset == offset).FirstOrDefault();

            if (zone != null)
            {
                DateTime utcDateTime = DateTime.UtcNow;

                DateTimeOffset utc = DateTimeOffset.UtcNow;

                DateTimeOffset service = TimeZoneService.SpecifyTimeZone(utc, zone);

                DateTimeOffset convert = zone.ConvertTime(utc);

                if (zone.IsDaylightSavingTime(convert) && ObservesDST)
                {
                    convert = convert.AddHours(-1);
                }

                Time = convert.DateTime;

                TimeZoneName = zone.StandardName;
            }
            else
            {
                // set default or throw error
            }
        }
    }
}
