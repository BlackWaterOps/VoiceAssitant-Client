using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;

using GalaSoft.MvvmLight.Command;

using LinqToVisualTree;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

using Plexi;
namespace Please2.ViewModels
{
    public class RealEstateViewModel : GalaSoft.MvvmLight.ViewModelBase, IViewModel
    {
        public ColorScheme Scheme { get { return ColorScheme.Commerce; } }

        private string templateName = "real_estate";

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

        public RealEstateViewModel(INavigationService navigationService, IPlexiService plexiService)
        {
            this.navigationService = navigationService;

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

        /*
        private void RealEstateItemSelected(RealEstateListing model)
        {
            var isSet = SetDetails(this.templateName, model);

            if (isSet)
            {
                var uri = String.Format(ViewModelLocator.DetailsUri, this.templateName);

                navigationService.NavigateTo(new Uri(uri, UriKind.Relative));
            }
            else
            {
                // no template found message
            }
        }

        private bool SetDetails(string template, RealEstateListing model)
        {
            var templates = ViewModelLocator.DetailsTemplates;

            var isSet = false;

            if (templates[template] != null)
            {
                var vm = ViewModelLocator.GetServiceInstance<RealEstateDetailsViewModel>();

                vm.CurrentItem = model;
                vm.Title = model.title;
                vm.Scheme = this.scheme;
                
                isSet = true;
            }

            return isSet;
        }
        */

        private void BuildMap()
        {
            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);

            var map = currentPage.Descendants<Map>().Cast<Map>().FirstOrDefault();

            if (map != null)
            {
                List<GeoCoordinate> geoList = new List<GeoCoordinate>();

                foreach (RealEstateListing listing in listings)
                {
                    Debug.WriteLine(String.Format("{0}:{1}", listing.location.latitude, listing.location.longitude));

                    GeoCoordinate geo = new GeoCoordinate(listing.location.latitude, listing.location.longitude);

                    geoList.Add(geo);

                    MapLayer mapLayer = MapService.Default.CreateMapLayer(geo);

                    map.Layers.Add(mapLayer);
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

        #region reflection methods
        public Dictionary<string, object> Populate(string templateName, Dictionary<string, object> structured)
        {
            var ret = new Dictionary<string, object>();

            var realestateResults = ((JObject)structured["item"]).ToObject<RealEstateModel>();

            Listings = realestateResults.listings;
            Stats = realestateResults.stats;

            ret.Add("title", "real estate");
            ret.Add("scheme", this.Scheme);
            //ret.Add("subtitle", ZodiacSign + " for " + date);

            return ret;
        }

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
        #endregion
    }
}
