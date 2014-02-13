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

using PlexiVoice.Models;
using PlexiVoice.Util;

using PlexiSDK;
namespace PlexiVoice.ViewModels
{
    public class RealEstateDetailsViewModel : DetailsBase
    {
        public RelayCommand MapLoaded { get; set; }

        public RealEstateDetailsViewModel() 
        {
            AttachEventHandlers();
        }

        private void AttachEventHandlers()
        {
            PinToStartCommand = new RelayCommand(PinToStart);
            ShowFullMapCommand = new RelayCommand(ShowFullMap);

            MapLoaded = new RelayCommand(AddDirectionsMap);
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

                // string content = (!String.IsNullOrEmpty(item.location.address)) ? item.location.address : null;

                System.Windows.Documents.Glyphs content = new System.Windows.Documents.Glyphs()
                {
                    FontUri = new Uri("/PlexiVoice;component/Assets/Fonts/FontAwesome.otf#FontAwesome", UriKind.Relative),
                    UnicodeString = "\uf015",
                    FontRenderingEmSize = 40,
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black)
                };

                MapLayer layer = MapService.Default.CreateMapLayer(item.location.latitude, item.location.longitude, content, false);

                map.Layers.Add(layer);


                map.Center = new GeoCoordinate(item.location.latitude, item.location.longitude);
            }
        }

        public async void ShowFullMap()
        {
            Debug.WriteLine("show full map");
            RealEstateListing listing = CurrentItem as RealEstateListing;
            GeoCoordinate geo = new GeoCoordinate(listing.location.latitude, listing.location.longitude);
            string title = listing.title;

            await base.ShowFullMap(title, geo);
        }
    }
}
