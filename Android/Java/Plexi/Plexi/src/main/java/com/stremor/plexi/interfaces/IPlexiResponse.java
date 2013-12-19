package com.stremor.plexi.interfaces;

import com.google.gson.JsonObject;
import com.stremor.plexi.models.ActorModel;
import com.stremor.plexi.models.ClassifierModel;
import com.stremor.plexi.models.ResponderModel;

/**
 * Created by jeffschifano on 10/29/13.
 */
public interface IPlexiResponse {
    public void onQueryResponse(Object queryResponse);

    public void onQueryResponse(ClassifierModel queryResponse);

    public void onQueryResponse(JsonObject queryResponse);

    public void onQueryResponse(ResponderModel queryResponse);

    public void onQueryResponse(ActorModel queryResponse);
}
