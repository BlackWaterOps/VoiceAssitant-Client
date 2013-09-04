using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

using Windows.Devices.Geolocation;

using Please2.Resources;

namespace Please2.Util
{
    public static class Location
    {
        private static Geolocator geolocator = new Geolocator();

        private static CancellationTokenSource _cts = null;

        private static Geoposition geoPosition;
        public static Geoposition GeoPosition
        {
            get
            {
                return geoPosition;
            }
        }

        private static Dictionary<string, object> currentPosition;
        public static Dictionary<string, object> CurrentPosition
        {
            get
            {
                return currentPosition;
            }
        }
        
        private static void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            try
            {
                geoPosition = e.Position;

                var lat = e.Position.Coordinate.Latitude;
                var lon =  e.Position.Coordinate.Longitude;

                SetCurrentLocation(lat, lon);         
            }
            catch (Exception err)
            {
                Debug.WriteLine("geoPosition Failure");
                Debug.WriteLine(err.Message);
            }
        }

        private static void OnStatusChanged(Geolocator sender, StatusChangedEventArgs e)
        {
            Debug.WriteLine("location status: " + e.Status);
            
            string statusText;

            switch (e.Status)
            {
                case Windows.Devices.Geolocation.PositionStatus.Ready:
                    // Location data is available
                    statusText = "Location is available.";
                    break;

                case Windows.Devices.Geolocation.PositionStatus.Initializing:
                    // This status indicates that a GPS is still acquiring a fix
                    statusText = "A GPS device is still initializing.";
                    break;

                case Windows.Devices.Geolocation.PositionStatus.NoData:
                    // No location data is currently available
                    statusText = "Data from location services is currently unavailable.";
                    break;

                case Windows.Devices.Geolocation.PositionStatus.Disabled:
                    // The app doesn't have permission to access location,
                    // either because location has been turned off.
                    statusText = "Your location is currently turned off. " +
                         "Change your settings through the Settings charm " +
                         " to turn it back on.";
                    break;

                case Windows.Devices.Geolocation.PositionStatus.NotInitialized:
                    // This status indicates that the app has not yet requested
                    // location data by calling GetGeolocationAsync() or
                    // registering an event handler for the positionChanged event.
                    statusText = "Location status is not initialized because " +
                        "the app has not requested location data.";
                    break;

                case Windows.Devices.Geolocation.PositionStatus.NotAvailable:
                    // Location is not available on this version of Windows
                    statusText = "You do not have the required location services " +
                        "present on your system.";
                    break;

                default:
                    statusText = "Unknown status";
                    break;
            }

            if (currentPosition == null)
            {
                currentPosition = new Dictionary<string, object>()
                {
                    {"status", e.Status},
                    {"statusText", statusText}
                };
            }
            else
            {
                currentPosition["status"] = e.Status;
                currentPosition["statusText"] = statusText;
            }
        }

        /// <summary>
        /// Get current Lat and Lon.
        /// <param name="Accuracy"></param>
        /// <param name="Interval"></param>
        /// <param name="maxAge"></param>
        /// <param name="timeout"></param>
        /// </summary>
        /// <returns>Dictionary<string, string></returns>
        public static async Task<Dictionary<string, object>> GetGeolocation(uint accuracy = 50, uint interval = 600000, int maxAge = 5, int timeout = 10)
        {            
            try
            {
                geolocator.DesiredAccuracyInMeters = accuracy;
                geolocator.ReportInterval = interval; // 10 min in milliseconds

                _cts = new CancellationTokenSource();
                CancellationToken token = _cts.Token;

                geoPosition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(maxAge),
                    timeout: TimeSpan.FromSeconds(timeout)
                ).AsTask(token);

                var lat = geoPosition.Coordinate.Latitude;
                var lon = geoPosition.Coordinate.Longitude;

                SetCurrentLocation(lat, lon);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);

                if ((uint)err.HResult == 0x80004004)
                {
                    // location has been diabled in phone settings. display appropriate message
                    currentPosition.Add("error", "location is disabled in phone settings");
                }
                else
                {
                    // unforeseen error
                    currentPosition.Add("error", err.ToString());
                }
            }

            return currentPosition;
        }

        /// <summary>
        /// Start tracking Geolocation and update local variable currentPosition on PositionChanged and StatusChanged events
        /// <param name="threshold"></param>
        /// <param name="reportInterval"></param>
        /// </summary>
        public static void StartTrackingGeolocation(double threshold = 50, uint reportInterval = 300000)
        {
            geolocator.MovementThreshold = threshold;
            geolocator.ReportInterval = reportInterval;
            geolocator.PositionChanged += new Windows.Foundation.TypedEventHandler<Geolocator, PositionChangedEventArgs>(OnPositionChanged);
            geolocator.StatusChanged += new Windows.Foundation.TypedEventHandler<Geolocator, StatusChangedEventArgs>(OnStatusChanged);
        }

        /// <summary>
        /// Stop update of currentPosition variable by removing the PositionChanged and StautusChanged events
        /// </summary>
        public static void StopTrackingGeolocation()
        {
            geolocator.PositionChanged -= new Windows.Foundation.TypedEventHandler<Geolocator, PositionChangedEventArgs>(OnPositionChanged);
            geolocator.StatusChanged -= new Windows.Foundation.TypedEventHandler<Geolocator, StatusChangedEventArgs>(OnStatusChanged);
        }

        /// <summary>
        /// Cancel any pending Geolocation request
        /// </summary>
        public static void CancelGetGeolocation()
        {
            if (Please2.Util.Location._cts != null)
            {
                Please2.Util.Location._cts.Cancel();
                Please2.Util.Location._cts = null;
            }
        }

        public static bool IsLocationEnabled()
        {
            return (geolocator.LocationStatus != PositionStatus.Disabled);
        }

        private static void SetCurrentLocation(double lat, double lon)
        {
            if (currentPosition == null)
            {
                currentPosition = new Dictionary<string, object>() 
                {
                    {"latitude", lat},
                    {"longitude", lon}
                };
            }
            else
            {
                currentPosition["latitude"] = lat;
                currentPosition["longitude"] = lon;
            }
        }
    }
}

