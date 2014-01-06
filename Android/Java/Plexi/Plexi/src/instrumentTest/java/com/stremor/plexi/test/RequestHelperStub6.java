package com.stremor.plexi.test;

import com.stremor.plexi.interfaces.IRequestHelper;
import com.stremor.plexi.interfaces.IResponseListener;
import com.stremor.plexi.models.ErrorModel;
import com.stremor.plexi.models.LoginRequest;
import com.stremor.plexi.models.LoginResponse;
import com.stremor.plexi.models.SignupRequest;
import com.stremor.plexi.models.SignupResponse;
import com.stremor.plexi.util.RequestTask;

import org.apache.http.Header;

import java.util.HashMap;

/**
 * An IRequestHelper stub implementation which simulates the following scenario:
 *
 * - Login attempted with invalid credentials
 * - Login re-attempted with valid credentials
 * - Login accepted and auth token returned
 *
 * And also the following (depending on how it is used):
 *
 * - Login attempted with invalid credentials
 * - Signup attempted with same credentials
 * - Login re-attempted with same credentials
 * - Login accepted and auth token returned
 *
 * Created by jon on 31.12.2013.
 */
public class RequestHelperStub6 implements IRequestHelper {
    @Override
    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method, Header[] headers, IResponseListener listener) {
        // pass
    }

    private HashMap<String, String> ACCOUNTS = new HashMap<String, String>() {{
        put("a", "a");
    }};

    @Override
    public <T> void doSerializedRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method, Header[] headers, Object data, boolean includeNulls, IResponseListener listener) {
        if (type == LoginResponse.class) {
            LoginRequest req = (LoginRequest) data;
            if (ACCOUNTS.containsKey(req.getUsername())
                    && ACCOUNTS.get(req.getUsername()).equals(req.getPassword())) {
                listener.onQueryResponse(new LoginResponse(new ErrorModel("bad password", 0)));
            } else {
                listener.onQueryResponse(new LoginResponse("a", "123456"));
            }
        } else if (type == SignupResponse.class) {
            SignupRequest req = (SignupRequest) data;
            if (ACCOUNTS.containsKey(req.getUsername())) {
                listener.onQueryResponse(new SignupResponse(new ErrorModel("existing username", 0)));
            } else {
                ACCOUNTS.put(req.getUsername(), req.getPassword());
                listener.onQueryResponse(new SignupResponse("success"));
            }
        }
    }
}
