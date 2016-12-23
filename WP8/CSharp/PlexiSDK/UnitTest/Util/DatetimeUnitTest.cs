using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PlexiSDK.Util;
namespace UnitTest.Util
{
    [TestClass]
    public class DatetimeUnitTest
    {
        private static string DateFormat = @"yyyy-MM-dd";
        private static string TimeFormat = @"HH:mm:ss";

        [TestMethod]
        public void TestSimplePassthrough1()
        {
            Tuple<string, string> dt = new Tuple<string, string>("2013-01-01", "12:34:56");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(dt.Item1, dt.Item2);

            Assert.AreEqual(dt.Item1, ret.Item1);
            Assert.AreEqual(dt.Item2, ret.Item2);
        }

        [TestMethod]
        public void TestSimplePassthrough2()
        {
            Tuple<string, string> dt = new Tuple<string, string>("2013-01-01", null);
            Tuple<string, string> ret = Datetime.DateTimeFromJson(dt.Item1, dt.Item2);
            Assert.AreEqual(dt.Item1, ret.Item1);
            Assert.IsNull(ret.Item2);
        }

        [TestMethod]
        public void TestSimplePassthrough3()
        {
            Tuple<String, String> dt = new Tuple<string, string>(null, "12:34:56");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(dt.Item1, dt.Item2);
            Assert.IsNull(ret.Item1);
            Assert.AreEqual(dt.Item2, ret.Item2);
        }

        [TestMethod]
        public void TestSimplePassthrough4()
        {
            Tuple<String, String> dt = new Tuple<string, string>(null, null);
            Tuple<string, string> ret = Datetime.DateTimeFromJson(dt.Item1, dt.Item2);
            Assert.IsNull(ret.Item1);
            Assert.IsNull(ret.Item2);
        }

        [TestMethod]
        public void TestDateNow()
        {
            DateTime date = DateTime.Now;
            Tuple<string, string> ret = Datetime.DateTimeFromJson("#date_now", null, date);
            Assert.AreEqual(date.ToString(DateFormat), ret.Item1);
            Assert.IsNull(ret.Item2);
        }

        [TestMethod]
        public void TestTimeNow()
        {
            DateTime date = DateTime.Now;
            Tuple<string, string> ret = Datetime.DateTimeFromJson(null, "#time_now", date);
            Assert.IsNull(ret.Item1);
            Assert.AreEqual(date.ToString(TimeFormat), ret.Item2);
        }

        [TestMethod]
        public void TestDateTimeNow()
        {
            DateTime date = DateTime.Now;
            Tuple<string, string> ret = Datetime.DateTimeFromJson("#date_now", "#time_now", date);
            Assert.AreEqual(date.ToString(DateFormat), ret.Item1);
            Assert.AreEqual(date.ToString(TimeFormat), ret.Item2);
        }

        [TestMethod]
        public void TestWeekdaySameDay()
        {
            DateTime now = new DateTime(2013, 1, 1, 0, 0, 0); // Tuesday Jan 1 2013 00:00
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"#date_weekday\": 2}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(input, null, now);
            Assert.AreEqual("2013-01-01", ret.Item1);
            Assert.IsNull(ret.Item2);
        }

        [TestMethod]
        public void TestWeekdayNextDay()
        {
            DateTime now = new DateTime(2013, 1, 1, 0, 0, 0); // Tuesday Jan 1 2013 00:00
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"#date_weekday\": 3}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(input, null, now);
            Assert.AreEqual("2013-01-02", ret.Item1);
            Assert.IsNull(ret.Item2);
        }

        [TestMethod]
        public void TestDateAdd()
        {
            DateTime now = new DateTime(2013, 1, 7, 0, 0, 0); // Monday Jan 7 2013 00:00
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"#date_add\": [\"#date_now\", 0]}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(input, null, now);
            Assert.AreEqual("2013-01-07", ret.Item1);
            Assert.IsNull(ret.Item2);
        }

        [TestMethod]
        public void TestDateAdd2()
        {
            DateTime now = new DateTime(2013, 1, 7, 0, 0, 0); // Monday Jan 7 2013 00:00
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"#date_add\": [\"#date_now\", 1]}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(input, null, now);
            Assert.AreEqual("2013-01-08", ret.Item1);
            Assert.IsNull(ret.Item2);
        }

        [TestMethod]
        public void TestDateAdd3()
        {
            DateTime now = new DateTime(2013, 1, 7, 0, 0, 0); // Monday Jan 7 2013 00:00
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"#date_add\": [\"#date_now\", 35]}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(input, null, now);
            Assert.AreEqual("2013-02-11", ret.Item1);
            Assert.IsNull(ret.Item2);
        }

        [TestMethod]
        public void TestDateAddNegative()
        {
            DateTime now = new DateTime(2013, 1, 7, 0, 0, 0); // Monday Jan 7 2013 00:00
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"#date_add\": [\"#date_now\", -1]}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(input, null, now);
            Assert.AreEqual("2013-01-06", ret.Item1);
            Assert.IsNull(ret.Item2);
        }

        [TestMethod]
        public void TestDateAddNegative2()
        {
            DateTime now = new DateTime(2013, 1, 7, 0, 0, 0); // Monday Jan 7 2013 00:00
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"#date_add\": [\"#date_now\", -7]}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(input, null, now);
            Assert.AreEqual("2012-12-31", ret.Item1);
            Assert.IsNull(ret.Item2);
        }

        [TestMethod]
        public void TestDateAddWeekday()
        {
            DateTime now = new DateTime(2013, 1, 7, 0, 0, 0); // Monday Jan 7 2013 00:00
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"#date_add\": [{\"#date_weekday\": 4}, 0]}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(input, null, now);
            Assert.AreEqual("2013-01-10", ret.Item1);
            Assert.IsNull(ret.Item2);
        }

        [TestMethod]
        public void TestDateAddWeekday2()
        {
            DateTime now = new DateTime(2013, 1, 7, 0, 0, 0); // Monday Jan 7 2013 00:00
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"#date_add\": [{\"#date_weekday\": 4}, 1]}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(input, null, now);
            Assert.AreEqual("2013-01-11", ret.Item1);
            Assert.IsNull(ret.Item2);
        }

        [TestMethod]
        public void TestDateAddWeekday3()
        {
            DateTime now = new DateTime(2013, 1, 7, 0, 0, 0); // Monday Jan 7 2013 00:00
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"#date_add\": [{\"#date_weekday\": 4}, -1]}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(input, null, now);
            Assert.AreEqual("2013-01-09", ret.Item1);
            Assert.IsNull(ret.Item2);
        }

        [TestMethod]
        public void TestTimeAdd()
        {
            DateTime now = new DateTime(2013, 1, 7, 12, 0, 0);
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"#time_add\": [\"#time_now\", 0]}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(null, input, now);
            Assert.IsNull(ret.Item1);
            Assert.AreEqual("12:00:00", ret.Item2);
        }

        [TestMethod]
        public void TestTimeAdd2()
        {
            DateTime now = new DateTime(2013, 1, 7, 12, 0, 0, 0);
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"#time_add\": [\"#time_now\", 60]}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(null, input, now);
            Assert.IsNull(ret.Item1);
            Assert.AreEqual("12:01:00", ret.Item2);
        }

        [TestMethod]
        public void TestTimeAdd3()
        {
            DateTime now = new DateTime(2013, 1, 7, 12, 0, 0);
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"#time_add\": [\"#time_now\", -90]}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(null, input, now);
            Assert.IsNull(ret.Item1);
            Assert.AreEqual("11:58:30", ret.Item2);
        }

        // add time which changes date
        [TestMethod]
        public void TestTimeAdd4()
        {
            DateTime now = new DateTime(2013, 1, 7, 23, 59, 0);
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"#time_add\": [\"#time_now\", 120]}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(now.ToString(DateFormat), input, now);
            Assert.AreEqual("2013-01-08", ret.Item1);
            Assert.AreEqual("00:01:00", ret.Item2);
        }

        // add time which changes date
        [TestMethod]
        public void TestTimeAdd5()
        {
            DateTime now = new DateTime(2013, 1, 7, 0, 1, 0);
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"#time_add\": [\"#time_now\", -120]}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(now.ToString(DateFormat), input, now);
            Assert.AreEqual("2013-01-06", ret.Item1);
            Assert.AreEqual("23:59:00", ret.Item2);
        }

        // test object returned from a time disambiguation due to an edit operation
        // {"model":"alarm","action":"edit","payload":{"to":{"time":["07:45:00","19:45:00"]},"from":"19:30:00"}}
        // {"time": {"time": "19:45:00"}}
        [TestMethod]
        public void TestTimeReplace()
        {
            DateTime now = new DateTime(2013, 1, 6, 0, 1, 0);
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"time\":\"19:45:00\"}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(now.ToString(DateFormat), input, now);
            Assert.AreEqual("2013-01-06", ret.Item1);
            Assert.AreEqual("19:45:00", ret.Item2);
        }

        [TestMethod]
        public void TestTimeReplace2()
        {
            DateTime now = DateTime.Now;
            JObject input = JsonConvert.DeserializeObject<JObject>("{\"time\":\"19:30:00\"}");
            Tuple<string, string> ret = Datetime.DateTimeFromJson(null, input, now);
            Assert.IsNull(ret.Item1);
            Assert.AreEqual("19:30:00", ret.Item2);
        }
    }
}
