package com.stremor.plexi.client.models;

public class ConversationItem {
    private String text;

    public ConversationItem(String text) {
        this.text = text;
    }

    public String getText() {
        return text;
    }

    public void setText(String text) {
        this.text = text;
    }
}