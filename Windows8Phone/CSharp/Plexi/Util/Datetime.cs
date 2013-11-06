using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Plexi.Models;

namespace Plexi.Util
{
    public class Datetime
    {
        private static Regex dateRegex = new Regex(@"/\d{2,4}[-]\d{2}[-]\d{2}/i");
        private static Regex timeRegex = new Regex(@"/\d{1,2}[:]\d{2}[:]\d{2}/i");

        /// <summary>
        /// Provides an easy way to convert Unix Timestamps to a DateTime object with locale awareness.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns>A DateTime representation of the Unix Timestamp</returns>
        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp).ToLocalTime();
        }

        /// <summary>
        /// Provides an easy way to convert a DateTime object to a Universal Unix Timestamp.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>A Unix Timestamp representation of a DateTime object</returns>
        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static Dictionary<string, string> BuildDatetimeFromJson(object date = null, object time = null)
        {
            DateTime? dateObj = null;

            if (date != null)
            {
                if (date.GetType() == typeof(string) && dateRegex.IsMatch((string)date) == false)
                {
                    dateObj = Datetime.BuildDatetimeHelper((string)date);
                }

                if (date.GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                {
                    dateObj = Datetime.BuildDatetimeHelper((Newtonsoft.Json.Linq.JObject)date);
                }
            }

            if (time != null)
            {
                if (time.GetType() == typeof(string) && timeRegex.IsMatch((string)time) == false)
                {
                    dateObj = Datetime.BuildDatetimeHelper((string)time, dateObj);
                }

                if (time.GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                {
                    dateObj = Datetime.BuildDatetimeHelper((Newtonsoft.Json.Linq.JObject)time, dateObj);
                }
            }

            Dictionary<string, string> newDate = new Dictionary<string,string>();

            newDate.Add("date", dateObj.Value.ToString("yyyy-MM-dd"));
            newDate.Add("time", dateObj.Value.ToString("H:mm:ss"));
          
            return newDate;
        }

        private static object GetPreference(string name)
        {
            return null;
            /*
            DatabaseModel db;

            using (db = new DatabaseModel(AppResources.DataStore))
            {
                //if (db.DatabaseExists() == false)
                //    return;
                IQueryable<PreferenceItem> query = from PreferenceItem preference in db.Preferences where preference.Name == name select preference;

                if (query.Count().Equals(0))
                {
                    return false;
                }

                return query.First();
            }
            */ 
        }

        private static DateTime WeekdayHelper(int dayOfWeek)
        {
            DateTime date = DateTime.Now;

            int currentDay = (int)date.DayOfWeek;

            int offset = (currentDay < dayOfWeek) ? (dayOfWeek - currentDay) : (7 - (currentDay - dayOfWeek));

            date.AddDays(offset);

            return date;
        }

        // {'#time_add': [{'#time_fuzzy': {'label': 'dinner', 'default': '19:00:00'}}, 3600]}
        private static DateTime FuzzyHelper(Newtonsoft.Json.Linq.JObject datetime, bool isDate)
        {
            string label = null;
            string def = null;
            string pref = null;

            foreach (var i in datetime)
            {
                if (i.Key.Equals("label"))
                {
                    label = (string)i.Value;
                }

                if (i.Key.Equals("default"))
                {
                    def = (string)i.Value;
                }
            }

            //object preference = GetPreference(label);

            //pref = (preference.GetType() == typeof(bool) && (bool)preference.Equals(false)) ? def : (preference as PreferenceItem).Value;
            pref = def;

            return DateTime.Parse(pref);               
        }

        // this will fail if we have a date obj and time is a string.
        private static DateTime? BuildDatetimeHelper(string dateortime, DateTime? newDate = null)
        {
            if (newDate.Equals(null))
            {
                Debug.WriteLine("item is a string");
                newDate = (dateortime.Contains("now")) ? DateTime.Now : DateTime.Parse(dateortime);
            }
            else
            {
                if (timeRegex.IsMatch(dateortime) && newDate > DateTime.Parse(dateortime))
                {
                    ((DateTime)newDate).AddDays(1);
                }
                /*
                if (timeRegex.IsMatch(dateortime))
                {
                    var split = dateortime.Split(':').Select(n => int.Parse(n)).ToArray();

                    var hours = newDate.Value.Hour;
                    var minutes = newDate.Value.Minute;

                    if ((hours > split[0]) || (hours == split[0] && minutes > split[1]))
                    {
                        newDate.Value.AddDays(1);
                    }
                }
                */
            }
            
            return newDate;
        }

        private static DateTime? BuildDatetimeHelper(Newtonsoft.Json.Linq.JObject dateortime, DateTime? newDate = null)
        {            
            foreach (KeyValuePair<string, Newtonsoft.Json.Linq.JToken> partial in dateortime)
            {
                if (partial.Key.Contains("weekday"))
                {
                    return WeekdayHelper((int)partial.Value);
                }
                else if (partial.Key.Contains("fuzzy"))
                {
                    bool isDate = (partial.Key.Contains("date")) ? true : false;
                    return FuzzyHelper((Newtonsoft.Json.Linq.JObject)partial.Value, isDate);
                }
                else
                {
                    if (partial.Value.Type.ToString().Equals("Array"))
                    {
                        Debug.WriteLine("step 1");

                        var val = (Newtonsoft.Json.Linq.JArray)partial.Value;

                        foreach (var item in val)
                        {
                            Debug.WriteLine("step 2");

                            if (newDate.Equals(null))
                            {
                                switch (item.Type.ToString())
                                {
                                    case "String":
                                        Debug.WriteLine("item is a string");
                                        try
                                        {
                                            newDate = ((string)item).Contains("now") ? DateTime.Now : DateTime.Parse((string)item);
                                        }
                                        catch (Exception stringErr)
                                        {
                                            Debug.WriteLine(stringErr.Message);
                                        }
                                        break;

                                    case "Object":
                                        Debug.WriteLine("item is an object");
                                        foreach (KeyValuePair<string, Newtonsoft.Json.Linq.JToken> itemToken in (Newtonsoft.Json.Linq.JObject)item)
                                        {
                                            if (newDate.Equals(null))
                                            {
                                                if (itemToken.Key.Contains("weekday"))
                                                {
                                                    Debug.WriteLine("weekday conversion");

                                                    newDate = WeekdayHelper((int)itemToken.Value);
                                                }
                                                else if (itemToken.Key.Contains("fuzzy"))
                                                {
                                                    Debug.WriteLine("fuzzy conversion");

                                                    bool isDate = (itemToken.Key.Contains("date")) ? true : false;

                                                    newDate = FuzzyHelper((Newtonsoft.Json.Linq.JObject)itemToken.Value, isDate);
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                            else if (item.Type.ToString().Equals("Integer"))
                            {
                                Debug.WriteLine("parse int value");
                                double interval = (double)item;

                                if (!interval.Equals(null))
                                {
                                    DateTime? temp = null;

                                    string action = partial.Key.ToString();

                                    if (action.Contains("time"))
                                    {
                                        temp = newDate.Value.AddSeconds(interval);
                                    }
                                    else if (action.Contains("date"))
                                    {
                                        temp = newDate.Value.AddDays(interval);
                                    }

                                    if (!temp.Equals(null))
                                    {
                                        newDate = temp;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return newDate;
        }
    }
}
