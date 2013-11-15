using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

using Plexi;
namespace Please2.ViewModels
{
    public class RealEstateViewModel : GalaSoft.MvvmLight.ViewModelBase, IViewModel
    {
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

        INavigationService navigationService;

        public RealEstateViewModel(INavigationService navigationService, IPlexiService plexiService)
        {
            this.navigationService = navigationService;

            RealEstateItemSelection = new RelayCommand<RealEstateListing>(RealEstateItemSelected);
        }

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

        private bool SetDetails(string template, object model)
        {
            var templates = ViewModelLocator.DetailsTemplates;

            var isSet = false;

            if (templates[template] != null)
            {
                var vm = ViewModelLocator.GetServiceInstance<DetailsViewModel>();

                vm.CurrentItem = model;
                isSet = true;
            }

            return isSet;
        }

        #region reflection methods
        public Dictionary<string, object> Populate(string templateName, Dictionary<string, object> structured)
        {
            var ret = new Dictionary<string, object>();

            var realestateResults = ((JObject)structured["item"]).ToObject<RealEstateModel>();

            Listings = realestateResults.listings;
            Stats = realestateResults.stats;

            ret.Add("title", "real estate");
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
