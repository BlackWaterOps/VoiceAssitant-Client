using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

using Newtonsoft.Json;

using Please2.Models;
using Please2.Util;

namespace Please2.Views
{
    public partial class EventDetailsPage : PhoneApplicationPage
    {
        private string testData;

        private EventModel currentEvent;

        public EventModel CurrentEvent
        {
            get { return currentEvent; }
        }

        private string formattedDate;

        public string FormattedDate
        {
            get { return formattedDate; }
        }

        private string formattedTime;

        public string FormattedTime
        {
            get { return formattedTime; }
        }

        public EventDetailsPage()
        {
            InitializeComponent();
            
            //NOTE: FOR TESTING ONLY
            testData = "{\"going_count\": null,\"tz_id\": null,\"start_time\": \"2013-08-15 07:30:00\",\"description\": \" <p> <p>Join us on Thursday morning,<strong> </strong>August 15,<strong> </strong>for a <em>special business networking event</em> at <strong>The Phoenician </strong>that will make you new connections, grow your business, and learn of new ideas and technologies on how to effectively network in Phoenix, Arizona.  Our meetings generate hundreds of business opportunities and <em><strong>there is no cost to attend</strong><strong></strong></em>. <p>Register early before the event closes out! And feel free to bring a guest!<p>  <p>DATE: August 15, 2013<br>TIME: 7:30 AM to 9:00 AM<br>COSTS: None<br>LOCATION: <br><strong><strong>THE PHOENICIAN</strong><br><strong>6000 EAST CAMELBACK ROAD</strong><br><strong>SCOTTSDALE, ARIZONA 85251</strong></strong><p>Conveniently located off of Camelback Road and Phoenician Blvd (Jokake). The Phoenician Resort is celebrated for its Five-Diamond service & quality!<p>Guests are welcome to self-park (complimentary) or valet (complimentary, gratuity is customary) upon arrival.<p>We will provide a Phoenician Continental Breakfast:<br>* Fresh orange juice <br>* Cranberry juice<br>* Seasonal fresh fruit and berries<br>* Phoenician bakeries with sweet butter and preserves<br>* Freshly brewed Starbucks® coffee, decaffeinated coffee and Numi Organic Teas<p><em>The meeting starts PROMPTLY at 7:30 AM.</em><p><em>Registration is required</em></p></p></p></p></p></p></p></p></p></p>\",\"tz_country\": null,\"url\": \"http://eventful.com/scottsdale/events/phoenix-business-networking-event-aug…2013-/E0-001-058423014-6?utm_source=apis&utm_medium=apim&utm_campaign=apic\",\"image\": \"http://s1.evcdn.com/images/medium/I0-001/014/354/084-8.jpeg_/glenn-tilbrook-84.jpeg\",\"created\": \"2013-06-19 16:48:37\",\"all_day\": \"0\",\"venue_url\": \"http://eventful.com/scottsdale/venues/the-phoenician-scottsdale-/V0-001-000290493-0?utm_source=apis&utm_medium=apim&utm_campaign=apic\",\"performers\": null,\"stop_time\": \"2013-08-15 09:00:00\",\"title\": \"Phoenix Business Networking Event for August 2013\",\"country_abbr2\": \"US\",\"id\": \"E0-001-058423014-6\",\"link_count\": null,\"location\": {\"lat\": \"33.5019665\",\"lon\": \"-111.9514406\",\"address\": \"The Phoenician - Scottsdale, 6000 East Camelback Road, Scottsdale, AZ, 85251\"}}";

            var jsonSettings = new JsonSerializerSettings();

            jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            jsonSettings.NullValueHandling = NullValueHandling.Include;

            try
            {
                currentEvent = Newtonsoft.Json.JsonConvert.DeserializeObject<EventModel>(testData, jsonSettings);

                currentEvent.description = StripTags(currentEvent.description);

                var coord = new GeoCoordinate(currentEvent.location.lat, currentEvent.location.lon);

                EventMap.Center = coord;
                EventMap.ZoomLevel = 15;

                AddMapLayer(coord);

                var datetime = currentEvent.start_time.Value;

                formattedDate = datetime.ToString("ddd, MMM d, yyyy");
                formattedTime = datetime.ToString("h:m tt");

                DataContext = this;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);

                /*
                string id = null;

                NavigationContext.QueryString.TryGetValue("id", out id);

                if (id != null)
                {
                    currentEvent = App.EventsViewModel.EventResults.Find(x => x.id == id);

                    var coord = new System.Device.Location.GeoCoordinate(currentEvent.location.lat, currentEvent.location.lon);

                    EventMap.Center = coord;
                    EventMap.ZoomLevel = 15;

                    AddMapLayer(coord);

                    var datetime = DateTime.Parse(currentEvent.start_time);

                    formattedDate = datetime.ToString("ddd, MMM d, yyyy");
                    formattedTime = datetime.ToString("h:m tt");

                    DataContext = this;
                }
                 */
            }
            catch (Exception err)
            {

            }
        }

        protected void EventMapButton_Click(object sender, EventArgs e)
        {
            GotoFullMap();
        }

        protected void EventLocation_Tap(object sender, EventArgs e)
        {
            GotoFullMap();
        }

        protected void EventDate_Tap(object sender, EventArgs e)
        {
            // navigate to reminders page
        }

        protected void PinToStartButton_Click(object sender, EventArgs e)
        {
            var tile = new FlipTileData();

            tile.BackgroundImage = new Uri(currentEvent.image, UriKind.Absolute);
            tile.BackContent = currentEvent.title;
            tile.Title = tile.BackTitle = "please event";
            tile.Count = 0;

            ShellTile.Create(new Uri("/EventDetailsPage.xaml?id=" + currentEvent.id), tile);
        }

        protected void GotoFullMap()
        {
            try
            {
                var fullMap = new MapsDirectionsTask();

                var currPos = Please2.Util.Location.CurrentPosition;

                var startGeo = new GeoCoordinate((double)currPos["latitude"], (double)currPos["longitude"]);

                var startLabeledMap = new LabeledMapLocation("current location", startGeo);

                fullMap.Start = startLabeledMap;

                var endGeo = new GeoCoordinate(currentEvent.location.lat, currentEvent.location.lon);

                var endLabeledMap = new LabeledMapLocation(currentEvent.title, endGeo);

                fullMap.End = endLabeledMap;

                fullMap.Show();
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        protected string StripTags(string text)
        {
            text = Regex.Replace(text, @"<[^>]*>", "", RegexOptions.IgnoreCase);

            return text;
        }

        protected void AddMapLayer(GeoCoordinate coord)
        {
            var EventMapPushpin = new UserLocationMarker();

            MapOverlay overlay = new MapOverlay();

            overlay.Content = EventMapPushpin;

            overlay.GeoCoordinate = coord;

            overlay.PositionOrigin = new Point(0, 0.5);

            MapLayer layer = new MapLayer();

            layer.Add(overlay);

            EventMap.Layers.Add(layer);
        }
    }
}