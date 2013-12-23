package com.stremor.plexi.models;

import com.google.gson.JsonArray;

import java.util.HashMap;

/**
 * Created by jeffschifano on 10/29/13.
 */
public class DisambiguatorModel {
    private Object payload;
    private String type;
    private JsonArray candidates;
    private HashMap<String, Object> deviceInfo;

    public DisambiguatorModel(Object payload, String type) {
        this.payload = payload;
        this.type = type;
    }

    public DisambiguatorModel(Object payload, String type, JsonArray candidates) {
        this.payload = payload;
        this.type = type;
        this.candidates = candidates;
    }

    public DisambiguatorModel(Object payload, String type, HashMap<String, Object> device_info) {
        this.payload = payload;
        this.type = type;
        this.deviceInfo = device_info;
    }

    public Object getPayload() {
        return payload;
    }

    public void setPayload(Object payload) {
        this.payload = payload;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public JsonArray getCandidates() {
        return candidates;
    }

    public void setCandidates(JsonArray candidates) {
        this.candidates = candidates;
    }

    public HashMap<String, Object> getDeviceInfo() {
        return deviceInfo;
    }

    public void setDeviceInfo(HashMap<String, Object> device_info) {
        this.deviceInfo = device_info;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;

        DisambiguatorModel that = (DisambiguatorModel) o;

        if (candidates != null ? !candidates.equals(that.candidates) : that.candidates != null)
            return false;
        if (deviceInfo != null ? !deviceInfo.equals(that.deviceInfo) : that.deviceInfo != null)
            return false;
        if (!payload.equals(that.payload)) return false;
        if (!type.equals(that.type)) return false;

        return true;
    }

    @Override
    public int hashCode() {
        int result = payload.hashCode();
        result = 31 * result + type.hashCode();
        result = 31 * result + (candidates != null ? candidates.hashCode() : 0);
        result = 31 * result + (deviceInfo != null ? deviceInfo.hashCode() : 0);
        return result;
    }
}
