using System;

using Windows.Devices.Geolocation;

namespace PlexiSDK.Models
{
    public class LocationModel
    {
        public double timestamp { get; set; }

        public TimeSpan timeoffset { get; set; }

        public PositionStatus status { get; set; }

        public string statusText { get; set; }

        public Geocoordinate geoCoordinate { get; set; }

        public CivicAddress civicAddress { get; set; }
    }
}
