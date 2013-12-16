package com.stremor.plexi.models;

import org.json.JSONArray;

import java.util.HashMap;

/**
 * Created by jeffschifano on 10/29/13.
 */
public class DisambiguatorModel {
    public Object payload;

    public String type;

    public JSONArray candidates;

    public HashMap<String, Object> device_info;
}
