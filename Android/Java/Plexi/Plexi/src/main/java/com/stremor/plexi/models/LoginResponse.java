package com.stremor.plexi.models;

/**
 * Created by jon on 31.12.2013.
 */
public class LoginResponse {
    private String username;
    private String token;
    private ErrorModel error;

    public LoginResponse(String username, String token) {
        this.username = username;
        this.token = token;
    }

    public LoginResponse(ErrorModel error) {
        this.error = error;
    }

    public String getUsername() {
        return username;
    }

    public String getToken() {
        return token;
    }

    public ErrorModel getError() {
        return error;
    }
}
