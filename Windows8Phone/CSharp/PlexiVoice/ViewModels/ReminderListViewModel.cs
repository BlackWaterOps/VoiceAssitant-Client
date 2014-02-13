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

namespace PlexiVoice.ViewModels
{
    public class ReminderListViewModel : NotificationsViewModel
    {
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

        public RelayCommand<Reminder> ReminderChecked { get; set; }
        public RelayCommand<Reminder> ReminderUnchecked { get; set; } 

        public ReminderListViewModel()
        {
            ReminderChecked = new RelayCommand<Reminder>(EnableReminder);
            ReminderUnchecked = new RelayCommand<Reminder>(DisableReminder);

            LoadReminders();
        }

        public void LoadReminders()
        {
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

        private void EnableReminder(Reminder reminder)
        {

        }

        private void DisableReminder(Reminder reminder)
        {

        }
    }
}
