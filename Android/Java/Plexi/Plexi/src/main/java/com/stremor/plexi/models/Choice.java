package com.stremor.plexi.models;

import com.google.gson.JsonElement;

/**
 * Created by jeffschifano on 10/28/13.
 */
public class Choice {
    private String text;
    private JsonElement data;

    public Choice(String text, JsonElement data) {
        this.text = text;
        this.data = data;
    }

    public String getText() {
        return text;
    }

    public JsonElement getData() {
        return data;
    }
}
