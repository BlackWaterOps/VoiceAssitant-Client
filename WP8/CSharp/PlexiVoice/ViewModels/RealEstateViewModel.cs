using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using LinqToVisualTree;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;

using PlexiSDK;
using PlexiSDK.Models;

namespace PlexiVoice.ViewModels
{
    public class RealEstateViewModel : ViewModelBase, IViewModel
    {
        private string location;
        public string Location
        {
            get { return location; }
            set
            {
                location = value;
                RaisePropertyChanged("Location");
            }
        }

        private List<RealEstateListing> listings;
        public List<RealEstateListing> Listings
        {
            get { return listings; }
            set
            {
                listings = value;
                RaisePropertyChanged("Listings");
            }
        }

        private RealEstateStats stats;
        public RealEstateStats Stats
        {
            get { return stats; }
            set
            {
                stats = value;
                RaisePropertyChanged("Stats");
            }
        }

        public RelayCommand<RealEstateListing> RealEstateItemSelection { get; set; }
        public RelayCommand MapLoaded { get; set; }

        INavigationService navigationService;
        IPlexiService plexiService;

        public RealEstateViewModel()
        {
            this.navigationService = ViewModelLocator.GetServiceInstance<INavigationService>();
            this.plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();
          
            RealEstateItemSelection = new RelayCommand<RealEstateListing>(RealEstateItemSelected);
            MapLoaded = new RelayCommand(BuildMap);
        }

        private void RealEstateItemSelected(RealEstateListing model)
        {
            RealEstateDetailsViewModel details = new RealEstateDetailsViewModel();

            details.CurrentItem = model;

            DetailsViewModel vm = ViewModelLocator.GetServiceInstance<DetailsViewModel>();

            vm.CurrentItem = details;

            navigationService.NavigateTo(ViewModelLocator.DetailsPageUri);
        }

        private void BuildMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var map = currentPage.Descendants<Map>().Cast<Map>().Where(x => x.Layers.Count == 0).FirstOrDefault();

            if (map != null)
            {
                List<GeoCoordinate> geoList = new List<GeoCoordinate>();

                foreach (RealEstateListing listing in listings)
                {
                    double lat = listing.location.latitude;
                    double lon = listing.location.longitude;

                    GeoCoordinate geo = new GeoCoordinate(lat, lon);

                    geoList.Add(geo);

                    try
                    {
                        MapLayer mapLayer = MapService.Default.CreateMapLayer(lat, lon, (listings.IndexOf(listing) + 1));

                        map.Layers.Add(mapLayer);
                    }
                    catch (Exception err)
                    {
                        Debug.WriteLine(err.Message);
                    }
                }

                if (geoList.Count > 0)
                {
                    LocationRectangle rect = LocationRectangle.CreateBoundingRectangle(geoList);
                    map.SetView(rect, MapAnimationKind.None);
                }
            }
        }

        private void SetLocation()
        {
            GeoLocation loc = plexiService.QueryLocation;

            if (loc != null)
            {
                Location = loc.city;
            }
        }

        private void MapMarker_Tap(object sender, System.Windows.Input.GestureEventArgs e, RealEstateListing listing)
        {
            var marker = (sender as UserLocationMarker);

            var transform = App.RootFrame.TransformToVisual(marker);

            Point markerPosition = transform.Transform(new Point(0, 0));

            Debug.WriteLine(String.Format("{0} {1}", markerPosition.X, markerPosition.Y));

            // show listing popup when marker is tapped
            Debug.WriteLine("map marker tapped");
        }


        #region reflection methods
        public void Load(string templateName, Dictionary<string, object> structured)
        {
            RealEstateModel realestateResults = ((JObject)structured["item"]).ToObject<RealEstateModel>();

            Listings = realestateResults.listings;
            Stats = realestateResults.stats;

            SetLocation();
        }

        /*
        public Dictionary<string, object> GetMapInfo(object model)
        {
            RealEstateListing listing = model as RealEstateListing;

            Dictionary<string, object> info = new Dictionary<string, object>();

            info.Add("geo", new GeoCoordinate(listing.location.latitude, listing.location.longitude));
            info.Add("title", listing.title);

            return info;
        }

        public Dictionary<string, object> GetPinInfo(object model)
        {
            RealEstateListing listing = model as RealEstateListing;

            Dictionary<string, object> info = new Dictionary<string, object>();

            info.Add("id", listing.id);
            info.Add("title", listing.title);
            info.Add("content", listing.location.address);
            info.Add("image", (listing.images as List<RealEstateImage>).ElementAt(0).src);

            return info;
        }
        */
        #endregion
    }
}
