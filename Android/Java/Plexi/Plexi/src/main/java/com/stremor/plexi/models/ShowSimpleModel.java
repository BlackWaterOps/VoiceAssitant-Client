package com.stremor.plexi.models;

/**
 * Created by jon on 27.12.2013.
 */
public class ShowSimpleModel {
    private String text;
    private String link;
    private DisambiguationCandidate[] list;

    public ShowSimpleModel(String text) {
        this(text, null, null);
    }

    public ShowSimpleModel(String text, String link) {
        this(text, link, null);
    }

    public ShowSimpleModel(String text, DisambiguationCandidate[] list) {
        this(text, null, list);
    }

    public ShowSimpleModel(String text, String link, DisambiguationCandidate[] list) {
        this.text = text;
        this.link = link;
        this.list = list;
    }

    public String getText() {
        return text;
    }

    public String getLink() {
        return link;
    }

    public DisambiguationCandidate[] getList() {
        return list;
    }
}
