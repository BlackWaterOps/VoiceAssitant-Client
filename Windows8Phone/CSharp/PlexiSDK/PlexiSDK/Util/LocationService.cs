using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

using Windows.Devices.Geolocation;

using PlexiSDK.Models;

namespace PlexiSDK.Util
{
    internal class LocationService
    {
        private Geolocator geolocator = new Geolocator();

        private CancellationTokenSource _cts = null;

        private bool isTracking = false;

        //public Geoposition GeoPosition { get; private set; }

        public LocationModel CurrentPosition { get; private set; }

        public static readonly LocationService Default = new LocationService();

        private LocationService()
        {

        }

        private void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            try
            {
                SetCurrentLocation(e.Position);         
            }
            catch (Exception err)
            {
                Debug.WriteLine("geoPosition Failure");
                Debug.WriteLine(err.Message);
            }
        }

        private void OnStatusChanged(Geolocator sender, StatusChangedEventArgs e)
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

            if (CurrentPosition == null)
            {
                CurrentPosition = new LocationModel();
            }

            CurrentPosition.status = e.Status;
            CurrentPosition.statusText = statusText;
        }

        /// <summary>
        /// Get current Lat and Lon.
        /// <param name="Accuracy"></param>
        /// <param name="Interval"></param>
        /// <param name="maxAge"></param>
        /// <param name="timeout"></param>
        /// </summary>
        /// <returns>Dictionary<string, string></returns>
        public async Task<LocationModel> GetGeolocation(uint accuracy = 50, uint interval = 600000, int maxAge = 5, int timeout = 10)
        {
            geolocator.DesiredAccuracyInMeters = accuracy;
            geolocator.ReportInterval = interval; // 10 min in milliseconds

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;

            Geoposition geoPosition = await geolocator.GetGeopositionAsync(
                maximumAge: TimeSpan.FromMinutes(maxAge),
                timeout: TimeSpan.FromSeconds(timeout)
            ).AsTask(token);

            SetCurrentLocation(geoPosition);

            return CurrentPosition;
        }

        /// <summary>
        /// Start tracking Geolocation and update local variable currentPosition on PositionChanged and StatusChanged events
        /// <param name="threshold"></param>
        /// <param name="reportInterval"></param>
        /// </summary>
        public void StartTrackingGeolocation(double threshold = 50, uint reportInterval = 300000)
        {
            if (isTracking == false)
            {
                Debug.WriteLine("start tracking");
                geolocator.MovementThreshold = threshold;
                geolocator.ReportInterval = reportInterval;
                geolocator.PositionChanged += new Windows.Foundation.TypedEventHandler<Geolocator, PositionChangedEventArgs>(OnPositionChanged);
                geolocator.StatusChanged += new Windows.Foundation.TypedEventHandler<Geolocator, StatusChangedEventArgs>(OnStatusChanged);

                isTracking = true;
            }
        }

        /// <summary>
        /// Stop update of currentPosition variable by removing the PositionChanged and StautusChanged events
        /// </summary>
        public void StopTrackingGeolocation()
        {
            geolocator.PositionChanged -= new Windows.Foundation.TypedEventHandler<Geolocator, PositionChangedEventArgs>(OnPositionChanged);
            geolocator.StatusChanged -= new Windows.Foundation.TypedEventHandler<Geolocator, StatusChangedEventArgs>(OnStatusChanged);

            isTracking = false;
        }

        /// <summary>
        /// Cancel any pending Geolocation request
        /// </summary>
        public void CancelGetGeolocation()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
            }
        }

        public bool IsLocationEnabled()
        {
            return (geolocator.LocationStatus != PositionStatus.Disabled);
        }

        private void SetCurrentLocation(Geoposition position)
        {
            if (CurrentPosition == null)
            {
                CurrentPosition = new LocationModel();
            }

            CurrentPosition.geoCoordinate = position.Coordinate;
            CurrentPosition.civicAddress = position.CivicAddress;
        }
    }
}

