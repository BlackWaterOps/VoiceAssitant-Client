using System;
using System.Diagnostics;

namespace Please.Util
{
   
    public class Datetime
    {
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
                elapseString = String.Format("{0,2:MM}/{0,2:dd}/{0,4:yyyy} {0,2:hh}:{0,2:mm}", date);
            }

            return elapseString;
        }
    }
}
