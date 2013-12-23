package com.stremor.plexi.models;

import com.google.gson.JsonArray;

import java.util.HashMap;

/**
 * Created by jeffschifano on 10/29/13.
 */
public class DisambiguatorModel {
    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;

        DisambiguatorModel that = (DisambiguatorModel) o;

        if (candidates != null ? !candidates.equals(that.candidates) : that.candidates != null)
            return false;
        if (device_info != null ? !device_info.equals(that.device_info) : that.device_info != null)
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
        result = 31 * result + (device_info != null ? device_info.hashCode() : 0);
        return result;
    }

    public Object payload;

    public String type;

    public JsonArray candidates;

    public HashMap<String, Object> device_info;
}
