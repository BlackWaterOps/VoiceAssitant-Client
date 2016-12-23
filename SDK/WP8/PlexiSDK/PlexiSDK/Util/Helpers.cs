using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO.IsolatedStorage;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PlexiSDK.Models;

namespace PlexiSDK.Util
{
    internal static class Helpers
    {
        // used by BuildDateTime
        private static List<Tuple<string, string>> datetimes = new List<Tuple<string, string>>();

        internal static string GetAuthToken()
        {
            string key = Resources.PlexiResources.SettingsAuthKey;

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (!settings.Contains(Resources.PlexiResources.SettingsAuthKey))
            {
                throw new KeyNotFoundException("no auth token could be found");
            }

            byte[] tokenBytes = (byte[])settings[key];

            return Security.Decrypt(tokenBytes);
        }

        // TODO: handle increase quota issue
        internal static void StoreAuthToken(string token)
        {
            byte[] byteToken = Security.Encrypt(token);

            string key = Resources.PlexiResources.SettingsAuthKey;

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            try
            {
                settings[key] = byteToken;
            }
            catch (KeyNotFoundException)
            {
                settings.Add(key, byteToken);
            }
            catch (ArgumentException)
            {
                settings.Add(key, byteToken);
            }

            settings.Save();
        }

        internal static async Task<ClassifierModel> DoClientOperations(ClassifierModel context, Dictionary<string, object> response)
        {
            response = await ReplaceLocation(response);
            response = BuildDateTime(response);
            context = PrependTo(context, response);

            return context;
        }

        internal static ClassifierModel PrependTo(ClassifierModel context, Dictionary<string, object> data)
        {
            try
            {
                if (!data.ContainsKey("unused_tokens"))
                {
                    return context;
                }

                if (((JArray)data["unused_tokens"]).Count <= 0)
                {
                    return context;
                }

                string prepend = (string)((JArray)data["unused_tokens"]).Aggregate((i, j) => i + " " + j);

                string field = (string)data["prepend_to"];

                string payloadField = "";

                if (context.payload.ContainsKey(field) && context.payload[field] != null)
                {
                    payloadField = " " + (string)context.payload[field];
                }

                context.payload[field] = prepend + payloadField;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }

            return context;
        }

        internal static ClassifierModel Replace(ClassifierModel context, string field, object type)
        {
            List<string> fields = field.Split('.').ToList();

            string last = fields[fields.Count - 1];

            // convert to generic object
            object obj = context.DeepCopy<object>();

            object t = fields.Aggregate(obj, (a, b) =>
            {
                if (b == last)
                {
                    return ((JObject)a)[b] = JToken.FromObject(type);
                }
                else
                {
                    return ((JObject)a)[b];
                }
            }
            );

            // convert back to classifier model
            return obj.DeepCopy<ClassifierModel>();
        }

        internal static object Find(ClassifierModel context, string field)
        {
            List<string> fields = field.Split('.').ToList();

            // convert to generic object
            object ctx = context.DeepCopy<object>();

            return fields.Aggregate(ctx, (a, b) => ((JObject)a)[b]);
        }

        internal static async Task<Dictionary<string, object>> ReplaceLocation(Dictionary<string, object> payload)
        {
            try
            {
                if (payload.ContainsKey("location") && payload["location"] != null)
                {
                    if (payload["location"].GetType() == typeof(string))
                    {
                        string location = (string)payload["location"];
                        if (location.Contains("current_location"))
                        {
                            //Debug.WriteLine(SerializeData(GetDeviceInfo()));
                            payload["location"] = await GetDeviceInfo();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }

            return payload;
        }

        internal static Dictionary<string, object> BuildDateTime(Dictionary<string, object> data)
        {
            return default(Dictionary<string, object>);

            /*
            try
            {
                if (data != null)
                {
                    if (datetimes.Count <= 0)
                    {
                        datetimes.Add(new Tuple<string, string>("date", "time"));
                        datetimes.Add(new Tuple<string, string>("start_date", "start_time"));
                        datetimes.Add(new Tuple<string, string>("end_date", "end_time"));
                    }

                    foreach (Tuple<string, string> datetime in datetimes)
                    {
                        if (data.ContainsKey(datetime.Item1) || data.ContainsKey(datetime.Item2))
                        {
                            bool removeDate = false;
                            bool removeTime = false;

                            // add placeholders to satisfy builder
                            if (!data.ContainsKey(datetime.Item1))
                            {
                                data[datetime.Item1] = null;
                                removeDate = true;
                            }

                            if (!data.ContainsKey(datetime.Item2))
                            {
                                data[datetime.Item2] = null;
                                removeTime = true;
                            }

                            // perform replacement
                            if (data[datetime.Item1] != null || data[datetime.Item2] != null)
                            {
                                Dictionary<string, string> build = Datetime.BuildDatetimeFromJson(data[datetime.Item1], data[datetime.Item2]);

                                Debug.WriteLine("datetime build - " + build["date"] + " " + build["time"]);

                                if (data[datetime.Item1] != null)
                                    data[datetime.Item1] = build["date"];

                                if (data[datetime.Item2] != null)
                                    data[datetime.Item2] = build["time"];
                            }

                            // cleanup
                            if (removeDate == true)
                            {
                                data.Remove(datetime.Item1);
                            }

                            if (removeTime == true)
                            {
                                data.Remove(datetime.Item2);
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("BuildDateTime Error: {0}", err.Message));
            }

            Debug.WriteLine("after build datetime");
            Debug.WriteLine(SerializeData(data));

            return data;
             */
        }

        internal static async Task<LocationModel> GetDeviceInfo()
        {
            try
            {
                LocationModel geolocation = LocationService.Default.CurrentPosition;

                // geo fell down so 'manually' get geolocation
                if (geolocation == null)
                {
                    geolocation = await LocationService.Default.GetGeolocation();
                }

                geolocation.timestamp = Datetime.ConvertToUnixTimestamp(DateTime.Now);
                geolocation.timeoffset = DateTimeOffset.Now.Offset;

                return geolocation;
            }
            catch (Exception err)
            {

                Debug.WriteLine(err.Message);

                /*
                if ((uint)err.HResult == 0x80004004)
                {
                    // location has been diabled in phone settings. display appropriate message
                }
                else
                {
                    // unforeseen error
                }
                */

                return default(LocationModel);
            }
        }

        internal static string SerializeData(object data, bool includeNulls = false)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();

            jsonSettings.DefaultValueHandling = DefaultValueHandling.Include;
            jsonSettings.NullValueHandling = (includeNulls == true) ? NullValueHandling.Include : NullValueHandling.Ignore;

            return JsonConvert.SerializeObject(data, jsonSettings);
        }
    }
}
