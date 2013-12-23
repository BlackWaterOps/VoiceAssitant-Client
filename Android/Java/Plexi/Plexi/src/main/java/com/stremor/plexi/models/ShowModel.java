package com.stremor.plexi.models;

import com.google.gson.JsonObject;

/**
 * Created by jeffschifano on 10/29/13.
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
