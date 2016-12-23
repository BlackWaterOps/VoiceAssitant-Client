using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Diagnostics;
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
    public class AlarmViewModel : AlarmBase
    {
        private AlarmItem currentAlarm;
        public AlarmItem CurrentAlarm
        {
            get { return currentAlarm; }
            set
            {
                currentAlarm = value;
                RaisePropertyChanged("CurrentAlarm");
            }
        }

        public RelayCommand AlarmItemSelection { get; set; }

        INavigationService navigationService;

        public AlarmViewModel()
        {
            this.navigationService = ViewModelLocator.GetServiceInstance<INavigationService>(); 

            AlarmItemSelection = new RelayCommand(AlarmSelected);
        }

        public override int SaveAlarm(string alarmName, DateTime alarmTime, List<string> daysOfWeek)
        {
            int id = base.SaveAlarm(alarmName, alarmTime, daysOfWeek);

            MainViewModel vm = ViewModelLocator.GetServiceInstance<MainViewModel>();

            // since this method is usually only called to save an alarm and show the template, 
            // let's go ahead and set the response message here.
            vm.AddDialog(DialogOwner.Plexi, String.Format("Ok. I set an alarm for {0}", alarmTime.ToString("h:mm tt")), DialogType.Complete);

            SetCurrentAlarm(id);

            return id;
        }

        public void SetCurrentAlarm(int id)
        {
            AlarmItem alarm = db.Alarms.Where(x => x.ID == id).FirstOrDefault();

            if (alarm != null)
            {
                CurrentAlarm = alarm;
            }
        }

        private void AlarmSelected()
        {
            DetailsViewModel vm = ViewModelLocator.GetServiceInstance<DetailsViewModel>();

            vm.CurrentItem = new AlarmListViewModel();

            navigationService.NavigateTo(ViewModelLocator.DetailsPageUri);
        }
    }
}
