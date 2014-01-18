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

using PlexiSDK;
namespace Please2.ViewModels
{
    public class RealEstateDetailsViewModel : DetailsViewModel
    {
        public RelayCommand ListingDirectionsLoaded { get; set; }

        public RealEstateDetailsViewModel(INavigationService navigationService, IPlexiService plexiService) 
            : base(navigationService, plexiService)
        {
            AttachEventHandlers();
        }

        private void AttachEventHandlers()
        {
            PinToStartCommand = new RelayCommand(PinToStart);
            ShowFullMapCommand = new RelayCommand(ShowFullMap);

            ListingDirectionsLoaded = new RelayCommand(AddDirectionsMap);
        }

        public void PinToStart()
        {
            RealEstateListing listing = CurrentItem as RealEstateListing;

            Uri uri = new Uri(String.Format("/Views/Details.xaml?id={0}", listing.id));
            
            string title = listing.title;
            string content = listing.location.address;
            string image = (listing.images as List<RealEstateImage>).ElementAt(0).src;
            
            base.PinToStart(uri, title, content, image);
        }

        public void AddDirectionsMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var map = currentPage.Descendants<Map>().Cast<Map>().FirstOrDefault();

            if (map != null)
            {
                var item = CurrentItem as RealEstateListing;

                var layer = base.CreateMapLayer(item.location.latitude, item.location.longitude);

                map.Layers.Add(layer);

                map.Center = new GeoCoordinate(item.location.latitude, item.location.longitude);
            }
        }

        public async void ShowFullMap()
        {
            RealEstateListing listing = CurrentItem as RealEstateListing;
            GeoCoordinate geo = new GeoCoordinate(listing.location.latitude, listing.location.longitude);
            string title = listing.title;

            await base.ShowFullMap(title, geo);
        }
    }
}
