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
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Toolkit;

namespace Please2.Util
{
    static class MapService
    {
        public static MapLayer CreateMapLayer(GeoCoordinate geo)
        {
            return CreateMapLayer(geo.Latitude, geo.Longitude);
        }
        
        public static MapLayer CreateMapLayer(double lat, double lon)
        {
            var coord = new GeoCoordinate(lat, lon);

            var EventMapPushpin = new UserLocationMarker();

            MapOverlay overlay = new MapOverlay();

            overlay.Content = EventMapPushpin;

            overlay.GeoCoordinate = coord;

            overlay.PositionOrigin = new Point(0, 0.5);

            MapLayer layer = new MapLayer();

            layer.Add(overlay);

            return layer;
        }

        public static MapPolyline CreatePolyline(List<GeoCoordinate> geoList)
        {
            MapPolyline polyline = new MapPolyline();

            polyline.StrokeColor = Colors.White;
            polyline.StrokeThickness = 5;
            
            GeoCoordinateCollection geoCollection = new GeoCoordinateCollection();

            foreach (GeoCoordinate geo in geoList)
            {
                geoCollection.Add(geo);
            }

            polyline.Path = geoCollection;

            return polyline;
        }

        public static void GeoQuery(string searchTerm, Action<MapLocation> callback)
        {
            var query = new GeocodeQuery();
            query.GeoCoordinate = new GeoCoordinate(0, 0);
            query.SearchTerm = searchTerm;
            query.MaxResultCount = 5;

            query.QueryCompleted += (s, e) =>
            {
                // take first value for now.
                // possibly return all results and show list
                if (e.Result.Count > 0)
                {
                    MapLocation first = e.Result.FirstOrDefault();

                    callback(first);
                }
            };

            query.QueryAsync();
        }

        public static GeoCoordinate GetCentrePointFromListOfCoordinates(List<GeoCoordinate> coordList)
        {
            int total = coordList.Count;

            double X = 0;
            double Y = 0;
            double Z = 0;

            foreach (var i in coordList)
            {
                double lat = i.Latitude * Math.PI / 180;
                double lon = i.Longitude * Math.PI / 180;

                double x = Math.Cos(lat) * Math.Cos(lon);
                double y = Math.Cos(lat) * Math.Sin(lon);
                double z = Math.Sin(lat);

                X += x;
                Y += y;
                Z += z;
            }

            X = X / total;
            Y = Y / total;
            Z = Z / total;

            double Lon = Math.Atan2(Y, X);
            double Hyp = Math.Sqrt(X * X + Y * Y);
            double Lat = Math.Atan2(Z, Hyp);

            return new GeoCoordinate(Lat * 180 / Math.PI, Lon * 180 / Math.PI);
        }

        /*
        public struct GeoCoordinate
        {
            private readonly double latitude;
            private readonly double longitude;

            public double Latitude { get { return latitude; } }
            public double Longitude { get { return longitude; } }

            public GeoCoordinate(double latitude, double longitude)
            {
                this.latitude = latitude;
                this.longitude = longitude;
            }

            public override string ToString()
            {
                return string.Format("{0},{1}", Latitude, Longitude);
            }
        }
        */
    }
}
