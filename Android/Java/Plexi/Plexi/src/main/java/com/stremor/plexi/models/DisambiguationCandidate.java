package com.stremor.plexi.models;

import com.google.gson.JsonElement;

/**
 * Created by jon on 27.12.2013.
 */
public class DisambiguationCandidate {
    private String text;
    private JsonElement data;

    public DisambiguationCandidate(String text, JsonElement data) {
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
