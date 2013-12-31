package com.stremor.plexi.test;

import com.stremor.plexi.interfaces.IRequestHelper;
import com.stremor.plexi.interfaces.IResponseListener;
import com.stremor.plexi.models.ErrorModel;
import com.stremor.plexi.models.LoginRequest;
import com.stremor.plexi.models.LoginResponse;
import com.stremor.plexi.util.RequestTask;

import org.apache.http.Header;

/**
 * An IRequestHelper stub implementation which simulates the following scenario:
 *
 * - Login attempted with invalid credentials
 * - Login re-attempted with valid credentials
 * - Login accepted and auth token returned
 *
 * Created by jon on 31.12.2013.
 */
public class RequestHelperStub6 implements IRequestHelper {
    @Override
    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method, Header[] headers, IResponseListener listener) {
        // pass
    }

    @Override
    public <T> void doSerializedRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method, Header[] headers, Object data, boolean includeNulls, IResponseListener listener) {
        assert type == LoginResponse.class;

        LoginRequest req = (LoginRequest) data;
        if (req.getPassword().equals("b")) {
            listener.onQueryResponse(new LoginResponse(new ErrorModel("bad password", 0)));
        } else if (req.getPassword().equals("a")) {
            listener.onQueryResponse(new LoginResponse("a", "123456"));
        }
    }
}
