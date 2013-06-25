using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

using Windows.Devices.Geolocation;

using Please.Resources;

namespace Please.Util
{
    public class Location
    {
        private Geolocator _geolocator = null;

        private CancellationTokenSource _cts = null;
        
        private Geoposition _currentPosition;
        public Geoposition currentPosition
        {
            get
            {
                return _currentPosition;
            }
            set
            {
                _currentPosition = value;
            }
        }

        public Location()
        {
            _geolocator = new Geolocator();
        }

        public void CancelGetGeolocation()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
            }
        }

        private void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            currentPosition = e.Position;
        }

        /// <summary>
        /// Get current Lat and Lon.
        /// </summary>
        public async Task<Dictionary<string, string>> GetGeolocation(uint Accuracy = 50, uint Interval = 600000)
        {
            var response = new Dictionary<string, string>();
           
            try
            {
                _geolocator.DesiredAccuracyInMeters = Accuracy;
                _geolocator.ReportInterval = Interval; // 10 min in milliseconds

                _cts = new CancellationTokenSource();
                CancellationToken token = _cts.Token;

                currentPosition = await _geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                ).AsTask(token);

                response.Add("latitude", currentPosition.Coordinate.Latitude.ToString("0.00"));
                response.Add("longitude", currentPosition.Coordinate.Longitude.ToString("0.00"));
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);

                if ((uint)err.HResult == 0x80004004)
                {
                    // location has been diabled in phone settings. display appropriate message
                    response.Add("error", "location is disabled in phone settings");
                }
                else
                {
                    // unforeseen error
                    response.Add("error", err.ToString());
                }
            }

            return response;
        }

        public void StartTrackingGeolocation()
        {
            _geolocator.PositionChanged += new Windows.Foundation.TypedEventHandler<Geolocator, PositionChangedEventArgs>(OnPositionChanged);
        }

        public void StopTrackingGeolocation()
        {
            _geolocator.PositionChanged -= new Windows.Foundation.TypedEventHandler<Geolocator, PositionChangedEventArgs>(OnPositionChanged);
        }
    }
}

