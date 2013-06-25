using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Phone.Tasks;
using Microsoft.Phone.Scheduler;

using Windows.Devices.Geolocation;

namespace Please.Util
{
    public class Actions
    {
        public static void DoCall(Dictionary<string, object> payload)
        {
            // use PhoneCallTask
            // payload.phone
            if (payload.ContainsKey("phone"))
            {
                var phoneCallTask = new PhoneCallTask();

                phoneCallTask.PhoneNumber = (string)payload["phone"];

                phoneCallTask.Show();
            }
        }

        public static void DoSms(Dictionary<string, object> payload)
        {
            // use SmsComposeTask
            // payload.phone
            // payload.message

            if (payload.ContainsKey("phone") && payload.ContainsKey("message"))
            {
                var smsComposeTask = new SmsComposeTask();

                smsComposeTask.To = (string)payload["phone"];
                smsComposeTask.Body = (string)payload["message"];

                smsComposeTask.Show();
            }
        }

        public static void DoEmail(Dictionary<string, object> payload)
        {
            // use EmailComposeTask
            // payload.subject
            // payload.message
            // payload.address
            if (payload.ContainsKey("subject") && payload.ContainsKey("message") && payload.ContainsKey("address"))
            {
                var emailComposeTask = new EmailComposeTask();

                emailComposeTask.Subject = (string)payload["subject"];
                emailComposeTask.Body = (string)payload["message"];
                emailComposeTask.To = (string)payload["address"];

                emailComposeTask.Show();
            }
        }

        public static void DoWeb(Dictionary<string, object> payload)
        {
            // use WebBrowserTask
            // payload.url
            if (payload.ContainsKey("url"))
            {
                var webBrowserTask = new WebBrowserTask();

                webBrowserTask.Uri = new Uri((string)payload["url"], UriKind.Absolute);

                webBrowserTask.Show();
            }
        }

        public static void DoCalendar(Dictionary<string, object> payload)
        {
            // use SaveAppoinmentTask
            // payload.time
            // payload.date
            // payload.duration
            // payload.location
            // payload.subject
            // payload.person
            if (payload.ContainsKey("date") && payload.ContainsKey("duration") && payload.ContainsKey("subject"))
            {
                var saveAppointmentTask = new SaveAppointmentTask();
                var location = (payload.ContainsKey("location") && payload["location"] != null) ? (string)payload["location"] : "";
                var details = (string)payload["subject"];

                if (payload.ContainsKey("person") && payload["person"] != null)
                {
                    details += " with " + (string)payload["person"];
                }

                if (location != "")
                {
                    details += " at " + location;
                }

                var time = (payload.ContainsKey("time") && payload["time"] != null) ? (string)payload["time"] : "12:00:00 PM";

                var startTime = DateTime.Parse((string)payload["date"] + " " + time);

                var endTime = startTime.AddMilliseconds(((double)payload["duration"] * 3600000));

                saveAppointmentTask.Subject = details;
                saveAppointmentTask.Location = location;
                saveAppointmentTask.StartTime = startTime;
                saveAppointmentTask.EndTime = endTime;
                saveAppointmentTask.Reminder = Microsoft.Phone.Tasks.Reminder.ThirtyMinutes;
                saveAppointmentTask.AppointmentStatus = Microsoft.Phone.UserData.AppointmentStatus.Tentative;

                saveAppointmentTask.Show();
            }
        }

        public static string DoTime(Dictionary<string, object> payload)
        {
            var now = DateTime.Now;

            // say("It is now " + hours + ":" + mins + " " + ampm + " on " + day + ", " + month + " " +  date + ", " + year + ".");
            //elapseString = String.Format("{0,2:MM}/{0,2:dd}/{0,4:yyyy} {0,2:hh}:{0,2:mm}", date);

            return String.Format("It is now {0,2:hh}:{0,2:mm} {0,2:tt} on {0,2:dddd}, {0,2:MMMM} {0,2:dd}, {0,4:yyyy}", now);
        }

        public static void DoDirections(Dictionary<string, object> payload, Geoposition myposition)
        {
            // use BingMapsDirectionsTask or MapsDirectionTask
            // payload.location
            if (payload.ContainsKey("location"))
            {
                /* ENDPOINT CURRENTLY DOES NOT SUPPORT THIS FEATURE */
                var mapsDirectionTask = new MapsDirectionsTask();

                var start = new LabeledMapLocation();
                var startLocation = new System.Device.Location.GeoCoordinate(myposition.Coordinate.Latitude, myposition.Coordinate.Longitude);
                start.Location = startLocation;

                var end = new LabeledMapLocation();
                // if payload["location"] is latlong value
                //var endLocation = new System.Device.Location.GeoCoordinate();
                //end.Location = endLocation;

                //if payload["location"] is city/state value
                end.Label = (string)payload["location"];

                mapsDirectionTask.Start = start;
                mapsDirectionTask.End = end;

                mapsDirectionTask.Show();
            }
        }

        public static void DoLocate(Dictionary<string, object> payload, Geoposition myposition)
        {
            var mapsTask = new MapsTask();
            var center = new System.Device.Location.GeoCoordinate(myposition.Coordinate.Latitude, myposition.Coordinate.Longitude);

            if (payload.ContainsKey("coordinates"))
            {
                // payload would need to send lat/long coords
                // center = new System.Device.Location.GeoCoordinate(payload.Latitude, payload.Longitude);
            }

            if (payload.ContainsKey("searchterm"))
            {
                // var searchTerm = (string)payload["searchterm"];
                // mapsTask.SearchTerm = searchTerm;
            }

            mapsTask.Center = center;
            mapsTask.Show();
        }

        public static void DoAlarm(Dictionary<string, object> payload)
        {
            if (payload.ContainsKey("datetime"))
            {
                Debug.WriteLine(payload["datetime"].GetType());

                if (payload["datetime"].GetType().Equals(typeof(DateTime)))
                {
                    Debug.WriteLine("convert datetime to string");
                    payload["datetime"] = payload["datetime"].ToString();
                }
                
                DateTime beginDate = DateTime.Parse((string)payload["datetime"]);
                
                DateTime endDate = beginDate.AddMinutes(1); // arbitrary end date for now

                // Make sure that the begin time has not already passed.
                if (beginDate < DateTime.Now)
                {
                    MessageBox.Show("the begin date must be in the future.");
                    return;
                }

                // Make sure that the expiration time is after the begin time.
                if (endDate < beginDate)
                {
                    MessageBox.Show("expiration time must be after the begin time.");
                    return;
                }

                var alarm = new Alarm("StremorPleaseAlarm");

                alarm.BeginTime = beginDate;
                alarm.ExpirationTime = endDate;
                //alarm.Content = "enter message here";
            }
        }

        public static void DoReminder(Dictionary<string, object> payload)
        {
            if (payload.ContainsKey("datetime"))
            {
                var beginDate = DateTime.Parse((string)payload["datetime"]);
                var endDate = beginDate.AddMinutes(1); // arbitrary end date for now


                // Make sure that the begin time has not already passed.
                if (beginDate < DateTime.Now)
                {
                    MessageBox.Show("the begin date must be in the future.");
                    return;
                }

                // Make sure that the expiration time is after the begin time.
                if (endDate < beginDate)
                {
                    MessageBox.Show("expiration time must be after the begin time.");
                    return;
                }

                var reminder = new Microsoft.Phone.Scheduler.Reminder("StremorPleaseReminder");
            }
        }

        public static async Task DoIntent(Dictionary<string, object> payload)
        {
            if (payload.ContainsKey("location"))
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri((string)payload["location"], UriKind.Absolute));
            }
        }
    }
}
