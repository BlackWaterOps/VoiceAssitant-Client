package com.stremor.plexi.interfaces;

import com.stremor.plexi.models.ActorModel;
import com.stremor.plexi.models.ClassifierModel;
import com.stremor.plexi.models.ResponderModel;
import org.json.JSONException;

import java.util.HashMap;

/**
 * Created by jeffschifano on 10/29/13.
 */
public interface IPlexiResponse {
    public void onQueryResponse(Object queryResponse) throws JSONException;

    public void onQueryResponse(ClassifierModel queryResponse) throws JSONException;

    public void onQueryResponse(HashMap<String, Object> queryResponse) throws JSONException;

    public void onQueryResponse(ResponderModel queryResponse) throws JSONException;

    public void onQueryResponse(ActorModel queryResponse) throws JSONException;
}
