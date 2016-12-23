using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Phone.Scheduler;

using GalaSoft.MvvmLight;

using PlexiVoice.Models;
using PlexiVoice.Resources;

namespace PlexiVoice.ViewModels
{
    public class NotificationsViewModel : ViewModelBase
    {
        protected DatabaseModel db = new DatabaseModel(AppResources.DataStore);

        public void LoadNotifications()
        {
        }
        
        // remove/delete either an alarm or a reminder
        protected void DeleteNotification(string name)
        {
            if (ScheduledActionService.Find(name) != null)
            {
                ScheduledActionService.Remove(name);
            }
        }

        protected DateTime GetBeginDate(DateTime beginDate)
        {
            return GetBeginDate(beginDate, null);
        }

        protected DateTime GetBeginDate(DateTime beginDate, DayOfWeek? day)
        {
            var currentDate = DateTime.Now;

            if (day.HasValue)
            {
                //var dayInt = (int)Enum.Parse(typeof(DayOfWeek), day, true);
                int dayInt = (int)day.Value;
                
                var currDayInt = (int)currentDate.DayOfWeek;

                var offset = (currDayInt < dayInt) ? (dayInt - currDayInt) : (7 - (currDayInt - dayInt));

                beginDate = beginDate.AddDays(offset);
            }

            if (currentDate > beginDate)
            {
                beginDate = (day == null) ? beginDate.AddDays(1) : beginDate.AddDays(7);
            }

            Debug.WriteLine(String.Format("Begin Date: {0}", beginDate));

            return beginDate;
        }
    }
}
