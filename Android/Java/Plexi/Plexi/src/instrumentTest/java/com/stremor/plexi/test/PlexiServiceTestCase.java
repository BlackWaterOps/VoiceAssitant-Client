package com.stremor.plexi.test;

import android.test.AndroidTestCase;

import com.google.gson.JsonNull;
import com.google.gson.JsonObject;
import com.google.gson.JsonParser;
import com.stremor.plexi.interfaces.IRequestHelper;
import com.stremor.plexi.interfaces.IResponseListener;
import com.stremor.plexi.models.ActorModel;
import com.stremor.plexi.models.ClassifierModel;
import com.stremor.plexi.models.DisambiguatorModel;
import com.stremor.plexi.models.ResponderModel;
import com.stremor.plexi.models.StateModel;
import com.stremor.plexi.PlexiService;
import com.stremor.plexi.util.RequestHelper;
import com.stremor.plexi.util.RequestTask;

import org.mockito.ArgumentMatcher;
import org.mockito.InOrder;
import org.mockito.Mockito;

import static org.mockito.Matchers.any;
import static org.mockito.Matchers.anyObject;
import static org.mockito.Matchers.anyString;
import static org.mockito.Matchers.argThat;
import static org.mockito.Matchers.contains;
import static org.mockito.Matchers.eq;
import static org.mockito.Matchers.isA;
import static org.mockito.Mockito.inOrder;
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

        verifyClassificationCall(mockRequestHelper);
//        verify(mockRequestHelper, times(1)).doRequest(any(Class.class), anyString(),
//                any(RequestTask.HttpMethod.class), any(IResponseListener.class));
    }

    public void testClassification() {
        RequestHelperStub1 spy = Mockito.spy(new RequestHelperStub1());
        PlexiService plexi = new PlexiService(getContext(), spy);

        plexi.query("Hello world");
        StateModel state = plexi.getCurrentState();
        assertEquals(PlexiService.State.AUDIT, state.getState());

        verifyClassificationCall(spy);
        verifyAuditCall(spy);
    }

    public void testClassificationAndAudit() {
        RequestHelperStub2 spy = Mockito.spy(new RequestHelperStub2());
        PlexiService plexi = new PlexiService(getContext(), spy);

        plexi.query("Hello world");
        StateModel state = plexi.getCurrentState();
        assertEquals(PlexiService.State.COMPLETED, state.getState());

        verifyClassificationCall(spy);
        verifyAuditCall(spy);
        verifyActorCall(spy);
    }

    /**
     * Tests a full flow, from query to active disambiguation to completion.
     *
     * Tightly coupled with RequestHelperStub3.
     */
    public void testWithStub3() {
        RequestHelperStub3 spy = Mockito.spy(new RequestHelperStub3());
        PlexiService plexi = new PlexiService(getContext(), spy);

        plexi.query("Hello world");

        // Validate current state
        assertEquals(PlexiService.State.IN_PROGRESS, plexi.getCurrentState().getState());

        // Verify that requests were made as expected so far
        verifyClassificationCall(spy);
        verifyAuditCall(spy);

        // OK, we're waiting for input. Give some input.
        plexi.query("Party");

        // That should yield an active disambiguation request
        verifyDisambiguateActiveCall(spy, "string", "Party");

        // Then an audit
        verifyAuditCall(spy, 2);

        // Verify final state: context should be cleared
        assertEquals(PlexiService.State.UNINITIALIZED, plexi.getCurrentState().getState());

        // We expect the client to have called the actor with this model
        ClassifierModel expected = new ClassifierModel("calendar", "create",
                parser.parse("{\"name\": \"Party\"}").getAsJsonObject());
        verifyActorCall(spy, expected, false);
    }

    /**
     * Tests a full flow: query -> passive disambiguation -> completion
     *
     * Tightly coupled with RequestHelperStub4.
     */
    public void testWithStub4() {
        RequestHelperStub4 spy = Mockito.spy(new RequestHelperStub4());
        PlexiService plexi = new PlexiService(getContext(), spy);

        plexi.query("Hello world");

        // Verify final state: context should be cleared
        assertEquals(PlexiService.State.UNINITIALIZED, plexi.getCurrentState().getState());

        // Custom matcher for disambiguator model that we expect
        ArgumentMatcher<DisambiguatorModel> isValidDisambig = new ArgumentMatcher<DisambiguatorModel>() {
            @Override
            public boolean matches(Object o) {
                if (!(o instanceof DisambiguatorModel))
                    return false;
                DisambiguatorModel d = (DisambiguatorModel) o;
                return (d.getPayload().equals(JsonNull.INSTANCE)
                        && d.getType().equals("location")
                        && d.getDeviceInfo() != null);
            }
        };

        InOrder order = inOrder(spy);

        order.verify(spy, times(1)).doRequest(eq(ClassifierModel.class), anyString(),
                eq(RequestTask.HttpMethod.GET), isA(IResponseListener.class));
        order.verify(spy, times(1)).doRequest(eq(ResponderModel.class), contains("audit"),
                eq(RequestTask.HttpMethod.POST), anyObject(), eq(false),
                isA(IResponseListener.class));
        order.verify(spy, times(1)).doRequest(eq(JsonObject.class), contains("disambiguate/passive"),
                eq(RequestTask.HttpMethod.POST), argThat(isValidDisambig), eq(true),
                isA(IResponseListener.class));
        order.verify(spy, times(1)).doRequest(eq(ResponderModel.class), contains("audit"),
                eq(RequestTask.HttpMethod.POST), anyObject(), eq(false),
                isA(IResponseListener.class));
        order.verify(spy, times(1)).doRequest(eq(ActorModel.class), contains("actor"),
                eq(RequestTask.HttpMethod.POST), eq(spy.MODEL_COMPLETE), eq(false),
                any(IResponseListener.class));
        order.verifyNoMoreInteractions();
    }

    private void verifyClassificationCall(IRequestHelper spy) {
        verify(spy, times(1)).doRequest(eq(ClassifierModel.class), anyString(),
                eq(RequestTask.HttpMethod.GET), isA(IResponseListener.class));
    }

    private void verifyAuditCall(IRequestHelper spy) {
        verifyAuditCall(spy, 1);
    }

    private void verifyAuditCall(IRequestHelper spy, int times) {
        verify(spy, times(times)).doRequest(eq(ResponderModel.class), contains("audit"),
                eq(RequestTask.HttpMethod.POST), anyObject(), eq(false),
                isA(IResponseListener.class));
    }

    private void verifyActorCall(IRequestHelper spy) {
        verify(spy, times(1)).doRequest(eq(ActorModel.class), contains("actor"),
                eq(RequestTask.HttpMethod.POST), isA(ClassifierModel.class), eq(false),
                isA(IResponseListener.class));
    }

    private void verifyActorCall(IRequestHelper spy, ClassifierModel expectedData,
                                 boolean includeNulls) {
        verify(spy, times(1)).doRequest(eq(ActorModel.class), contains("actor"),
                eq(RequestTask.HttpMethod.POST), eq(expectedData), eq(includeNulls),
                any(IResponseListener.class));
    }

    private void verifyDisambiguateActiveCall(IRequestHelper spy, String expectedType,
                                              String expectedPayload) {
        DisambiguatorModel expected = new DisambiguatorModel(expectedPayload, expectedType);

        verify(spy, times(1)).doRequest(eq(JsonObject.class), contains("disambiguate/active"),
                eq(RequestTask.HttpMethod.POST), eq(expected), eq(true),
                isA(IResponseListener.class));
    }

    private void verifyDisambiguatePassiveCall(IRequestHelper spy, String expectedType,
                                               String expectedPayload) {
        DisambiguatorModel expected = new DisambiguatorModel(expectedPayload, expectedType);

        verify(spy, times(1)).doRequest(eq(JsonObject.class), contains("disambiguate/passive"),
                eq(RequestTask.HttpMethod.POST), eq(expected), eq(true),
                isA(IResponseListener.class));
    }
}
