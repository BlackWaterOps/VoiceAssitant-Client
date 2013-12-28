package com.stremor.plexi.util;

import android.util.Pair;

import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

import org.joda.time.LocalDate;
import org.joda.time.LocalDateTime;
import org.joda.time.LocalTime;
import org.joda.time.format.DateTimeFormat;
import org.joda.time.format.DateTimeFormatter;

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

    public static Pair<String, String> datetimeFromJson(JsonElement dateO, JsonElement timeO)
            throws ParseException {
        return datetimeFromJson(dateO, timeO, LocalDateTime.now());
    }

    public static Pair<String, String> datetimeFromJson(JsonElement dateO, JsonElement timeO,
                                                        LocalDateTime now) throws ParseException {
        LocalDate date = null;
        LocalTime time = null;

        if (dateO == null) {
            /* pass */
        } else if (dateO.isJsonPrimitive() && dateO.getAsJsonPrimitive().isString()) {
            String dateString = dateO.getAsJsonPrimitive().getAsString();
            if (dateString.equals("#date_now"))
                date = now.toLocalDate();
            else if (dateRegex.matcher(dateString).matches())
                date = DATE_FORMATTER.parseLocalDate(dateString);
        } else if (dateO.isJsonObject()) {
            try {
                date = parseDateObject(dateO.getAsJsonObject(), now.toLocalDate());
            } catch (Exception e) { /* pass */ }
        }

        if (timeO == null) {
            /* pass */
        } else if (timeO.isJsonPrimitive() && timeO.getAsJsonPrimitive().isString()) {
            String timeString = timeO.getAsJsonPrimitive().getAsString();
            if (timeString.equals("#time_now"))
                time = now.toLocalTime();
            else if (timeRegex.matcher(timeString).matches())
                time = TIME_FORMATTER.parseLocalTime(timeString);
        } else if (timeO.isJsonObject()) {
            boolean hasDate = date != null;
            LocalDate baseDate = hasDate ? date : now.toLocalDate();

            LocalDateTime ret = null;
            try {
                ret = parseTimeObject(timeO.getAsJsonObject(), baseDate, now.toLocalTime());
            } catch (Exception e) { /* pass */ }

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

    private static LocalDate parseDateObject(JsonObject dateO, LocalDate now) {
        // There shouldn't be more than one property in this object
        if (dateO.has("#date_weekday"))
            return parseDateWeekdayObject(dateO, now);
        else if (dateO.has("#date_add"))
            return parseDateAddObject(dateO, now);

        return null;
    }

    private static LocalDate parseDateWeekdayObject(JsonObject object, LocalDate now) {
        // Joda Time uses ISO 8601 standard
        int dayNum = object.get("#date_weekday").getAsInt();
        dayNum = dayNum == 0 ? 7 : dayNum;
        return now.withDayOfWeek(dayNum);
    }

    private static LocalDate parseDateAddObject(JsonObject object, LocalDate now) {
        JsonArray operands = object.getAsJsonArray("#date_add");
        JsonElement base = operands.get(0);

        LocalDate baseDate;
        if (base.isJsonObject() && base.getAsJsonObject().has("#date_weekday"))
            baseDate = parseDateObject(base.getAsJsonObject(), now);
        else if (base.isJsonPrimitive() && base.getAsJsonPrimitive().isString() &&
                base.getAsString().equals("#date_now"))
            baseDate = now;
        else
            return null;

        return baseDate.plusDays(operands.get(1).getAsInt());
    }

    private static LocalDateTime parseTimeObject(JsonObject timeO, LocalDate baseDate,
                                                 LocalTime now) {
        if (timeO.has("#time_add"))
            return parseTimeAddObject(timeO, baseDate, now);
        else if (timeO.has("#fuzzy_time"))
            return parseTimeFuzzyObject(timeO, baseDate, now);
        else
            return null;
    }

    private static LocalDateTime parseTimeAddObject(JsonObject object, LocalDate baseDate,
                                                    LocalTime now) {
        JsonArray operands = object.getAsJsonArray("#time_add");

        // Base for addition
        JsonElement base = operands.get(0);
        LocalDateTime baseDateTime;

        if (base.isJsonObject() && base.getAsJsonObject().has("#fuzzy_time"))
            baseDateTime = parseTimeObject(base.getAsJsonObject(), baseDate, now);
        else if (base.isJsonPrimitive() && base.getAsJsonPrimitive().isString()
                && base.getAsString().equals("#time_now"))
            baseDateTime = baseDate.toLocalDateTime(now);
        else
            return null;

        return baseDateTime.plusSeconds(operands.get(1).getAsInt());
    }

    private static LocalDateTime parseTimeFuzzyObject(JsonObject object, LocalDate baseDate,
                                                      LocalTime now) {
        String label = object.get("label").getAsString();
        LocalTime defaultTime = TIME_FORMATTER.parseLocalTime(object.get("default").getAsString());

        LocalTime time = getFuzzyTimeValue(label);
        time = time == null ? defaultTime : time;
        return baseDate.toLocalDateTime(time);
    }

    private static LocalTime getFuzzyTimeValue(String label) {
        // TODO
        return null;
    }
}
