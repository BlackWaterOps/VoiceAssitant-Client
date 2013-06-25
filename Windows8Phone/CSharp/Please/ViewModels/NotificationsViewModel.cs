using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Phone.Scheduler;

namespace Please.ViewModels
{
    class NotificationsViewModel : ViewModelBase
    {       
        private ObservableCollection<ScheduledNotification> _notifications;
        public ObservableCollection<ScheduledNotification> Notifications
        {
            get 
            {
                return _notifications;
            }
            set
            {
                _notifications = value;
                NotifyPropertyChanged("Notifications");
            }
        }

        public NotificationsViewModel()
        {
            LoadNotifications();
        }

        public void LoadNotifications()
        {
            
            var temp = new ObservableCollection<ScheduledNotification>();

            var enumerable = ScheduledActionService.GetActions<ScheduledNotification>();

            foreach (var notification in enumerable)
            {
                temp.Add(notification);
            }

            Notifications = temp;
        }

        public ObservableCollection<ScheduledNotification> getNotifications()
        {
            return Notifications;
        }
        
        public void deleteNotification(string name)
        {
            // Call Remove to unregister the scheduled action with the service.
            ScheduledActionService.Remove(name);

            // might need to remove it from the collection as well to see the change.
            /*
            foreach (var notification in Notifications)
            {
                if (notification.Name.Equals(name))
                {
                    Notifications.Remove(notification);
                }
            }
            */
        }
    }
}
