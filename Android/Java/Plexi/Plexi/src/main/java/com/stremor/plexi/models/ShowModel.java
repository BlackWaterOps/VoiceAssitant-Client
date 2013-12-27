package com.stremor.plexi.models;

import com.google.gson.JsonObject;

/**
 * Describes content to be displayed to the user.
 *
 * TODO Make more strongly typed (don't throw JSON objects through a public interface!)
 */
public class ShowModel {
    private JsonObject simple;
    private JsonObject structured;

    public ShowModel(JsonObject simple, JsonObject structured) {
        this.simple = simple;
        this.structured = structured;
    }

    public JsonObject getSimple() {
        return simple;
    }

    public JsonObject getStructured() {
        return structured;
    }
}
