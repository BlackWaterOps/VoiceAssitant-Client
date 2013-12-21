package com.stremor.plexi.util;

import android.os.AsyncTask;
import android.util.Log;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.stremor.plexi.interfaces.IResponseListener;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.protocol.HTTP;
import org.apache.http.util.EntityUtils;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URI;
import java.net.URISyntaxException;

/**
 * Created by jeffschifano on 10/29/13.
 */
public class RequestTask<T> extends AsyncTask<Object, Void, T> {
    public enum HttpMethod { GET, POST };

    private static final String TAG = "QueryTask";

    private Class<T> type;
    private IResponseListener listener;
    private HttpMethod method;
    private String contentType;

    public RequestTask(Class<T> classType, IResponseListener responseListener) {
        this(classType, responseListener, HttpMethod.POST);
    }

    public RequestTask(Class<T> classType, IResponseListener responseListener, HttpMethod method) {
        this.type = classType;
        this.listener = responseListener;
        this.method = method;
    }

    /**
     * Sends a query to the server.
     *
     * @param args Three arguments of the form:
     *             1. String endpoint
     *             2. String postData
     */
    @Override
    protected T doInBackground(Object... args) {
        assert args.length == 2;

        String endpoint = (String) args[0];
        String postData = (String) args[1];

        URI uri;

        try {
            uri = new URI(endpoint);
        } catch (URISyntaxException e) {
            Log.e(TAG, "Failed to create uri object from endpoint");
            return null;
        }

        HttpClient client = new DefaultHttpClient();
        HttpResponse response = null;

        try {
            if (method == HttpMethod.GET) {
                HttpGet get = new HttpGet();

                get.setURI(uri);

                response = client.execute(get);
            } else if (method == HttpMethod.POST) {
                HttpPost post = new HttpPost();
                post.setURI(uri);

                if (postData != null) {
                    HttpEntity entity = new StringEntity(postData, HTTP.UTF_8);
                    post.setEntity(entity);
                }

                response = client.execute(post);
            }
        } catch (UnsupportedEncodingException e) {
            Log.e(TAG, "Failed to encode query JSON", e);
            return null;
        } catch (IOException e) {
            Log.e(TAG, "Failed to read HTTP response: " + e.getLocalizedMessage(), e);
            return null;
        }

        HttpEntity responseEntity = response.getEntity();
        if ( responseEntity == null ) {
            Log.e(TAG, "Failed to determine HTTP response");
        }

        String responseBody = null;
        try {
            responseBody = EntityUtils.toString(responseEntity);
        } catch (IOException e) {
            Log.e(TAG, "Failed to parse HTTP response string", e);
            return null;
        }

        Gson gson = new GsonBuilder().serializeNulls().create();
        return gson.fromJson(responseBody, this.type);

        /*
         JSONObject responseObject = null;
        try {
            // need to cast against model here!!
            responseObject = new JSONObject(responseBody);
        } catch (JSONException e) {
            Log.e(TAG, "Failed to parse HTTP response JSON", e);
            return null;
        }

        Object qResponse = null;
        try {
            qResponse = new QueryResponse(responseObject);
        } catch (JSONException e) {
            Log.e(TAG, "Failed to parse query response fields", e);
            return null;
        }

        return qResponse;
        */
    }

    @Override
    protected void onPostExecute(T queryResponse) {
        if ( listener != null )
            listener.onQueryResponse(queryResponse);
    }

    public HttpMethod getMethod() {
        return method;
    }

    public void setMethod(HttpMethod method) {
        this.method = method;
    }

    public String getContentType() {
        return contentType;
    }

    public void setContentType(String contentType) {
        this.contentType = contentType;
    }
}
