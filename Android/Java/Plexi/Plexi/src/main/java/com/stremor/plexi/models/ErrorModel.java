package com.stremor.plexi.models;

/**
 * Created by jeffschifano on 11/1/13.
 */
public class ErrorModel implements Cloneable {
    public ErrorModel(String message, int code) {
        this.message = message;
        this.code = code;
    }

    public ErrorModel clone() {
        try {
            return (ErrorModel) super.clone();
        } catch (CloneNotSupportedException e) { return null; }
    }

    public String getMessage() {
        return message;
    }

    public int getCode() {
        return code;
    }

    private String message;
    private int code;
}
