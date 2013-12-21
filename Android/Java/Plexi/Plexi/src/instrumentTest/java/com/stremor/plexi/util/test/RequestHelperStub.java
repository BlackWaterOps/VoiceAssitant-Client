package com.stremor.plexi.util.test;

import com.google.gson.JsonParser;
import com.stremor.plexi.interfaces.IRequestHelper;
import com.stremor.plexi.interfaces.IResponseListener;
import com.stremor.plexi.models.ClassifierModel;
import com.stremor.plexi.util.RequestTask;

/**
 * Created by jon on 20.12.2013.
 */
public class RequestHelperStub implements IRequestHelper {
    private static JsonParser parser = new JsonParser();

    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method,
                              IResponseListener listener) {
        if (type == ClassifierModel.class) {
            ClassifierModel response = new ClassifierModel();
            response.model = "calendar";
            response.action = "create";
            response.payload = parser.parse("{\"name\":\"Party\"}").getAsJsonObject();

            listener.onQueryResponse(response);
        }
    }

    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method,
                              Object data, boolean includeNulls, IResponseListener listener) {

    }
}
