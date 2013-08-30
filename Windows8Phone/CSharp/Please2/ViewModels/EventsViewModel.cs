using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Please2.Models;

namespace Please2.ViewModels
{
    public class EventsViewModel : ViewModelBase
    {
        private List<EventModel> _eventResults;
        public List<EventModel> EventResults
        {
            get { return _eventResults; }
            set { _eventResults = value; }
        }

        // NOTE: FOR TESTING ONLY
        public EventsViewModel()
        {
            string testData = "{\"going_count\": null,\"tz_id\": null,\"start_time\": \"2013-08-15 07:30:00\",\"description\": \" <p> <p>Join us on Thursday morning,<strong> </strong>August 15,<strong> </strong>for a <em>special business networking event</em> at <strong>The Phoenician </strong>that will make you new connections, grow your business, and learn of new ideas and technologies on how to effectively network in Phoenix, Arizona.  Our meetings generate hundreds of business opportunities and <em><strong>there is no cost to attend</strong><strong></strong></em>. <p>Register early before the event closes out! And feel free to bring a guest!<p>  <p>DATE: August 15, 2013<br>TIME: 7:30 AM to 9:00 AM<br>COSTS: None<br>LOCATION: <br><strong><strong>THE PHOENICIAN</strong><br><strong>6000 EAST CAMELBACK ROAD</strong><br><strong>SCOTTSDALE, ARIZONA 85251</strong></strong><p>Conveniently located off of Camelback Road and Phoenician Blvd (Jokake). The Phoenician Resort is celebrated for its Five-Diamond service & quality!<p>Guests are welcome to self-park (complimentary) or valet (complimentary, gratuity is customary) upon arrival.<p>We will provide a Phoenician Continental Breakfast:<br>* Fresh orange juice <br>* Cranberry juice<br>* Seasonal fresh fruit and berries<br>* Phoenician bakeries with sweet butter and preserves<br>* Freshly brewed Starbucks® coffee, decaffeinated coffee and Numi Organic Teas<p><em>The meeting starts PROMPTLY at 7:30 AM.</em><p><em>Registration is required</em></p></p></p></p></p></p></p></p></p></p>\",\"tz_country\": null,\"url\": \"http://eventful.com/scottsdale/events/phoenix-business-networking-event-aug…2013-/E0-001-058423014-6?utm_source=apis&utm_medium=apim&utm_campaign=apic\",\"image\": \"http://s1.evcdn.com/images/medium/I0-001/014/354/084-8.jpeg_/glenn-tilbrook-84.jpeg\",\"created\": \"2013-06-19 16:48:37\",\"all_day\": \"0\",\"venue_url\": \"http://eventful.com/scottsdale/venues/the-phoenician-scottsdale-/V0-001-000290493-0?utm_source=apis&utm_medium=apim&utm_campaign=apic\",\"performers\": null,\"stop_time\": \"2013-08-15 09:00:00\",\"title\": \"Phoenix Business Networking Event for August 2013\",\"country_abbr2\": \"US\",\"id\": \"E0-001-058423014-6\",\"link_count\": null,\"location\": {\"lat\": \"33.5019665\",\"lon\": \"-111.9514406\",\"address\": \"The Phoenician - Scottsdale, 6000 East Camelback Road, Scottsdale, AZ, 85251\"}}";

            List<EventModel> list = new List<EventModel>();

            var jsonSettings = new JsonSerializerSettings();

            jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            jsonSettings.NullValueHandling = NullValueHandling.Include;

            EventModel newEvent;

            for (var i = 0; i < 4; i++)
            {
                newEvent = Newtonsoft.Json.JsonConvert.DeserializeObject<EventModel>(testData, jsonSettings);

                list.Add(newEvent);
            }

            EventResults = list;
        }
    }
}
