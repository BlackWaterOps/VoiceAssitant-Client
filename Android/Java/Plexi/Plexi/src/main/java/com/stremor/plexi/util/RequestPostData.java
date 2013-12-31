package com.stremor.plexi.util;

import org.apache.http.HttpEntity;
import org.apache.http.NameValuePair;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.entity.StringEntity;
import org.apache.http.protocol.HTTP;

import java.io.UnsupportedEncodingException;
import java.util.List;

/**
 * Encapsulates data to be sent via POST request.
 *
 * Created by jon on 31.12.2013.
 */
public class RequestPostData {
    private String postString;
    private List<NameValuePair> postFields;

    private String charset;

    public RequestPostData(String postString) {
        this(postString, HTTP.UTF_8);
    }

    public RequestPostData(String postString, String postStringCharset) {
        this.postString = postString;
        this.charset = postStringCharset;
    }

    public RequestPostData(List<NameValuePair> postFields) {
        this(postFields, HTTP.UTF_8);
    }

    public RequestPostData(List<NameValuePair> postFields, String postFieldsCharset) {
        this.postFields = postFields;
        this.charset = postFieldsCharset;
    }

    public HttpEntity makeEntity() throws UnsupportedEncodingException {
        if (postString != null)
            return new StringEntity(postString, charset);
        else if (postFields != null)
            return new UrlEncodedFormEntity(postFields, charset);

        return null;
    }
}
