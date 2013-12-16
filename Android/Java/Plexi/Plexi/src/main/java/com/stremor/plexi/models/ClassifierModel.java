package com.stremor.plexi.models;

import java.util.HashMap;
import java.util.List;

/**
 * Created by jeffschifano on 10/28/13.
 */
public class ClassifierModel implements Cloneable {
    public ClassifierModel clone() throws CloneNotSupportedException {
        return (ClassifierModel) super.clone();
    }

    public String model;

    public String action;

    public HashMap<String, Object> payload;

    public List<String> project;

    public ErrorModel error;
}
