package com.stremor.plexi.models;

import com.google.gson.JsonObject;
import com.google.gson.JsonParser;

/**
 * Describes content to be displayed to the user.
 */
public class ShowModel implements Cloneable {
    private ShowSimpleModel simple;
    private JsonObject structured;

    public ShowModel(ShowSimpleModel simple, JsonObject structured) {
        this.simple = simple;
        this.structured = structured;
    }

    public ShowModel(JsonObject simple, JsonObject structured) {
        this.simple = new ShowSimpleModel(simple.get("text").getAsString(),
                simple.has("link") ? simple.get("link").getAsString() : null);
        this.structured = structured;
    }

    private static JsonParser _jsonParser = new JsonParser();
    public ShowModel clone() {
        ShowModel clone = null;
        try {
            clone = (ShowModel) super.clone();
        } catch (CloneNotSupportedException e) { /* pass */ }
        clone.simple = simple.clone();

        clone.structured = structured == null ? null
                : _jsonParser.parse(structured.toString()).getAsJsonObject();

        return clone;
    }

    public ShowSimpleModel getSimple() {
        return simple;
    }

    public JsonObject getStructured() {
        return structured;
    }
}
