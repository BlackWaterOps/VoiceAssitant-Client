using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Scheduler;

using Please2.Models;
using Please2.Resources;


// TODO: need to add properties so we can bind data returned from please
namespace Please2.ViewModels
{
    public class NotificationsViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private Visibility? reminderVisibility;
        public Visibility ReminderVisibility
        {
            get { return (reminderVisibility.HasValue == false) ? Visibility.Visible : reminderVisibility.Value; }
            set
            {
                reminderVisibility = value;
                RaisePropertyChanged("ReminderVisibility");
            }
        }
        
        private ObservableCollection<Reminder> reminders;
        public ObservableCollection<Reminder> Reminders
        {
            get { return reminders; }
            set 
            { 
                reminders = value;
                RaisePropertyChanged("Reminders");
            }
        }
        
        private Visibility? alarmVisibility;
        public Visibility AlarmVisibility
        {
            get { return (alarmVisibility.HasValue == false) ? Visibility.Visible : alarmVisibility.Value; }
            set
            {
                alarmVisibility = value;
                RaisePropertyChanged("AlarmVisibility");
            }
        }
        
        private ObservableCollection<Please2.Models.Alarm> alarms;
        public ObservableCollection<Please2.Models.Alarm> Alarms
        {
            get { return alarms; }
            set 
            { 
                alarms = value;
                RaisePropertyChanged("Alarms");
            }
        }

        public void LoadNotifications()
        {
            LoadReminders();
            //LoadAlarms();
        }

        #region Reminders
        public void LoadReminders()
        {
            var r = ScheduledActionService.GetActions<Reminder>().OrderBy(x => x.BeginTime);
            Debug.WriteLine(r);
            Debug.WriteLine(r.Count());

            foreach (var t in r)
            {
                Debug.WriteLine(t.Name);
                Debug.WriteLine(t.BeginTime);
                Debug.WriteLine(t.Title);
            }

            if (r.Count() > 0)
            {
                reminders = new ObservableCollection<Reminder>(r);
            }
            else
            {
                reminders = new ObservableCollection<Reminder>();
            }

            reminderVisibility = (reminders.Count > 0) ? Visibility.Collapsed : Visibility.Visible;
        }

        public void CreateReminder(DateTime reminderDate, string title)
        {
            string name = System.Guid.NewGuid().ToString();

            var reminder = new Microsoft.Phone.Scheduler.Reminder(name);

            reminder.RecurrenceType = RecurrenceInterval.None;

            reminder.Title = title;

            reminder.BeginTime = reminderDate;

            ScheduledActionService.Add(reminder);
        }

        public Reminder GetReminder(string name)
        {
            return ScheduledActionService.Find(name) as Reminder;
        }

        public void UpdateReminder(string name, DateTime reminderDate, string title)
        {
            var action = ScheduledActionService.Find(name);

            if (action != null)
            {
                var oldReminder = (Microsoft.Phone.Scheduler.Reminder)action;

                oldReminder.BeginTime = reminderDate;

                oldReminder.Title = title;

                ScheduledActionService.Replace(oldReminder);
            }
        }

        public void DeleteReminder(Reminder reminder)
        {
            DeleteNotification(reminder.Name);
        }

        // only for demos. remove for prod
        private void CreateDemoReminders()
        {
            // reset scheduled reminders
            var currentReminders = ScheduledActionService.GetActions<Microsoft.Phone.Scheduler.Reminder>();

            foreach (var reminder in currentReminders)
            {
                ScheduledActionService.Remove(reminder.Name);
            }

            // add dummy reminders
            var today = DateTime.Today;

            CreateReminder(today + new TimeSpan(1, 17, 30, 0), "go to grocery store");
            CreateReminder(DateTime.Now + new TimeSpan(1, 15, 0, 0), "pick up the dog from the vet");
            CreateReminder(DateTime.Now + new TimeSpan(2, 11, 30, 0), "brunch with the team");
        }
        #endregion

        #region Alarms
        public void LoadAlarms()
        {
            try
            {
                using (var db = new DatabaseModel(AppResources.DataStore))
                {
                    if (db.DatabaseExists().Equals(false))
                    {
                        this.alarms = new ObservableCollection<Please2.Models.Alarm>();
                        return;
                    }

                    IQueryable<Please2.Models.Alarm> query = from Please2.Models.Alarm alarms in db.Alarms select alarms;

                    if (query.Count() > 0)
                    {
                        // TODO: order by beginDate
                        // IOrderedQueryable<Please2.Models.Alarm> orderedQuery = query.OrderBy(cat => cat.OrderID);

                        this.alarms = new ObservableCollection<Please2.Models.Alarm>(query);
                    }
                    else
                    {
                        this.alarms = new ObservableCollection<Please2.Models.Alarm>();
                    }

                    alarmVisibility = (this.alarms.Count > 0) ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        public void CreateAlarm(string name, DateTime alarmTime, RecurrenceInterval interval, string day = null)
        {
            var alarm = new Microsoft.Phone.Scheduler.Alarm(name);

            alarm.RecurrenceType = interval;

            alarm.BeginTime = GetBeginDate(alarmTime, day);

            ScheduledActionService.Add(alarm);
        }

        public Please2.Models.Alarm GetAlarm(string id)
        {
            using (var db = new DatabaseModel(AppResources.DataStore))
            {
                 return db.Alarms.First(x => x.ID == int.Parse(id));
            }
        }

        public void SaveAlarm(DateTime alarmTime, List<DayOfWeek> daysOfWeek)
        {
            try
            {
                RecurrenceInterval interval = RecurrenceInterval.None;

                var names = new List<string>();

                string name;

                // create alarm(s) and add to scheduler
                if (daysOfWeek.Count == 7 || daysOfWeek.Count == 0)
                {
                    name = System.Guid.NewGuid().ToString();

                    names.Add(name);

                    interval = (daysOfWeek.Count == 7) ? RecurrenceInterval.Daily : RecurrenceInterval.None;

                    CreateAlarm(name, alarmTime, interval);
                }
                else if (daysOfWeek.Count > 0)
                {
                    foreach (DayOfWeek day in daysOfWeek)
                    {
                        name = System.Guid.NewGuid().ToString();

                        names.Add(name);

                        interval = RecurrenceInterval.Weekly;

                        string dayOfWeek = Enum.GetName(typeof(DayOfWeek), day);

                        CreateAlarm(name, alarmTime, interval, dayOfWeek);
                    }
                }

                // save alarm info to DB
                using (var db = new DatabaseModel(AppResources.DataStore))
                {
                    if (db.DatabaseExists() == false)
                    {
                        db.CreateDatabase();

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

                        DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();
                        dbUpdater.DatabaseSchemaVersion = App.APP_VERSION;
                        dbUpdater.Execute();
                    }
                    else
                    {
                        // check if db needs to be updated
                        /*
                         DatabaseSchemaUpdater dbUpdater = readerDB.CreateDatabaseSchemaUpdater();

                         if (dbUpdater.DatabaseSchemaVersion < App.APP_VERSION)
                         {
                         
                         }
                        */
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("Save Alarm Error: " + err.Message);
            }
        }

        public void UpdateAlarm(Please2.Models.Alarm alarm, DateTime alarmTime, RecurrenceInterval currentInterval, RecurrenceInterval newInterval, List<DayOfWeek> daysOfWeek)
        {
            try
            {
                var names = new List<string>();

                string newName;

                if (currentInterval != newInterval)
                {
                    if (currentInterval == RecurrenceInterval.Weekly || newInterval == RecurrenceInterval.Weekly)
                    {
                        // viewModel.DeleteAlarm(currentAlarm);

                        // remove old alarms from scheduler
                        foreach (var name in alarm.Names)
                        {
                            DeleteNotification(name);
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
                            foreach (DayOfWeek day in daysOfWeek)
                            {
                                newName = System.Guid.NewGuid().ToString();

                                names.Add(newName);

                                string dayOfWeek = Enum.GetName(typeof(DayOfWeek), day);

                                CreateAlarm(newName, alarmTime, newInterval, dayOfWeek);
                            }
                        }
                    }
                }

                // update interval and time on existing alarm
                else
                {
                    foreach (var name in alarm.Names)
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
                using (var db = new DatabaseModel(AppResources.DataStore))
                {
                    alarm.Time = alarmTime;
                    alarm.Names = names;
                    alarm.DaysOfWeek = daysOfWeek;
                    alarm.Interval = newInterval;

                    db.SubmitChanges();
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("Update Alarm Error: " + err.Message);
            }
        }

        public void DeleteAlarm(Please2.Models.Alarm alarm)
        {
            // remove alarms from scheduler
            foreach (var name in alarm.Names)
            {
                DeleteNotification(name);
            }

            // delete record from DB
            using (var db = new DatabaseModel(AppResources.DataStore))
            {
                db.Alarms.DeleteOnSubmit(alarm);
                db.SubmitChanges();
            }
        }

        // only for demos. remove for prod
        private void CreateDemoAlarms()
        {
            // reset scheduled alarms
            var currentAlarms = ScheduledActionService.GetActions<Microsoft.Phone.Scheduler.Alarm>();

            foreach (var alarm in currentAlarms)
            {
                ScheduledActionService.Remove(alarm.Name);
            }

            // add dummy alarms
            var now = DateTime.Now;

            SaveAlarm(now + new TimeSpan(2, 0, 0), new List<DayOfWeek>() { DayOfWeek.Monday });
            SaveAlarm(now + new TimeSpan(3, 0, 0), new List<DayOfWeek>() { DayOfWeek.Wednesday });
            SaveAlarm(now + new TimeSpan(4, 0, 0), new List<DayOfWeek>() { DayOfWeek.Saturday, DayOfWeek.Sunday });
        }
        #endregion

        #region Helpers
        // remove/delete either an alarm or a reminder
        protected void DeleteNotification(string name)
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
        #endregion
    }
}
