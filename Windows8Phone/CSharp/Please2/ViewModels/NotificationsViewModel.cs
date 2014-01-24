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

using GalaSoft.MvvmLight;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Resources;
using Please2.Util;

// TODO: split vm into a Reminder vm and Alarm vm which inherit Notification vm base
namespace Please2.ViewModels
{
    public class NotificationsViewModel : ViewModelBase
    {
        public ColorScheme Scheme { get { return ColorScheme.Notifications; } }

        public DatabaseModel db = new DatabaseModel(AppResources.DataStore);

        #region Alarm Properties
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
        public DateTime? ReminderDate
        {
            get { return (!reminderDate.HasValue) ? DateTime.Now : reminderDate.Value; }
            set
            {
                reminderDate = value;
                RaisePropertyChanged("ReminderDate");
            }
        }

        private DateTime? reminderTime;
        public DateTime? ReminderTime
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

        public NotificationsViewModel()
        {
            LoadNotifications();
        }

        public void LoadNotifications()
        {
            LoadReminders();

            CheckExpiredAlarms();
            LoadAlarms();
        }

        #region Reminders
        public void LoadReminders()
        {
            Debug.WriteLine("load reminders");
            var query = ScheduledActionService.GetActions<Microsoft.Phone.Scheduler.Reminder>().OrderBy(x => x.BeginTime);
           
            if (query.Count() > 0)
            {
                Reminders = new ObservableCollection<Microsoft.Phone.Scheduler.Reminder>(query);
            }
            else
            {
                Reminders = new ObservableCollection<Microsoft.Phone.Scheduler.Reminder>();
            }

            ReminderVisibility = (reminders.Count > 0) ? Visibility.Collapsed : Visibility.Visible;
        }

        public void SetCurrentReminder(string name)
        {
            Microsoft.Phone.Scheduler.Reminder reminder = GetReminder(name);

            SetCurrentReminder(reminder, null);
        }

        public void SetCurrentReminder(Microsoft.Phone.Scheduler.Reminder reminder, DateTime? newDateTime)
        {
            ReminderSubject = reminder.Title;

            if (newDateTime.HasValue)
            {
                ReminderDate = newDateTime.Value;
                ReminderTime = newDateTime.Value;
            }
            else
            {
                ReminderDate = reminder.BeginTime;
                ReminderTime = reminder.BeginTime;
            }
        }

        public void CreateReminder(DateTime reminderDate, string title)
        {
            string name = System.Guid.NewGuid().ToString();

            var reminder = new Microsoft.Phone.Scheduler.Reminder(name);

            reminder.RecurrenceType = RecurrenceInterval.None;

            reminder.Title = title;

            reminder.BeginTime = reminderDate;

            ScheduledActionService.Add(reminder);

            LoadReminders();
        }

        public Microsoft.Phone.Scheduler.Reminder GetReminder(string name)
        {
            return ScheduledActionService.Find(name) as Microsoft.Phone.Scheduler.Reminder;
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

                LoadReminders();
            }
        }

        public void DeleteReminder(Microsoft.Phone.Scheduler.Reminder reminder)
        {
            DeleteNotification(reminder.Name);

            ReminderSubject = String.Empty;
            ReminderDate = null;
            ReminderTime = null;

            LoadReminders();
        }

        // only for demos. remove for prod
        /*
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
        */
        #endregion

        #region Alarms
        public void CheckExpiredAlarms()
        {
            try
            {
                if (db.DatabaseExists() == false)
                {
                    return;
                }

                IEnumerable<AlarmItem> query = (from alarm in db.Alarms where alarm.Interval == RecurrenceInterval.None && alarm.IsEnabled == true select alarm).AsEnumerable().Where(x => x.Time < DateTime.Now);

                if (query.Count() > 0)
                {
                    foreach (AlarmItem alarm in query)
                    {
                        alarm.IsEnabled = false;
                    }

                    db.SubmitChanges();
                }
            }
            catch { }
        }

        public void LoadAlarms()
        {
            LoadAlarms(null);
        }

        public void LoadAlarms(DateTime? atTime)
        {
            try
            {
                if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();
                }

                IEnumerable<AlarmItem> query;

                if (atTime.HasValue)
                {
                    query = (from a in db.Alarms select a).AsEnumerable().Where(x => x.Time.ToString("hh:mm") == atTime.Value.ToString("hh:mm"));
                }
                else
                {
                   query = from a in db.Alarms select a;
                }

                if (query.Count() > 0)
                {
                    Alarms = new ObservableCollection<AlarmItem>(query);
                }
                else
                {
                    Alarms = new ObservableCollection<AlarmItem>();
                }

                AlarmVisibility = (this.alarms.Count > 0) ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("Load Alarms Error: {0}", err.Message));
            }
        }

        public void CreateAlarm(string name, DateTime alarmTime, RecurrenceInterval interval)
        {
            CreateAlarm(name, alarmTime, interval, null);
        }

        public void CreateAlarm(string name, DateTime alarmTime, RecurrenceInterval interval, string day)
        {
            var alarm = new Microsoft.Phone.Scheduler.Alarm(name);

            alarm.RecurrenceType = interval;

            alarm.BeginTime = GetBeginDate(alarmTime, day);

            ScheduledActionService.Add(alarm);
        }

        public void SetCurrentAlarm(string id)
        {
            int idInt = int.Parse(id);

            AlarmItem alarm = GetAlarm(idInt);

            SetCurrentAlarm(alarm, null);
        }

        public void SetCurrentAlarm(AlarmItem alarm, DateTime? newTime)
        {
            AlarmTime = (newTime.HasValue) ? newTime.Value : alarm.Time;

            AlarmName = alarm.DisplayName;

            List<string> days = new List<string>();

            foreach (AlarmDayItem item in alarm.Days)
            {
                days.Add(Enum.GetName(typeof(DayOfWeek), item.Day));
            }

            AlarmSelectedItems = days;
        }

        public AlarmItem GetAlarm(int id)
        {
            AlarmItem alarm = db.Alarms.First(x => x.ID == id);

            // gross hack
            /*
            foreach (var day in alarm.Days)
            {
            }
            foreach (var name in alarm.Names)
            {
            }
            */

            return alarm;
        }

        public void SaveAlarm(string alarmName, DateTime alarmTime, List<DayOfWeek> daysOfWeek)
        {
            try
            {
                RecurrenceInterval interval = RecurrenceInterval.None;

                EntitySet<AlarmNameItem> names = new EntitySet<AlarmNameItem>();

                string name;

                // create alarm(s) and add to scheduler
                if (daysOfWeek.Count == 7 || daysOfWeek.Count == 0)
                {
                    name = System.Guid.NewGuid().ToString();

                    //names.Add(name);
                    names.Add(new AlarmNameItem() { Name = name });

                    interval = (daysOfWeek.Count == 7) ? RecurrenceInterval.Daily : RecurrenceInterval.None;

                    CreateAlarm(name, alarmTime, interval);
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

                        CreateAlarm(name, alarmTime, interval, dayOfWeek);
                    }
                }

                EntitySet<AlarmDayItem> days = new EntitySet<AlarmDayItem>();

                foreach (DayOfWeek day in daysOfWeek)
                {
                    days.Add(new AlarmDayItem() { Day = day });
                }

                // save alarm info to DB

                if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();
                }
                else
                {
                    // check if db needs to be updated
                    /*
                     DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();

                     if (dbUpdater.DatabaseSchemaVersion < App.APP_VERSION)
                     {
                         
                     }
                    */
                }

                AlarmItem newAlarm = new AlarmItem
                    {
                        DisplayName = alarmName,
                        IsEnabled = true,
                        Names = names,
                        Time = alarmTime,
                        Interval = interval,
                        Days = days
                    };

                db.Alarms.InsertOnSubmit(newAlarm);

                db.SubmitChanges();

                // DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();
                // dbUpdater.DatabaseSchemaVersion = App.APP_VERSION;
                // dbUpdater.Execute();

                LoadAlarms();
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("Save Alarm Error: {0}", err.Message));
            }
        }

        public void UpdateAlarm(int alarmID, string newAlarmName, DateTime newAlarmTime, RecurrenceInterval newInterval, List<DayOfWeek> newDaysOfWeek)
        {
            try
            {

                AlarmItem alarm = db.Alarms.FirstOrDefault(x => x.ID == alarmID);

                if (alarm != null)
                {

                    // get the current alarm interval so we can run a comparison
                    RecurrenceInterval currentInterval = alarm.Interval;

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

                                CreateAlarm(newName, newAlarmTime, newInterval);
                            }
                            // switching from daily or single to weekly
                            else if (newInterval == RecurrenceInterval.Weekly)
                            {
                                foreach (DayOfWeek day in newDaysOfWeek)
                                {
                                    newName = System.Guid.NewGuid().ToString();

                                    newNames.Add(new AlarmNameItem() { Name = newName });

                                    string dayOfWeek = Enum.GetName(typeof(DayOfWeek), day);

                                    CreateAlarm(newName, newAlarmTime, newInterval, dayOfWeek);
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

                                oldAlarm.BeginTime = newAlarmTime;
                                oldAlarm.RecurrenceType = newInterval;

                                ScheduledActionService.Replace(oldAlarm);
                            }
                        }
                    }

                    // create the alarm days entity reference
                    EntitySet<AlarmDayItem> days = new EntitySet<AlarmDayItem>();

                    foreach (DayOfWeek day in newDaysOfWeek)
                    {
                        days.Add(new AlarmDayItem() { Day = day });
                    }

                    // update DB record
                    alarm.DisplayName = newAlarmName;
                    alarm.Time = newAlarmTime;
                    alarm.Names = newNames;
                    alarm.Days = days;
                    alarm.Interval = newInterval;
                    alarm.IsEnabled = true;

                    db.SubmitChanges();
                }

                LoadAlarms();
            }
            catch (Exception err)
            {
                Debug.WriteLine("Update Alarm Error: " + err.Message);
            }
        }

        public void DeleteAlarm(int alarmID)
        {
            try
            {
                AlarmItem alarm = db.Alarms.FirstOrDefault(x => x.ID == alarmID);

                if (alarm != null)
                {
                    // remove alarms from scheduler
                    foreach (AlarmNameItem item in alarm.Names)
                    {
                        DeleteNotification(item.Name);
                    }

                    // delete record from DB
                    db.AlarmDays.DeleteAllOnSubmit(alarm.Days);
                    db.AlarmNames.DeleteAllOnSubmit(alarm.Names);
                    db.Alarms.DeleteOnSubmit(alarm);
                    db.SubmitChanges();
                }

                AlarmName = String.Empty;
                AlarmTime = null;
                AlarmSelectedItems = new List<string>();

                LoadAlarms();
            }
            catch (Exception e)
            {
                Debug.WriteLine(String.Format("Delete Alarm Error: {0}", e.Message));
            }
        }

        // only for demos. remove for prod
        /*
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
        */
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
