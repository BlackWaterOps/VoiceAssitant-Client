package com.stremor.plexi.models;

/**
 * Created by jeffschifano on 11/1/13.
 */
public class ErrorModel {
    public ErrorModel clone() {
        try {
            return (ErrorModel) super.clone();
        } catch (CloneNotSupportedException e) { return null; }
    }

    public String msg;
    public String message;
    public int code;
}
