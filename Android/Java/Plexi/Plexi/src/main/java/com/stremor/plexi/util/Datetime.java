package com.stremor.plexi.util;

import android.util.Pair;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.Iterator;
import java.util.TimeZone;
import java.util.regex.Pattern;

/**
 * Created by jeffschifano on 10/28/13.
 */
public class Datetime {

    private static SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd");
    private static SimpleDateFormat timeFormat = new SimpleDateFormat("HH:mm:ss");
    private static SimpleDateFormat dateTimeFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
    private static Pattern dateRegex = Pattern.compile("\\d{4}-\\d{2}-\\d{2}",
            Pattern.CASE_INSENSITIVE);
    private static Pattern timeRegex = Pattern.compile("\\d{2}:\\d{2}:\\d{2}",
            Pattern.CASE_INSENSITIVE);

    public static int ConvertToUnixTimestamp(Calendar cal) {
        cal.setTimeZone(TimeZone.getTimeZone("UTC"));
        int timestamp = (int) (cal.getTimeInMillis() / 1000);

        return timestamp;
    }

    public static Pair<String, String> BuildDatetimeFromJson() throws ParseException {
        return BuildDatetimeFromJson(null, null, new Date());
    }

    public static Pair<String, String> BuildDatetimeFromJson(Object dateO, Object timeO)
        throws ParseException {
        return BuildDatetimeFromJson(dateO, timeO, new Date());
    }

    public static Pair<String, String> BuildDatetimeFromJson(Object dateO, Object timeO,
                                                                Date now) throws ParseException {
        String dateRet = null;
        String timeRet = null;

        Date date = null;
        if (dateO instanceof String) {
            if (dateO.equals("now")) {
                date = now;
            } else if (dateRegex.matcher((String) dateO).matches()) {
                date = dateFormat.parse((String) dateO);
            }

            dateRet = dateFormat.format(date);

//            if (date instanceof JSONObject) {
//                cal = Datetime.BuildDatetimeHelper((JSONObject) date, null);
//            }
        }

        if (timeO instanceof String) {
            if (timeO.equals("now")) {
                timeRet = timeFormat.format(now);
            } else if ( timeRegex.matcher((String) timeO).matches() ) {
                timeRet = (String) timeO;
            }

//            if (time instanceof JSONObject) {
//                cal = Datetime.BuildDatetimeHelper((JSONObject)time, cal);

                  // TODO: put both date and time
//            }
        }

        return new Pair<String, String>(dateRet, timeRet);
    }

    private static Object GetPreference(String Name) {
        return null;
    }

    private static Calendar WeekdayHelper(int dayOfWeek) {
        Calendar cal = Calendar.getInstance();

        // dayOfWeek value is 0 indexed so add 1
        cal.set(Calendar.DAY_OF_WEEK, (dayOfWeek + 1));

        return cal;
    }

    private static Calendar FuzzyHelper(JSONObject datetime, boolean isDate) {
        try {
            String label = null;
            String def = null;

            Iterator<String> keys = datetime.keys();

            while (keys.hasNext()) {
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

    private static Date parseDateObject(String dateString) {
        if (dateString.equals("now"))
            return new Date();

        try {
            Date d = dateFormat.parse(dateString);
            return d;
        } catch (ParseException e) {
            return null;
        }
    }

    private static Date parseTimeObject(String timeString, Date basis) {
        if (timeString.equals("now"))
            return new Date();

        if (basis != null) {
            timeString = dateFormat.format(basis) + "T" + timeString;
        }

        try {
            Date d = dateTimeFormat.parse(timeString);
            return d;
        } catch (ParseException e) {
            return null;
        }
    }

    private static Calendar BuildDatetimeHelper(String dateortime, Calendar newDate) {
        SimpleDateFormat dateFormatter = new SimpleDateFormat("yyyy-MM-dd");
        SimpleDateFormat timeFormatter = new SimpleDateFormat("HH:mm:ss");

        Date d;

        if (newDate == null) {
            newDate = Calendar.getInstance();

            if (dateortime.equals("now")) {
                newDate.setTime(new Date());
                return newDate;
            }

            try {
                d = dateFormatter.parse(dateortime);
                newDate.setTime(d);
                return newDate;
            } catch (ParseException e) { /* pass */ }

            try {
                d = timeFormatter.parse(dateortime);
                newDate.setTime(d);
                return newDate;
            } catch (ParseException e) { /* pass */ }
        } else {
            if (timeRegex.matcher(dateortime).matches()) {
                try {
                    d = timeFormatter.parse(dateortime);
                } catch (ParseException e) {
                    return null;
                }

                Calendar cal = Calendar.getInstance();
                cal.setTime(d);

                if (newDate.after(cal)) {
                    newDate.add(Calendar.DATE, 1);
                }
            }
        }

        return newDate;
    }

    private static Calendar BuildDatetimeHelper(JSONObject dateortime, Calendar newDate) {
        try {
            SimpleDateFormat formatter = new SimpleDateFormat();
            Date d;

            Iterator<String> keys = dateortime.keys();

            while (keys.hasNext()) {
                String key = keys.next();

                if (key.contains("weekday")) {

                } else if (key.contains("fuzzy")) {

                } else {
                    if (dateortime.get(key) instanceof JSONArray) {
                        System.out.print("step 1");

                        JSONArray val = (JSONArray) dateortime.get(key);

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
