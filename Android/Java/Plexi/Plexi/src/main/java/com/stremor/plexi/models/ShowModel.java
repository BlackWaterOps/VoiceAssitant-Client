package com.stremor.plexi.models;

import com.google.gson.JsonObject;

/**
 * Describes content to be displayed to the user.
 */
public class ShowModel {
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

    public ShowSimpleModel getSimple() {
        return simple;
    }

    public JsonObject getStructured() {
        return structured;
    }
}
