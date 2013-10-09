using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json;

using Please2.Models;
using Please2.Util;

namespace Please2.ViewModels
{
    public class EventsViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private List<EventModel> _eventResults;
        public List<EventModel> EventResults
        {
            get { return _eventResults; }
            set { _eventResults = value; }
        }

        private INavigationService navigationService;

        public RelayCommand<EventModel> EventItemSelection { get; set; }

        public EventsViewModel(INavigationService navigationService, IPleaseService pleaseService)
        {
            this.navigationService = navigationService;

            // attach event handlers
            EventItemSelection = new RelayCommand<EventModel>(EventItemSelected);
        }

        public void EventItemSelected(EventModel e)
        {
           // navigationService.NavigateTo(new Uri("/Views/EventDetailsPage.xaml?id=" + e.id, UriKind.Relative));
            navigationService.NavigateTo(new Uri("/Views/Details.xaml?template=event&id=" + e.id, UriKind.Relative));
        }
    }
}
