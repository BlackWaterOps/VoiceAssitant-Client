package com.stremor.plexi.util;

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParser;
import com.google.gson.JsonPrimitive;

import java.util.Arrays;
import java.util.List;

/**
 * A class which provides helper methods for operating on deeply nested JSON objects.
 *
 * Created by jon on 19.12.2013.
 */
public class JsonObjectUtil {
    private static JsonParser parser = new JsonParser();

    public static JsonObject deepCopy(JsonObject obj) {
        // TODO: performance
        return parser.parse(obj.toString()).getAsJsonObject();
    }

    /**
     * Retrieve a property's value held at some address within an object.
     * @param obj
     * @param address
     * @return The property value at the specified address, or null if no such property exists in
     *   the object.
     */
    public static JsonElement find(JsonObject obj, String address) {
        List<String> fields = Arrays.asList(address.split("\\."));
        String last = fields.get(fields.size() - 1);
        fields = fields.subList(0, fields.size() - 1);

        JsonObject current = obj;
        for ( String field : fields ) {
            JsonObject next;
            try {
                next = current.getAsJsonObject(field);
            } catch (ClassCastException e) {
                return null;
            } catch (IllegalArgumentException e) {
                return null;
            }

            if ( next == null )
                return null;

            current = next;
        }

        return current.get(last);
    }

    /**
     * See {@link #replace(com.google.gson.JsonObject, String, com.google.gson.JsonElement)}.
     */
    public static void replace(JsonObject obj, String address, boolean value) {
        replace(obj, address, new JsonPrimitive(value));
    }

    /**
     * See {@link #replace(com.google.gson.JsonObject, String, com.google.gson.JsonElement)}.
     */
    public static void replace(JsonObject obj, String address, char value) {
        replace(obj, address, new JsonPrimitive(value));
    }

    /**
     * See {@link #replace(com.google.gson.JsonObject, String, com.google.gson.JsonElement)}.
     */
    public static void replace(JsonObject obj, String address, Number value) {
        replace(obj, address, new JsonPrimitive(value));
    }

    /**
     * See {@link #replace(com.google.gson.JsonObject, String, com.google.gson.JsonElement)}.
     */
    public static void replace(JsonObject obj, String address, String value) {
        replace(obj, address, new JsonPrimitive(value));
    }

    /**
     * Replace a property's value held at some address within an object. Writes to the input object
     * `obj`.
     *
     * @param obj
     * @param address The period-separated address of the field to be modified.
     * @param data    The new value for the field.
     * @exception java.lang.IllegalArgumentException If the address provided does not map to an
     *     actual property in `obj`.
     */
    public static void replace(JsonObject obj, String address, JsonElement data) {
        List<String> fields = Arrays.asList(address.split("\\."));
        String last = fields.get(fields.size() - 1);
        fields = fields.subList(0, fields.size() - 1);

        JsonObject current = obj;
        for (String field : fields) {
            JsonObject next;
            try {
                next = current.getAsJsonObject(field);
            } catch (ClassCastException e) {
                throw new IllegalArgumentException("Invalid address");
            } catch (IllegalArgumentException e) {
                throw new IllegalArgumentException("Invalid address");
            }

            if ( next == null )
                throw new IllegalArgumentException("Invalid address");

            current = next;
        }

        if (!current.has(last))
            throw new IllegalArgumentException("Invalid address");

        current.add(last, data);
    }
}
