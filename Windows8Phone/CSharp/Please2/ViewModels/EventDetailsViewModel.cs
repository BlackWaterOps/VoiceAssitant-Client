using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Shell;

using GalaSoft.MvvmLight.Command;

using LinqToVisualTree;

using Please2.Models;
using Please2.Util;

using Plexi;

namespace Please2.ViewModels
{
    public class EventDetailsViewModel : DetailsViewModel
    {
        public RelayCommand EventDirectionsLoaded { get; set; }

        public EventDetailsViewModel(INavigationService navigationService, IPlexiService plexiService) 
            : base(navigationService, plexiService)
        {
            AttachEventHandlers();
        }

        private void AttachEventHandlers()
        {
            PinToStartCommand = new RelayCommand(PinToStart);
            ShowFullMapCommand = new RelayCommand(ShowFullMap);

            EventDirectionsLoaded = new RelayCommand(AddDirectionsMap);
        }

        public void PinToStart()
        {
            EventModel ev = CurrentItem as EventModel;

            Uri uri = new Uri(String.Format("/Views/Details.xaml?id={0}", ev.id));
            string title = ev.title;
            string content = ev.location.address;
            string image = ev.image;

            base.PinToStart(uri, title, content, image);
        }

        public void AddDirectionsMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var map = currentPage.Descendants<Map>().Cast<Map>().FirstOrDefault();

            if (map != null)
            {
                var item = CurrentItem as EventModel;

                var layer = base.CreateMapLayer(item.location.lat, item.location.lon);

                map.Layers.Add(layer);
                map.Center = new GeoCoordinate(item.location.lat, item.location.lon);
            }
        }
        
        public async void ShowFullMap()
        {
            EventModel ev = CurrentItem as EventModel;
             
            GeoCoordinate geo = new GeoCoordinate(ev.location.lat, ev.location.lon);
            string title = ev.title;

            await base.ShowFullMap(title, geo);
        }
    }
}
