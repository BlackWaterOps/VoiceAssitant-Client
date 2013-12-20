package com.stremor.plexi.models;

/**
 * Created by jeffschifano on 10/28/13.
 */
public class StateModel<T extends Enum<T>> {
    private T state;
    private Object response;

    public StateModel(T state, Object response) {
        set(state, response);
    }

    public void set(T state, Object response) {
        this.state = state;
        this.response = response;
    }

    public void reset() {
        this.state = null;
        this.response = null;
    }

    public T getState() { return state; }
    public void setState(T value) { this.state = value; }

    public Object getResponse() { return this.response; }
    public void setResponse(Object value) { this.response = value; }
}
