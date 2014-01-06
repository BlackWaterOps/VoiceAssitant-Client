package com.stremor.plexi.models;

import java.util.Arrays;

/**
 * Created by jon on 27.12.2013.
 */
public class ShowSimpleModel implements Cloneable {
    private String text;
    private String link;
    private Choice[] list;

    public ShowSimpleModel(String text) {
        this(text, null, null);
    }

    public ShowSimpleModel(String text, String link) {
        this(text, link, null);
    }

    public ShowSimpleModel(String text, Choice[] list) {
        this(text, null, list);
    }

    public ShowSimpleModel(String text, String link, Choice[] list) {
        this.text = text;
        this.link = link;
        this.list = list;
    }

    public ShowSimpleModel clone() {
        ShowSimpleModel clone = null;
        try {
            clone = (ShowSimpleModel) super.clone();
        } catch (CloneNotSupportedException e) { /* pass */ }

        // Safe to do a shallow copy -- Choice objects are immutable POJOs
        clone.list = list == null ? null : Arrays.copyOf(list, list.length);

        return clone;
    }

    public String getText() {
        return text;
    }

    public String getLink() {
        return link;
    }

    public Choice[] getList() {
        return list;
    }
}
