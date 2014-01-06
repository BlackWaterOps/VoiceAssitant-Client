package com.stremor.plexi.models;

/**
 * Created by jon on 31.12.2013.
 */
public class SignupResponse {
    private String msg;
    private ErrorModel error;

    public SignupResponse(String msg) {
        this.msg = msg;
    }

    public SignupResponse(ErrorModel error) {
        this.error = error;
    }

    public String getMessage() {
        return msg;
    }

    public ErrorModel getError() {
        return error;
    }
}
