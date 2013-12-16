package com.stremor.plexi.models;

import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by jeffschifano on 10/28/13.
 */
public class StateModel {
    private List<PropertyChangeListener> listener = new ArrayList<PropertyChangeListener>();

    private String state;
    private Object response;

    public StateModel() {

    }

    public StateModel(String state, Object response) {
        set(state, response);
    }

    public void set(String state, Object response) {
        this.state = state;
        this.response = response;
    }

    public void reset() {
        this.state = null;
        this.response = null;
    }

    public String getState() { return this.state; }
    public void setState(String value) { this.state = value; }

    public Object getResponse() { return this.response; }
    public void setResponse(Object value) { this.response = value; }

    private void notifyListeners(Object object, String property, String oldValue, String newValue) {
        for (PropertyChangeListener state : listener) {
            state.propertyChange(new PropertyChangeEvent(object, property, oldValue, newValue));
        }
    }

    public void addChangeListener(PropertyChangeListener newListener) {
        listener.add(newListener);
    }
}
