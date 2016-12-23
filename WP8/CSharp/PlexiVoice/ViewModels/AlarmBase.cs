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

using PlexiVoice.Models;
using PlexiVoice.Resources;

namespace PlexiVoice.ViewModels
{
    public class AlarmBase : NotificationsViewModel
    {
        protected void CreateAlarm(string name, DateTime alarmTime, RecurrenceInterval interval)
        {
            CreateAlarm(name, alarmTime, interval, null);
        }

        protected void CreateAlarm(string name, DateTime alarmTime, RecurrenceInterval interval, DayOfWeek? day)
        {
            var alarm = new Microsoft.Phone.Scheduler.Alarm(name);

            alarm.RecurrenceType = interval;

            alarm.BeginTime = GetBeginDate(alarmTime, day);

            ScheduledActionService.Add(alarm);
        }

        public AlarmItem GetAlarm(int id)
        {
            return db.Alarms.First(x => x.ID == id);
        }

        public virtual int SaveAlarm(string alarmName, DateTime alarmTime, List<string> daysOfWeek)
        {
            List<DayOfWeek> list = new List<DayOfWeek>();

            foreach (string day in daysOfWeek)
            {
                list.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), (string)day, true));
            }

            return SaveAlarm(alarmName, alarmTime, list);
        }

        public int SaveAlarm(string alarmName, DateTime alarmTime, List<DayOfWeek> daysOfWeek)
        {
            try
            {
                if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();

                    DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();
                    dbUpdater.DatabaseSchemaVersion = App.APP_VERSION;
                    dbUpdater.Execute();
                }
                else
                {
                    DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();

                    if (dbUpdater.DatabaseSchemaVersion < 2)
                    {
                        dbUpdater.AddColumn<AlarmNameItem>("Day");
                        dbUpdater.DatabaseSchemaVersion = App.APP_VERSION;
                        dbUpdater.Execute();
                    }
                }

                RecurrenceInterval interval = RecurrenceInterval.None;

                EntitySet<AlarmNameItem> names = new EntitySet<AlarmNameItem>();

                string name;

                // create alarm(s) and add to scheduler
                if (daysOfWeek.Count == 7 || daysOfWeek.Count == 0)
                {
                    name = System.Guid.NewGuid().ToString();

                    names.Add(new AlarmNameItem() { Name = name });

                    interval = (daysOfWeek.Count == 7) ? RecurrenceInterval.Daily : RecurrenceInterval.None;

                    CreateAlarm(name, alarmTime, interval);
                }
                else if (daysOfWeek.Count > 0)
                {
                    foreach (DayOfWeek day in daysOfWeek)
                    {
                        name = System.Guid.NewGuid().ToString();

                        names.Add(new AlarmNameItem() { Name = name, Day = day });

                        interval = RecurrenceInterval.Weekly;

                        //string dayOfWeek = Enum.GetName(typeof(DayOfWeek), day);

                        CreateAlarm(name, alarmTime, interval, day);
                    }
                }

                /*
                EntitySet<AlarmDayItem> days = new EntitySet<AlarmDayItem>();

                foreach (DayOfWeek day in daysOfWeek)
                {
                    days.Add(new AlarmDayItem() { Day = day });
                }
                */

                // save alarm info to DB

                AlarmItem newAlarm = new AlarmItem
                {
                    DisplayName = alarmName,
                    IsEnabled = true,
                    Names = names,
                    Time = alarmTime,
                    Interval = interval,
                    //Days = days
                };

                db.Alarms.InsertOnSubmit(newAlarm);

                db.SubmitChanges();

                return newAlarm.ID;
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("Save Alarm Error: {0}", err.Message));

                return default(int);
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

                                    newNames.Add(new AlarmNameItem() { Name = newName, Day = day });

                                    //string dayOfWeek = Enum.GetName(typeof(DayOfWeek), day);

                                    CreateAlarm(newName, newAlarmTime, newInterval, day);
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
                    /*
                    EntitySet<AlarmDayItem> days = new EntitySet<AlarmDayItem>();

                    foreach (DayOfWeek day in newDaysOfWeek)
                    {
                        days.Add(new AlarmDayItem() { Day = day });
                    }
                    */

                    // update DB record
                    alarm.DisplayName = newAlarmName;
                    alarm.Time = newAlarmTime;
                    alarm.Names = newNames;
                    //alarm.Days = days;
                    alarm.Interval = newInterval;

                    db.SubmitChanges();
                }
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
                    //db.AlarmDays.DeleteAllOnSubmit(alarm.Days);
                    db.AlarmNames.DeleteAllOnSubmit(alarm.Names);
                    db.Alarms.DeleteOnSubmit(alarm);
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(String.Format("Delete Alarm Error: {0}", e.Message));
            }
        }
    }
}
