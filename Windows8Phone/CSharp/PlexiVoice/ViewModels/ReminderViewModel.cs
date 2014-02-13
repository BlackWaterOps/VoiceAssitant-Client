using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Phone.Scheduler;

using GalaSoft.MvvmLight.Command;

using PlexiVoice.Models;
using PlexiVoice.Resources;
using PlexiVoice.Util;

namespace PlexiVoice.ViewModels
{
    public class ReminderViewModel : ReminderBase
    {
        private Reminder currentReminder;
        public Reminder CurrentReminder
        {
            get { return currentReminder; }
            set
            {
                currentReminder = value;
                RaisePropertyChanged("CurrentReminder");
            }
        }
        
        public RelayCommand ReminderItemSelection { get; set; }

        INavigationService navigationService;

        public ReminderViewModel()
        {
            this.navigationService = ViewModelLocator.GetServiceInstance<INavigationService>(); 

            ReminderItemSelection = new RelayCommand(ReminderSelected);
        }

        public override string CreateReminder(DateTime reminderDate, string title)
        {
            string guid = base.CreateReminder(reminderDate, title);

            MainViewModel vm = ViewModelLocator.GetServiceInstance<MainViewModel>();

            // since this method is usually only called to save an alarm and show the template, 
            // let's go ahead and set the response message here.
            vm.AddDialog(DialogOwner.Plexi, String.Format("Ok. I set a reminder for {0}", reminderDate.ToString("h:mm tt")), DialogType.Complete);

            SetCurrentReminder(guid);

            return guid;
        }

        public void SetCurrentReminder(string guid)
        {
            Reminder reminder = ScheduledActionService.GetActions<Reminder>().Where(x => x.Name == guid).FirstOrDefault();

            if (reminder != null)
            {
                CurrentReminder = reminder;
            }
        }

        private void ReminderSelected()
        {
            DetailsViewModel vm = ViewModelLocator.GetServiceInstance<DetailsViewModel>();

            vm.CurrentItem = new ReminderListViewModel();

            navigationService.NavigateTo(ViewModelLocator.DetailsPageUri);
        }
    }
}
