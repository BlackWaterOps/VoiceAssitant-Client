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
import java.util.Calendar;
import java.util.TimeZone;
import java.util.regex.Pattern;

/**
 * Created by jeffschifano on 10/28/13.
 */
public class Datetime {

    private static DateTimeFormatter DATE_FORMATTER = DateTimeFormat.forPattern("yyyy-MM-dd");
    private static DateTimeFormatter TIME_FORMATTER = DateTimeFormat.forPattern("HH:mm:ss");

    private static Pattern dateRegex = Pattern.compile("\\d{4}-\\d{2}-\\d{2}",
            Pattern.CASE_INSENSITIVE);
    private static Pattern timeRegex = Pattern.compile("\\d{2}:\\d{2}:\\d{2}",
            Pattern.CASE_INSENSITIVE);

    public static int ConvertToUnixTimestamp(Calendar cal) {
        cal.setTimeZone(TimeZone.getTimeZone("UTC"));
        int timestamp = (int) (cal.getTimeInMillis() / 1000);

        return timestamp;
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
            if (timeO.equals("#time_now"))
                time = now.toLocalTime();
            else if (timeRegex.matcher((String) timeO).matches())
                time = TIME_FORMATTER.parseLocalTime((String) timeO);
        } else if (timeO instanceof JSONObject) {
            boolean hasDate = date != null;
            LocalDate baseDate = hasDate ? date : now.toLocalDate();

            LocalDateTime ret = null;
            try {
                ret = parseTimeObject((JSONObject) timeO, baseDate, now.toLocalTime());
            } catch (JSONException e) { /* pass */ }

            if ( ret != null ) {
                time = ret.toLocalTime();
                if ( hasDate )
                    date = ret.toLocalDate();
            }
        }

        String dateString = date == null ? null : DATE_FORMATTER.print(date);
        String timeString = time == null ? null : TIME_FORMATTER.print(time);

        return new Pair<String, String>(dateString, timeString);
    }

    private static LocalDate parseDateObject(JSONObject dateO, LocalDate now) throws JSONException {
        // There shouldn't be more than one property in this object
        if (dateO.length() != 1)
            return null;

        if (dateO.has("#date_weekday"))
            return parseDateWeekdayObject(dateO, now);
        else if (dateO.has("#date_add"))
            return parseDateAddObject(dateO, now);

        return null;
    }

    private static LocalDate parseDateWeekdayObject(JSONObject object, LocalDate now) throws JSONException {
        // Joda Time uses ISO 8601 standard
        int dayNum = object.getInt("#date_weekday");
        dayNum = dayNum == 0 ? 7 : dayNum;
        return now.withDayOfWeek(dayNum);
    }

    private static LocalDate parseDateAddObject(JSONObject object, LocalDate now) throws JSONException {
        JSONArray operands = object.getJSONArray("#date_add");

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

    private static LocalDateTime parseTimeObject(JSONObject timeO, LocalDate baseDate,
                                                 LocalTime now) throws JSONException {
        // There shouldn't be more than one property in this object
        if (timeO.length() != 1)
            return null;

        if (timeO.has("#time_add"))
            return parseTimeAddObject(timeO, baseDate, now);
        else if (timeO.has("#fuzzy_time"))
            return parseTimeFuzzyObject(timeO, baseDate, now);
        else
            return null;
    }

    private static LocalDateTime parseTimeAddObject(JSONObject object, LocalDate baseDate,
                                                    LocalTime now) throws JSONException {
        JSONArray operands = object.getJSONArray("#time_add");

        // Base for addition
        Object base = operands.get(0);
        LocalDateTime baseDateTime;

        if (base instanceof String && base.equals("#time_now"))
            baseDateTime = baseDate.toLocalDateTime(now);
        else if (base instanceof JSONObject && ((JSONObject) base).has("#fuzzy_time"))
            baseDateTime = parseTimeObject((JSONObject) base, baseDate, now);
        else
            return null;

        return baseDateTime.plusSeconds(operands.getInt(1));
    }

    private static LocalDateTime parseTimeFuzzyObject(JSONObject object, LocalDate baseDate,
                                                      LocalTime now) throws JSONException {
        String label = object.getString("label");
        LocalTime defaultTime = TIME_FORMATTER.parseLocalTime(object.getString("default"));

        LocalTime time = getFuzzyTimeValue(label);
        time = time == null ? defaultTime : time;
        return baseDate.toLocalDateTime(time);
    }

    private static LocalTime getFuzzyTimeValue(String label) {
        // TODO
        return null;
    }
}
