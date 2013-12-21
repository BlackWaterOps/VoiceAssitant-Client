package com.stremor.plexi.util.test;

import com.google.gson.JsonObject;
import com.google.gson.JsonParser;
import com.stremor.plexi.util.JsonObjectUtil;

import junit.framework.TestCase;

/**
 * Created by jon on 17.12.2013.
 */
public class JsonObjectUtilTestCase extends TestCase {
    private JsonParser parser = new JsonParser();

    private JsonObject _(String s) {
        return parser.parse(s).getAsJsonObject();
    }

    public void testReplace() {
        JsonObject obj = _("{\"a\":{\"b\":{\"c\":5}}}");
        JsonObjectUtil.replace(obj, "a.b.c", 15);
        assertEquals(15, obj.getAsJsonObject("a").getAsJsonObject("b").get("c").getAsInt());
    }

    public void testFailedReplace() {
        JsonObject obj = _("{\"a\":{\"b\":{\"c\":5}}}");

        try {
            JsonObjectUtil.replace(obj, "a.b.c.d", 15);
            fail("Expected IllegalArgumentException");
        } catch (IllegalArgumentException e) { }
    }

    public void testFailedReplace2() {
        JsonObject obj = _("{\"a\":{\"b\":{\"c\":5}}}");

        try {
            JsonObjectUtil.replace(obj, "a.b.d", 15);
            fail("Expected IllegalArgumentException");
        } catch (IllegalArgumentException e) { }
    }

    public void testFind() {
        JsonObject obj = _("{\"a\":{\"b\":{\"c\":5}}}");
        assertEquals(obj.getAsJsonObject("a"), JsonObjectUtil.find(obj, "a"));
    }

    public void testFind2() {
        JsonObject obj = _("{\"a\":{\"b\":{\"c\":5}}}");
        assertEquals(obj.getAsJsonObject("a").getAsJsonObject("b"),
                JsonObjectUtil.find(obj, "a.b"));
    }

    public void testFind3() {
        JsonObject obj = _("{\"a\":{\"b\":{\"c\":5}}}");
        assertEquals(5, JsonObjectUtil.find(obj, "a.b.c").getAsInt());
    }

    public void testFailedFind() {
        JsonObject obj = _("{\"a\":{\"b\":{\"c\":5}}}");
        assertNull(JsonObjectUtil.find(obj, "b"));
    }

    public void testFailedFind2() {
        JsonObject obj = _("{\"a\":{\"b\":{\"c\":5}}}");
        assertNull(JsonObjectUtil.find(obj, "a.c"));
    }

    public void testFailedFind3() {
        JsonObject obj = _("{\"a\":{\"b\":{\"c\":5}}}");
        assertNull(JsonObjectUtil.find(obj, "a.b.c.d"));
    }
}
