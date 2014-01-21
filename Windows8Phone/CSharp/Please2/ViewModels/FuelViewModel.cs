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

using Please2.Models;
using Please2.Util;

using PlexiSDK;

namespace Please2.ViewModels
{
    class FuelViewModel : ListViewModel
    {
        public RelayCommand<AltFuelModel> FuelItemSelection { get; set; }
        public RelayCommand MapLoaded { get; set; }

        public FuelViewModel()
        {
            FuelItemSelection = new RelayCommand<AltFuelModel>(FuelItemSelected);
            MapLoaded = new RelayCommand(BuildMap);
        }

        public Dictionary<string, object> Load(string templateName, Dictionary<string, object> structured)
        {
            Items = (structured["items"] as JArray).ToObject<List<AltFuelModel>>();

            this.Scheme = ColorScheme.Information;
            this.Title = "fuel";

            return new Dictionary<string,object>()
            {
                { "scheme", this.Scheme },
                { "title", this.Title }
            };
        }

        public void FuelItemSelected(AltFuelModel model)
        {
            FuelDetailsViewModel vm = ViewModelLocator.GetServiceInstance<FuelDetailsViewModel>();

            vm.CurrentItem = model;

            navigationService.NavigateTo(new Uri("/Views/FuelDetails.xaml", UriKind.Relative));
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
                    map.SetView(rect);
                }
            }
        }
    }
}
