package com.stremor.plexi.models;

import com.google.gson.JsonObject;
import com.google.gson.JsonParser;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by jeffschifano on 10/28/13.
 */
public class ClassifierModel implements Cloneable {
    public ClassifierModel(String model, String action, JsonObject payload) {
        this.model = model;
        this.action = action;
        this.payload = payload;
    }

    public ClassifierModel(String model, String action, JsonObject payload, String[] project,
                           ErrorModel error) {
        this.model = model;
        this.action = action;
        this.payload = payload;
        this.project = project;
        this.error = error;
    }

    public String getModel() {
        return model;
    }

    public String getAction() {
        return action;
    }

    public JsonObject getPayload() {
        return payload;
    }

    public String[] getProject() {
        return project;
    }

    public ErrorModel getError() {
        return error;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;

        ClassifierModel that = (ClassifierModel) o;

        if (!action.equals(that.action)) return false;
        if (error != null ? !error.equals(that.error) : that.error != null) return false;
        if (!model.equals(that.model)) return false;
        if (!payload.equals(that.payload)) return false;
        if (project != null ? !project.equals(that.project) : that.project != null) return false;

        return true;
    }

    @Override
    public int hashCode() {
        int result = model.hashCode();
        result = 31 * result + action.hashCode();
        result = 31 * result + payload.hashCode();
        result = 31 * result + (project != null ? project.hashCode() : 0);
        result = 31 * result + (error != null ? error.hashCode() : 0);
        return result;
    }

    public ClassifierModel clone() throws CloneNotSupportedException {
        ClassifierModel result = (ClassifierModel) super.clone();
        result.payload = new JsonParser().parse(payload.toString()).getAsJsonObject();
        result.project = project == null ? null : new ArrayList<String>(project);
        result.error = error == null ? null : error.clone();

        return result;
    }

    private String model;
    private String action;
    private JsonObject payload;
    private String[] project;
    private ErrorModel error;
}
