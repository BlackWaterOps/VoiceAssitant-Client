package com.stremor.plexi;

import android.content.Context;
import android.location.Location;
import android.os.CountDownTimer;
import android.util.Log;
import android.util.Pair;

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.stremor.plexi.interfaces.IPlexiListener;
import com.stremor.plexi.interfaces.IPlexiService;
import com.stremor.plexi.interfaces.IRequestHelper;
import com.stremor.plexi.interfaces.IResponseListener;
import com.stremor.plexi.models.ActorModel;
import com.stremor.plexi.models.Choice;
import com.stremor.plexi.models.ClassifierModel;
import com.stremor.plexi.models.DisambiguatorModel;
import com.stremor.plexi.models.LoginRequest;
import com.stremor.plexi.models.LoginResponse;
import com.stremor.plexi.models.ResponderModel;
import com.stremor.plexi.models.ShowModel;
import com.stremor.plexi.models.SignupRequest;
import com.stremor.plexi.models.SignupResponse;
import com.stremor.plexi.models.StateModel;
import com.stremor.plexi.util.Datetime;
import com.stremor.plexi.util.Installation;
import com.stremor.plexi.util.JsonObjectUtil;
import com.stremor.plexi.util.LocationTracker;
import com.stremor.plexi.util.RequestHelper;
import com.stremor.plexi.util.RequestTask;

import org.apache.http.Header;
import org.apache.http.message.BasicHeader;
import org.joda.time.DateTimeZone;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Created by jeffschifano on 10/28/13.
 */
public final class PlexiService implements IPlexiService, IResponseListener {
    // endpoints
    private static final String CLASSIFIER = "http://casper-cached.stremor-nli.appspot.com/v1";
    private static final String DISAMBIGUATOR = CLASSIFIER + "/disambiguate";
    private static final String RESPONDER = "http://rez.stremor-apier.appspot.com/v1/";
    private static final String PUD = "http://stremor-pud.appspot.com/v1/";
    private static final String LOGIN = "http://stremor-pud.appspot.com/v1/login";
    private static final String SIGNUP = "http://stremor-pud.appspot.com/v1/signup";

    // header names
    private static final String HEADER_STREMOR_AUTH_DEVICE = "Stremor-Auth-Device";

    // tag for logging
    private static final String TAG = "PlexiService";

    // States which indicate Plexi should contact the auditor again after action is taken
    private static final List<String> auditorStates = Arrays.asList(
            new String[] {"disambiguate", "inprogress", "choice"});

    // gets completed with all the necessary fields in order to fulfill an action
    private ClassifierModel mainContext = null;

    // indicates fields that need to be completed in the main context
    private ResponderModel tempContext = null;

    // Observers
    private List<IPlexiListener> listeners = new ArrayList<IPlexiListener>();

    // Android/state-related members
    private Context context;
    private CountDownTimer contextTimer;
    private LocationTracker locationTracker;
    private String originalQuery;

    // HTTP-related members
    private IRequestHelper requestHelper;
    private Header[] headers;

    /**
     * State data
     */
    public enum State {
        UNINITIALIZED, INIT, AUDIT, DISAMBIGUATE, DISAMBIGUATE_PERSONAL, DISAMBIGUATE_ACTIVE,
        DISAMBIGUATE_CANDIDATE, IN_PROGRESS, ERROR, CHOICE, RESTART, COMPLETED, EXCEPTION
    }

    private static HashMap<String, State> STATE_MAP = new HashMap<String, State>() {{
        put(null, State.UNINITIALIZED);
        put("init", State.INIT);
        put("audit", State.AUDIT);
        put("disambiguate", State.DISAMBIGUATE);
        put("disambiguate:personal", State.DISAMBIGUATE_PERSONAL);
        put("disambiguate:active", State.DISAMBIGUATE_ACTIVE);
        put("disambiguate:candidate", State.DISAMBIGUATE_CANDIDATE);
        put("inprogress", State.IN_PROGRESS);
        put("error", State.ERROR);
        put("choice", State.CHOICE);
        put("restart", State.RESTART);
        put("completed", State.COMPLETED);
        put("exception", State.EXCEPTION);
    }};

    private StateModel<State> currentState = new StateModel<State>(State.UNINITIALIZED, null);

    public PlexiService(Context context) {
        this(context, new RequestHelper());
    }

    public PlexiService(Context context, IRequestHelper requestHelper) {
        this.context = context;
        this.requestHelper = requestHelper;
        this.locationTracker = new LocationTracker(context);

        createTimer();
    }

    public String getOriginalQuery() { return originalQuery; }
    public StateModel getCurrentState() { return currentState; }

    /**
     * Handles any and all changes in state within the Plexi service. Simply dispatches to other
     * handlers based on state.
     */
    public void changeState(State state, Object data) {
        currentState.set(state, data);

        switch (state) {
            case INIT:
                classify((String) data);
                break;
            case AUDIT:
                auditor((ClassifierModel) data);
                break;
            case DISAMBIGUATE:
                disambiguatePassive((ResponderModel) data);
                break;
            case DISAMBIGUATE_PERSONAL:
                disambiguatePersonal((ResponderModel) data);
                break;
            case DISAMBIGUATE_ACTIVE:
                disambiguateActive((String) data);
                break;
            case DISAMBIGUATE_CANDIDATE:
                disambiguateCandidate((String) data);
                break;
            case IN_PROGRESS:
            case ERROR:
                show((ResponderModel) data);
                break;
            case CHOICE:
                requestChoice((ResponderModel) data);
                break;
            case RESTART:
                restart((ResponderModel) data);
                break;
            case COMPLETED:
                actor((ResponderModel) data);
                break;
            case EXCEPTION:
                errorMessage((String) data);
        }
    }

    /**
     * Handle arbitrary input provided by the user.
     */
    public void query(String query) {
        originalQuery = query;

        State currState = currentState.getState();
        State newState;

        switch (currState) {
            case UNINITIALIZED:
                newState = State.INIT;
                break;
            case IN_PROGRESS:
            case ERROR:
                newState = State.DISAMBIGUATE_ACTIVE;
                break;
            case CHOICE:
                newState = State.DISAMBIGUATE_CANDIDATE;
                break;
            default:
                newState = State.INIT;
                break;
        }

        changeState(newState, query);
    }

    public void clearContext()
    {
        mainContext = null;
        tempContext = null;
        currentState.set(State.UNINITIALIZED, null);

        resetTimer();
    }

    public void resetTimer()
    {
        contextTimer.cancel();
    }

    private void createTimer() {
        contextTimer = new CountDownTimer(120000, 120000) {
            @Override
            public void onTick(long l) {
                return;
            }

            @Override
            public void onFinish() {
                clearContext();
            }
        };
    }

    /**
     * Log in to a Stremor account. Plexi listeners will be called at
     * {@link com.stremor.plexi.interfaces.IPlexiListener#onLoginResponse(com.stremor.plexi.models.LoginResponse)}
     * when the login request completes.
     *
     * @param username
     * @param password
     */
    public void login(String username, String password) {
        String deviceId = Installation.id(context);
        Header[] headers = new Header[] {
                new BasicHeader(HEADER_STREMOR_AUTH_DEVICE, deviceId)
        };

        LoginRequest req = new LoginRequest(username, password);

        requestHelper.doSerializedRequest(LoginResponse.class, LOGIN, RequestTask.HttpMethod.POST,
                headers, req, true, this);
    }

    private void handleLoginResponse(LoginResponse response) {
        notifyListeners(PublicEvent.LOGIN_RESPONSE, response);
    }

    /**
     * Sign up for a Stremor account.
     *
     * @param username
     * @param password
     */
    public void register(String username, String password) {
        SignupRequest req = new SignupRequest(username, password);
        requestHelper.doSerializedRequest(SignupResponse.class, SIGNUP, RequestTask.HttpMethod.POST,
                null, req, true, this);
    }

    private void handleSignupResponse(SignupResponse response) {
        notifyListeners(PublicEvent.SIGNUP_RESPONSE, response);
    }

    /**
     * From a response model present a choice list to the user.
     *
     * @param response
     */
    private void requestChoice(ResponderModel response) {
        Choice[] list = response.getShow().getSimple().getList();

        if (list == null)
            Log.e(TAG, "choiceList called with an invalid responder model (empty list)");

        notifyListeners(PublicEvent.DO_REQUEST_CHOICE, new Object[] {list});
    }

    /**
     * Called by a client after the user makes a choice from a choice list.
     *
     * @param choice
     */
    public void choice(Choice choice) {
        // Replace fields in a clone of the current context
        ClassifierModel clone = null;
        try {
            clone = mainContext.clone();
        } catch (CloneNotSupportedException e) {
            Log.e(TAG, "Clone of context failed", e);
            return;
        }

        String field = tempContext.getField().replace("payload.", "");
        JsonObjectUtil.replace(clone.getPayload(), field, choice.getData());
        changeState(State.AUDIT, clone);
    }

    // called from "inprogress" status
    private void show(ResponderModel response) {
        notifyListeners(PublicEvent.DO_SHOW, response.getShow(), response.getSpeak());
    }

    private void errorMessage(String message) {
        notifyListeners(PublicEvent.ERROR, message);
    }

    /**
     * Classification methods
     */

    private void classify(String query) {
        try {
            requestHelper.doRequest(ClassifierModel.class, CLASSIFIER + "?query=" +
                    URLEncoder.encode(query, "utf-8"), RequestTask.HttpMethod.GET, headers, this);
        } catch (UnsupportedEncodingException e) {
            Log.e(TAG, "UnsupportedEncodingException during classification request building", e);
        }
    }

    // classifier response handler
    public void handleClassifierResponse(ClassifierModel response) {
        if (response.getError() != null) {
            changeState(State.EXCEPTION, response.getError().getMessage());
        } else {
            doClientOperations(response.getPayload());
            changeState(State.AUDIT, response);
        }
    }

    /**
     * Disambiguation methods
     */

    private void disambiguateActive(String data) {
        DisambiguatorModel postData = new DisambiguatorModel(data, tempContext.getType());
        requestHelper.doSerializedRequest(JsonObject.class, DISAMBIGUATOR + "/active",
                RequestTask.HttpMethod.POST, headers, postData, true, this);
    }

    private void disambiguateCandidate(String data) {
        Choice[] candidates = tempContext.getShow().getSimple().getList();
        // String field = tempContext.field;

        candidates = candidates == null ? new Choice[]{} : candidates;
        DisambiguatorModel postData = new DisambiguatorModel(data, tempContext.getType(),
                candidates);
        requestHelper.doSerializedRequest(JsonObject.class, DISAMBIGUATOR + "/candidate",
                RequestTask.HttpMethod.POST, headers, postData, true, this);
    }

    private void disambiguatePassive(ResponderModel data) {
        String field = data.getField();
        String type = data.getType();

        Object payload = JsonObjectUtil.find(mainContext.getPayload(), field.replace("payload.", ""));

        DisambiguatorModel postData = new DisambiguatorModel(payload, type, getDeviceInfo());
        requestHelper.doSerializedRequest(JsonObject.class, DISAMBIGUATOR + "/passive",
                RequestTask.HttpMethod.POST, headers, postData, true, this);
    }

    private void disambiguatePersonal(ResponderModel data) {
        String field = data.getField();
        String type = data.getType();

        Object payload = JsonObjectUtil.find(mainContext.getPayload(), field.replace("payload.", ""));

        DisambiguatorModel postData = new DisambiguatorModel(payload, type);
        requestHelper.doSerializedRequest(JsonObject.class, PUD + "disambiguate",
                RequestTask.HttpMethod.POST, headers, postData, true, this);
    }

    // disambiguator response handler
    public void handleDisambiguationResponse(JsonObject response) {
        if (response != null) {
            if (response.has("error")) {
                // TODO
                // currentState.set("exception", error.message);
            } else {
                // do client operations
                doClientOperations(response);

                String field = this.tempContext.getField().replace("payload.", "");
                String type = this.tempContext.getType();

                // Replace fields in a clone of the current context
                ClassifierModel clone = null;
                try {
                    clone = mainContext.clone();
                } catch (CloneNotSupportedException e) {
                    Log.e(TAG, "Clone of context failed", e);
                    return;
                }

                if (response.has(type)) {
                    JsonObjectUtil.replace(clone.getPayload(), field, response.get(type));
                } else {
                    Log.e(TAG, "disambiguation response is missing type");
                }

                changeState(State.AUDIT, clone);
            }
        }
    }

    /**
     * Auditor methods
     */

    private void auditor(ClassifierModel context) {
        if (!context.equals(mainContext)) {
            this.mainContext = context;

            requestHelper.doSerializedRequest(ResponderModel.class, RESPONDER + "audit",
                    RequestTask.HttpMethod.POST, headers, context, false, this);
        } else {
            throw new RuntimeException("potential request loop detected");
//            Log.e(TAG, "potential request loop detected");
        }
    }

    // auditor response handler
    public void handleAuditorResponse(ResponderModel response) {
        if (response.getError() != null) {
            changeState(State.EXCEPTION, response.getError().getMessage());
        } else {
            String stateString = response.getStatus().replace(" ", "");
            String crossCheck = stateString.split(":")[0];

            if (auditorStates.contains(crossCheck)) {
                tempContext = response;
                contextTimer.start();
            }

            changeState(STATE_MAP.get(stateString), response);
        }
    }

    private void restart(ResponderModel data) {
        if (data.getData() == null) {
            Log.e(TAG, "missing new replacement context");
        }

        changeState(State.AUDIT, data.getData());
    }

    /**
     * Actor methods
     */

    private void actor(ResponderModel data) {
        String actor = data.getActor();

        if (actor != null) {
            String endpoint = RESPONDER + "actors/" + actor;

            if (actor.contains("private:")) {
                endpoint = PUD + "actors/" + actor.replace("private:", "");
            }

            requestHelper.doSerializedRequest(ActorModel.class, endpoint,
                    RequestTask.HttpMethod.POST, headers, this.mainContext, false, this);
        } else {
            show(data);
        }
    }

    public void handleActorResponse(ActorModel response) {
        this.clearContext();

        if (response.getError() != null) {
            changeState(State.EXCEPTION, response.getError().getMessage());
        } else {
            notifyListeners(PublicEvent.DO_SHOW, response.getShow(), response.getSpeak());
        }
    }

    // helpers

    /**
     * Perform client-side operations in-place on a server response.
     *
     * @param response
     */
    private void doClientOperations(JsonObject response) {
        replaceLocation(response);
        buildDateTime(response);
//        prependTo(context, response);
    }

    /**
     * Perform prepend_to operations.
     *
     * @param context
     * @param data
     */
    private void prependTo(ClassifierModel context, JsonObject data) {
        if (!data.has("unused_tokens") || data.getAsJsonArray("unused_tokens").size() == 0)
            return;

        // TODO not implemented on backend
//        if (((JSONArray) data.get("unused_tokens")).length() <= 0) {
//            return context;
//        }
//
//        String prepend = "";
//        //String prepend (String) ((JSONArray) data.get("unused_tokens")).
//
//        String field = (String) data.get("prepend_to");
//
//        String payloadField = "";
//
//        if (context.payload.containsKey("field") && context.payload.get(field) != null) {
//            payloadField = " " + (String) context.payload.get(field);
//        }
//
//        context.payload.put(field, (prepend + payloadField));
    }

    /**
     * Replace `#current_location` client operators with latitude / longitude location objects.
     * @param payload
     */
    private void replaceLocation(JsonObject payload) {
        replaceLocation(payload, null);
    }

    /**
     * Inner recursive function. See {@link #replaceLocation(com.google.gson.JsonObject)}.
     * @param payload
     * @param location
     */
    private void replaceLocation(JsonObject payload, JsonObject location) {
        for (Map.Entry<String, JsonElement> entry : payload.entrySet()) {
            if (entry.getValue().isJsonPrimitive()
                    && entry.getValue().getAsString().equals("#current_location")) {
                if (location == null) {
                    Location locationObj = locationTracker.getLocation();
                    if (locationObj != null) {
                        location = new JsonObject();
                        location.addProperty("latitude", locationObj.getLatitude());
                        location.addProperty("longitude", locationObj.getLongitude());
                    }
                }

                payload.add(entry.getKey(), location);
            } else if (entry.getValue().isJsonObject()) {
                replaceLocation(entry.getValue().getAsJsonObject(), location);
            }
        }
    }

    private static Pair[] DATETIME_KEY_NAMES = new Pair[] {
            new Pair("date", "time"),
            new Pair("start_date", "start_time"),
            new Pair("end_date", "end_time")
    };

    /**
     * Process any date/time values stored in model data.
     *
     * @param data
     * @return
     */
    private JsonObject buildDateTime(JsonObject data) {
        if (data != null) {
            for (Pair<String, String> datetime : DATETIME_KEY_NAMES) {
                String first = datetime.first;
                String second = datetime.second;

                if (data.has(first) || data.has(second)) {
                    boolean includeDate = true, includeTime = true;

                    if (!data.has(first))
                        includeDate = false;

                    if (!data.has(second))
                        includeTime = false;

                    Pair<String, String> result;
                    try {
                        result = Datetime.datetimeFromJson(data.get(first), data.get(second));
                    } catch ( ParseException e ) {
                        return data;
                    }

                    if (includeDate)
                        data.addProperty(first, result.first);
                    if (includeTime)
                        data.addProperty(second, result.second);
                }
            }
        }

        return data;
    }

    private JsonObject getLocation() {
        JsonObject ret = new JsonObject();

        Location location = locationTracker.getLocation();
        if (location != null) {
            ret.addProperty("latitude", location.getLatitude());
            ret.addProperty("longitude", location.getLongitude());
        }

        return ret;
    }

    private JsonObject getDeviceInfo() {
        JsonObject deviceInfo = getLocation();

        long time = System.currentTimeMillis();
        int offset = DateTimeZone.getDefault().getOffset(time) / 1000;

        int hours = offset / 3600;
        int minutes = (offset % 3600) / 60;
        String timeOffset = String.format("%d:%02d", hours, minutes);

        deviceInfo.addProperty("timestamp", System.currentTimeMillis() / 1000L);
        deviceInfo.addProperty("timeoffset", timeOffset);

        return deviceInfo;
    }

    @Override
    public void onQueryResponse(Object response) {
        // Dispatch based on response type

        if (response instanceof ClassifierModel)
            handleClassifierResponse((ClassifierModel) response);
        else if (response instanceof ResponderModel)
            handleAuditorResponse((ResponderModel) response);
        else if (response instanceof ActorModel)
            handleActorResponse((ActorModel) response);
        else if (response instanceof LoginResponse)
            handleLoginResponse((LoginResponse) response);
        else if (response instanceof SignupResponse)
            handleSignupResponse((SignupResponse) response);
        else if (response instanceof JsonObject)
            handleDisambiguationResponse((JsonObject) response);
        else
            Log.e(TAG, "unhandled query response");
    }

    @Override
    public void onInternalError() {
        notifyListeners(PublicEvent.INTERNAL_ERROR);
    }

    private enum PublicEvent {
        DO_SHOW, DO_REQUEST_CHOICE, LOGIN_RESPONSE, SIGNUP_RESPONSE, ERROR, INTERNAL_ERROR
    };

    public void addListener(IPlexiListener listener) {
        listeners.add(listener);
    }

    public void removeListener(IPlexiListener listener) {
        listeners.remove(listener);
    }

    private void notifyListeners(PublicEvent event, Object... data) {
        switch (event) {
            case DO_SHOW:
                for (IPlexiListener listener : listeners)
                    listener.show((ShowModel) data[0], (String) data[1]);
                break;
            case DO_REQUEST_CHOICE:
                for (IPlexiListener listener : listeners)
                    listener.requestChoice((Choice[]) data[0]);
                break;
            case LOGIN_RESPONSE:
                for (IPlexiListener listener : listeners)
                    listener.onLoginResponse((LoginResponse) data[0]);
                break;
            case SIGNUP_RESPONSE:
                for (IPlexiListener listener : listeners)
                    listener.onSignupResponse((SignupResponse) data[0]);
                break;
            case ERROR:
                for (IPlexiListener listener : listeners)
                    listener.onError((String) data[0]);
                break;
            case INTERNAL_ERROR:
                for (IPlexiListener listener : listeners)
                    listener.onInternalError();
                break;
            default:
                throw new IllegalArgumentException(
                        "PublicEvent value missing dispatch implementation");
        }
    }
}
