using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PlexiSDK.Models;

namespace PlexiSDK.Util
{
    public static class Datetime
    {
        private static Regex dateRegex = new Regex(@"\d{2,4}[-]\d{2}[-]\d{2}", RegexOptions.IgnoreCase);
        private static Regex timeRegex = new Regex(@"\d{2}:\d{2}:\d{2}", RegexOptions.IgnoreCase);

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

        /// <summary>
        ///  Get the elapsed time of a DateTime object in minutes, hours, or date if over 24 hours
        /// </summary>
        /// <param name="date"></param>
        /// <returns>A string representation of the elapsed time</returns>
        public static string GetTimeElapsed(DateTime date)
        {
            DateTime now = DateTime.Now;

            TimeSpan elapsed = now.Subtract(date);

            double daysAgo = Math.Round(Math.Abs(elapsed.TotalDays));

            string elapseString = "";

            if (daysAgo <= 1)
            {
                double minutesAgo = Math.Round(Math.Abs(elapsed.TotalMinutes));

                double hoursAgo = Math.Round(Math.Abs(elapsed.TotalHours));

                if (minutesAgo < 60)
                {
                    elapseString = String.Format("About {0} minutes ago", minutesAgo);
                }
                else
                {
                    if (hoursAgo == 1)
                    {
                        elapseString = String.Format("About {0} hour ago", hoursAgo);
                    }
                    else
                    {
                        elapseString = String.Format("About {0} hours ago", hoursAgo);
                    }
                }
            }
            else
            {
                elapseString = date.ToString("MM/dd/yyyy h:mm tt");
            }

            return elapseString;
        }

        public static Tuple<string, string> DateTimeFromJson(object dateO, object timeO)
        {
            return DateTimeFromJson(dateO, timeO, DateTime.Now);
        }

        public static Tuple<string, string> DateTimeFromJson(object dateO, object timeO, DateTime now)
        {            
            DateTime? date = null;
            DateTime? time = null;

            if (dateO == null)
            {

            }
            else if (dateO.GetType() == typeof(string))
            {
                string dateString = (string)dateO;
                if (dateString == "#date_now")
                {
                    date = now;
                }
                else if (dateRegex.IsMatch(dateString))
                {
                    date = DateTime.Parse(dateString);
                }
            }
            else if (dateO.GetType() == typeof(JObject))
            {
                try
                {
                    date = parseDateObject((JObject)dateO, now);
                }
                catch (Exception) { }
            }
            
            if (timeO == null)
            {

            }
            else if (timeO.GetType() == typeof(string))
            {
                string timeString = (string)timeO;

                if (timeString == "#time_now")
                {
                    time = now;
                }
                else if (timeRegex.IsMatch(timeString))
                {
                    time = DateTime.Parse(timeString);
                }
            }
            else if (timeO.GetType() == typeof(JObject))
            {
                bool hasDate = date != null;

                DateTime? baseDate = hasDate ? date : now;

                DateTime? ret = null;

                try
                {
                    ret = parseTimeObject((JObject)timeO, baseDate, now);
                }
                catch (Exception) { }

                if (ret.HasValue)
                {
                    time = ret;
                    if (hasDate)
                    {
                        date = ret;
                    }
                }
            }

            string dString = (!date.HasValue) ? null : ((DateTime)date).ToString("yyyy-MM-dd");
            string tString = (!time.HasValue) ? null : ((DateTime)time).ToString("HH:mm:ss");

            return new Tuple<string, string>(dString, tString);
        }

        private static DateTime? parseDateObject(JObject dateO, DateTime now)
        {
            JToken value;

            if (dateO.TryGetValue("#date_weekday", out value))
            {
                return parseDateWeekdayObject(dateO, now);
            }
            else if (dateO.TryGetValue("#date_add", out value))
            {
                return (DateTime)parseDateAddObject(dateO, now);
            }

            return null;
        }

        private static DateTime? parseDateWeekdayObject(JObject obj, DateTime now)
        {
            int dayNum = (int)obj["#date_weekday"];

            int currentDay = (int)now.DayOfWeek;

            if (dayNum != currentDay)
            {
                int offset = (currentDay < dayNum) ? (dayNum - currentDay) : (7 - (currentDay - dayNum));

                return now.AddDays(offset);
            }

            return now;
        }

        private static DateTime? parseDateAddObject(JObject obj, DateTime now)
        {
            JArray operands = (JArray)obj["#date_add"];

            JToken first = operands[0];

            DateTime? baseDate = null;

            JToken value;
            
            if (first.GetType() == typeof(JObject) && ((JObject)first).TryGetValue("#date_weekday", out value))
            {
                baseDate = (DateTime)parseDateObject((JObject)first, now);
            }
            else if (first.GetType() == typeof(JValue) && ((JValue)first).Type == JTokenType.String && (string)first == "#date_now")
            {
                baseDate = now;
            }
            
            if (!baseDate.HasValue)
            {
                return null;
            }
            
            return ((DateTime)baseDate).AddDays((int)operands[1]);
        }

        private static DateTime? parseTimeObject(JObject timeO, DateTime? baseDate, DateTime now)
        {
            JToken value;

            if (timeO.TryGetValue("#time_add", out value))
            {
                return parseTimeAddObject(timeO, baseDate, now);
            }
            else if (timeO.TryGetValue("#fuzzy_time", out value))
            {
                return parseTimeFuzzyObject(timeO, baseDate, now);
            }
            else
            {
                return null;
            }
        }

        private static DateTime? parseTimeAddObject(JObject obj, DateTime? baseDate, DateTime now)
        {
            JArray operands = (JArray)obj["#time_add"];

            JToken first = operands[0];

            JValue firstValue = (JValue)first;

            DateTime? baseDateTime = null;

            JToken value;

            if (first.GetType() == typeof(JObject) && ((JObject)first).TryGetValue("#fuzzy_time", out value))
            {
                baseDateTime = (DateTime)parseTimeObject((JObject)first, baseDate, now);
            }
            else if (first.GetType() == typeof(JValue) && ((JValue)first).Type == JTokenType.String && (string)first == "#time_now")
            {
                baseDateTime = now;
            }
            
            if (!baseDateTime.HasValue)
            {
                return null;
            }

            return ((DateTime)baseDateTime).AddSeconds((int)operands[1]);
        }

        private static DateTime? parseTimeFuzzyObject(JObject obj, DateTime? baseDate, DateTime now)
        {
            string label = (string)obj["label"];

            DateTime defaultTime = DateTime.Parse((string)obj["default"]);

            DateTime? time = getFuzzyTimeValue(label);

            DateTime datetime = (!time.HasValue) ? defaultTime : (DateTime)time;

            return ((DateTime)baseDate).Add(new TimeSpan(datetime.Hour, datetime.Minute, datetime.Second));
        }

        private static DateTime? getFuzzyTimeValue(string label)
        {
            // TODO: lookup settings
            return null;

        }
        /*
        internal static Dictionary<string, string> BuildDatetimeFromJson(object date = null, object time = null)
        {
            DateTime? dateObj = null;

            if (date != null)
            {
                if (date.GetType() == typeof(string) && dateRegex.IsMatch((string)date) == false)
                {
                    dateObj = Datetime.BuildDatetimeHelper((string)date);
                }

                if (date.GetType() == typeof(JObject))
                {
                    dateObj = Datetime.BuildDatetimeHelper((JObject)date);
                }
            }

            if (time != null)
            {
                if (time.GetType() == typeof(string) && timeRegex.IsMatch((string)time) == false)
                {
                    dateObj = Datetime.BuildDatetimeHelper((string)time, dateObj);
                }

                if (time.GetType() == typeof(JObject))
                {
                    dateObj = Datetime.BuildDatetimeHelper((JObject)time, dateObj);
                }
            }

            Dictionary<string, string> newDate = new Dictionary<string,string>();

            newDate.Add("date", dateObj.Value.ToString("yyyy-MM-dd"));
            newDate.Add("time", dateObj.Value.ToString("H:mm:ss"));
          
            return newDate;
        }
        */

        //private static object GetPreference(string name)
        //{
            //return null;
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
        //}
        /*
        private static DateTime WeekdayHelper(int dayOfWeek)
        {
            DateTime date = DateTime.Now;

            int currentDay = (int)date.DayOfWeek;

            int offset = (currentDay < dayOfWeek) ? (dayOfWeek - currentDay) : (7 - (currentDay - dayOfWeek));

            date.AddDays(offset);

            return date;
        }
        */
        
        /*
        private static DateTime FuzzyHelper(JObject datetime, bool isDate)
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
        */

        // this will fail if we have a date obj and time is a string.
        /*
        private static DateTime? BuildDatetimeHelper(string dateortime, DateTime? newDate = null)
        {
            if (newDate.Equals(null))
            {
                if (dateortime.Contains("now"))
                {
                    newDate = DateTime.Now;
                }
                else 
                {
                    newDate = DateTime.Parse(dateortime);

                    if (newDate < DateTime.Now)
                    {
                        newDate = ((DateTime)newDate).AddDays(1);
                    }
                }
            }
            else
            {
                if (timeRegex.IsMatch(dateortime) && newDate > DateTime.Parse(dateortime))
                {
                    newDate = ((DateTime)newDate).AddDays(1);
                }*/
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
            //}
            
            //return newDate;
        //}

        /*
        private static DateTime? BuildDatetimeHelper(JObject dateortime, DateTime? newDate = null)
        {            
            foreach (KeyValuePair<string, JToken> partial in dateortime)
            {
                if (partial.Key.Contains("weekday"))
                {
                    return WeekdayHelper((int)partial.Value);
                }
                else if (partial.Key.Contains("fuzzy"))
                {
                    bool isDate = (partial.Key.Contains("date")) ? true : false;
                    return FuzzyHelper((JObject)partial.Value, isDate);
                }
                else
                {
                    if (partial.Value.Type.ToString().Equals("Array"))
                    {
                        Debug.WriteLine("step 1");

                        var val = (JArray)partial.Value;

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
                                        foreach (KeyValuePair<string, JToken> itemToken in (JObject)item)
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

                                                    newDate = FuzzyHelper((JObject)itemToken.Value, isDate);
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
         */
    }
}
