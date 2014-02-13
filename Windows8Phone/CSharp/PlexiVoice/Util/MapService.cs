using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Toolkit;

namespace PlexiVoice.Util
{
    class MapService
    {
        public static readonly MapService Default = new MapService();

        private MapService()
        {

        }
        /* useful, but not used
        public Color HexToColor(string hex)
        {
            //remove the # at the front
            hex = hex.Replace("#", "");

            byte a = 255;
            byte r = 255;
            byte g = 255;
            byte b = 255;

            int start = 0;

            //handle ARGB strings (8 characters long)
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                start = 2;
            }

            //convert RGB characters to bytes
            r = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber);
            g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber);
            b = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber);

            return Color.FromArgb(a, r, g, b);
        }
        */

        public MapLayer CreateMapLayer(GeoCoordinate geo)
        {
            return CreateMapLayer(geo.Latitude, geo.Longitude);
        }

        public MapLayer CreateMapLayer(double lat, double lon)
        {
            return CreateMapLayer(lat, lon, null, null, true);
        }

        public MapLayer CreateMapLayer(double lat, double lon, object content)
        {
            return CreateMapLayer(lat, lon, content, null, true);
        }

        public MapLayer CreateMapLayer(double lat, double lon, object content, bool usePushpin)
        {
            return CreateMapLayer(lat, lon, content, null, usePushpin);
        }

        public MapLayer CreateMapLayer(double lat, double lon, object content, Brush pushpinColor)
        {
            return CreateMapLayer(lat, lon, content, pushpinColor, true);
        }

        public MapLayer CreateMapLayer(double lat, double lon, object content, Brush pushpinColor, bool usePushpin)
        {
            GeoCoordinate coord = new GeoCoordinate(lat, lon);
            
            MapOverlay overlay = new MapOverlay();

            if (usePushpin == true)
            {
                Pushpin eventMapPushpin = new Pushpin();
                eventMapPushpin.MaxWidth = 200;

                if (pushpinColor != null)
                {
                    eventMapPushpin.Background = pushpinColor;
                }

                if (content != null)
                {
                    if (content is string)
                    {
                        content = new TextBlock()
                            {
                                Text = content as string,
                                TextWrapping = TextWrapping.Wrap
                            };
                    }

                    eventMapPushpin.Content = content;
                }

                overlay.Content = eventMapPushpin;
            }
            else
            {
                overlay.Content = content;
            }
            
            overlay.GeoCoordinate = coord;

            overlay.PositionOrigin = new Point(0, 0.5);

            MapLayer layer = new MapLayer();

            layer.Add(overlay);

            return layer;
        }

        public MapPolyline CreatePolyline(List<GeoCoordinate> geoList)
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

        public async Task<IList<MapLocation>> GeoQuery(string searchTerm)
        {
            Debug.WriteLine(searchTerm);

            GeocodeQuery query = new GeocodeQuery();
            query.GeoCoordinate = new GeoCoordinate(0, 0);
            query.SearchTerm = searchTerm;
            query.MaxResultCount = 5;

            return await query.ExecuteAsync();
        }

        public async Task<IList<MapLocation>> ReverseGeoQuery(double lat, double lon)
        {
            GeoCoordinate coordinate = new GeoCoordinate(lat, lon);

            return await ReverseGeoQuery(coordinate);
        }

        public async Task<IList<MapLocation>> ReverseGeoQuery(GeoCoordinate coordinate)
        {
            ReverseGeocodeQuery query = new ReverseGeocodeQuery();
            query.GeoCoordinate = coordinate;

            return await query.ExecuteAsync();
        }

        public GeoCoordinate GetCentrePointFromListOfCoordinates(List<GeoCoordinate> coordList)
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
    }

    public static class MapExtensions
    {
        public static Task<T> ExecuteAsync<T>(this Query<T> query)
        {
            var taskSource = new TaskCompletionSource<T>();

            EventHandler<QueryCompletedEventArgs<T>> handler = null;

            handler = (s, e) =>
            {
                query.QueryCompleted -= handler;

                if (e.Cancelled)
                {
                    taskSource.SetCanceled();
                }
                else if (e.Error != null)
                {
                    taskSource.SetException(e.Error);
                }
                else
                {
                    taskSource.SetResult(e.Result);
                }
            };

            query.QueryCompleted += handler;

            query.QueryAsync();

            return taskSource.Task;
        }
    }
}
