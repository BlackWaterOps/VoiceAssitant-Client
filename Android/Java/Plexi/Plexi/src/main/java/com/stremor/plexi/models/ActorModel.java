package com.stremor.plexi.models;

/**
 * Created by jeffschifano on 10/29/13.
 */
public class ActorModel implements Cloneable {
    private String speak;
    private ShowModel show;
    private ErrorModel error;

    public ActorModel(String speak, ShowModel show) {
        this.speak = speak;
        this.show = show;
    }

    public ActorModel(ShowModel show, String speak, ErrorModel error) {
        this.speak = speak;
        this.show = show;
        this.error = error;
    }

    public ActorModel clone() {
        ActorModel clone = null;
        try {
            clone = (ActorModel) super.clone();
        } catch (CloneNotSupportedException e) { /* pass */ }
        clone.show = show.clone();
        clone.error = error == null ? null : error.clone();

        return clone;
    }

    public String getSpeak() {
        return speak;
    }

    public ShowModel getShow() {
        return show;
    }

    public ErrorModel getError() {
        return error;
    }
}
