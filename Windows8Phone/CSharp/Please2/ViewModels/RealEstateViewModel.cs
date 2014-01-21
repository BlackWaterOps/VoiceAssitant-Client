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

using Please2.Models;
using Please2.Util;

using PlexiSDK;
namespace Please2.ViewModels
{
    public class RealEstateViewModel : ViewModelBase, IViewModel
    {
        public ColorScheme Scheme { get { return ColorScheme.Commerce; } }

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

        public RealEstateViewModel()
        {
            this.navigationService = ViewModelLocator.GetServiceInstance<INavigationService>();

            RealEstateItemSelection = new RelayCommand<RealEstateListing>(RealEstateItemSelected);
            MapLoaded = new RelayCommand(BuildMap);
        }

        private void RealEstateItemSelected(RealEstateListing model)
        {
            RealEstateDetailsViewModel vm = ViewModelLocator.GetServiceInstance<RealEstateDetailsViewModel>();

            vm.CurrentItem = model;
            vm.Title = model.title;
            vm.Scheme = this.Scheme;

            navigationService.NavigateTo(new Uri("/Views/RealEstateDetails.xaml", UriKind.Relative));
        }

        private void BuildMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var map = currentPage.Descendants<Map>().Cast<Map>().FirstOrDefault();

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
                        string colorKey = String.Format("{0}Background", this.Scheme.ToString());

                        SolidColorBrush brush = (SolidColorBrush)App.Current.Resources[colorKey];

                        MapLayer mapLayer = MapService.Default.CreateMapLayer(lat, lon, (listings.IndexOf(listing) + 1), brush);

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
                    map.SetView(rect);
                }
                else
                {
                    PivotItem item = map.Parent as PivotItem;
                    Pivot pivot = item.Parent as Pivot;
                    pivot.Items.Remove(item);
                }
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
        public Dictionary<string, object> Load(string templateName, Dictionary<string, object> structured)
        {
            var ret = new Dictionary<string, object>();

            RealEstateModel realestateResults = ((JObject)structured["item"]).ToObject<RealEstateModel>();

            Listings = realestateResults.listings;
            Stats = realestateResults.stats;

            ret.Add("title", "real estate");
            ret.Add("scheme", this.Scheme);
            //ret.Add("subtitle", ZodiacSign + " for " + date);

            return ret;
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
