package com.stremor.plexi.util;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.stremor.plexi.interfaces.IRequestHelper;
import com.stremor.plexi.interfaces.IResponseListener;

import org.apache.http.Header;
import org.apache.http.NameValuePair;

import java.util.List;

/**
 * Primary implementation of the {@link com.stremor.plexi.interfaces.IRequestHelper} interface.
 *
 * Created by jon on 20.12.2013.
 */
public class RequestHelper implements IRequestHelper {
    /**
     * Perform a server request without data.
     *
     * @param type The class of `T`
     * @param endpoint URI endpoint
     * @param method HTTP method
     * @param headers Extra HTTP headers to attach (can be null)
     * @param listener A listener to be notified upon response
     * @param <T> The type expected as a result. This type will be instantiated using the JSON
     *            results returned by the server.
     */
    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method,
                              Header[] headers, IResponseListener listener) {
        doRequest(type, endpoint, method, headers, null, listener);
    }

    /**
     * Perform a server request with optional form data.
     *
     * @param type The class of `T`
     * @param endpoint URI endpoint
     * @param method HTTP method
     * @param headers Extra HTTP headers to attach (can be null)
     * @param dataFields Data fields (will be URL-encoded)
     * @param listener A listener to be notified upon response
     * @param <T> The type expected as a result. This type will be instantiated using the JSON
     *            results returned by the server.
     */
    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method,
                              Header[] headers, List<NameValuePair> dataFields,
                              IResponseListener listener) {
        new RequestTask<T>(type, listener).execute(method, endpoint,
                new RequestPostData(dataFields), headers);
    }

    /**
     * Perform a server request with arbitrary serializable body data.
     *
     * @param type The class of `T`
     * @param endpoint URI endpoint
     * @param method HTTP method
     * @param headers Extra HTTP headers to attach (can be null)
     * @param data A JSON-serializable object
     * @param includeNulls `true` if `null` properties should be included in the serialized output
     * @param listener A listener to be notified upon response
     * @param <T> The type expected as a result. This type will be instantiated using the JSON
     *            results returned by the server.
     */
    public <T> void doSerializedRequest(Class<T> type, String endpoint,
                                        RequestTask.HttpMethod method, Header[] headers,
                                        Object data, boolean includeNulls,
                                        IResponseListener listener) {
        RequestTask<T> req = new RequestTask<T>(type, listener);

        if (method == RequestTask.HttpMethod.GET) {
            req.execute(method, endpoint, null);
        } else {
            req.setContentType("application/json");
            req.execute(method, endpoint, new RequestPostData(serializeData(data, includeNulls)));
        }
    }

    private String serializeData(Object data, boolean includeNulls) {
        Gson gson = includeNulls
                ? new GsonBuilder().serializeNulls().create()
                : new Gson();

        return gson.toJson(data);
    }
}
