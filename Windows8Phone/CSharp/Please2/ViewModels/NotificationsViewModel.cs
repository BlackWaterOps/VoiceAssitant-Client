using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Scheduler;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Resources;
using Please2.Util;

// TODO: split vm into a Reminder vm and Alarm vm which inherit Notification vm base
namespace Please2.ViewModels
{
    public class NotificationsViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        public ColorScheme Scheme { get { return ColorScheme.Notifications; } }

        #region Alarm Properties
        private DateTime? alarmTime;
        public DateTime AlarmTime
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

        private Visibility? alarmVisibility;
        public Visibility AlarmVisibility
        {
            get { return (!alarmVisibility.HasValue) ? Visibility.Visible : alarmVisibility.Value; }
            set
            {
                alarmVisibility = value;
                RaisePropertyChanged("AlarmVisibility");
            }
        }

        private ObservableCollection<AlarmItem> alarms;
        public ObservableCollection<AlarmItem> Alarms
        {
            get { return alarms; }
            set
            {
                alarms = value;
                RaisePropertyChanged("Alarms");
            }
        }
        #endregion

        #region Reminder Properties
        private DateTime? reminderDate;
        public DateTime ReminderDate
        {
            get { return (!reminderDate.HasValue) ? DateTime.Now : reminderDate.Value; }
            set
            {
                reminderDate = value;
                RaisePropertyChanged("ReminderDate");
            }
        }

        private DateTime? reminderTime;
        public DateTime ReminderTime
        {
            get { return (!reminderTime.HasValue) ? DateTime.Now : reminderTime.Value; }
            set
            {
                reminderTime = value;
                RaisePropertyChanged("ReminderTime");
            }
        }

        private string reminderSubject;
        public string ReminderSubject
        {
            get { return reminderSubject; }
            set
            {
                reminderSubject = value;
                RaisePropertyChanged("ReminderSubject");
            }
        }

        private Visibility? reminderVisibility;
        public Visibility ReminderVisibility
        {
            get { return (!reminderVisibility.HasValue) ? Visibility.Visible : reminderVisibility.Value; }
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
        #endregion

        public void LoadNotifications()
        {
            //LoadReminders();
            LoadAlarms();
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

            ReminderVisibility = (reminders.Count > 0) ? Visibility.Collapsed : Visibility.Visible;
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
                        this.alarms = new ObservableCollection<AlarmItem>();
                        return;
                    }

                    var query = from a in db.Alarms 
                                select a;

                    if (query.Count() > 0)
                    {
                        // TODO: order by beginDate
                        // IOrderedQueryable<Please2.Models.Alarm> orderedQuery = query.OrderBy(cat => cat.OrderID);

                        // hack to force the entityset enumeration for days and names
                        foreach (var item in query)
                        {
                            foreach (var day in item.Days)
                            { }

                            foreach (var name in item.Names)
                            { }
                        }

                        Alarms = new ObservableCollection<AlarmItem>(query);
                    }
                    else
                    {
                        Alarms = new ObservableCollection<AlarmItem>();
                    }

                    AlarmVisibility = (this.alarms.Count > 0) ? Visibility.Collapsed : Visibility.Visible;
                    
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("Load Alarms Error: {0}", err.Message));
            }
        }

        public void CreateAlarm(string name, DateTime alarmTime, RecurrenceInterval interval, string day = null)
        {
            var alarm = new Microsoft.Phone.Scheduler.Alarm(name);

            alarm.RecurrenceType = interval;

            alarm.BeginTime = GetBeginDate(alarmTime, day);

            ScheduledActionService.Add(alarm);
        }

        public AlarmItem GetAlarm(string id)
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

                //var names = new List<string>();

                EntitySet<AlarmNameItem> names = new EntitySet<AlarmNameItem>();

                string name;

                // create alarm(s) and add to scheduler
                if (daysOfWeek.Count == 7 || daysOfWeek.Count == 0)
                {
                    name = System.Guid.NewGuid().ToString();

                    //names.Add(name);
                    names.Add(new AlarmNameItem() { Name = name });

                    interval = (daysOfWeek.Count == 7) ? RecurrenceInterval.Daily : RecurrenceInterval.None;

                    //CreateAlarm(name, alarmTime, interval);
                }
                else if (daysOfWeek.Count > 0)
                {
                    foreach (DayOfWeek day in daysOfWeek)
                    {
                        name = System.Guid.NewGuid().ToString();

                        //names.Add(name);
                        names.Add(new AlarmNameItem() { Name = name });

                        interval = RecurrenceInterval.Weekly;

                        string dayOfWeek = Enum.GetName(typeof(DayOfWeek), day);

                        //CreateAlarm(name, alarmTime, interval, dayOfWeek);
                    }
                }

                EntitySet<AlarmDayItem> days = new EntitySet<AlarmDayItem>();

                foreach (DayOfWeek day in daysOfWeek)
                {
                    days.Add(new AlarmDayItem() { Day = day });
                }
              
                // save alarm info to DB
                using (var db = new DatabaseModel(AppResources.DataStore))
                {
                    if (db.DatabaseExists() == false)
                    {
                        db.CreateDatabase();

                        db.Alarms.InsertOnSubmit(
                            new AlarmItem
                            {
                                IsEnabled = true,
                                Names = names,
                                Time = alarmTime,
                                Interval = interval,
                                Days = days
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

        public void UpdateAlarm(AlarmItem alarm, DateTime alarmTime, RecurrenceInterval currentInterval, RecurrenceInterval newInterval, List<DayOfWeek> daysOfWeek)
        {
            try
            {
                // a list of alarm guids currently in the DB
                EntitySet<AlarmNameItem> currNames = alarm.Names;

                // a list of newly generated guids for new alarms
                EntitySet<AlarmNameItem> newNames = new EntitySet<AlarmNameItem>();

                string newName;

                if (currentInterval != newInterval)
                {
                    if (currentInterval == RecurrenceInterval.Weekly || newInterval == RecurrenceInterval.Weekly)
                    {
                        // viewModel.DeleteAlarm(currentAlarm);

                        // remove old alarms from scheduler
                        foreach (AlarmNameItem item in currNames)
                        {
                            DeleteNotification(item.Name);
                        }

                        // switching from a weekly alarm to daily or single
                        if (currentInterval == RecurrenceInterval.Weekly)
                        {
                            newName = System.Guid.NewGuid().ToString();

                            newNames.Add(new AlarmNameItem() { Name = newName });

                            CreateAlarm(newName, alarmTime, newInterval);
                        }
                        // switching from daily or single to weekly
                        else if (newInterval == RecurrenceInterval.Weekly)
                        {
                            foreach (DayOfWeek day in daysOfWeek)
                            {
                                newName = System.Guid.NewGuid().ToString();

                                newNames.Add(new AlarmNameItem() { Name = newName });

                                string dayOfWeek = Enum.GetName(typeof(DayOfWeek), day);

                                CreateAlarm(newName, alarmTime, newInterval, dayOfWeek);
                            }
                        }
                    }
                }
                 
                // update interval and time on existing alarm
                else
                {
                    foreach (AlarmNameItem item in currNames)
                    {
                        var action = ScheduledActionService.Find(item.Name);

                        if (action != null)
                        {
                            var oldAlarm = (Microsoft.Phone.Scheduler.Alarm)action;

                            oldAlarm.BeginTime = alarmTime;
                            oldAlarm.RecurrenceType = newInterval;

                            ScheduledActionService.Replace(oldAlarm);
                        }
                    }
                }

                // create the alarm days entity reference
                EntitySet<AlarmDayItem> days = new EntitySet<AlarmDayItem>();

                foreach (DayOfWeek day in daysOfWeek)
                {
                    days.Add(new AlarmDayItem() { Day = day });
                }

                // update DB record
                using (var db = new DatabaseModel(AppResources.DataStore))
                {
                    alarm.Time = alarmTime;
                    alarm.Names = newNames;
                    alarm.Days = days;
                    alarm.Interval = newInterval;

                    db.SubmitChanges();
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("Update Alarm Error: " + err.Message);
            }
        }

        public void DeleteAlarm(AlarmItem alarm)
        {
            // remove alarms from scheduler
            foreach (AlarmNameItem item in alarm.Names)
            {
                DeleteNotification(item.Name);
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

                beginDate = beginDate.AddDays(offset);
            }

            if (currentDate > beginDate)
            {
                beginDate = (day == null) ? beginDate.AddDays(1) : beginDate.AddDays(7);
            }

            Debug.WriteLine(String.Format("Begin Date: {0}", beginDate));

            return beginDate;
        }
        #endregion
    }
}
