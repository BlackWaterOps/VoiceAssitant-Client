package com.stremor.plexi.util;

import android.util.Pair;

import org.joda.time.LocalDate;
import org.joda.time.LocalDateTime;
import org.joda.time.LocalTime;
import org.joda.time.format.DateTimeFormat;
import org.joda.time.format.DateTimeFormatter;
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

    private static DateTimeFormatter DATE_FORMATTER = DateTimeFormat.forPattern("yyyy-MM-dd");
    private static DateTimeFormatter TIME_FORMATTER = DateTimeFormat.forPattern("HH:mm:ss");

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

    public static Pair<String, String> datetimeFromJson() throws ParseException {
        return datetimeFromJson(null, null, LocalDateTime.now());
    }

    public static Pair<String, String> datetimeFromJson(Object dateO, Object timeO)
            throws ParseException {
        return datetimeFromJson(dateO, timeO, LocalDateTime.now());
    }

    public static Pair<String, String> datetimeFromJson(Object dateO, Object timeO,
                                                        LocalDateTime now) throws ParseException {
        LocalDate date = null;
        LocalTime time = null;

        if (dateO instanceof String) {
            if (dateO.equals("#date_now"))
                date = now.toLocalDate();
            else if (dateRegex.matcher((String) dateO).matches())
                date = DATE_FORMATTER.parseLocalDate((String) dateO);

//            if (date instanceof JSONObject) {
//                cal = Datetime.BuildDatetimeHelper((JSONObject) date, null);
//            }
        } else if (dateO instanceof JSONObject) {
            date = parseDateObject((JSONObject) dateO);
        }

        if (timeO instanceof String) {
            if (timeO.equals("#time_now")) {
                time = now.toLocalTime();
            } else if (timeRegex.matcher((String) timeO).matches()) {
                time = TIME_FORMATTER.parseLocalTime((String) timeO);
            }

//            if (time instanceof JSONObject) {
//                cal = Datetime.BuildDatetimeHelper((JSONObject)time, cal);

            // TODO: put both date and time
//            }
        }

        String dateString = date == null ? null : DATE_FORMATTER.print(date);
        String timeString = time == null ? null : TIME_FORMATTER.print(time);

        return new Pair<String, String>(dateString, timeString);
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

    private static LocalDate parseDateObject(JSONObject dateO) {
        if ( dateO.has("#date_weekday") ) {

        } else if ( dateO.has("#date_fuzzy") ) {

        } else if ( dateO.has("#date_add") ) {

        }

        return null;
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
