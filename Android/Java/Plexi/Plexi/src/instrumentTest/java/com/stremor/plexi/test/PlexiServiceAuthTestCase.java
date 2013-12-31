package com.stremor.plexi.test;

import android.test.AndroidTestCase;

import com.stremor.plexi.PlexiService;
import com.stremor.plexi.interfaces.IPlexiListener;
import com.stremor.plexi.interfaces.IResponseListener;
import com.stremor.plexi.models.LoginRequest;
import com.stremor.plexi.models.LoginResponse;
import com.stremor.plexi.util.Installation;
import com.stremor.plexi.util.RequestTask;

import org.apache.http.Header;
import org.mockito.ArgumentMatcher;
import org.mockito.InOrder;
import org.mockito.Mockito;

import static org.mockito.Mockito.*;

/**
 * Created by jon on 31.12.2013.
 */
public class PlexiServiceAuthTestCase extends AndroidTestCase {

    private static ArgumentMatcher<LoginResponse> isLoginFailure = new ArgumentMatcher<LoginResponse>() {
        @Override
        public boolean matches(Object o) {
            if (!(o instanceof LoginResponse))
                return false;

            LoginResponse resp = (LoginResponse) o;
            return resp.getUsername() == null && resp.getToken() == null
                    && resp.getError() != null;
        }
    };

    private static ArgumentMatcher<LoginResponse> isLoginSuccess = new ArgumentMatcher<LoginResponse>() {
        @Override
        public boolean matches(Object o) {
            if (!(o instanceof LoginResponse))
                return false;

            LoginResponse resp = (LoginResponse) o;
            return resp.getUsername() != null && resp.getToken() != null
                    && resp.getError() == null;
        }
    };

    private ArgumentMatcher<Header[]> isProperLoginHeaders = new ArgumentMatcher<Header[]>() {
        @Override
        public boolean matches(Object o) {
            if (!(o instanceof Header[]))
                return false;

            Header[] headers = (Header[]) o;
            if (headers.length != 1)
                return false;
            Header header = headers[0];
            return header.getName().equals("Stremor-Auth-Device")
                    && header.getValue().equals(Installation.id(getContext()));
        }
    };

    /**
     * Tightly coupled with {@link com.stremor.plexi.test.RequestHelperStub6}.
     */
    public void testLoginFailAndFix() {
        RequestHelperStub6 spy = Mockito.spy(new RequestHelperStub6());
        IPlexiListener listener = Mockito.mock(IPlexiListener.class);

        PlexiService plexi = new PlexiService(getContext(), spy);
        plexi.addListener(listener);

        plexi.login("a", "b");
        plexi.login("a", "a");

        /////////

        InOrder order = inOrder(spy, listener);

        order.verify(spy, times(1)).doSerializedRequest(eq(LoginResponse.class), contains("login"),
                eq(RequestTask.HttpMethod.POST), argThat(isProperLoginHeaders),
                eq(new LoginRequest("a", "b")), anyBoolean(), isA(IResponseListener.class));
        order.verify(listener, times(1)).onLoginResponse(argThat(isLoginFailure));

        order.verify(spy, times(1)).doSerializedRequest(eq(LoginResponse.class), contains("login"),
                eq(RequestTask.HttpMethod.POST), argThat(isProperLoginHeaders),
                eq(new LoginRequest("a", "a")), anyBoolean(), isA(IResponseListener.class));
        order.verify(listener, times(1)).onLoginResponse(argThat(isLoginSuccess));

        order.verifyNoMoreInteractions();
    }
}
