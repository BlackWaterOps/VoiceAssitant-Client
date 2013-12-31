package com.stremor.plexi.test;

import com.google.gson.JsonParser;
import com.stremor.plexi.interfaces.IRequestHelper;
import com.stremor.plexi.interfaces.IResponseListener;
import com.stremor.plexi.models.ClassifierModel;
import com.stremor.plexi.util.RequestTask;

import org.apache.http.Header;

/**
 * An IRequestHelper stub implementation which returns a fake classification but answers no other
 * requests.
 *
 * Created by jon on 20.12.2013.
 */
public class RequestHelperStub1 implements IRequestHelper {
    private static JsonParser parser = new JsonParser();

    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method,
                              Header[] headers, IResponseListener listener) {
        if (type == ClassifierModel.class) {
            ClassifierModel response = new ClassifierModel("calendar", "create",
                    parser.parse("{\"name\":\"Party\"}").getAsJsonObject());
            listener.onQueryResponse(response);
        }
    }

    public <T> void doSerializedRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method, Header[] headers,
                                        Object data, boolean includeNulls, IResponseListener listener) {

    }
}
