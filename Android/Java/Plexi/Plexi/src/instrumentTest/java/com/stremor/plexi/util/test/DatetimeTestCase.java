package com.stremor.plexi.util.test;

import android.util.Pair;

import com.stremor.plexi.util.Datetime;

import junit.framework.TestCase;

import org.joda.time.LocalDateTime;
import org.joda.time.format.DateTimeFormat;
import org.joda.time.format.DateTimeFormatter;
import org.json.JSONException;
import org.json.JSONObject;

import java.text.ParseException;

/**
 * Created by jon on 17.12.2013.
 */
public class DatetimeTestCase extends TestCase {
    private static DateTimeFormatter PROTOCOL_DATE_FORMATTER = DateTimeFormat.forPattern("yyyy-MM-dd");
    private static DateTimeFormatter PROTOCOL_TIME_FORMATTER = DateTimeFormat.forPattern("HH:mm:ss");

    /**
     * Functional tests
     */
    public void testSimplePassthrough1() throws ParseException {
        Pair dt = new Pair("2013-01-01", "12:34:56");
        Pair ret = Datetime.datetimeFromJson(dt.first, dt.second);
        assertEquals(dt.first, ret.first);
        assertEquals(dt.second, ret.second);
    }

    public void testSimplePassthrough2() throws ParseException {
        Pair dt = new Pair("2013-01-01", null);
        Pair ret = Datetime.datetimeFromJson(dt.first, dt.second);
        assertEquals(dt.first, ret.first);
        assertNull(ret.second);
    }

    public void testSimplePassthrough3() throws ParseException {
        Pair dt = new Pair(null, "12:34:56");
        Pair ret = Datetime.datetimeFromJson(dt.first, dt.second);
        assertNull(ret.first);
        assertEquals(dt.second, ret.second);
    }

    public void testSimplePassthrough4() throws ParseException {
        Pair dt = new Pair(null, null);
        Pair ret = Datetime.datetimeFromJson(dt.first, dt.second);
        assertNull(ret.first);
        assertNull(ret.second);
    }

    public void testDateNow() throws ParseException {
        LocalDateTime date = LocalDateTime.now();
        Pair ret = Datetime.datetimeFromJson("#date_now", null, date);
        assertEquals(date.toLocalDate().toString(), ret.first);
        assertNull(ret.second);
    }

    public void testTimeNow() throws ParseException {
        LocalDateTime date = LocalDateTime.now();
        Pair ret = Datetime.datetimeFromJson(null, "#time_now", date);
        assertNull(ret.first);
        assertEquals(PROTOCOL_TIME_FORMATTER.print(date), ret.second);
    }

    public void testDatetimeNow() throws ParseException {
        LocalDateTime date = LocalDateTime.now();
        Pair ret = Datetime.datetimeFromJson("#date_now", "#time_now", date);
        assertEquals(PROTOCOL_DATE_FORMATTER.print(date), ret.first);
        assertEquals(PROTOCOL_TIME_FORMATTER.print(date), ret.second);
    }

    public void testWeekdaySameDay() throws ParseException, JSONException {
        LocalDateTime now = new LocalDateTime(2013, 1, 1, 0, 0); // Tuesday Jan 1 2013 00:00
        JSONObject input = new JSONObject("{\"#date_weekday\": 2}");
        Pair ret = Datetime.datetimeFromJson(input, null, now);
        assertEquals("2013-01-01", ret.first);
        assertNull(ret.second);
    }

    public void testWeekdayNextDay() throws ParseException, JSONException {
        LocalDateTime now = new LocalDateTime(2013, 1, 1, 0, 0); // Tuesday Jan 1 2013 00:00
        JSONObject input = new JSONObject("{\"#date_weekday\": 3}");
        Pair ret = Datetime.datetimeFromJson(input, null, now);
        assertEquals("2013-01-02", ret.first);
        assertNull(ret.second);
    }
}
