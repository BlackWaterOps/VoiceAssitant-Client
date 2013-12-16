package com.stremor.plexi.util;

import android.os.AsyncTask;
import android.util.Log;

import com.google.gson.GsonBuilder;
import org.apache.http.Header;
import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpRequestBase;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.protocol.HTTP;
import org.apache.http.util.EntityUtils;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URI;
import java.net.URISyntaxException;

import com.google.gson.Gson;

import com.stremor.plexi.interfaces.IPlexiResponse;

/**
 * Created by jeffschifano on 10/29/13.
 */
public class RequestTask<T> extends AsyncTask<Object, Void, T> {
    private static final String TAG = "QueryTask";
    private IPlexiResponse listener;
    private Class<T> type;

    private StopWatch stopWatch;

    public String ContentType = null;
    public String AcceptType = null;
    public String Method = "GET";


    public RequestTask(Class<T> classType, IPlexiResponse responseListener) {
        this.type = classType;
        this.listener = responseListener;

        if (stopWatch == null) {
            stopWatch = new StopWatch();
        }
    }

    @Override
    protected void onPreExecute() {
        stopWatch.start();

        // send progress message
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
            if (this.Method.equalsIgnoreCase("get")) {
                HttpGet get = new HttpGet();

                get.setURI(uri);

                response = client.execute(get);
            }

            if (this.Method.equalsIgnoreCase("post")) {
                HttpPost post = new HttpPost();
                post.setURI(uri);

                if (this.ContentType != null) {
                    post.setHeader("ContentType", this.ContentType);
                }

                if (this.AcceptType != null) {
                    post.setHeader("Accept", this.AcceptType);
                }

                if (postData != null) {
                    /*
                    JSONObject request = new JSONObject();
                    request.put("query", query);
                    request.put("context", context);

                    HttpEntity entity = new StringEntity(request.toString(), HTTP.UTF_8);
                    */
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
        stopWatch.stop();

        Log.d(TAG, String.valueOf(stopWatch.getElapsedTime()));

        // send progress message

        if ( listener != null ) {
            try {
                listener.onQueryResponse(queryResponse);
            } catch (JSONException e) {
                Log.w(TAG, "Query response listener failed", e);
            }
        }
    }
}
