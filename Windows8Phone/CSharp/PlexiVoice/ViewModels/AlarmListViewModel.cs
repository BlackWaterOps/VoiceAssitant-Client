using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Phone.Scheduler;

using GalaSoft.MvvmLight.Command;

using PlexiVoice.Models;
namespace PlexiVoice.ViewModels
{
    public class AlarmListViewModel : AlarmBase
    {
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

        public RelayCommand<AlarmItem> AlarmChecked { get; set; }
        public RelayCommand<AlarmItem> AlarmUnchecked { get; set; } 

        public AlarmListViewModel()
        {
            
            //dirty way to reset the db and notifications
            /*
            db.DeleteDatabase();
            var actions = ScheduledActionService.GetActions<ScheduledNotification>();

            foreach (var action in actions)
            {
                ScheduledActionService.Remove(action.Name);
            }
            */

            AlarmChecked = new RelayCommand<AlarmItem>(EnableAlarm);
            AlarmUnchecked = new RelayCommand<AlarmItem>(DisableAlarm);

            LoadAlarms();
        }

        public void LoadAlarms()
        {
            /*
            try
            {
                if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();
                }

                IQueryable<AlarmItem> query = from a in db.Alarms select a;

                if (query.Count() > 0)
                {
                    // TODO: order by beginDate
                    // IOrderedQueryable<Please2.Models.Alarm> orderedQuery = query.OrderBy(cat => cat.OrderID);

                    // hack to force the entityset enumeration for days and names
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
            */
        }

        private void EnableAlarm(AlarmItem alarm)
        {
            foreach (AlarmNameItem item in alarm.Names)
            {
                // remove any lingering scheduled/unscheduled alarms
                if (ScheduledActionService.Find(item.Name) != null)
                {
                    base.DeleteNotification(item.Name);
                }

                // recreate scheduled alarms
                //CreateAlarm()
            }

            alarm.IsEnabled = true;

            db.SubmitChanges();
        }

        private void DisableAlarm(AlarmItem alarm)
        {
            // the alarm db record will remain but the actual scheduled alarms will be deleted
            foreach (AlarmNameItem item in alarm.Names)
            {
                base.DeleteNotification(item.Name);
            }

            alarm.IsEnabled = false;

            db.SubmitChanges();
        }
    }
}
