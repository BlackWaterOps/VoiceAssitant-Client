package com.stremor.plexi.util.test;

import com.stremor.plexi.util.Datetime;

import junit.framework.TestCase;

import java.util.HashMap;

/**
 * Created by jon on 17.12.2013.
 */
public class DatetimeTestCase extends TestCase {
    /**
     * Functional tests
     */
    public void testSimplePassthrough1() {
        String date = "2013-01-01";
        String time = "12:34:56";
        HashMap<String, String> ret = Datetime.BuildDatetimeFromJson(date, time);
        assertEquals(date, ret.get("date"));
        assertEquals(time, ret.get("time"));
    }

    public void testSimplePassthrough2() {
        String date = "2013-01-01";
        String time = null;
        HashMap<String, String> ret = Datetime.BuildDatetimeFromJson(date, time);
        assertEquals(date, ret.get("date"));
        assertEquals(time, ret.get("time"));
    }

    public void testSimplePassthrough3() {
        String date = null;
        String time = "12:34:56";
        HashMap<String, String> ret = Datetime.BuildDatetimeFromJson(date, time);
        assertEquals(date, ret.get("date"));
        assertEquals(time, ret.get("time"));
    }

    public void testSimplePassthrough4() {
        String date = null;
        String time = null;
        HashMap<String, String> ret = Datetime.BuildDatetimeFromJson(date, time);
        assertEquals(date, ret.get("date"));
        assertEquals(time, ret.get("time"));
    }
}
