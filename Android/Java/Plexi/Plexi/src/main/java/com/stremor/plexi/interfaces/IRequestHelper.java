package com.stremor.plexi.interfaces;

import com.stremor.plexi.util.RequestTask;

import org.apache.http.Header;

/**
 * Mediates requests between a client and a Plexi server request mechanism.
 *
 * Created by jon on 20.12.2013.
 */
public interface IRequestHelper {
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
                              Header[] headers, IResponseListener listener);

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
                                        IResponseListener listener);
}
