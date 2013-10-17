using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

using GalaSoft.MvvmLight.Command;

using Please2.Util;

namespace Please2.ViewModels
{
    public class DetailsViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private object currentItem;
        public object CurrentItem
        {
            get { return currentItem; }
            set
            {
                currentItem = value;
                RaisePropertyChanged("CurrentItem");
            }
        }

        private DataTemplate contentTemplate;
        public DataTemplate ContentTemplate
        {
            get { return contentTemplate; }
            set
            {
                contentTemplate = value;
                RaisePropertyChanged("ContentTemplate");
            }
        }

        public RelayCommand<object> PinToStartCommand { get; set; }
        public RelayCommand<object> ShowFullMapCommand { get; set; }

        public DetailsViewModel(INavigationService navigationService, IPleaseService pleaseService)
        {
            PinToStartCommand = new RelayCommand<object>(PinToStart);
            ShowFullMapCommand = new RelayCommand<object>(ShowFullMap);
        }

        public void SetDetailsTemplate(string template)
        {
            var templates = ViewModelLocator.DetailsTemplates;

            if (templates[template] != null)
            {
                ContentTemplate = templates[template] as DataTemplate;
                Debug.WriteLine("add content template");
            }
        }

        private void PinToStart(object e)
        {
            var tile = new FlipTileData();

           // tile.BackgroundImage = new Uri(currentEvent.image, UriKind.Absolute);
           // tile.BackContent = currentEvent.title;
           // tile.Title = tile.BackTitle = "please event";
           // tile.Count = 0;

           // ShellTile.Create(new Uri("/EventDetailsPage.xaml?id=" + currentEvent.id), tile);
        }

        private void ShowFullMap(object e)
        {
            /*
            var fullMap = new MapsDirectionsTask();

            var currPos = Please2.Util.Location.CurrentPosition;

            var startGeo = new GeoCoordinate((double)currPos["latitude"], (double)currPos["longitude"]);

            var startLabeledMap = new LabeledMapLocation("current location", startGeo);

            fullMap.Start = startLabeledMap;

            var endGeo = new GeoCoordinate(currentEvent.location.lat, currentEvent.location.lon);

            var endLabeledMap = new LabeledMapLocation(currentEvent.title, endGeo);

            fullMap.End = endLabeledMap;

            fullMap.Show();
            */
        }
    }
}
