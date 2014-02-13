using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PlexiVoice.Models;
namespace PlexiVoice.ViewModels
{
    public class AlarmDetailsViewModel : AlarmBase
    {
        private string alarmName;
        public string AlarmName
        {
            get { return alarmName; }
            set
            {
                alarmName = value;
                RaisePropertyChanged("AlarmName");
            }
        }

        private DateTime? alarmTime;
        public DateTime? AlarmTime
        {
            get { return (!alarmTime.HasValue) ? DateTime.Now : alarmTime.Value; }
            set
            {
                alarmTime = value;
                RaisePropertyChanged("AlarmTime");
            }
        }

        private List<string> alarmSelectedItems;
        public List<string> AlarmSelectedItems
        {
            get { return alarmSelectedItems; }
            set
            {
                alarmSelectedItems = value;
                RaisePropertyChanged("AlarmSelectedItems");
            }
        }

        public AlarmDetailsViewModel()
        {
        }

        public void SetCurrentAlarm(string id)
        {
            int idInt = int.Parse(id);

            AlarmItem alarm = base.GetAlarm(idInt);

            AlarmTime = alarm.Time;

            AlarmName = alarm.DisplayName;

            List<string> days = new List<string>();

            foreach (AlarmNameItem item in alarm.Names)
            {
                if (item.Day.HasValue)
                {
                    days.Add(Enum.GetName(typeof(DayOfWeek), item.Day.Value));
                }
            }
            
            /*
            foreach (AlarmDayItem item in alarm.Days)
            {
                days.Add(Enum.GetName(typeof(DayOfWeek), item.Day));
            }
            */

            AlarmSelectedItems = days;
        }
    }
}
