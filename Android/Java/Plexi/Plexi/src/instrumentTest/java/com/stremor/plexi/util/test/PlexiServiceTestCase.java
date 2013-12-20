package com.stremor.plexi.util.test;

import android.test.AndroidTestCase;

import com.stremor.plexi.models.StateModel;
import com.stremor.plexi.util.PlexiService;
import com.stremor.plexi.util.RequestTask;

import org.mockito.Mockito;

/**
 * Created by jon on 19.12.2013.
 */
public class PlexiServiceTestCase extends AndroidTestCase {
    private PlexiService plexi;
    private RequestTask mockRequestTask;

    public void setUp() {
        plexi = new PlexiService(getContext());
        mockRequestTask = Mockito.mock(RequestTask.class);
    }

    public void testInitialState() {
        StateModel state = plexi.getCurrentState();
        assertNull(state.getState());
        assertNull(state.getResponse());
    }

    /**
     * When provided a query the service should enter the "init" state
     */
    public void testEntersInit() {
        plexi.query("Hello world");

        StateModel state = plexi.getCurrentState();
        assertEquals("init", state.getState());
        assertNull(state.getResponse());
    }
}
