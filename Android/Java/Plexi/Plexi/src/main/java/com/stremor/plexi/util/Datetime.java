package com.stremor.plexi.util;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.*;
import java.util.regex.*;

/**
 * Created by jeffschifano on 10/28/13.
 */
public class Datetime {

    private static String dateRegex = "/\\d{2,4}[-]\\d{2}[-]\\d{2}/i";
    private static String timeRegex = "/\\d{1,2}[:]\\d{2}[:]\\d{2}/i";

    public static Calendar ConvertFromUnixTimestamp(int timestamp)
    {
        long l = timestamp * 1000;

        Calendar c = Calendar.getInstance(TimeZone.getTimeZone("UTC"));

        c.setTimeInMillis(l);

        c.setTimeZone(TimeZone.getDefault());

        return c;
    }

    public static int ConvertToUnixTimestamp(Calendar cal)
    {
        cal.setTimeZone(TimeZone.getTimeZone("UTC"));

        int timestamp = (int)(cal.getTimeInMillis() / 1000);

        return timestamp;
    }

    public static HashMap<String, String> BuildDatetimeFromJson()
    {
        return BuildDatetimeFromJson(null, null);
    }

    public static HashMap<String, String> BuildDatetimeFromJson(Object date, Object time)
    {
        Calendar cal = null;

        if (date != null)
        {
            if (date instanceof String && Pattern.matches(dateRegex, (String)date)) {
                cal = Datetime.BuildDatetimeHelper((String)date, null);
            }

            if (date instanceof JSONObject) {
                cal = Datetime.BuildDatetimeHelper((JSONObject)date, null);
            }
        }

        if (time != null)
        {
            if (time instanceof String && Pattern.matches(timeRegex, (String)time)) {
                cal = Datetime.BuildDatetimeHelper((String)time, cal);
            }

            if (time instanceof JSONObject) {
                cal = Datetime.BuildDatetimeHelper((JSONObject)time, cal);
            }
        }

        SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd");
        SimpleDateFormat timeFormat = new SimpleDateFormat("H:mm:ss");

        Date d = cal.getTime();

        HashMap<String, String> newDate = new HashMap<String, String>();

        newDate.put("date", dateFormat.format(d));
        newDate.put("time", timeFormat.format(d));

        return newDate;
    }

    private static Object GetPreference(String Name)
    {
        return null;
    }

    private static Calendar WeekdayHelper(int dayOfWeek)
    {
        Calendar cal = Calendar.getInstance();

        // dayOfWeek value is 0 indexed so add 1
        cal.set(Calendar.DAY_OF_WEEK, (dayOfWeek + 1));

        return cal;
    }

    private static Calendar FuzzyHelper(JSONObject datetime, boolean isDate)
    {
        try {
            String label = null;
            String def = null;

            Iterator<String> keys = datetime.keys();

            while(keys.hasNext()) {
                String key = keys.next();

                if (key == "label") {
                    label = datetime.getString(key);
                }

                if (key == "default") {
                    def = datetime.getString(key);
                }
            }

            Object preference = GetPreference(label);

            String pref = (preference == null) ? def : "";

            SimpleDateFormat formatter = new SimpleDateFormat();

            Date d = formatter.parse(pref);

            Calendar cal = Calendar.getInstance();

            cal.setTime(d);

            return cal;
        } catch (JSONException jsonError) {
            System.out.print(jsonError.getMessage());
            return null;
        } catch (ParseException parseError) {
            System.out.print(parseError.getMessage());
            return null;
        }
    }

    private static Calendar BuildDatetimeHelper(String dateortime, Calendar newDate)
    {
        try {
            SimpleDateFormat formatter = new SimpleDateFormat();
            Date d;
            Calendar cal;

            if (newDate == null) {
                cal = Calendar.getInstance();
                d = (dateortime.contains("now")) ? new Date() : formatter.parse(dateortime);
                cal.setTime(d);
            } else {
                if (Pattern.matches(timeRegex, dateortime)) {
                    d = formatter.parse(dateortime);
                    cal = Calendar.getInstance();
                    cal.setTime(d);

                    if (newDate.after(cal)) {
                        newDate.add(Calendar.DATE, 1);
                    }
                }
            }

            return newDate;
        } catch (ParseException parseError) {
            System.out.print(parseError.getMessage());
            return null;
        }
    }

    private static Calendar BuildDatetimeHelper(JSONObject dateortime, Calendar newDate)
    {
        try {
            SimpleDateFormat formatter = new SimpleDateFormat();
            Date d;

            Iterator<String> keys = dateortime.keys();

            while(keys.hasNext()) {
                String key = keys.next();

                if (key.contains("weekday")) {

                } else if (key.contains("fuzzy")) {

                } else {
                    if (dateortime.get(key) instanceof JSONArray) {
                        System.out.print("step 1");

                        JSONArray val = (JSONArray)dateortime.get(key);

                        for (int i = 0; i < val.length(); i++) {
                            System.out.print("step 2");

                            Object item = val.get(i);

                            if (newDate == null) {
                                if (item instanceof String) {
                                    System.out.print("item is a string");

                                    d = (((String) item).contains("now")) ? new Date() : formatter.parse((String) item);

                                    newDate = Calendar.getInstance();
                                    newDate.setTime(d);
                                }

                                if (item instanceof JSONObject) {
                                    System.out.print("item is an object");

                                    Iterator<String> objKeys = ((JSONObject) item).keys();

                                    while (objKeys.hasNext()) {
                                        String objKey = objKeys.next();

                                        if (newDate == null) {
                                            if (objKey.contains("weekday")) {
                                                System.out.print("weekday conversion");

                                                newDate = WeekdayHelper((Integer) ((JSONObject) item).get(objKey));
                                            } else if (objKey.contains("fuzzy")) {
                                                System.out.print("fuzzy conversion");

                                                boolean isDate = objKey.contains("date") ? true : false;

                                                newDate = FuzzyHelper((JSONObject) item, isDate);
                                            }
                                        }
                                    }
                                }
                            } else if (item instanceof Integer) {
                                System.out.print("parse int value");

                                int interval = (Integer) item;

                                newDate = Calendar.getInstance();

                                if (key.contains("time")) {
                                    newDate.add(Calendar.SECOND, interval);
                                } else if (key.contains("date")) {
                                    newDate.add(Calendar.DATE, interval);
                                }
                            }
                        }
                    }
                }
            }

            return newDate;
        } catch (JSONException JSONError) {
            System.out.print(JSONError.getMessage());
            return null;
        } catch (ParseException ParseError) {
            System.out.print(ParseError.getMessage());
            return null;
        } catch (ClassCastException CastError) {
            System.out.print(CastError.getMessage());
            return null;
        }
    }
}
