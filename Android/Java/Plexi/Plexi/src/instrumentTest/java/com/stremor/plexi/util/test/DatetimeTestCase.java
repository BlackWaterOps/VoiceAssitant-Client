package com.stremor.plexi.util.test;

import com.stremor.plexi.util.Datetime;

import junit.framework.TestCase;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.HashMap;

/**
 * Created by jon on 17.12.2013.
 */
public class DatetimeTestCase extends TestCase {

    private static SimpleDateFormat PROTOCOL_DATE_FORMAT = new SimpleDateFormat("yyyy-MM-dd");
    private static SimpleDateFormat PROTOCOL_TIME_FORMAT = new SimpleDateFormat("HH:mm:ss");

    /**
     * Functional tests
     */
    public void testSimplePassthrough1() throws ParseException {
        String date = "2013-01-01";
        String time = "12:34:56";
        HashMap<String, String> ret = Datetime.BuildDatetimeFromJson(date, time);
        assertEquals(date, ret.get("date"));
        assertEquals(time, ret.get("time"));
    }

    public void testSimplePassthrough2() throws ParseException {
        String date = "2013-01-01";
        String time = null;
        HashMap<String, String> ret = Datetime.BuildDatetimeFromJson(date, time);
        assertEquals(date, ret.get("date"));
        assertEquals(time, ret.get("time"));
    }

    public void testSimplePassthrough3() throws ParseException {
        String date = null;
        String time = "12:34:56";
        HashMap<String, String> ret = Datetime.BuildDatetimeFromJson(date, time);
        assertEquals(date, ret.get("date"));
        assertEquals(time, ret.get("time"));
    }

    public void testSimplePassthrough4() throws ParseException {
        String date = null;
        String time = null;
        HashMap<String, String> ret = Datetime.BuildDatetimeFromJson(date, time);
        assertEquals(date, ret.get("date"));
        assertEquals(time, ret.get("time"));
    }

    public void testDateNow() throws ParseException {
        Date date = new Date();
        HashMap<String, String> ret = Datetime.BuildDatetimeFromJson("now", null, date);
        assertEquals(PROTOCOL_DATE_FORMAT.format(date), ret.get("date"));
        assertEquals(null, ret.get("time"));
    }

    public void testTimeNow() throws ParseException {
        Date date = new Date();
        HashMap<String, String> ret = Datetime.BuildDatetimeFromJson(null, "now", date);
        assertEquals(null, ret.get("date"));
        assertEquals(PROTOCOL_TIME_FORMAT.format(date), ret.get("time"));
    }

    public void testDatetimeNow() throws ParseException {
        Date date = new Date();
        HashMap<String, String> ret = Datetime.BuildDatetimeFromJson("now", "now", date);
        assertEquals(PROTOCOL_DATE_FORMAT.format(date), ret.get("date"));
        assertEquals(PROTOCOL_TIME_FORMAT.format(date), ret.get("time"));
    }
}
