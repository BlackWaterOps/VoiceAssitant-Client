using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;

using System.Windows.Threading;
using System.Threading;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

using Windows.Storage;

using Please2.Models;
using Please2.Resources;

namespace Please2.Pages
{
    public partial class AlarmPage : PhoneApplicationPage
    {
        Please2.Models.Alarm currentAlarm = null;

        DatabaseModel db;

        public AlarmPage()
        {
            InitializeComponent();

            DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string alarmID;

            if (NavigationContext.QueryString.TryGetValue("id", out alarmID))
            {
                DeleteSavePanel.Visibility = Visibility.Visible;
                SavePanel.Visibility = Visibility.Collapsed;

                using (db = new DatabaseModel(AppResources.DataStore))
                {
                    currentAlarm = db.Alarms.First(x => x.ID == int.Parse(alarmID));
                }
            }
        }

        protected void SaveAlarmButton_Click(object sender, EventArgs e)
        {
            try
            {
                var daysOfWeek = new List<DayOfWeek>();

                var names = new List<string>();

                string name;

                DateTime alarmTime = (DateTime)AlarmTime.Value;

                RecurrenceInterval interval = RecurrenceInterval.None;

                // add selected days to a list which will be stored in the DB
                foreach (var day in AlarmRecurringDays.SelectedItems)
                {
                    daysOfWeek.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), (string)day, true));
                }

                // create alarm(s) and add to scheduler
                if (AlarmRecurringDays.SelectedItems.Count == 7 || AlarmRecurringDays.SelectedItems.Count == 0)
                {
                    name = System.Guid.NewGuid().ToString();

                    names.Add(name);

                    interval = (AlarmRecurringDays.SelectedItems.Count == 7) ? RecurrenceInterval.Daily : RecurrenceInterval.None;

                    CreateAlarm(name, alarmTime, interval); 
                }
                else if (AlarmRecurringDays.SelectedItems.Count > 0)
                {
                    foreach (string day in AlarmRecurringDays.SelectedItems)
                    {
                        name = System.Guid.NewGuid().ToString();

                        names.Add(name);

                        interval = RecurrenceInterval.Weekly;

                        CreateAlarm(name, alarmTime, interval, day);
                    }
                }
                
                // save alarm info to DB
                using (db = new DatabaseModel(AppResources.DataStore))
                {
                    if (db.DatabaseExists().Equals(false))
                    {
                        db.CreateDatabase();
                    }

                    db.Alarms.InsertOnSubmit(
                        new Please2.Models.Alarm
                        {
                            IsEnabled = true,
                            Names = names,
                            Time = alarmTime,
                            Interval = interval,
                            DaysOfWeek = daysOfWeek
                        }
                    );
              
                    db.SubmitChanges();
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        protected void UpdateAlarmButton_Click(object sender, EventArgs e)
        {
            if (currentAlarm != null)
            {
                var daysOfWeek = new List<DayOfWeek>();
                
                var names = new List<string>(); 
                
                string newName;

                var currentInterval = currentAlarm.Interval;

                RecurrenceInterval newInterval;

                var alarmTime = (DateTime)AlarmTime.Value;

                // populate new list of DayOfWeek
                foreach (var day in AlarmRecurringDays.SelectedItems)
                {
                    daysOfWeek.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), (string)day, true));
                }

                // set new alarm interval
                if (AlarmRecurringDays.SelectedItems.Count == 7)
                {
                    newInterval = RecurrenceInterval.Daily;
                }
                else if (AlarmRecurringDays.SelectedItems.Count > 0)
                {
                    newInterval = RecurrenceInterval.Weekly;
                }
                else
                {
                    newInterval = RecurrenceInterval.None;
                }

                if (currentInterval != newInterval)
                {
                    if (currentInterval == RecurrenceInterval.Weekly || newInterval == RecurrenceInterval.Weekly)
                    {
                        // remove old alarms from scheduler
                        foreach (var name in currentAlarm.Names)
                        {
                            DeleteAlarm(name);
                        }

                        // switching from a weekly alarm to daily or single
                        if (currentInterval == RecurrenceInterval.Weekly)
                        {
                            newName = System.Guid.NewGuid().ToString();

                            names.Add(newName);

                            CreateAlarm(newName, alarmTime, newInterval);
                        }
                        // switching from daily or single to weekly
                        else if (newInterval == RecurrenceInterval.Weekly)
                        {
                            foreach (string day in AlarmRecurringDays.SelectedItems)
                            {
                                newName = System.Guid.NewGuid().ToString();

                                names.Add(newName);

                                CreateAlarm(newName, alarmTime, newInterval, day);
                            }
                        }
                    }
                }

                // update interval and time on existing alarm
                else
                {
                    foreach (var name in currentAlarm.Names)
                    {
                        var action = ScheduledActionService.Find(name);

                        if (action != null)
                        {
                            var oldAlarm = (Microsoft.Phone.Scheduler.Alarm)action;

                            oldAlarm.BeginTime = alarmTime;
                            oldAlarm.RecurrenceType = newInterval;

                            ScheduledActionService.Replace(oldAlarm);
                        }
                    }
                }
                
                // update DB record
                using (db = new DatabaseModel(AppResources.DataStore))
                {
                    currentAlarm.Time = alarmTime;
                    currentAlarm.Names = names;
                    currentAlarm.DaysOfWeek = daysOfWeek;
                    currentAlarm.Interval = newInterval;

                    db.SubmitChanges();
                }
            }
        }

        protected void DeleteAlarmButton_Click(object sender, EventArgs e)
        {

            if (currentAlarm != null)
            {
                // remove alarms from scheduler
                foreach (var name in currentAlarm.Names)
                {
                    DeleteAlarm(name);
                }

                // delete record from DB
                using (db = new DatabaseModel(AppResources.DataStore))
                {
                    db.Alarms.DeleteOnSubmit(currentAlarm);
                    db.SubmitChanges();
                }
            }
        }

        protected void CreateAlarm(string name, DateTime alarmTime, RecurrenceInterval interval, string day = null)
        {
            var alarm = new Microsoft.Phone.Scheduler.Alarm(name);

            alarm.RecurrenceType = interval;

            alarm.BeginTime = GetBeginDate(alarmTime, day);

            ScheduledActionService.Add(alarm);
        }

        protected void DeleteAlarm(string name)
        {
            if (ScheduledActionService.Find(name) != null)
            {
                ScheduledActionService.Remove(name);
            }
        }

        protected DateTime GetBeginDate(DateTime beginDate, string day = null)
        {
            var currentDate = DateTime.Now;

            if (day != null)
            {
                var dayInt = (int)Enum.Parse(typeof(DayOfWeek), day, true);
                var currDayInt = (int)currentDate.DayOfWeek;

                var offset = (currDayInt < dayInt) ? (dayInt - currDayInt) : (7 - (currDayInt - dayInt));

                beginDate.AddDays(offset);
            }

            if (currentDate > beginDate)
            {
                if (day == null)
                {
                    // daily or single 
                    beginDate.AddDays(1);
                }
                else
                {
                    // weekly
                    beginDate.AddDays(7);
                }
            }

            return beginDate;
        }
    }
}