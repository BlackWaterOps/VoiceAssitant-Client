package com.stremor.plexi.models;

/**
 * Created by jeffschifano on 10/28/13.
 */
public class ResponderModel {
    private String status;
    private String type;
    private String field;
    private ShowModel show;
    private String speak;
    private String followup;
    private String actor;
    private ClassifierModel data;
    private ErrorModel error;

    public ResponderModel(String status) {
        this.setStatus(status);
    }

    // Passive disambiguation indicator response
    public ResponderModel(String status, String type, String field) {
        this.setStatus(status);
        this.setType(type);
        this.setField(field);
    }

    // Active disambiguation indicator response
    public ResponderModel(String status, String type, String field, ShowModel show, String speak) {
        this.setStatus(status);
        this.setType(type);
        this.setField(field);
        this.setShow(show);
        this.setSpeak(speak);
    }

    // Actor response
    public ResponderModel(String status, String actor, String followup, ClassifierModel data) {
        this.setStatus(status);
        this.setActor(actor);
        this.setFollowup(followup);
        this.setData(data);
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public String getField() {
        return field;
    }

    public void setField(String field) {
        this.field = field;
    }

    public ShowModel getShow() {
        return show;
    }

    public void setShow(ShowModel show) {
        this.show = show;
    }

    public String getSpeak() {
        return speak;
    }

    public void setSpeak(String speak) {
        this.speak = speak;
    }

    public String getFollowup() {
        return followup;
    }

    public void setFollowup(String followup) {
        this.followup = followup;
    }

    public String getActor() {
        return actor;
    }

    public void setActor(String actor) {
        this.actor = actor;
    }

    public ClassifierModel getData() {
        return data;
    }

    public void setData(ClassifierModel data) {
        this.data = data;
    }

    public ErrorModel getError() {
        return error;
    }

    public void setError(ErrorModel error) {
        this.error = error;
    }
}
