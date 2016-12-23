using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using Microsoft.Phone.Controls;
using Microsoft.Phone.UserData;

using Windows.Phone.PersonalInformation;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;

using PlexiSDK;
using PlexiSDK.Events;
using PlexiSDK.Models;
using PlexiSDK.Util;
namespace PlexiVoice.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private static Dictionary<string, Actor> ActorMap = new Dictionary<string, Actor>()
        {
            {"alarm", Actor.Alarm},
            {"calendar", Actor.Calendar},
            {"call", Actor.Call},
            {"directions", Actor.Directions},
            {"email", Actor.Email},
            {"reminder", Actor.Reminder},
            {"sms", Actor.Sms},
            {"time", Actor.Time}
        };

        private static Dictionary<Tuple<Actor, string>, Delegate> Actors = new Dictionary<Tuple<Actor, string>, Delegate>();

        private ObservableCollection<object> items;
        public ObservableCollection<object> Items
        {
            get { return items; }
            set
            {
                items = value;
                RaisePropertyChanged("Items");
            }
        }

        private INavigationService navigationService;
        private IPlexiService plexiService;
        private ISpeechService speechService;

        public MainViewModel(INavigationService navigationService, IPlexiService plexiService, ISpeechService speechService)
        {
            this.navigationService = navigationService;
            this.plexiService = plexiService;
            this.speechService = speechService;

            plexiService.Choose += OnChoose;
            plexiService.Error += OnError;
            plexiService.Authorize += OnAuthorize;
            plexiService.InProgress += OnProgress;
            plexiService.Show += OnShow;
            plexiService.Act += OnAct;
            plexiService.Complete += OnComplete;

            // make sure we have a collection to add to!!
            Items = new ObservableCollection<object>();

            BuildActors();

            try
            {
                BuildMockDialog();
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        /*
        public void MigrateContacts()
        {
            Contacts contacts = new Contacts();

            contacts.SearchCompleted += async (s, e) =>
                {
                    IEnumerable<Contact> results = e.Results;

                    ContactStore store = await ContactStore.CreateOrOpenAsync(ContactStoreSystemAccessMode.ReadWrite, ContactStoreApplicationAccessMode.LimitedReadOnly);

                    StoredContact contact = new StoredContact(store);
                };

            contacts.SearchAsync(String.Empty, FilterKind.None, "migration test");
        }
        */

        public void BuildActors()
        {
            if (Actors.Count == 0)
            {
                ITaskService tasks = ViewModelLocator.GetServiceInstance<ITaskService, TaskService>();

                Actors.Add(new Tuple<Actor, string>(Actor.Alarm, "create"), new Action<Dictionary<string, object>>(tasks.SetAlarm));
                Actors.Add(new Tuple<Actor, string>(Actor.Alarm, "edit"), new Action<Dictionary<string, object>>(tasks.UpdateAlarm));

                Actors.Add(new Tuple<Actor, string>(Actor.Reminder, "create"), new Action<Dictionary<string, object>>(tasks.SetReminder));
                Actors.Add(new Tuple<Actor, string>(Actor.Reminder, "edit"), new Action<Dictionary<string, object>>(tasks.UpdateReminder));

                Actors.Add(new Tuple<Actor, string>(Actor.Time, "query"), new Action<Dictionary<string, object>>(tasks.ShowClock));
                
                Actors.Add(new Tuple<Actor, string>(Actor.Email, "create"), new Action<Dictionary<string, object>>(tasks.ComposeEmail));
                
                Actors.Add(new Tuple<Actor, string>(Actor.Sms, "create"), new Action<Dictionary<string, object>>(tasks.ComposeSms));
                
                Actors.Add(new Tuple<Actor, string>(Actor.Directions, "query"), new Action<Dictionary<string, object>>(tasks.GetDirections));
                
                Actors.Add(new Tuple<Actor, string>(Actor.Call, "trigger"), new Action<Dictionary<string, object>>(tasks.PhoneCall));
                
                Actors.Add(new Tuple<Actor, string>(Actor.Calendar, "create"), new Action<Dictionary<string, object>>(tasks.SetAppointment));
            }
        }

        public void BuildMockAction(Actor actor)
        {
            string actionPayload = String.Empty;

            ActorEventArgs args;

            switch (actor)
            {
                case Actor.Alarm:
                    actionPayload = "{\"action\": \"create\",\"model\": \"alarm\",\"payload\": {\"date\": null,\"time\": \"19:30:00\"}}";
                    break;

                case Actor.Reminder:
                    actionPayload = "{\"action\":\"create\",\"model\":\"reminder\",\"payload\":{\"date\":\"2014-02-06\",\"time\":\"17:30:00\",\"subject\":\"Pick up dog food\"}}";
                    break;

                case Actor.Directions:
                    actionPayload = "{\"action\": \"query\",\"model\": \"directions\",\"payload\": {\"to\": \"seattle\"}";
                    break;

                case Actor.Time:
                    actionPayload = "{\"action\":\"query\",\"model\":\"time\",\"payload\":{\"location\":{\"city\":\"new york\",\"state\":\"ny\",\"latitude\":40.765243,\"dst\":true,\"time_offset\":-5,\"zipcode\":\"10106\",\"longitude\":-73.980438}}}";
                    break;
            }

            if (!String.IsNullOrEmpty(actionPayload))
            {
                args = new ActorEventArgs(JsonConvert.DeserializeObject<ClassifierModel>(actionPayload));

                OnAct(this, args);
            }
        }

        private void BuildMockDialog()
        {
            string resultString;

            CompleteEventArgs args;

            // string: json repsonse
            // bool: add to results list in view
            List<Tuple<string, bool>> queries = new List<Tuple<string, bool>>();

            // fitbit food
            resultString = "{\"show\":{\"simple\":{\"text\":\"I'm sorry. I cannot update your food log right now\"},\"structured\":{\"item\":{\"foods\":[{\"logId\":397779132,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397781203,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":82393,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":100,\"amount\":8,\"units\":[304,179,204,319,209,189,128,364,349,91,256,279,401,226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"fl oz\",\"id\":128,\"name\":\"fl oz\"},\"name\":\"Pepsi\"},\"nutritionalValues\":{\"carbs\":27.5,\"fiber\":0,\"sodium\":25.5,\"calories\":100,\"fat\":0,\"protein\":0},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397782407,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":42111,\"locale\":\"en_US\",\"brand\":\"Johnny Rockets\",\"calories\":170,\"amount\":1,\"units\":[304],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"servings\",\"id\":304,\"name\":\"serving\"},\"name\":\"Coke\"},\"nutritionalValues\":{\"carbs\":28,\"fiber\":0,\"sodium\":80,\"calories\":170,\"fat\":0,\"protein\":0},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397787791,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":81313,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":193,\"amount\":1,\"units\":[304,226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"servings\",\"id\":304,\"name\":\"serving\"},\"name\":\"Cheese\"},\"nutritionalValues\":{\"carbs\":32.5,\"fiber\":3,\"sodium\":490,\"calories\":193,\"fat\":8.5,\"protein\":11.5},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397796670,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":77,\"amount\":3,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"oz\",\"id\":226,\"name\":\"oz\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":17,\"fiber\":1.5,\"sodium\":1.5,\"calories\":77,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397799001,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397899468,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397903970,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397911826,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false},{\"logId\":397913846,\"loggedFood\":{\"mealTypeId\":7,\"foodId\":4843,\"locale\":\"en_US\",\"brand\":\"\",\"calories\":90,\"amount\":100,\"units\":[226,180,147,389],\"accessLevel\":\"PUBLIC\",\"unit\":{\"plural\":\"grams\",\"id\":147,\"name\":\"gram\"},\"name\":\"Banana\"},\"nutritionalValues\":{\"carbs\":20,\"fiber\":2,\"sodium\":2,\"calories\":90,\"fat\":0.5,\"protein\":1},\"logDate\":\"2013-10-10\",\"isFavorite\":false}],\"goals\":{\"calories\":1669,\"estimatedCaloriesOut\":2169},\"summary\":{\"carbs\":226.54649353027344,\"fiber\":15.330870628356934,\"sodium\":609.0343017578125,\"calories\":1080,\"fat\":10.555147171020508,\"water\":0,\"protein\":19.55388069152832}},\"template\":\"single:fitbit:log-food\"}},\"speak\":\"I'm sorry. I cannot update your food log right now\"}";
            queries.Add(new Tuple<string, bool>(resultString, false));

            // horoscopes
            resultString = "{\"show\":{\"simple\":{\"text\":\"Today you will be unlucky in family. Keep an eye out for a man in blue to have a major impact on your week. Your lucky number for today is 1.\"},\"structured\":{\"item\":{\"zodiac_sign\":\"leo\",\"horoscope\":\"Today you will be unlucky in family. Keep an eye out for a man in blue to have a major impact on your week. Your lucky number for today is 1.\"},\"template\":\"single:horoscope\"}},\"speak\":\"Today you will be unlucky in family. Keep an eye out for a man in blue to have a major impact on your week. Your lucky number for today is 1.\"}";
            queries.Add(new Tuple<string, bool>(resultString, false));

            // geopolitics
            resultString = "{\"show\":{\"simple\":{\"text\":\"The population of South Africa is 48,601,098.\"},\"structured\":{\"item\":{\"country\":\"South Africa\",\"flag\":\"http://stremor-apier.appspot.com/static/flags/sf-lgflag.gif\",\"stats\":{\"area\":\"1,219,090\",\"leader\":\"President Jacob Zuma\",\"population\":\"48,601,098\"},\"text\":\"The population of South Africa is 48,601,098.\"},\"template\":\"single:geopolitics\"}},\"speak\":\"The population of South Africa is 48,601,098.\"}";
            queries.Add(new Tuple<string, bool>(resultString, false));

            // flights 
            resultString = "{\"show\":{\"simple\":{\"text\":\"I found multiple flights. Here is the closest match:\n\nUAL350 is scheduled to leave Phoenix, AZ at 02:15 pm, and arrive in Newark, NJ at 04:23 pm\"},\"structured\":{\"item\":{\"flight_number\":\"350\",\"airline\":{\"code\":\"UAL\",\"name\":\"United Air Lines Inc.\",\"url\":\"http://www.united.com/\",\"country\":\"US\",\"phone\":\"+1-800-225-5833\",\"callsign\":\"United\",\"location\":\"\",\"shortname\":\"United\"},\"details\":[{\"origin\":{\"city\":\"Phoenix, AZ\",\"airport_code\":\"KPHX\",\"airport_name\":\"Phoenix Sky Harbor Intl\"},\"status\":\"future\",\"schedule\":{\"estimated_arrival\":\"2014-02-04T16:23:00-07:00\",\"filed_departure\":\"2014-02-04T14:15:00-05:00\"},\"destination\":{\"city\":\"Newark, NJ\",\"airport_code\":\"KEWR\",\"airport_name\":\"Newark Liberty Intl\"},\"delay\":null,\"identification\":\"UAL350\",\"duration\":\"3:58\"},{\"origin\":{\"city\":\"Cleveland, OH\",\"airport_code\":\"KCLE\",\"airport_name\":\"Cleveland-Hopkins Intl\"},\"status\":\"arrived\",\"schedule\":{\"estimated_arrival\":\"2014-02-04T11:20:00-07:00\",\"actual_departure\":\"2014-02-04T07:05:00-07:00\",\"filed_departure\":\"2014-02-04T08:47:00-05:00\",\"actual_arrival\":\"2014-02-04T11:20:00-07:00\"},\"destination\":{\"city\":\"Phoenix, AZ\",\"airport_code\":\"KPHX\",\"airport_name\":\"Phoenix Sky Harbor Intl\"},\"delay\":null,\"identification\":\"UAL350\",\"duration\":\"4:35\"}]},\"template\":\"single:flights\"}},\"speak\":\"I found multiple flights. Here is the closest match:\n\nUAL350 is scheduled to leave Phoenix, AZ at 02:15 pm, and arrive in Newark, NJ at 04:23 pm\"}";
            queries.Add(new Tuple<string, bool>(resultString, false));

            // fuel
            resultString = "{\"show\":{\"simple\":{\"text\":\"I've found 20 car chargers matching your criteria\"},\"structured\":{\"items\":[{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":\"2013-09-25T14:43:48Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":49249,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-09-25\",\"ev_dc_fast_num\":null,\"latitude\":33.623098,\"open_date\":\"2012-08-10\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":4,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Maricopa County - Environmental Services Department\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.9166,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"15023 N 75th St, Scottsdale, AZ, 85260\",\"distance\":0.41675},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":\"2013-03-20T02:08:00Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":45278,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-02-28\",\"ev_dc_fast_num\":null,\"latitude\":33.62543,\"open_date\":\"2012-01-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":2,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Westin Kierland Resort & Spa\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.933,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"6902 E Greenway, Scottsdale, AZ, 85254\",\"distance\":0.6094},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":null,\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":50986,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-02-25\",\"ev_dc_fast_num\":null,\"latitude\":33.63374,\"open_date\":\"2013-02-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":5,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Scottsdale Financial Center III\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.926,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"16220 N Scottsdale Rd, Scottsdale, AZ, 85254\",\"distance\":0.85811},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":\"2013-09-25T14:43:48Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":49972,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-09-25\",\"ev_dc_fast_num\":null,\"latitude\":33.630926,\"open_date\":\"2012-11-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":4,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Danny's Family Car Wash\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.9123,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"15880 N Greenway Hayden Loop, Scottsdale, AZ, 85260\",\"distance\":0.92035},{\"access_days_time\":\"Dealership business hours\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":\"2013-09-25T14:43:49Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":39862,\"geocode_status\":\"200-8\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-09-25\",\"ev_dc_fast_num\":null,\"latitude\":33.634505,\"open_date\":\"2011-03-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - call ahead\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":1,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":null,\"station_name\":\"Pinnacle Nissan\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":null,\"longitude\":-111.915838,\"ev_level1_evse_num\":null,\"station_phone\":\"480-998-9800\",\"address\":\"7601 E Frank Lloyd Wright, Scottsdale, AZ, 85260\",\"distance\":1.0033},{\"access_days_time\":null,\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":\"2013-09-25T14:43:48Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":48344,\"geocode_status\":\"200-8\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-09-25\",\"ev_dc_fast_num\":null,\"latitude\":33.634505,\"open_date\":\"2011-03-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Private access only\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":1,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":null,\"station_name\":\"Pinnacle Nissan - Service Center\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":null,\"longitude\":-111.915838,\"ev_level1_evse_num\":null,\"station_phone\":null,\"address\":\"7601 E Frank Lloyd Wright, Scottsdale, AZ, 85260\",\"distance\":1.0033},{\"access_days_time\":\"6pm-6am M-F, 24 hours daily Sat-Sun; employee use only during business hours\",\"status_code\":\"E\",\"intersection_directions\":\"East of Scottsdale Road\",\"updated_at\":\"2013-03-20T02:08:00Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":40293,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-02-28\",\"ev_dc_fast_num\":null,\"latitude\":33.635649,\"open_date\":\"2011-05-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - see hours\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":2,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Prudential Financial\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.929371,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"16260 N 71st Ave, Scottsdale, AZ, 85254\",\"distance\":1.03389},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":\"Located at the west entrance off N 85th St at the NW corner of building\",\"updated_at\":null,\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":52718,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-08-31\",\"ev_dc_fast_num\":null,\"latitude\":33.634113,\"open_date\":\"2013-08-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":2,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Infiniti of Scottsdale\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.912335,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"7801 E Frank Lloyd Wright Blvd, Scottsdale, AZ, 85260\",\"distance\":1.08596},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":\"2013-09-25T14:43:48Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":48342,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-09-25\",\"ev_dc_fast_num\":null,\"latitude\":33.60259,\"open_date\":\"2012-06-11\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":4,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Valley of the Sun Jewish Community Center\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.925,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"12701 N Scottsdale Rd, Scottsdale, AZ, 85254\",\"distance\":1.30304},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":\"Southwest corner of building\",\"updated_at\":\"2013-03-20T02:08:00Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":45921,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-02-28\",\"ev_dc_fast_num\":null,\"latitude\":33.619412,\"open_date\":\"2012-03-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":2,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"AAA - Scottsdale\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.8980932,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"14740 N Northsight Blvd, Scottsdale, AZ, 85260\",\"distance\":1.47498},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":\"2013-09-25T14:43:47Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":50141,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-09-25\",\"ev_dc_fast_num\":null,\"latitude\":33.617303,\"open_date\":\"2012-12-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":4,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Painted Rock Asset Management\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.8926,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"14301 N 87th St, Scottsdale, AZ, 85260\",\"distance\":1.80781},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":\"2013-03-20T02:08:00Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":45277,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-02-28\",\"ev_dc_fast_num\":null,\"latitude\":33.6296087,\"open_date\":\"2012-01-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":2,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Twin Peaks\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.8909014,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"8787 Frank Lloyd Wright Blvd, Scottsdale, AZ, 85260\",\"distance\":1.9648},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":\"2013-03-20T02:08:00Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":46703,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-02-28\",\"ev_dc_fast_num\":null,\"latitude\":33.627196,\"open_date\":\"2012-04-01\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":2,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Spectrum Court LLC\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.977266,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"15433 N Tatum Blvd, Phoenix, AZ, 85032\",\"distance\":3.12183},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":\"2013-03-20T02:08:00Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":45223,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-02-28\",\"ev_dc_fast_num\":null,\"latitude\":33.611913,\"open_date\":\"2012-01-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":2,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Mobil on the Run #10189\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.977682,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"13801 N Tatum Blvd, Phoenix, AZ, 85032\",\"distance\":3.18952},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":\"2013-03-20T02:08:00Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":45276,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-02-28\",\"ev_dc_fast_num\":null,\"latitude\":33.5823378,\"open_date\":\"2012-01-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":2,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Pima Crossing\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.8919,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"8730 E Shea Blvd, Scottsdale, AZ, 85260\",\"distance\":3.25614},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":\"2013-03-20T02:08:00Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":45275,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-02-28\",\"ev_dc_fast_num\":null,\"latitude\":33.659908,\"open_date\":\"2012-01-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":3,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Mayo Clinic\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.956896,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"5777 E Mayo Blvd, Phoenix, AZ, 85054\",\"distance\":3.27351},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":null,\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":52080,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-06-25\",\"ev_dc_fast_num\":null,\"latitude\":33.640675,\"open_date\":\"2013-06-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":2,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Fry's Food Stores\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.975666,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"4842 E Bell Rd, Scottsdale, AZ, 85254\",\"distance\":3.28319},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":\"2013-09-25T14:43:47Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":50136,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-09-25\",\"ev_dc_fast_num\":2,\"latitude\":33.60391,\"open_date\":\"2012-12-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":null,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Sears\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.9792,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"4604 E Cactus Rd, Phoenix, AZ, 85032\",\"distance\":3.42898},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":\"2013-03-20T02:08:00Z\",\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":45274,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-02-28\",\"ev_dc_fast_num\":null,\"latitude\":33.583112,\"open_date\":\"2012-01-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":2,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"La Camarilla Racquet Fitness & Swim Club\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"P\",\"ev_network\":\"Blink Network\",\"longitude\":-111.966224,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"5320 E Shea Blvd, Scottsdale, AZ, 85254\",\"distance\":3.61098},{\"access_days_time\":\"24 hours daily; network card required\",\"status_code\":\"E\",\"intersection_directions\":null,\"updated_at\":null,\"e85_blender_pump\":null,\"hy_status_link\":null,\"id\":51494,\"geocode_status\":\"GPS\",\"ng_fill_type_code\":null,\"ng_vehicle_class\":null,\"date_last_confirmed\":\"2013-04-25\",\"ev_dc_fast_num\":null,\"latitude\":33.604665,\"open_date\":\"2013-04-15\",\"ng_psi\":null,\"ev_other_evse\":null,\"cards_accepted\":null,\"groups_with_access_code\":\"Public - card key at all times\",\"fuel_type_code\":\"ELEC\",\"ev_level2_evse_num\":2,\"lpg_primary\":null,\"bd_blends\":null,\"ev_network_web\":\"http://www.blinknetwork.com/\",\"station_name\":\"Mesquite Library\",\"plus4\":null,\"expected_date\":null,\"owner_type_code\":\"LG\",\"ev_network\":\"Blink Network\",\"longitude\":-111.983689,\"ev_level1_evse_num\":null,\"station_phone\":\"888-998-2546\",\"address\":\"4525 E Paradise Village Pkwy, Phoenix, AZ, 85032\",\"distance\":3.65552}],\"template\":\"list:fuel:electric\"}},\"speak\":\"I've found 20 car chargers matching your criteria\"}";
            queries.Add(new Tuple<string, bool>(resultString, false));

            // search
            resultString = "{\"show\":{\"simple\":{\"text\":\"Here are some search results for i had a pepsi\",\"link\":\"http://m.samuru.com/?q=i+had+a+pepsi\"},\"structured\":{\"items\":[{\"url\":\"http://www.youtube.com/watch?v=jQgJeQz3Irg\",\"title\":\"Had to put one in a pepsi - YouTube\",\"opengraph_image\":\"http://i1.ytimg.com/vi/jQgJeQz3Irg/hqdefault.jpg?feature=og\",\"summary\":\"Sign in with your Google Account to like 214marty's video. Sign in with your Google Account to dislike 214marty's video. Standard YouTube License Ratings have been disabled for this video. Rating is available when the video has been rented. This feature isn't available right now. Please try again later.\"},{\"url\":\"http://www.youtube.com/watch?v=cPNneDdYY0A\",\"title\":\"i had to much pepsi !!!!!!!!!!!!!!! - YouTube\",\"opengraph_image\":\"http://i1.ytimg.com/vi/cPNneDdYY0A/hqdefault.jpg?feature=og\",\"summary\":\"Sign in with your Google Account to like psychochild10's video. Sign in with your Google Account to dislike psychochild10's video. I was to hyper! The interactive transcript couldn't be loaded. Ratings have been disabled for this video. Rating is available when the video has been rented. Please try again later.\"},{\"url\":\"http://www.snopes.com/politics/religion/undergod.asp\",\"title\":\"snopes.com: Pepsi Removes 'Under God'\",\"opengraph_image\":null,\"summary\":\"Pepsi Distances Itself from Dr Pepper Row. Pledge Purist Seeks to Give Pause a Rest. Dr Pepper Under Fire; Girl Says It Left Out God. Rodengen, Jeffrey L.   The Legend of Dr Pepper/Seven-Up. Students Upset Dr Pepper Edits God Out of Pledge. Under God' Reference Added to Pledge in '54. The Pledge.\"},{\"url\":\"http://www.snopes.com/horrors/food/syringe.asp\",\"title\":\"snopes.com: Needle in Pepsi\",\"opengraph_image\":null,\"summary\":\"In an eerie way, that 1990 find was the precursor to the 1993 Pepsi syringe panic. 9 June 1993 from an 82-year-old man in Tacoma, Washington, who said he looked into a can of Diet Pepsi to see if he had won a prize and found a syringe. A 1993 instance of product tampering resulted in syringes being found in cans of Pepsi.\"},{\"url\":\"http://www.pepsi.com/PepsiLegacy_Book.pdf\",\"title\":\"FREE Pepsi Legacy book\",\"opengraph_image\":null,\"summary\":\"Pepsi’s flagship brand now had company with one-calorie Pepsi ONE; Pepsi Twist, featuring a twist of lemon; Pepsi Vanilla; and Pepsi Edge, a cola with half the\"},{\"url\":\"http://www.pepsi.com/en-us/d\",\"title\":\"Pepsi Pulse\",\"opengraph_image\":null,\"summary\":\"Pepsi Pulse lets you live for NOW with our picks of the hottest updates on music, sports, and entertainment.\"},{\"url\":\"http://www.sirpepsi.com/pepsi11.htm\",\"title\":\"Pepsi History - Sirpepsi\",\"opengraph_image\":null,\"summary\":\"Come alive! You're in the Pepsi Generation makes advertising          history. 1964 A new product,          Diet Pepsi, is introduced into Pepsi-Cola advertising. 1990 Teen stars Fred          Savage and Cameron join the New Generation campaign, and          football legend Joe MT returns in a spot challenging other          celebrities to taste test their colas against Pepsi.\"},{\"url\":\"http://www.sirpepsi.com/megargel.htm\",\"title\":\"Roy C Megargel - Sirpepsi\",\"opengraph_image\":null,\"summary\":\"I had long thought \n                that Roy C Megargel the Wall Street Investment Banker and 2nd \n                owner of Pepsi-Cola, just might also be the Roy C Megargel , \n                Railroad Tycoon, President of the \n                GULF, \n                TEXAS AND WESTERN RAILROAD. Roy C. Megargel on \n                the other hand, wasn't willing to let it go, he believed in \n                Bradham’s dream and purchased for $35,000 the Pepsi Cola \n                trademark and business under his newly created Pepsi Cola \n                Corporation.\"},{\"url\":\"http://solarnavigator.net/sponsorship/pepsi_cola.htm\",\"title\":\"HISTORY OF PEPSI COLA INVENTED BY CALEB BRADHAM PHARMACIST 1893 ...\",\"opengraph_image\":null,\"summary\":\"www.solarnavigator.net - Refreshment and beverages - Pepsi Cola and the history of soft drinks, energy drinks, soda's: lemonade, cherryade and cola's.\"},{\"url\":\"http://www.pkhalchal.com/pepsi-had-in-store-for-him/\",\"title\":\"Pepsi Had In Store For Him. | PK Halchal\",\"opengraph_image\":null,\"summary\":\"WoW what a Surprise by Pepsi ? A Telephone is Placed in the Shopping Mall and Ringing. A Question is Asked to the Listener, Are you Willing to Spend Whole Day with Pepsi ? Only One Person says “Yes” Checkout What Happens Next !\"}],\"template\":\"list:search:samuru\"}},\"speak\":\"Here are some search results for i had a pepsi\"}";
            queries.Add(new Tuple<string, bool>(resultString, false));
                
            // events
            resultString = "{\"show\":{\"simple\":{\"text\":\"Here are some events, including Phoenix Business Networking Event for October 2013, Phoenix Business Networking Event for November 2013, Grants Management Reform Updates and Agency Insights – What to Expect in Fiscal Year 2014\"},\"structured\":{\"items\":[{\"going_count\":null,\"tz_id\":null,\"start_time\":\"2013-10-17 07:30:00\",\"description\":\" <p>Join us on Thursday morning,<strong> </strong>October 17,<strong> </strong>for a business networking event that will make you new connections, grow your business, and learn of new ideas and technologies on how to effectively network in Phoenix, Arizona.  Our meetings often generate hundreds of business opportunities and there is no cost to attend. <p>Register early before the event closes out!  And feel free to bring a guest! <p>DATE: Thursday morning, October 17, 2013<br>TIME: 7:30 AM to 9:00 AM<br>COSTS: None<br>LOCATION: <br><strong>Sandler Training<br>8901 E. Pima Center Parkway, Suite 150<br>Scottsdale, AZ  85250</strong><p> <p><em>The meeting starts PROMPTLY at 7:30 AM.</em><p><em>Registration is required</em></p></p></p></p></p></p>\",\"tz_country\":null,\"url\":\"http://eventful.com/scottsdale/events/phoenix-business-networking-event-oct…2013-/E0-001-060146051-0?utm_source=apis&utm_medium=apim&utm_campaign=apic\",\"image\":null,\"created\":\"2013-08-14 13:43:59\",\"all_day\":\"0\",\"venue_url\":\"http://eventful.com/scottsdale/venues/sandler-training-/V0-001-006086523-3?utm_source=apis&utm_medium=apim&utm_campaign=apic\",\"performers\":null,\"stop_time\":\"2013-10-17 09:00:00\",\"title\":\"Phoenix Business Networking Event for October 2013\",\"country_abbr2\":\"US\",\"id\":\"E0-001-060146051-0\",\"link_count\":null,\"location\":{\"lat\":\"33.5599736\",\"lon\":\"-111.8893363\",\"address\":\"Sandler Training, 8901 E Pima Center Pkwy, Scottsdale, AZ, 85258\"}},{\"going_count\":null,\"tz_id\":null,\"start_time\":\"2013-11-21 07:30:00\",\"description\":\" <p>Join us on Thursday morning,<strong> </strong>November 21,<strong> </strong>for a business networking event that will make you new connections, grow your business, and learn of new ideas and technologies on how to effectively network in Phoenix, Arizona.  Our meetings often generate hundreds of business opportunities and there is no cost to attend. <p>Register early before the event closes out!  And feel free to bring a guest! <p>DATE: November 21, 2013<br>TIME: 7:30 AM to 9:00 AM<br>COSTS: None<br>LOCATION: <br><strong>Sandler Training<br>8901 E. Pima Center Parkway #150<br>Scottsdale, AZ  85250</strong><p> <p><em>The meeting starts PROMPTLY at 7:30 AM.</em><p><em>Registration is required</em></p></p></p></p></p></p>\",\"tz_country\":null,\"url\":\"http://eventful.com/scottsdale/events/phoenix-business-networking-event-nov…2013-/E0-001-061418550-3?utm_source=apis&utm_medium=apim&utm_campaign=apic\",\"image\":null,\"created\":\"2013-09-18 18:19:17\",\"all_day\":\"0\",\"venue_url\":\"http://eventful.com/scottsdale/venues/sandler-training-/V0-001-006086523-3?utm_source=apis&utm_medium=apim&utm_campaign=apic\",\"performers\":null,\"stop_time\":\"2013-11-21 09:00:00\",\"title\":\"Phoenix Business Networking Event for November 2013\",\"country_abbr2\":\"US\",\"id\":\"E0-001-061418550-3\",\"link_count\":null,\"location\":{\"lat\":\"33.5599736\",\"lon\":\"-111.8893363\",\"address\":\"Sandler Training, 8901 E Pima Center Pkwy, Scottsdale, AZ, 85258\"}},{\"going_count\":null,\"tz_id\":null,\"start_time\":\"2013-10-23 10:30:00\",\"description\":\" <p>The Federal government issues over $600 billion in grants each year to states, localities, tribes, universities, and non-profits.  In February, the Office of Management and Budget (OMB) published its long-awaited proposed guidance on grant reforms “Reform of Federal Policies Relating to Grants and Cooperative Agreements; Cost Principles and Administrative Requirements.”  The public comment period closed on June 2. What’s next?  Join AGA and industry leaders for a conversational session on:<br><br><ul><li>Grant reform and modernization<li>OMB’s preliminary reaction to public comments on reform updates<li>Council on Financial Assistance Reform (COFAR) priorities for 2014-2015<li>Agency experiences with improving the administration, compliance, monitoring, performance of grant programs, and overcoming human capital challenges<li>Partnership collaboration, pilots, and cooperative audit resolution</li></li></li></li></li></ul><p>This is a live event, so in addition to the speakers’ presentations, participants will have the opportunity to ask questions.<br><br><strong>Speakers</strong><ul><li><strong>Norman Dong</strong>, Interim Controller, Office of Management and Budget<li><strong>Rich Rasa,</strong> Director, State and Local Advisory and Assistance Services, Department of Education, Office of Inspector General<li><strong>Merril Oliver</strong>, CGMS, Deputy Director, State of Maryland Governor’s Grants Office<li><strong>Dale Bell</strong>, Deputy Division Director, National Science Foundation, Division of Institution and Award Support<li><strong>Gloria Jarmon, CPA, CGFM</strong>, Deputy Inspector General for Audit Services, Department of Health and Human Services<li><strong>Leslie Leager</strong>, Division Coordinator, State of Iowa Economic Development Authority<li><strong>Helena Sims</strong>, Director of Intergovernmental Relations, AGA</li></li></li></li></li></li></li></ul><p><strong>Date:</strong> Wednesday, October 23, 2013<br><strong><br>Time:</strong>  Lunch 10:30am - 11:00am     Audio 11:00am - 1:00pm<br><br><strong>Learning Objective: </strong>To understand changes to federal grants and cooperative agreements guidelines.<br><br><strong>Prerequisite: </strong>None<br><br><strong>Advance Preparation: </strong>None required<br><br><strong>CPE: </strong>Two credit hours<br><br><strong>Field of Study:</strong> Accounting (Governmental)</p></p></p>\",\"tz_country\":null,\"url\":\"http://eventful.com/scottsdale/events/grants-management-reform-updates-and-…insi-/E0-001-060687452-3?utm_source=apis&utm_medium=apim&utm_campaign=apic\",\"image\":\"http://s1.evcdn.com/images/medium/I0-001/014/608/392-8.jpeg_/grants-management-reform-updates-and-agency-insigh-92.jpeg\",\"created\":\"2013-08-30 14:04:10\",\"all_day\":\"0\",\"venue_url\":\"http://eventful.com/scottsdale/venues/asu-skysong-/V0-001-005173774-3?utm_source=apis&utm_medium=apim&utm_campaign=apic\",\"performers\":null,\"stop_time\":\"2013-10-23 13:00:00\",\"title\":\"Grants Management Reform Updates and Agency Insights – What to Expect in Fiscal Year 2014\",\"country_abbr2\":\"US\",\"id\":\"E0-001-060687452-3\",\"link_count\":null,\"location\":{\"lat\":\"33.4656\",\"lon\":\"-111.925\",\"address\":\"ASU SkySong, Room 301,Ingenuity Conference Room, Scottsdale, AZ, 85257\"}},{\"going_count\":null,\"tz_id\":null,\"start_time\":\"2013-10-10 19:00:00\",\"description\":\"Get back in the singles mix at four Mix and Mingle salsa, Latin and ballroom dance classes for just $10.00 With  Passion for Dancing, world salsa dancing champion Victor Tango leads singles, couples and professionals in dancing for exercise, meeting new people and exhilarating fun. \nYou know you’re in need of some socialization if the first thing that comes to mind when you hear “singles mixer” is a blender full of dollar bills, but that image isn’t that far off from this deal; just replace the George Washington’s with single men and women and swap out the blender for a dance floor. \nPassion for Dancing is serving up this deal for four spinning, blending, Mix and Mingle singles dance classes for just $10.00. Single men and women young and old are invited to learn from top-rated dance instructor Victor Tango—yes, that’s his real name—to learn the magical moves of Latin, salsa and ballroom dance classes Thursdays at 7pm Salsa and 8 p.m Ballroom.\nThe things dancing can help you with can also help your social life flourish: \n\n• Concentrate and achieve your goals \n• Build confidence and raise self-esteem \n• Gain flexibility and exercise \n• Come out of your shell and live again \n\nAbout the instructor \n\nWith a name like Victor Tango, a career like dentistry or car maintenance just didn’t seem like the right fit, so this aptly-named individual found success in none other than the dance industry. His resume includes winning the world salsa championship in 1987 and recognition as the top salsa dance instructor in Washington, D.C., and New York before moving to Arizona. In his lessons he breaks down each step to help every student know exactly what they are doing and which \nhttp://www.passionfordancing.com\",\"tz_country\":null,\"url\":\"http://eventful.com/scottsdale/events/mix-and-mingle-meet-new-s-/E0-001-039969098-6@2013101019?utm_source=apis&utm_medium=apim&utm_campaign=apic\",\"image\":\"http://s4.evcdn.com/images/medium/I0-001/004/142/271-8.jpeg_/mix-and-mingle-meet-new-singles-dance-71.jpeg\",\"created\":\"2011-06-27 16:23:57\",\"all_day\":\"0\",\"venue_url\":\"http://eventful.com/scottsdale/venues/art-of-dance-school-/V0-001-003045526-0?utm_source=apis&utm_medium=apim&utm_campaign=apic\",\"performers\":null,\"stop_time\":\"2013-10-10 21:00:00\",\"title\":\"Mix and Mingle, Meet New Singles Dance Classes.\",\"country_abbr2\":\"US\",\"id\":\"E0-001-039969098-6@2013101019\",\"link_count\":null,\"location\":{\"lat\":\"33.4930947\",\"lon\":\"-111.928558\",\"address\":\"Art of Dance School, 7077 East Main Street suite 11-12, Scottsdale, AZ, 85251\"}}],\"template\":\"list:events\"}},\"speak\":\"Here are some events, including Phoenix Business Networking Event for October 2013, Phoenix Business Networking Event for November 2013, Grants Management Reform Updates and Agency Insights – What to Expect in Fiscal Year 2014\"}";
            queries.Add(new Tuple<string, bool>(resultString, false));

            // weather
            resultString = "{\"show\":{\"simple\":{\"text\":\"Here's our forecast for Wednesday: Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening.\"},\"structured\":{\"item\":{\"week\":[{\"date\":\"2013-10-02\",\"night\":{\"text\":\"Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening. \",\"sky\":\"Clear\",\"temp\":\"65\"}},{\"date\":\"2013-10-03\",\"daytime\":{\"text\":\"Sunny, with a high near 89. Light and variable wind becoming southwest 5 to 10 mph in the morning. \",\"sky\":\"Sunny\",\"temp\":\"89\"},\"night\":{\"text\":\"Mostly clear, with a low around 63. West wind 5 to 9 mph becoming light and variable. \",\"sky\":\"Mostly Clear\",\"temp\":\"63\"}},{\"date\":\"2013-10-04\",\"daytime\":{\"text\":\"Sunny, with a high near 86. Light and variable wind becoming northwest around 6 mph in the afternoon. \",\"sky\":\"Sunny\",\"temp\":\"86\"},\"night\":{\"text\":\"Mostly clear, with a low around 59. Breezy, with a north northwest wind 9 to 14 mph becoming northeast 15 to 20 mph after midnight. Winds could gust as high as 28 mph. \",\"sky\":\"Breezy\",\"temp\":\"59\"}},{\"date\":\"2013-10-05\",\"daytime\":{\"text\":\"Sunny, with a high near 85. Breezy. \",\"sky\":\"Breezy\",\"temp\":\"85\"},\"night\":{\"text\":\"Mostly clear, with a low around 57.\",\"sky\":\"Mostly Clear\",\"temp\":\"57\"}},{\"date\":\"2013-10-06\",\"daytime\":{\"text\":\"Sunny, with a high near 88.\",\"sky\":\"Sunny\",\"temp\":\"88\"},\"night\":{\"text\":\"Mostly clear, with a low around 61.\",\"sky\":\"Mostly Clear\",\"temp\":\"61\"}},{\"date\":\"2013-10-07\",\"daytime\":{\"text\":\"Sunny, with a high near 90.\",\"sky\":\"Sunny\",\"temp\":\"90\"},\"night\":{\"text\":\"Mostly clear, with a low around 62.\",\"sky\":\"Mostly Clear\",\"temp\":\"62\"}},{\"date\":\"2013-10-08\",\"daytime\":{\"text\":\"Sunny, with a high near 88.\",\"sky\":\"Sunny\",\"temp\":\"88\"},\"night\":{\"text\":\"Mostly clear, with a low around 63.\",\"sky\":\"Mostly Clear\",\"temp\":\"63\"}}],\"now\":{\"sky\":\"Mostly Clear\",\"temp\":\"89\"},\"location\":null},\"template\":\"single:weather\"}},\"speak\":\"Here's our forecast for Wednesday: Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening.\"}";
            queries.Add(new Tuple<string, bool>(resultString, false));

            // real estate
            resultString = "{\"show\":{\"simple\":{\"text\":\"I've found 5484 listings. The average price is $1543.\"},\"structured\":{\"item\":{\"listings\":[{\"body\":\"WELCOME TO LUXURY IN LAS SENDAS *MAGNIFICENT SINGLE LEVEL GREATROOM FLOORLAN*RARE DIAMOND POINT MODEL*GATED COURTYARD W/FOUNTAIN WELCOMES YOU INTO THIS SPOTLESS,WELL APPOINTED HOME*FULL BATHS IN ALL 3 BEDROOMS+1/2 BATH IN HALL*HUGE 1/3 ACRE PREMIUM CDS LOT*FANTASIC CITY LTS AND MTN VIEWS. SPA, PUTTING GREEN, B/I BBQ, LARGE COVERED PATIO W/C-FANS*B/I SPEAKERS IN PATIO, MASTER AND GREATROOM.CLASSY CLOSETS IN ALL CLOSSETS AND LAUNDRY RM. LAUNDRY RM SINK. B/I GARAGE CABS *OVERSIZED GARAGE* 12+FT CEILINGS T/O .75GAL WTR HTR,SUN SCREENS*WALKIN SNAIL SHOWER*ELECTRIC SUN SHADE ON PATIO*PLANTATION SHUTTERS T/O*42''CABS IN KITCHEN.\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":6,\"ctime\":1379463372,\"title\":\"8044 E Sienna St\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3487481235-P28p557,29-14F621F1F930/apar…D0uw2YdD0gFMrPbWk9Rt8zyXAu0tfaSRBkIgq2k6Y_t4TTXMuOBBe2EPWnLVAJXzvwf6E8gF9_\",\"similar_url\":\"http://apartments.oodle.com/85207/homes-for-rent/3-bedrooms/4-bedrooms/price_1500_2600/?r=5\",\"paid\":\"Yes\",\"source\":{\"id\":\"facebook\",\"name\":\"Facebook\"},\"location\":{\"citycode\":\"usa:az:mesa\",\"name\":\"Mesa\",\"zip\":\"85207\",\"country\":\"USA\",\"longitude\":\"-111.6571\",\"state\":\"AZ\",\"address\":\"8044 E Sienna St\",\"latitude\":\"33.4860\",\"addresscode\":\"addr:usa:az:mesa:8044+e+sienna+st\"},\"images\":[],\"attributes\":{\"price_display\":\"$2,300\",\"fee\":\"No\",\"bathrooms\":3.5,\"price\":\"2300\",\"bedrooms\":3,\"square_feet\":2786,\"currency\":\"USD\",\"amenities\":\"Gated,Parking,Patio/Deck\",\"has_photo\":\"Thumbnail\",\"user_id\":\"42847680\"},\"id\":\"3487481235\",\"user\":{\"url\":\"http://www.oodle.com/profile/arizona-real-estate/42847680/\",\"photo\":\"http://graph.facebook.com/150648118301315/picture?type=large\",\"id\":\"42847680\",\"name\":\"Arizona Real Estate\"}},{\"body\":\"MUST SEE!! Classic style home with a midwest / east coast feel and wrap around porch! The home is nestled away from Lone Mountain Road for total privacy. This unique home has an upgraded kitchen and a full second master bedroom on the first floor. Spacious grass backyard perfect for entertaining the kids or guests. Come see this home for yourself and fall in love with its charm, TODAY!!\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":2,\"ctime\":1381764805,\"title\":\"House\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3407227674-P1u557,29-14F621F1F930/apart…hFxDD6H-AVn5xsYt3caZYQ2hbP2AEOlDA4JUyyyc7qktiL5l6izahvTvZ0OOz8VwX5RuyHJE1t\",\"similar_url\":\"http://apartments.oodle.com/85331/homes-for-rent/4+-bedrooms/price_1500_2600/?r=5\",\"paid\":\"No\",\"source\":{\"id\":\"www\",\"name\":\"Oodle\"},\"location\":{\"citycode\":\"usa:az:phoenix\",\"name\":\"Phoenix\",\"zip\":\"85331\",\"country\":\"USA\",\"longitude\":\"-111.9632\",\"state\":\"AZ\",\"address\":\"5440 E LONE MOUNTAIN RD\",\"latitude\":\"33.7715\",\"addresscode\":\"addr:usa:az:phoenix:5440+e+lone+mountain\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3407227674u_0s_homes_for_rent_in_phoenix_az/?1381764806\",\"height\":75,\"width\":100,\"num\":\"0\",\"alt\":\"House\",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$2,300\",\"fee\":\"No\",\"bathrooms\":3.5,\"price\":\"2300\",\"bedrooms\":4,\"square_feet\":3084,\"currency\":\"USD\",\"pets_allowed\":\"Dogs\",\"amenities\":\"Patio/Deck\",\"has_photo\":\"Thumbnail\",\"user_id\":\"58065512\"},\"id\":\"3407227674\",\"user\":{\"url\":\"http://www.oodle.com/profile/position-realty/58065512/\",\"photo\":\"http://graph.facebook.com/1028560339/picture?type=large\",\"id\":\"58065512\",\"name\":\"Position Realty\"}},{\"body\":\"Rental Home in Gate community of Lookout Mountainside. 4 beds *2.5baths *Pool *Hot-tub *3-Car garage * Open Floor Plan * Views *Gated. This is a great home on an elevated lot with an ideal floorplan for a family or entertaining. Home has a huge family room with large kitchen, formal dining and living room. Covered patio overlooking the pebble tec pool and beautifully landscaped yard. Much more to see. *Pool and Yard Service include with rent.\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":2,\"ctime\":1381601873,\"title\":\"House Rental\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3445090771-P1u557,29-14F621F1F930/apart…pD2eGMLGQ3zkxi8OgapW454axjm79xka9q0n8cK_nqNg1TxoLSJbDKsP7gwVM18-MF8pUyFcZY,\",\"similar_url\":\"http://apartments.oodle.com/85022/homes-for-rent/4+-bedrooms/price_1500_2600/?r=5\",\"paid\":\"No\",\"source\":{\"id\":\"www\",\"name\":\"Oodle\"},\"location\":{\"citycode\":\"usa:az:phoenix\",\"name\":\"Phoenix\",\"zip\":\"85022\",\"country\":\"USA\",\"longitude\":\"-112.0408\",\"state\":\"AZ\",\"address\":\"1928 E Vista Dr.\",\"latitude\":\"33.6222\",\"addresscode\":\"addr:usa:az:phoenix:1928+e+vista+dr\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3445090771u_0s_homes_for_rent_in_phoenix_az/?1381601874\",\"height\":75,\"width\":100,\"num\":\"0\",\"alt\":\"House Rental\",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$2,295\",\"fee\":\"No\",\"bathrooms\":2.5,\"price\":\"2295\",\"bedrooms\":4,\"square_feet\":2801,\"currency\":\"USD\",\"pets_allowed\":\"None\",\"amenities\":\"Gated,Parking,Patio/Deck,Pool\",\"has_photo\":\"Thumbnail\",\"user_id\":\"775297\"},\"id\":\"3445090771\",\"user\":{\"url\":\"http://www.oodle.com/profile/thomas-bartz-phoenix-real-estate/775297/\",\"photo\":\"http://graph.facebook.com/274413855930978/picture?type=large\",\"id\":\"775297\",\"name\":\"Thomas Bartz -Phoenix Real Estate\"}},{\"body\":\"Available November 1st!\n4 Bedroom Home with 2 Baths\nCul de Sac Lot... Great for Kids!\n2 Car Garage\nDesert Landscaping in Frontyard\nGrass in Backyard\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":2,\"ctime\":1380729044,\"title\":\"House to Rent 4054 W Windrose Drive, 85029\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3499501572-P1u557,29-14F621F1F930/apart…po4nTpvgl0v5PbpZd9gK2DNo0TuS8R1JAiG4TzbwqYYw1Ce9W4m_S6tI7ZmXR9kmOuV8SG5xnQ,,\",\"similar_url\":\"http://apartments.oodle.com/85029/homes-for-rent/4+-bedrooms/price_800_1400/?r=5\",\"paid\":\"No\",\"source\":{\"id\":\"www\",\"name\":\"Oodle\"},\"location\":{\"citycode\":\"usa:az:phoenix\",\"name\":\"Phoenix\",\"zip\":\"85029\",\"country\":\"USA\",\"longitude\":\"-112.1476\",\"state\":\"AZ\",\"address\":\"4054 W Windrose Drive\",\"latitude\":\"33.6015\",\"addresscode\":\"addr:usa:az:phoenix:4054+w+windrose+driv\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3499501572u_0s_homes_for_rent_in_phoenix_az/?1380729044\",\"height\":75,\"width\":100,\"num\":\"0\",\"alt\":\"House to Rent 4054 W Windrose \",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$1,200\",\"fee\":\"No\",\"bathrooms\":2,\"price\":\"1200\",\"bedrooms\":4,\"square_feet\":1700,\"currency\":\"USD\",\"amenities\":\"Parking\",\"has_photo\":\"Thumbnail\",\"user_id\":\"70658468\"},\"id\":\"3499501572\",\"user\":{\"url\":\"http://www.oodle.com/profile/brad-j/70658468/\",\"photo\":\"http://i.oodleimg.com/a/oodle-profile50x50.gif\",\"id\":\"70658468\",\"name\":\"Brad J.\"}},{\"body\":\"Beautiful 2-story home with 2050 Sqft of living area with swimming pool on a 7200 Sqft lot. In immaculate condition & available for occupancy Sep 17th 2013.\n\n- Matured yard(front & back) and professionally maintaned.(Bi-weekly for Yard). Paid by Owner\n- Pool is maintained weekly by the landlord who gets all the needed chemicals. Paid by Owner\n- Complete bed & bathroom downstairs. \n- Spacious 2 car garage with cabinets. \n- Offers formal dining, formal living, eat-in kitchen, large pantry. \n- Very close to major companies in southeast valley: Intel, Wellsfargo, Orbital, Amkor technology, Honeywell, paypal, ebay\n- Major shopping centers minutes away: Chandler fashion center, phoenix premium outlets \n- Minutes away from major freeways: 101/202/I-10\n- Two beautiful parks in walking distance & has easy access to green belt. \n- Master planned community with elementary & junior high school in walking distance. Schools: Tarwater elementary/Bogle junior high/Hamilton High (on a Short Drive)\n\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":2,\"ctime\":1380261555,\"title\":\"5 Br/3Ba home with swimming pool- clemente Ranch\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3495174200-P28u557,29-14F621F1F930/apar…P8HFsJ_BboE2km5o7pqiz490hnW9ve2gFngKV_SdReXD5-LeaxyS0x7jga0J7TAsgSE_zQ6OHB\",\"similar_url\":\"http://apartments.oodle.com/chandler-az/homes-for-rent/4+-bedrooms/price_1000_1800/?r=10\",\"paid\":\"No\",\"source\":{\"id\":\"facebook\",\"name\":\"Facebook\"},\"location\":{\"citycode\":\"usa:az:chandler\",\"name\":\"Chandler\",\"country\":\"USA\",\"longitude\":\"-111.8667\",\"state\":\"AZ\",\"latitude\":\"33.3029\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3495174200u_0s_homes_for_rent_in_chandler_az/?1380261555\",\"height\":75,\"width\":100,\"num\":\"0\",\"alt\":\"5 Br/3Ba home with swimming po\",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$1,595\",\"fee\":\"No\",\"bathrooms\":3,\"price\":\"1595\",\"bedrooms\":5,\"square_feet\":2050,\"currency\":\"USD\",\"pets_allowed\":\"None\",\"amenities\":\"Dishwasher,Parking,Pool,Refrigerator,Washer Dryer\",\"has_photo\":\"Thumbnail\",\"user_id\":\"49059349\"},\"id\":\"3495174200\",\"user\":{\"url\":\"http://www.oodle.com/profile/madhu-m/49059349/\",\"photo\":\"http://graph.facebook.com/100001004085083/picture?type=large\",\"id\":\"49059349\",\"name\":\"Madhu M.\"}},{\"body\":\"$700 / 1br - 250ft² - ONLY ONE ROOM IN A COZY LUXURY HOME IN UPSCALE AREA AVAILABLE FOR RENT (SCOTTSDALE RD & DOUBLETREE PLEASE APPLY ONLY BY PHONE TEXT MSG OR EMAILS WILL BE IGNORED thx\nONLY ONE ROOM IN A COZY LUXURY HOME IN UPSCALE AREA FOR RENT\n$700 / 250ft² - Room available UNBELIEVABLE VIEW Lakefront & Golfcourse (SCOTTSDALE RD AND DOUBLETREE )\nONE MILE VIEW OF MC CORMICK RANCH GOLF COURSE IN YOUR BACKYARD AND LAKEFRONT WITH A PANTOON BOAT ALL KITCHEN SUPPLIES DISHES AND MORE\nNEEDS UPSCALE AND PROFESSIONAL NEEDS THE GOOD CREDIT AND REFRENCES ONE MONTH DEPOSIT AND SHARED THE UTILITY\n1) centrally located (17 MINUTES TO SKY HARBOR AIRPORT) \n2) unique \n3) use of amazing view Located on a private lake. have a pontoon boat E-drive \n4) white cabinets \n5) refrigrator in kitchen ,small microwave ,fireplace,HDtv and small caouch in sitting room, dinningtable in the dinning room \n6) onyx floor in bathroom \n7) Washer& Dryer IN LAUNDR ROOM\n8) Everything you need to enjoy the home: dishes, misc supplies, etc. \",\"category\":{\"id\":\"housing/rent/apartment\",\"name\":\"Apartments for Rent\"},\"revenue_score\":2,\"ctime\":1380125880,\"title\":\"Rentting a Room in My Private Home (Lake and Golfcourse Property\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3455817360-P1u550,29-14F621F1F930/apart…6J3gVk1j-7sDYjMJMmQ2PUZs9tybpFM2YtkNT_9oWPiSMTfJ-VqE3NiRttg7ggAG78JSkT-JjA,,\",\"similar_url\":\"http://apartments.oodle.com/scottsdale-az/apartments-for-rent/3-bedrooms/4-bedrooms/price_460_810/?r=10\",\"paid\":\"No\",\"source\":{\"id\":\"www\",\"name\":\"Oodle\"},\"location\":{\"citycode\":\"usa:az:scottsdale\",\"name\":\"Scottsdale\",\"country\":\"USA\",\"longitude\":\"-111.8902\",\"state\":\"AZ\",\"latitude\":\"33.5783\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3455817360u_0s_apartments_for_rent_in_scottsdale_az/?1380125882\",\"height\":75,\"width\":100,\"num\":\"0\",\"alt\":\"Rentting a Room in My Private \",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$700\",\"fee\":\"No\",\"bathrooms\":2,\"price\":\"700\",\"bedrooms\":3,\"square_feet\":2050,\"currency\":\"USD\",\"pets_allowed\":\"None\",\"amenities\":\"Fireplace,Parking,View,Washer Dryer\",\"has_photo\":\"Thumbnail\",\"user_id\":\"58483163\"},\"id\":\"3455817360\",\"user\":{\"url\":\"http://www.oodle.com/profile/transtar-inc/58483163/\",\"photo\":\"http://graph.facebook.com/100003388433367/picture?type=large\",\"id\":\"58483163\",\"name\":\"TRANSTAR INC\"}},{\"body\":\"Beautiful 2-story home with 2050 Sqft of living area with pool on a 7200 Sqft lot. In immaculate condition & available for immediate occupancy!!\n\n- Brand new carpet\n- Yard maintanance & HOA dues included in the rent and paid by the owner\n- Pool maintance done by the owner weekly. Included in the rent \n- Complete bed & bathroom downstairs. \n- Spacious 2 car garage with cabinets. \n- Offers formal dining, formal living, eat-in kitchen, large pantry. \n- Very close to Intel, Honeywell, paypal, Wellsfargo, Orbital, Amkor technology, Microchip, Chandler Fashion Center, Ocotillo Golf Course. \n- Minutes away from 101/202/I-10 freeways \n- Two beautiful parks in walking distance & has easy access to green belt. \n- Master planned community with elementary & junior high school in walking distance. Schools: Tarwater elementary/Bogle junior high/Hamilton High (on a Short Drive) \n\n- Rent includes everything (except utilities: Gas/Water/Electricity) \n- Freshly painted in & out, tile at all right places. \",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":2,\"ctime\":1379538282,\"title\":\"5 Bd/3Ba home available for rent immediately\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3488094300-P28u557,29-14F621F1F930/apar…tAoU9LyuTkVkwKdr-Zb0TZqRn-rmDdVll5qSQYtPcqI_pIG3aLBq6vFsYRiLytpIULV6j1_1TA,,\",\"similar_url\":\"http://apartments.oodle.com/chandler-az/homes-for-rent/4+-bedrooms/price_1000_1800/?r=10\",\"paid\":\"No\",\"source\":{\"id\":\"facebook\",\"name\":\"Facebook\"},\"location\":{\"citycode\":\"usa:az:chandler\",\"name\":\"Chandler\",\"country\":\"USA\",\"longitude\":\"-111.8667\",\"state\":\"AZ\",\"latitude\":\"33.3029\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3488094300u_0s_homes_for_rent_in_chandler_az/?1379538282\",\"height\":75,\"width\":100,\"num\":\"0\",\"alt\":\"5 Bd/3Ba home available for re\",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$1,595\",\"fee\":\"No\",\"bathrooms\":3,\"price\":\"1595\",\"bedrooms\":5,\"square_feet\":2050,\"currency\":\"USD\",\"pets_allowed\":\"None\",\"amenities\":\"Dishwasher,Parking,Pool,Refrigerator,Washer Dryer\",\"has_photo\":\"Thumbnail\",\"user_id\":\"49059349\"},\"id\":\"3488094300\",\"user\":{\"url\":\"http://www.oodle.com/profile/madhu-m/49059349/\",\"photo\":\"http://graph.facebook.com/100001004085083/picture?type=large\",\"id\":\"49059349\",\"name\":\"Madhu M.\"}},{\"body\":\"Home in Avondale, Arizona at Avondale Rd. and Van Buren. 4bdr/2.5 baths with a HUGE backyard.\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":2,\"ctime\":1379526273,\"title\":\"House\",\"url\":\"http://apartments.oodle.com/u_a2xx_/3488015241-P28u557,29-14F621F1F930/apar…NRiHqHRIDsacrwzEMvqNUnTxHFcdQCcmWXmpJBi09yoj-kgbdosGrq8WxhGIvK2khQtXqPX_VM\",\"similar_url\":\"http://apartments.oodle.com/avondale-az/homes-for-rent/4+-bedrooms/price_700_1300/?r=10\",\"paid\":\"No\",\"source\":{\"id\":\"facebook\",\"name\":\"Facebook\"},\"location\":{\"citycode\":\"usa:az:avondale\",\"name\":\"Avondale\",\"country\":\"USA\",\"longitude\":\"-112.3453\",\"state\":\"AZ\",\"latitude\":\"33.4305\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3488015241u_0s_homes_for_rent_in_avondale_az/?1379526274\",\"height\":75,\"width\":100,\"num\":\"0\",\"alt\":\"House\",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$1,100\",\"fee\":\"No\",\"bathrooms\":2.5,\"price\":\"1100\",\"bedrooms\":4,\"square_feet\":1900,\"currency\":\"USD\",\"pets_allowed\":\"Cats,Dogs\",\"has_photo\":\"Thumbnail\",\"user_id\":\"70621709\"},\"id\":\"3488015241\",\"user\":{\"url\":\"http://www.oodle.com/profile/stacey-h/70621709/\",\"photo\":\"http://graph.facebook.com/1504432473/picture?type=large\",\"id\":\"70621709\",\"name\":\"Stacey H.\"}},{\"body\":\"Rare opportunity to rent this great home*you will be in walkable distance to school, near shopping, restaurants, and freeways, but still tucked away in the lovely mountains of west wing*this home has 4 beds and two bathrooms, formal living and dining, as well as a family room*kitchen has stainless appliances and you have new carpet, fresh paint, and tile in all the right places*jack and jill bath, c-fans, low-maintenance and private back yard, protected patio*this home is very clean and ready for you*\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":0,\"ctime\":1381959878,\"title\":\"Four BR in Westwing Mountain Homes Houses Rental For Rent in Peoria Arizona\",\"url\":\"http://apartments.oodle.com/j_a2xx_/3510598519-21257u557,29-14F621F1F930/ap…QcOcCMLPVAQ2hbP2AEOlDA4JUyyyc7qIYANwUvNrbsTQg3DxTyVeTHuOBrQntMCyBIT_NDo4cE,\",\"similar_url\":\"http://apartments.oodle.com/85383/homes-for-rent/4+-bedrooms/price_900_1600/?r=5\",\"paid\":\"No\",\"source\":{\"id\":\"propertynut-realestate\",\"name\":\"PropertyNut.Com\"},\"location\":{\"citycode\":\"usa:az:peoria\",\"name\":\"Peoria\",\"zip\":\"85383\",\"country\":\"USA\",\"longitude\":\"-112.2429\",\"state\":\"AZ\",\"address\":\"26933 84th Ave\",\"latitude\":\"33.7268\",\"addresscode\":\"addr:usa:az:peoria:26933+84th+avenue\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3510598519t_1s_homes_for_rent_in_peoria_az/?1381960038\",\"height\":75,\"width\":100,\"num\":\"1\",\"alt\":\"Four BR in Westwing Mountain H\",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$1,395\",\"fee\":\"No\",\"bathrooms\":2,\"price\":\"1395\",\"bedrooms\":4,\"square_feet\":2001,\"currency\":\"USD\",\"amenities\":\"Patio/Deck\",\"has_photo\":\"Thumbnail\"},\"id\":\"3510598519\",\"user\":[]},{\"body\":\"Spacious yet surprising cozy! The interior of this single level phoenix home welcomes you with a spacious living room, bay windows adorning the dedicated dining area room, and a must see kitchen that comes with gorgeous stainless appliances!\",\"category\":{\"id\":\"housing/rent/home\",\"name\":\"Homes for Rent\"},\"revenue_score\":2,\"ctime\":1381916551,\"title\":\"8664 W Holly St\",\"url\":\"http://apartments.oodle.com/j_a2xx_/3510287749-11403u557,29-14F621F1F930/ap…fZNJIf3zN-feBCvMFmpLV928MU4n4WnH9I3Xl0zdW4-YlpZuS3KCqkm3KfajWYpb90hzsDTsRA,\",\"similar_url\":\"http://apartments.oodle.com/85037/homes-for-rent/3-bedrooms/4-bedrooms/price_620_1060/?r=5\",\"paid\":\"No\",\"source\":{\"id\":\"realrentals\",\"name\":\"RealRentals\"},\"location\":{\"citycode\":\"usa:az:phoenix\",\"name\":\"Phoenix\",\"zip\":\"85037\",\"country\":\"USA\",\"longitude\":\"-112.2469\",\"state\":\"AZ\",\"address\":\"8664 W Holly St\",\"latitude\":\"33.4699\",\"addresscode\":\"addr:usa:az:phoenix:8664+w+holly+st\"},\"images\":[{\"src\":\"http://i.oodleimg.com/item/3510287749t_1s_homes_for_rent_in_phoenix_az/?1381916791\",\"height\":75,\"width\":100,\"num\":\"1\",\"alt\":\"8664 W Holly St\",\"size\":\"s\"}],\"attributes\":{\"price_display\":\"$950 Yearly\",\"fee\":\"No\",\"bathrooms\":2,\"price_type\":\"Yearly\",\"price\":\"950\",\"bedrooms\":3,\"currency\":\"USD\",\"pets_allowed\":\"Cats,Dogs\",\"amenities\":\"AC,Fireplace,Parking,Pool\",\"has_photo\":\"Thumbnail\"},\"id\":\"3510287749\",\"user\":[]}],\"stats\":{\"price\":{\"std\":558.132,\"min\":700,\"max\":2300,\"median\":1595,\"range\":1600,\"mode\":1595,\"mean\":1543},\"bedrooms\":{\"std\":1,\"min\":3,\"max\":5,\"median\":4,\"range\":2,\"mode\":4,\"mean\":3},\"bathrooms\":{\"std\":0,\"min\":2,\"max\":3,\"median\":2,\"range\":1,\"mode\":2,\"mean\":2},\"square_feet\":{\"std\":458.277,\"min\":1700,\"max\":3084,\"median\":2050,\"range\":1384,\"mode\":2050,\"mean\":2269}}},\"template\":\"single:real_estate\"}},\"speak\":\"I've found 5484 listings. The average price is $1543.\"}";
            queries.Add(new Tuple<string, bool>(resultString, false));

            // movies
            resultString = "{\"show\":{\"simple\":{\"text\":\"Here are some movies that are currently playing.\"},\"structured\":{\"items\":[{\"ratings\":{\"critics_score\":5,\"audience_score\":51,\"audience_rating\":\"Spilled\",\"critics_rating\":\"Rotten\"},\"title\":\"I, Frankenstein\",\"image\":\"http://content6.flixster.com/movie/11/17/45/11174524_pro.jpg\",\"critics_consensus\":\"Loud, incoherent, and dramatically listless, I, Frankenstein is a remarkably dull fantasy adventure that fails to generate much excitement or interest in its characters.\",\"id\":\"771257268\",\"abridged_cast\":[{\"name\":\"Aaron Eckhart\",\"characters\":[\"Adam\"],\"id\":\"162668655\"},{\"name\":\"Bill Nighy\",\"characters\":[\"Naberius\"],\"id\":\"162652300\"},{\"name\":\"Yvonne Strahovski\",\"characters\":[\"Terra\"],\"id\":\"770790627\"},{\"name\":\"Miranda Otto\",\"characters\":[\"Leonore\"],\"id\":\"162652765\"},{\"name\":\"Jai Courtney\",\"characters\":[\"Gideon\"],\"id\":\"771383833\"}],\"synopsis\":\"Set in a dystopic present where vigilant gargoyles and ferocious demons rage in a battle for ultimate power, Victor Frankenstein's creation Adam (Aaron Eckhart) finds himself caught in the middle as both sides race to discover the secret to his immortality. From the creators of the hit supernatural saga, UNDERWORLD, comes the action thriller I, FRANKENSTEIN, written for the screen and directed by Stuart Beattie based on the graphic novel I, Frankenstein by Kevin Grevioux, and brought to life by a cast that includes Aaron Eckhart, Bill Nighy, Yvonne Strahovski, Miranda Otto, Jai Courtney, Socratis Otto, Mahesh Jadu, Caitlin Stasey and Aden Young as Victor Frankenstein. (c) Lionsgate\",\"mpaa_rating\":\"PG-13\",\"year\":2013,\"runtime\":100,\"release_date\":\"2014-01-24\"},{\"ratings\":{\"critics_score\":89,\"audience_score\":89,\"audience_rating\":\"Upright\",\"critics_rating\":\"Certified Fresh\"},\"title\":\"Frozen\",\"image\":\"http://content6.flixster.com/movie/11/17/35/11173584_pro.jpg\",\"critics_consensus\":\"Beautifully animated, smartly written, and stocked with singalong songs, Frozen adds another worthy entry to the Disney canon.\",\"id\":\"771246543\",\"abridged_cast\":[{\"name\":\"Kristen Bell\",\"characters\":[\"Anna\"],\"id\":\"326395677\"},{\"name\":\"Idina Menzel\",\"characters\":[\"Elsa\"],\"id\":\"162655508\"},{\"name\":\"Jonathan Groff\",\"characters\":[\"Kristoff\"],\"id\":\"770836783\"},{\"name\":\"Josh Gad\",\"characters\":[\"Olaf the Snowman\"],\"id\":\"770683794\"},{\"name\":\"Santino Fontana\",\"characters\":[\"Hans\"],\"id\":\"771406662\"}],\"synopsis\":\"Featuring the voices of Kristen Bell and Idina Menzel, Frozen is the coolest comedy-adventure ever to hit the big screen. When a prophecy traps a kingdom in eternal winter, Anna, a fearless optimist, teams up with extreme mountain man Kristoff and his sidekick reindeer Sven on an epic journey to find Anna's sister Elsa, the Snow Queen, and put an end to her icy spell. Encountering mystical trolls, a funny snowman named Olaf, Everest-like extremes and magic at every turn, Anna and Kristoff battle the elements in a race to save the kingdom from destruction. (c) Disney\",\"mpaa_rating\":\"PG\",\"year\":2013,\"runtime\":85,\"release_date\":\"2013-11-27\"},{\"ratings\":{\"critics_score\":12,\"audience_score\":53,\"audience_rating\":\"Spilled\",\"critics_rating\":\"Rotten\"},\"title\":\"The Nut Job\",\"image\":\"http://content6.flixster.com/movie/11/17/48/11174840_pro.jpg\",\"critics_consensus\":\"Hampered by an unlikable central character and source material stretched too thin to cover its brief running time, The Nut Job will provoke an allergic reaction in all but the least demanding moviegoers.\",\"id\":\"771354446\",\"abridged_cast\":[{\"name\":\"Will Arnett\",\"characters\":[\"Surly\"],\"id\":\"335717464\"},{\"name\":\"Brendan Fraser\",\"characters\":[\"Grayson\"],\"id\":\"162662763\"},{\"name\":\"Liam Neeson\",\"characters\":[\"Raccoon\"],\"id\":\"162652242\"},{\"name\":\"Katherine Heigl\",\"characters\":[\"Andie\"],\"id\":\"162670134\"},{\"name\":\"Stephen Lang\",\"characters\":[\"King\"],\"id\":\"162662195\"}],\"synopsis\":\"In animated 3D, THE NUT JOB is an action-packed comedy in fictional Oakton that follows the travails of Surly (voiced by Will Arnett), a mischievous squirrel, and his rat friend Buddy, who plan a nut store heist of outrageous proportions and unwittingly find themselves embroiled in a much more complicated and hilarious adventure. (c) Open Road\",\"mpaa_rating\":\"PG\",\"year\":2014,\"runtime\":86,\"release_date\":\"2014-01-17\"},{\"ratings\":{\"critics_score\":73,\"audience_score\":90,\"audience_rating\":\"Upright\",\"critics_rating\":\"Fresh\"},\"title\":\"Lone Survivor\",\"image\":\"http://content7.flixster.com/movie/11/17/26/11172665_pro.jpg\",\"critics_consensus\":\"While it may deliver its messages of patriotism, courage, and sacrifice a tad heavy-handedly, Lone Survivor finds writer/director Peter Berg wielding enough visceral power to mitigate many of his movie's jingoistic flaws.\",\"id\":\"771356130\",\"abridged_cast\":[{\"name\":\"Mark Wahlberg\",\"characters\":[\"Marcus Luttrell\"],\"id\":\"162653181\"},{\"name\":\"Ben Foster\",\"characters\":[\"Matt Axelson\"],\"id\":\"162669838\"},{\"name\":\"Emile Hirsch\",\"characters\":[\"Danny Dietz\"],\"id\":\"162652589\"},{\"name\":\"Taylor Kitsch\",\"characters\":[\"Mike Murphy\"],\"id\":\"326299816\"},{\"name\":\"Eric Bana\",\"characters\":[\"Erik Kristensen\"],\"id\":\"162662190\"}],\"synopsis\":\"LONE SURVIVOR, starring Mark Wahlberg, tells the story of four Navy SEALs on an ill-fated covert mission to neutralize a high-level Taliban operative who are ambushed by enemy forces in the Hindu Kush region of Afghanistan. Based on The New York Times bestseller, this story of heroism, courage and survival directed by Peter Berg (Friday Night Lights) also stars Taylor Kitsch, Emile Hirsch, Ben Foster and Eric Bana. LONE SURVIVOR will be released by Universal Pictures in platform engagements on Friday, December 27, 2013, and will go wide on Friday, January 10, 2014. (c) Universal Pictures\",\"mpaa_rating\":\"R\",\"year\":2013,\"runtime\":121,\"release_date\":\"2014-01-10\"},{\"ratings\":{\"critics_score\":16,\"audience_score\":71,\"audience_rating\":\"Upright\",\"critics_rating\":\"Rotten\"},\"title\":\"Ride Along\",\"image\":\"http://content6.flixster.com/movie/11/17/48/11174812_pro.jpg\",\"critics_consensus\":\"Kevin Hart's livewire presence gives Ride Along a shot of necessary energy, but it isn't enough to rescue this would-be comedy from the buddy-cop doldrums.\",\"id\":\"771320491\",\"abridged_cast\":[{\"name\":\"Ice Cube\",\"characters\":[\"James\"],\"id\":\"162652332\"},{\"name\":\"Kevin Hart\",\"characters\":[\"Ben\"],\"id\":\"770671077\"},{\"name\":\"John Leguizamo\",\"characters\":[\"Santiago\"],\"id\":\"162652698\"},{\"name\":\"Bruce McGill\",\"characters\":[\"Lt. Brooks\"],\"id\":\"162652571\"},{\"name\":\"Tika Sumpter\",\"characters\":[\"Angela Payton\"],\"id\":\"771078913\"}],\"synopsis\":\"Kevin Hart and Ice Cube lead the lineup in Ride Along, the new film from the director and the producer of the blockbuster comedy Think Like a Man. When a fast-talking guy joins his girlfriend's brother-a hot-tempered cop-to patrol the streets of Atlanta, he gets entangled in the officer's latest case. Now, in order to prove that he deserves his future bride, he must survive the most insane 24 hours of his life. For the past two years, high-school security guard Ben (Hart) has been trying to show decorated APD detective James (Cube) that he's more than just a video-game junkie who's unworthy of James' sister, Angela (Tika Sumpter). When Ben finally gets accepted into the academy, he thinks he's earned the seasoned policeman's respect and asks for his blessing to marry Angela. Knowing that a ride along will demonstrate if Ben has what it takes to take care of his sister, James invites him on a shift designed to scare the hell out of the trainee. But when the wild night leads them to the most notorious criminal in the city, James will find that his new partner's rapid-fire mouth is just as dangerous as the bullets speeding at it. John Leguizamo and Laurence Fishburne join the cast of the action-comedy directed by Tim Story. Ride Along is produced by Will Packer (Think Like a Man), alongside Ice Cube, Matt Alvarez (Barbershop) and Larry Brezner (Good Morning, Vietnam). (C) Universal\",\"mpaa_rating\":\"PG-13\",\"year\":2014,\"runtime\":99,\"release_date\":\"2014-01-17\"},{\"ratings\":{\"critics_score\":22,\"audience_score\":56,\"audience_rating\":\"Spilled\",\"critics_rating\":\"Rotten\"},\"title\":\"That Awkward Moment\",\"image\":\"http://content8.flixster.com/movie/11/17/44/11174486_pro.jpg\",\"critics_consensus\":\"Formulaic and unfunny, That Awkward Moment wastes a charming cast on a contrived comedy that falls short of the date movies it seems to be trying to subvert.\",\"id\":\"771359339\",\"abridged_cast\":[{\"name\":\"Zac Efron\",\"characters\":[\"Jason\"],\"id\":\"171852647\"},{\"name\":\"Michael B. Jordan\",\"characters\":[\"Mikey\"],\"id\":\"770683889\"},{\"name\":\"Miles Teller\",\"characters\":[\"Daniel\"],\"id\":\"771077409\"},{\"name\":\"Imogen Poots\",\"characters\":[\"Ellie\"],\"id\":\"770678493\"},{\"name\":\"Mackenzie Davis\",\"characters\":[\"Chelsea\"],\"id\":\"771423798\"}],\"synopsis\":\"Zac Efron, Miles Teller and Michael B. Jordan star in the R-rated comedy, THAT AWKWARD MOMENT, about three best friends who find themselves where we've all been- at that confusing moment in every dating relationship when you have to decide So...where is this going? (c) FilmDistrict\",\"mpaa_rating\":\"R\",\"year\":2014,\"runtime\":94,\"release_date\":\"2014-01-31\"},{\"ratings\":{\"critics_score\":57,\"audience_score\":63,\"audience_rating\":\"Upright\",\"critics_rating\":\"Rotten\"},\"title\":\"Jack Ryan: Shadow Recruit\",\"image\":\"http://content9.flixster.com/movie/11/17/39/11173959_pro.jpg\",\"critics_consensus\":\"It doesn't reinvent the action-thriller wheel, but Jack Ryan: Shadow Recruit offers a sleek, reasonably diverting reboot for a long-dormant franchise.\",\"id\":\"771314857\",\"abridged_cast\":[{\"name\":\"Chris Pine\",\"characters\":[\"Jack Ryan\"],\"id\":\"326393041\"},{\"name\":\"Keira Knightley\",\"characters\":[\"Cathy Muller\"],\"id\":\"162654560\"},{\"name\":\"Kevin Costner\",\"characters\":[\"William Harper\"],\"id\":\"162662350\"},{\"name\":\"Peter Andersson\",\"characters\":[\"Dimitri Lemkov\"],\"id\":\"326395717\"},{\"name\":\"Kenneth Branagh\",\"characters\":[\"Viktor Cherevin\"],\"id\":\"162659680\"}],\"synopsis\":\"Based on the character created by bestselling author Tom Clancy, Jack Ryan is a global action thriller set in the present day. This original story follow a young Jack (Chris Pine) as he uncovers a financial terrorist plot. The story follows him from 9/11, through his tour of duty in Afghanistan, which scarred him forever, and into his early days in the Financial Intelligence Unit of the modern CIA where he becomes an analyst, under the guardianship of his handler, Harper (Kevin Costner). When Ryan believes he's uncovered a Russian plot to collapse the United States economy, he goes from being an analyst to becoming a spy and must fight to save his own life andthose of countless others, while also trying to protect the thing that's more important to him than anything, his relationship with his fiancee Cathy (Keira Knightley). (c) Paramount\",\"mpaa_rating\":\"PG-13\",\"year\":2013,\"runtime\":105,\"release_date\":\"2014-01-17\"},{\"ratings\":{\"critics_score\":32,\"audience_score\":62,\"audience_rating\":\"Upright\",\"critics_rating\":\"Rotten\"},\"title\":\"Labor Day\",\"image\":\"http://content9.flixster.com/movie/11/17/55/11175503_pro.jpg\",\"critics_consensus\":\"Kate Winslet and Josh Brolin make for an undeniably compelling pair, but they can't quite rescue Labor Day from the pallid melodrama of its exceedingly ill-advised plot.\",\"id\":\"770820337\",\"abridged_cast\":[{\"name\":\"Josh Brolin\",\"characters\":[\"Frank Chambers\"],\"id\":\"162654237\"},{\"name\":\"Kate Winslet\",\"characters\":[\"Adele Wheeler\"],\"id\":\"162659302\"},{\"name\":\"Gattlin Griffith\",\"characters\":[\"Henry Wheeler\"],\"id\":\"770819198\"},{\"name\":\"Brooke Smith\",\"characters\":[\"Evelyn\"],\"id\":\"162654447\"},{\"name\":\"Clark Gregg\",\"characters\":[\"Gerald\"],\"id\":\"364607667\"}],\"synopsis\":\"Labor Day centers on 13-year-old Henry Wheeler, who struggles to be the man of his house and care for his reclusive mother Adele while confronting all the pangs of adolescence. On a back-to-school shopping trip, Henry and his mother encounter Frank Chambers, a man both intimidating and clearly in need of help, who convinces them to take him into their home and later is revealed to be an escaped convict. The events of this long Labor Day weekend will shape them for the rest of their lives. --(C) Paramount\",\"mpaa_rating\":\"PG-13\",\"year\":2014,\"runtime\":111,\"release_date\":\"2014-01-31\"},{\"ratings\":{\"critics_score\":65,\"audience_score\":72,\"audience_rating\":\"Upright\",\"critics_rating\":\"Fresh\"},\"title\":\"August: Osage County\",\"image\":\"http://content9.flixster.com/movie/11/17/41/11174159_pro.jpg\",\"critics_consensus\":\"The sheer amount of acting going on in August: Osage County threatens to overwhelm, but when the actors involved are as talented as Meryl Streep and Julia Roberts, it's difficult to complain.\",\"id\":\"771188478\",\"abridged_cast\":[{\"name\":\"Meryl Streep\",\"characters\":[\"Violet Weston\"],\"id\":\"162654900\"},{\"name\":\"Julia Roberts\",\"characters\":[\"Barbara Weston\"],\"id\":\"162659460\"},{\"name\":\"Juliette Lewis\",\"characters\":[\"Karen Weston\"],\"id\":\"162654115\"},{\"name\":\"Ewan McGregor\",\"characters\":[\"Bill Fordham\"],\"id\":\"162652152\"},{\"name\":\"Abigail Breslin\",\"characters\":[\"Jean Fordham\"],\"id\":\"162653854\"}],\"synopsis\":\"AUGUST: OSAGE COUNTY tells the dark, hilarious and deeply touching story of the strong-willed women of the Weston family, whose lives have diverged until a family crisis brings them back to the Midwest house they grew up in, and to the dysfunctional woman who raised them. Letts' play made its Broadway debut in December 2007 after premiering at Chicago's legendary Steppenwolf Theatre earlier that year. It continued with a successful international run. (c) Weinstein\",\"mpaa_rating\":\"R\",\"year\":2013,\"runtime\":130,\"release_date\":\"2013-12-25\"},{\"ratings\":{\"critics_score\":93,\"audience_score\":79,\"audience_rating\":\"Upright\",\"critics_rating\":\"Certified Fresh\"},\"title\":\"American Hustle\",\"image\":\"http://content6.flixster.com/movie/11/17/44/11174440_pro.jpg\",\"critics_consensus\":\"Riotously funny and impeccably cast, American Hustle compensates for its flaws with unbridled energy and some of David O. Russell's most irrepressibly vibrant direction.\",\"id\":\"771352406\",\"abridged_cast\":[{\"name\":\"Jennifer Lawrence\",\"id\":\"770800260\"},{\"name\":\"Bradley Cooper\",\"id\":\"351525448\"},{\"name\":\"Christian Bale\",\"id\":\"162652645\"},{\"name\":\"Amy Adams\",\"id\":\"162653029\"},{\"name\":\"Jeremy Renner\",\"id\":\"309973652\"}],\"synopsis\":\"A fictional film set in the alluring world of one of the most stunning scandals to rock our nation, American Hustle tells the story of brilliant con man Irving Rosenfeld (Christian Bale), who along with his equally cunning and seductive British partner Sydney Prosser (Amy Adams) is forced to work for a wild FBI agent Richie DiMaso (Bradley Cooper). DiMaso pushes them into a world of Jersey powerbrokers and mafia that's as dangerous as it is enchanting. Jeremy Renner is Carmine Polito, the passionate, volatile, New Jersey political operator caught between the con-artists and Feds. Irving's unpredictable wife Rosalyn (Jennifer Lawrence) could be the one to pull the thread that brings the entire world crashing down. Like David O. Russell's previous films, American Hustle defies genre, hinging on raw emotion, and life and death stakes. (c) Sony\",\"mpaa_rating\":\"R\",\"year\":2013,\"runtime\":129,\"release_date\":\"2013-12-20\"}],\"template\":\"list:movies\"}},\"speak\":\"Here are some movies that are currently playing.\"}";
            queries.Add(new Tuple<string, bool>(resultString, false));

            // dictionary
            resultString = "{\"show\":{\"simple\":{\"text\":\"grab, take hold of so as to seize or restrain or stop the motion of\"},\"structured\":{\"item\":{\"headword\":\"grab\",\"senses\":{\"verb\":[{\"definition\":\"take hold of so as to seize or restrain or stop the motion of\",\"kind\":\"contact\",\"examples\":[\"Catch the ball!\",\"Grab the elevator door!\"],\"lemmas\":[\"catch\",\"grab\",\"take hold of\"]},{\"definition\":\"get hold of or seize quickly and easily\",\"kind\":\"possession\",\"examples\":[\"I snapped up all the good buys during the garage sale\"],\"lemmas\":[\"snap up\",\"snaffle\",\"grab\"]},{\"definition\":\"make a grasping or snatching motion with the hand\",\"kind\":\"motion\",\"examples\":[\"The passenger grabbed for the oxygen mask\"],\"lemmas\":[\"grab\"]},{\"definition\":\"obtain illegally or unscrupulously\",\"kind\":\"possession\",\"examples\":[\"Grab power\"],\"lemmas\":[\"grab\"]},{\"definition\":\"take or grasp suddenly\",\"kind\":\"contact\",\"examples\":[\"She grabbed the child's hand and ran out of the room\"],\"lemmas\":[\"grab\"]},{\"definition\":\"capture the attention or imagination of\",\"kind\":\"cognition\",\"examples\":[\"This story will grab you\",\"The movie seized my imagination\"],\"lemmas\":[\"grab\",\"seize\"]}],\"noun\":[{\"definition\":\"a mechanical device for gripping an object\",\"kind\":\"artifact\",\"examples\":[],\"lemmas\":[\"grab\"]},{\"definition\":\"the act of catching an object with the hands\",\"kind\":\"act\",\"examples\":[\"Mays made the catch with his back to the plate\",\"he made a grab for the ball before it landed\",\"Martin's snatch at the bridle failed and the horse raced away\",\"the infielder's snap and throw was a single motion\"],\"lemmas\":[\"catch\",\"grab\",\"snatch\",\"snap\"]}]},\"pos\":[\"verb\",\"noun\"]},\"template\":\"single:dictionary\"}},\"speak\":\"grab, take hold of so as to seize or restrain or stop the motion of\"}";
            queries.Add(new Tuple<string, bool>(resultString, false));

            // stock 
            resultString = "{\"show\":{\"simple\":{\"text\":\"Facebook stock is trading at $49.19, down 2.17%\"},\"structured\":{\"item\":{\"opening_price\":50.46,\"low_price\":49.06,\"P/E Ratio\":227.51,\"share_price\":49.19,\"stock_exchange\":\"NasdaqNM\",\"trade_volume\":\"48.20 million\",\"52_week_high\":51.6,\"average_trade_volume\":\"70.12 million\",\"pe\":0.221,\"high_price\":50.72,\"share_price_change\":-1.09,\"market_cap\":\"119.80 billion\",\"5_day_moving_average\":43.542,\"symbol\":\"FB\",\"share_price_change_percent\":2.17,\"name\":\"Facebook\",\"yield\":\"N/A\",\"52_week_low\":18.8,\"share_price_direction\":\"down\"},\"template\":\"simple:stock\"}},\"speak\":\"Facebook stock is trading at $49.19, down 2.17%\"}";
            queries.Add(new Tuple<string, bool>(resultString, false));

            // shopping
            resultString = "{\"show\":{\"simple\":{\"text\":\"The top shopping result I found is: Xbox One Console - Standard Edition, for $499.96.\",\"link\":\"http://www.amazon.com/Xbox-One-Console-Standard-Edition/dp/B00CMQTVMI%3FSub…nkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB00CMQTVMI\"},\"structured\":{\"items\":[{\"url\":\"http://www.amazon.com/Xbox-One-Console-Standard-Edition/dp/B00CMQTVMI%3FSub…nkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB00CMQTVMI\",\"price\":\"$499.96\",\"image\":\"http://ecx.images-amazon.com/images/I/41WVjYBpLGL._SL160_.jpg\",\"title\":\"Xbox One Console - Standard Edition\"},{\"url\":\"http://www.amazon.com/Xbox-360-4GB/dp/B00D9EPI38%3FSubscriptionId%3D016S53A…nkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB00D9EPI38\",\"price\":\"$179.00\",\"image\":\"http://ecx.images-amazon.com/images/I/310D-sfUdXL._SL160_.jpg\",\"title\":\"Xbox 360 4GB\"},{\"url\":\"http://www.amazon.com/Xbox-360-4GB-Console/dp/B003O6JKLC%3FSubscriptionId%3…nkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB003O6JKLC\",\"price\":\"$179.00\",\"image\":\"http://ecx.images-amazon.com/images/I/411d9xAlGRL._SL160_.jpg\",\"title\":\"Xbox 360 4GB Console\"},{\"url\":\"http://www.amazon.com/Xbox-360-4GB-Kinect-Value-Bundle/dp/B0096KENEO%3FSubs…nkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB0096KENEO\",\"price\":\"$309.99\",\"image\":null,\"title\":\"Xbox 360 4GB Kinect Value Bundle\"},{\"url\":\"http://www.amazon.com/Xbox-360-Wireless-Controller-Glossy-Black/dp/B003ZSP0…nkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB003ZSP0WW\",\"price\":\"$35.45\",\"image\":\"http://ecx.images-amazon.com/images/I/41mVXvLfOtL._SL160_.jpg\",\"title\":\"Xbox 360 Wireless Controller - Glossy Black\"},{\"url\":\"http://www.amazon.com/Xbox-360-250GB-Spring-Value-Bundle/dp/B00BBU8VFY%3FSu…nkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB00BBU8VFY\",\"price\":\"$249.37\",\"image\":\"http://ecx.images-amazon.com/images/I/41EaIngkbqL._SL160_.jpg\",\"title\":\"Xbox 360 S 250GB Spring Value Bundle\"},{\"url\":\"http://www.amazon.com/Xbox-360-4GB-Kinect-Nike-Bundle/dp/B009VS8G80%3FSubsc…nkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB009VS8G80\",\"price\":\"$249.00\",\"image\":\"http://ecx.images-amazon.com/images/I/51BHF-GDNKL._SL160_.jpg\",\"title\":\"Xbox 360 S 4GB with Kinect Nike+ Bundle\"},{\"url\":\"http://www.amazon.com/Microsoft-XBOX-360-Console-Kinect-Sensor/dp/B00D9FQ9C…nkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB00D9FQ9CQ\",\"price\":\"$279.00\",\"image\":\"http://ecx.images-amazon.com/images/I/41KVV6W5bVL._SL160_.jpg\",\"title\":\"Microsoft XBOX 360 E 4GB Console with Kinect Sensor\"},{\"url\":\"http://www.amazon.com/Xbox-360-250GB-Holiday-Value-Bundle/dp/B0096KEMUY%3FS…nkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB0096KEMUY\",\"price\":\"$292.95\",\"image\":\"http://ecx.images-amazon.com/images/I/51nJFBcNbsL._SL160_.jpg\",\"title\":\"Xbox 360 250GB Holiday Value Bundle\"}],\"template\":\"list:shopping:amazon\"}},\"speak\":\"Here are some great shopping results courtesy of Amazon.com!\"}";
            queries.Add(new Tuple<string, bool>(resultString, false));

            if (queries.Count > 0)
            {
                foreach (Tuple<string, bool> query in queries)
                {
                    if (query.Item2)
                    {
                        AddDialog(DialogOwner.User, "test query", DialogType.Query);

                        args = new CompleteEventArgs(JsonConvert.DeserializeObject<ActorModel>(query.Item1));

                        OnComplete(this, args);
                    }
                }

                DispatcherTimer timer = new DispatcherTimer();

                timer.Interval = TimeSpan.FromSeconds(15);
                timer.Tick += (s, e) =>
                    {
                        //timer.Stop();
                        //RemoveOldestQuery();

                        string followUpString = "{\"show\":{\"simple\":{\"text\":\"Here's our forecast for Wednesday: Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening.\"},\"structured\":{\"item\":{\"week\":[{\"date\":\"2013-10-02\",\"night\":{\"text\":\"Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening. \",\"sky\":\"Clear\",\"temp\":\"65\"}},{\"date\":\"2013-10-03\",\"daytime\":{\"text\":\"Sunny, with a high near 89. Light and variable wind becoming southwest 5 to 10 mph in the morning. \",\"sky\":\"Sunny\",\"temp\":\"89\"},\"night\":{\"text\":\"Mostly clear, with a low around 63. West wind 5 to 9 mph becoming light and variable. \",\"sky\":\"Mostly Clear\",\"temp\":\"63\"}},{\"date\":\"2013-10-04\",\"daytime\":{\"text\":\"Sunny, with a high near 86. Light and variable wind becoming northwest around 6 mph in the afternoon. \",\"sky\":\"Sunny\",\"temp\":\"86\"},\"night\":{\"text\":\"Mostly clear, with a low around 59. Breezy, with a north northwest wind 9 to 14 mph becoming northeast 15 to 20 mph after midnight. Winds could gust as high as 28 mph. \",\"sky\":\"Breezy\",\"temp\":\"59\"}},{\"date\":\"2013-10-05\",\"daytime\":{\"text\":\"Sunny, with a high near 85. Breezy. \",\"sky\":\"Breezy\",\"temp\":\"85\"},\"night\":{\"text\":\"Mostly clear, with a low around 57.\",\"sky\":\"Mostly Clear\",\"temp\":\"57\"}},{\"date\":\"2013-10-06\",\"daytime\":{\"text\":\"Sunny, with a high near 88.\",\"sky\":\"Sunny\",\"temp\":\"88\"},\"night\":{\"text\":\"Mostly clear, with a low around 61.\",\"sky\":\"Mostly Clear\",\"temp\":\"61\"}},{\"date\":\"2013-10-07\",\"daytime\":{\"text\":\"Sunny, with a high near 90.\",\"sky\":\"Sunny\",\"temp\":\"90\"},\"night\":{\"text\":\"Mostly clear, with a low around 62.\",\"sky\":\"Mostly Clear\",\"temp\":\"62\"}},{\"date\":\"2013-10-08\",\"daytime\":{\"text\":\"Sunny, with a high near 88.\",\"sky\":\"Sunny\",\"temp\":\"88\"},\"night\":{\"text\":\"Mostly clear, with a low around 63.\",\"sky\":\"Mostly Clear\",\"temp\":\"63\"}}],\"now\":{\"sky\":\"Mostly Clear\",\"temp\":\"89\"},\"location\":null},\"template\":\"single:weather\"}},\"speak\":\"Here's our forecast for Wednesday: Mostly clear, with a low around 65. Southwest wind around 5 mph becoming calm  in the evening.\"}";

                        AddDialog(DialogOwner.User, "follow up query", DialogType.Query);

                        args = new CompleteEventArgs(JsonConvert.DeserializeObject<ActorModel>(followUpString));

                        OnComplete(this, args);
                    };

                //timer.Start();
            }
        }

        
        #region Plexi Handlers
        private void Show(ShowModel model, string speak)
        {
            Show(model, speak, DialogType.None);
        }

        private void Show(ShowModel model, string speak, DialogType type)
        {
            Dictionary<string, object> simple = model.simple;

            if (simple.ContainsKey("text"))
            {
                string show = (string)simple["text"];

                string link = null;

                if (simple.ContainsKey("link"))
                {
                    link = (string)simple["link"];
                }

                AddDialog(DialogOwner.Plexi, show, link, type);

                //this.speechService.Speak(speak);
            }
        }


        private void OnChoose(object sender, ChoiceEventArgs e)
        {
            ChoiceViewModel vm = new ChoiceViewModel();

            vm.Load(null, e.results.show.simple);

            Items.Add(vm);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            MessageBoxResult response = MessageBox.Show(e.message, "Server Error", MessageBoxButton.OK);

            if (response == MessageBoxResult.OK)
            {
                plexiService.ClearContext();
            }
        }

        private void OnAuthorize(object sender, AuthorizationEventArgs e)
        {
            string message;

            if (e.model != null)
            {
                string model = e.model.Split('_')[0];

                message = String.Format("Oops, it looks like we don't have an account synced for {0}. Please sync an account to continue.", model);
            }
            else
            {
                message = "Oops, it looks like we don't have an account synced. Please sync an account to continue.";
            }

            MessageBoxResult response = MessageBox.Show(message, "Account Authorization", MessageBoxButton.OKCancel);

            if (response.Equals(MessageBoxResult.OK))
            {
                // navigate to settings page to sync various accounts
                navigationService.NavigateTo(ViewModelLocator.SettingsPageUri);

            }
            else if (response.Equals(MessageBoxResult.Cancel))
            {
                // 1. set flag in plexi service so this event isn't raised every time personal user data is needed.
                // 2. the user will have to go into the settings views afterwards to setup a stremor account which 
                //    will unlock the ability to auth accounts like google, facebook, fitbit
                // 3. search local 
            }
        }

        private void OnProgress(object sender, ProgressEventArgs e)
        {
            try
            {
                Messenger.Default.Send(new ProgressMessage(e.inProgress));
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("OnProgress Error: {0}", err.Message));
            }
        }

        /*
        private void OnShow(object sender, ShowEventArgs e)
        {
            Messenger.Default.Send(new ShowMessage(e.show, e.speak, e.link));

            if (e.status == State.InProgress)
            {
                PhoneApplicationFrame frame = App.Current.RootVisual as PhoneApplicationFrame;

                if (!frame.CurrentSource.Equals(ViewModelLocator.ConversationPageUri))
                {
                    this.navigationService.NavigateTo(ViewModelLocator.ConversationPageUri);
                }
            }
        }
        */

        private void OnShow(object sender, ShowEventArgs e)
        {
            // TODO: send speak text off to ViewBase via Messenger or SpeechService
            // e.speak;

            string text = e.show;
            string link = e.link;

            if (text != null)
            {
                AddDialog(DialogOwner.Plexi, text, link);
            }
        }

        private void OnAct(object sender, ActorEventArgs e)
        {
            Debug.WriteLine("on act");
            ClassifierModel data = e.data;

            string action = data.action;

            // run local actors
            if (ActorMap.ContainsKey(data.model))
            {
                Actor actor = ActorMap[e.data.model];

                Dictionary<string, object> payload = e.data.payload;

                if (Actors != null && Actors.Count > 0)
                {
                    Tuple<Actor, string> actorAction = new Tuple<Actor, string>(actor, action);

                    if (Actors.ContainsKey(actorAction) && e.handled == false)
                    {
                        e.handled = true;
                        Actors[actorAction].DynamicInvoke(payload);
                    }
                    else
                    {
                        Debug.WriteLine(String.Format("action'{0}' is not supported by {1}", action, actor));
                    }
                }
                else
                {
                    Debug.WriteLine("no actors set up. nothing to do");
                }
            }
        }

        private void OnComplete(object sender, CompleteEventArgs e)
        {
            try
            {
                // remove dialogModels that are followups
                RemoveFollowUpDialog();

                ActorModel response = e.actor;

                Show(response.show, response.speak, DialogType.Complete);

                if (response.show.structured != null && response.show.structured.ContainsKey("template"))
                {
                    Dictionary<string, object> structured = response.show.structured;

                    string[] template = (structured["template"] as string).Split(':');

                    string type = template[0];
                    string templateName = null;
                    object viewModel = null;

                    bool viewModelLoaded = false;

                    // try to load specific viewmodel. ie. shopping:amazon
                    if (template.Count() > 2)
                    {
                        string secondaryString = template[2];
                        
                        // normalize fitbit secondary names
                        switch (template[2])
                        {
                            case "weight":
                                secondaryString = "weight";
                                break;

                            case "food":
                            case "log-food":
                            case "calories":
                                secondaryString = "food";
                                break;

                            case "activity":
                            case "log-activity":
                                secondaryString = "activity";
                                break;
                        }                         

                        templateName = String.Format("{0}:{1}", template[1], secondaryString);

                        viewModelLoaded = TryLoadViewModel(templateName, structured, out viewModel);
                    }
                    
                    // if a specific vm was not found, try to load up a generic one. ie. shopping
                    if (!viewModelLoaded && template.Count() > 1)
                    {
                        templateName = template[1];

                        viewModelLoaded = TryLoadViewModel(templateName, structured, out viewModel);
                    }

                    // if a vm was loaded, add it to the list to display
                    if (viewModelLoaded)
                    {
                        Items.Add(viewModel);
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("OnComplete Error: {0}", err.Message));
            }
        }

        private bool TryLoadViewModel(string templateName, Dictionary<string, object> structured, out object vm)
        {
            try
            {
                string viewModelName = String.Format("{0}ViewModel", templateName.CamelCase());

                Debug.WriteLine(Assembly.GetExecutingAssembly().GetName().Name);

                string fullTypeName = String.Format("{0}.ViewModels.{1}", Assembly.GetExecutingAssembly().GetName().Name, viewModelName);

                Type type = Type.GetType(fullTypeName);

                if (type == null)
                {
                    Debug.WriteLine(String.Format("TryLoadViewModel: view model {0} could not be found", templateName));
                    vm = null;
                    return false;
                }

                var viewModel = Activator.CreateInstance(type);
            
                MethodInfo loadMethod = viewModel.GetType().GetMethod("Load");

                if (loadMethod == null)
                {
                    Debug.WriteLine(String.Format("TryLoadViewModel: 'Load' method not implemented in {0}", templateName));
                    vm = null;
                    return false;
                }

                if (!structured.ContainsKey("item") && !structured.ContainsKey("items"))
                {
                    Debug.WriteLine("TryLoadViewModel: unable to find 'item' or 'items' in response");
                    vm = null;
                    return false;
                }

                if (structured.ContainsKey("items") && ((JArray)structured["items"]).Count <= 0)
                {
                    Debug.WriteLine("TryLoadViewModel: items list is emtpy nothing to set");
                    vm = null;
                    return false;
                }

                if (structured.ContainsKey("item") && ((JObject)structured["item"]).Count <= 0)
                {
                    Debug.WriteLine("TryLoadViewModel: item object is emtpy nothing to set");
                    vm = null;
                    return false;
                }

                object[] parameters = new object[] { templateName, structured };

                loadMethod.Invoke(viewModel, parameters);

                vm = viewModel;

                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("TryLoadViewModel Error: {0}", err.Message));
                vm = null;
                return false;
            }
        }

        #endregion

        #region Helpers
        public void AddDialog(DialogOwner sender, string message)
        {
            AddDialog(sender, message, null, DialogType.None);
        }

        public void AddDialog(DialogOwner sender, string message, DialogType type)
        {
            AddDialog(sender, message, null, type);
        }

        public void AddDialog(DialogOwner sender, string message, string link)
        {
            AddDialog(sender, message, link, DialogType.None);
        }

        public void AddDialog(DialogOwner sender, string message, string link, DialogType type)
        {
            Debug.WriteLine(message);

            if (Items == null)
            {
                Items = new ObservableCollection<object>();
            }

            Items.Add(new DialogModel(sender, message, link, type));
        }

        // TODO: make this process animated :)
        public void RemoveFollowUpDialog()
        {
            IEnumerable<object> t = Items.Where(x => x.GetType() == typeof(DialogModel) && ((DialogModel)x).type == DialogType.None).ToList();

            foreach (var item in t)
            {
                Items.Remove(item);
            }
        }

        public void RemoveOldestQuery()
        {
            DialogModel secondQuery = Items.Where((x, i) => x is DialogModel && (x as DialogModel).type == DialogType.Query && i != 0).Cast<DialogModel>().FirstOrDefault();

            if (secondQuery != null)
            {
                int index = Items.IndexOf(secondQuery);

                IEnumerable<object> taken = Items.TakeWhile((x, i) => i < index).ToList();

                try
                {
                    foreach (var item in taken)
                    {
                        Items.Remove(item);
                    } 
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                }
            }
        }

        public void ClearDialog()
        {

        }
        #endregion
    }
}
