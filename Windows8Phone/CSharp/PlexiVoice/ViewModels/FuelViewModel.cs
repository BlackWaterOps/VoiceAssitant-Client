using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;

using GalaSoft.MvvmLight.Command;

using LinqToVisualTree;

using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;

using PlexiSDK;
using PlexiSDK.Models;
namespace PlexiVoice.ViewModels
{
    class FuelViewModel : ListBase
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

        public RelayCommand<AltFuelModel> FuelItemSelection { get; set; }
        public RelayCommand MapLoaded { get; set; }

        public FuelViewModel()
        {
            this.plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();

            FuelItemSelection = new RelayCommand<AltFuelModel>(FuelItemSelected);
            MapLoaded = new RelayCommand(BuildMap);
        }

        public void Load(string templateName, Dictionary<string, object> structured)
        {
            List<AltFuelModel> items = (structured["items"] as JArray).ToObject<List<AltFuelModel>>();

            Items = (items.Count > 10) ? items.Take(10) : items;

            SetLocation();
        }

        public void FuelItemSelected(AltFuelModel model)
        {
            FuelDetailsViewModel details = new FuelDetailsViewModel();

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

                // create list so we can get index of elements
                List<object> itemsList = Items.ToList();

                foreach (AltFuelModel item in itemsList)
                {
                    double lat = item.latitude;
                    double lon = item.longitude;

                    GeoCoordinate geo = new GeoCoordinate(lat, lon);

                    geoList.Add(geo);

                    try
                    {
                        MapLayer mapLayer = MapService.Default.CreateMapLayer(lat, lon, (itemsList.IndexOf(item) + 1));

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
    }
}
