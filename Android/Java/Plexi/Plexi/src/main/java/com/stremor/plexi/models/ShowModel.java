package com.stremor.plexi.models;

import com.google.gson.JsonObject;

/**
 * Describes content to be displayed to the user.
 *
 * TODO Make more strongly typed (don't throw JSON objects through a public interface!)
 */
public class ShowModel {
    private ShowSimpleModel simple;
    private JsonObject structured;

    public ShowModel(JsonObject simple, JsonObject structured) {
        this.simple = new ShowSimpleModel(simple.get("text").getAsString(),
                simple.has("link") ? simple.get("link").getAsString() : null);
        this.structured = structured;
    }

    public ShowSimpleModel getSimple() {
        return simple;
    }

    public JsonObject getStructured() {
        return structured;
    }
}
