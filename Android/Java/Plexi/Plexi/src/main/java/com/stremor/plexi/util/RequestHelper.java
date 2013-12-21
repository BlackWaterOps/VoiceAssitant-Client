package com.stremor.plexi.util;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.stremor.plexi.interfaces.IRequestHelper;
import com.stremor.plexi.interfaces.IResponseListener;

/**
 * Created by jon on 20.12.2013.
 */
public class RequestHelper implements IRequestHelper {
    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method,
                              IResponseListener listener) {
        doRequest(type, endpoint, method, null, false, listener);
    }

    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method,
                              Object data, boolean includeNulls, IResponseListener listener) {
        RequestTask req = new RequestTask<T>(type, listener, method);

        if (method == RequestTask.HttpMethod.GET) {
            req.execute(endpoint, null);
        } else {
            req.setContentType("application.json");
            req.execute(endpoint, serializeData(data, includeNulls));
        }
    }

    private String serializeData(Object data, boolean includeNulls) {
        Gson gson = includeNulls
                ? new GsonBuilder().serializeNulls().create()
                : new Gson();

        return gson.toJson(data);
    }
}
