package com.stremor.plexi.util.test;

import android.test.AndroidTestCase;

import com.google.gson.JsonParser;
import com.stremor.plexi.interfaces.IResponseListener;
import com.stremor.plexi.models.ClassifierModel;
import com.stremor.plexi.models.StateModel;
import com.stremor.plexi.util.PlexiService;
import com.stremor.plexi.util.RequestHelper;
import com.stremor.plexi.util.RequestTask;

import org.mockito.Mockito;

import static org.mockito.Matchers.any;
import static org.mockito.Matchers.anyString;
import static org.mockito.Matchers.eq;
import static org.mockito.Mockito.times;
import static org.mockito.Mockito.verify;

/**
 * Created by jon on 19.12.2013.
 */
public class PlexiServiceTestCase extends AndroidTestCase {
    private static JsonParser parser = new JsonParser();
    private PlexiService plexi;
    private PlexiService plexiWithMock;
    private RequestHelper mockRequestHelper;

    public void setUp() {
        plexi = new PlexiService(getContext());

        mockRequestHelper = Mockito.mock(RequestHelper.class);
        plexiWithMock = new PlexiService(getContext(), mockRequestHelper);
    }

    public void testInitialState() {
        StateModel state = plexi.getCurrentState();
        assertEquals(PlexiService.State.UNINITIALIZED, state.getState());
        assertNull(state.getResponse());
    }

    /**
     * When provided a query the service should enter the "init" state and make a request
     */
    public void testEntersInit() {
        plexiWithMock.query("Hello world");

        StateModel state = plexiWithMock.getCurrentState();

        /**
         * Because we mocked the RequestHelper, Plexi will not get a callback for the request and
         * thus should not change state
         */
        assertEquals(PlexiService.State.INIT, state.getState());
        assertEquals("Hello world", state.getResponse());

        verify(mockRequestHelper, times(1)).doRequest(any(Class.class), anyString(),
                any(RequestTask.HttpMethod.class), any(IResponseListener.class));
    }

    public void testClassification() {
        RequestHelperStub spy = Mockito.spy(new RequestHelperStub());
        PlexiService plexi = new PlexiService(getContext(), spy);

        plexi.query("Hello world");
        StateModel state = plexi.getCurrentState();
        assertEquals(PlexiService.State.AUDIT, state.getState());

        verify(spy).doRequest(eq(ClassifierModel.class), anyString(),
                eq(RequestTask.HttpMethod.GET), any(IResponseListener.class));
    }
}
