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
        } else if (dateO instanceof JSONObject) {
            try {
                date = parseDateObject((JSONObject) dateO, now.toLocalDate());
            } catch (JSONException e) { /* pass */ }
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

//    private static Object GetPreference(String Name) {
//        return null;
//    }
//
//    private static Calendar FuzzyHelper(JSONObject datetime, boolean isDate) {
//        try {
//            String label = null;
//            String def = null;
//
//            Iterator<String> keys = datetime.keys();
//
//            while (keys.hasNext()) {
//                String key = keys.next();
//
//                if (key == "label") {
//                    label = datetime.getString(key);
//                }
//
//                if (key == "default") {
//                    def = datetime.getString(key);
//                }
//            }
//
//            Object preference = GetPreference(label);
//
//            String pref = (preference == null) ? def : "";
//
//            SimpleDateFormat formatter = new SimpleDateFormat();
//
//            Date d = formatter.parse(pref);
//
//            Calendar cal = Calendar.getInstance();
//
//            cal.setTime(d);
//
//            return cal;
//        } catch (JSONException jsonError) {
//            System.out.print(jsonError.getMessage());
//            return null;
//        } catch (ParseException parseError) {
//            System.out.print(parseError.getMessage());
//            return null;
//        }
//    }

    private static LocalDate parseDateObject(JSONObject dateO, LocalDate now) throws JSONException {
        // There shouldn't be more than one property in this object
        if (dateO.length() != 1)
            return null;

        if (dateO.has("#date_weekday")) {
            // Joda Time uses ISO 8601 standard
            int dayNum = dateO.getInt("#date_weekday");
            dayNum = dayNum == 0 ? 7 : dayNum;
            return now.withDayOfWeek(dayNum);
        } else if (dateO.has("#date_add")) {
            JSONArray operands = dateO.getJSONArray("#date_add");

            Object base = operands.get(0);
            LocalDate baseDate;
            if (base instanceof String && base.equals("#date_now"))
                baseDate = now;
            else if (base instanceof JSONObject && ((JSONObject) base).has("#date_weekday"))
                baseDate = parseDateObject((JSONObject) base, now);
            else
                return null;

            return baseDate.plusDays(operands.getInt(1));
        }

        return null;
    }
}
