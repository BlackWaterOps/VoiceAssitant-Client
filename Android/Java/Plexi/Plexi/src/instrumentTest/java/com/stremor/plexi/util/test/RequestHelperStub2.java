package com.stremor.plexi.util.test;

import com.google.gson.JsonParser;
import com.stremor.plexi.interfaces.IRequestHelper;
import com.stremor.plexi.interfaces.IResponseListener;
import com.stremor.plexi.models.ClassifierModel;
import com.stremor.plexi.models.ResponderModel;
import com.stremor.plexi.util.RequestTask;

/**
 * An IRequestHelper stub implementation which returns a fake classification and accepts a single
 * audit (returns "complete" state immediately), but does not answer an actor request.
 *
 * Created by jon on 20.12.2013.
 */
public class RequestHelperStub2 implements IRequestHelper {
    private static JsonParser parser = new JsonParser();

    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method,
                              IResponseListener listener) {
        if (type == ClassifierModel.class) {
            ClassifierModel response = new ClassifierModel("calendar", "create",
                    parser.parse("{\"name\":\"Party\"}").getAsJsonObject());
            listener.onQueryResponse(response);
        }
    }

    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method,
                              Object data, boolean includeNulls, IResponseListener listener) {
        if (endpoint.contains("audit")) {
            ClassifierModel classifierData = new ClassifierModel("calendar", "create",
                    parser.parse("{\"name\":\"Party\"}").getAsJsonObject());
            ResponderModel response = new ResponderModel("completed", "foobar", null,
                    classifierData);

            listener.onQueryResponse(response);
        }
    }
}
