package com.stremor.plexi;

import android.content.Context;
import android.os.CountDownTimer;
import android.util.Log;
import android.util.Pair;

import com.google.gson.JsonObject;
import com.stremor.plexi.interfaces.IPlexiListener;
import com.stremor.plexi.interfaces.IPlexiService;
import com.stremor.plexi.interfaces.IRequestHelper;
import com.stremor.plexi.interfaces.IResponseListener;
import com.stremor.plexi.models.ActorModel;
import com.stremor.plexi.models.ChoiceModel;
import com.stremor.plexi.models.ClassifierModel;
import com.stremor.plexi.models.DisambiguationCandidate;
import com.stremor.plexi.models.DisambiguatorModel;
import com.stremor.plexi.models.ResponderModel;
import com.stremor.plexi.models.ShowModel;
import com.stremor.plexi.models.StateModel;
import com.stremor.plexi.util.Datetime;
import com.stremor.plexi.util.JsonObjectUtil;
import com.stremor.plexi.util.LocationTracker;
import com.stremor.plexi.util.RequestHelper;
import com.stremor.plexi.util.RequestTask;

import org.joda.time.DateTimeZone;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;

/**
 * Created by jeffschifano on 10/28/13.
 */
public final class PlexiService implements IPlexiService, IResponseListener {
    // endpoints
    private static final String CLASSIFIER = "http://casper-cached.stremor-nli.appspot.com/v1";
    private static final String DISAMBIGUATOR = CLASSIFIER + "/disambiguate";
    private static final String RESPONDER = "http://rez.stremor-apier.appspot.com/v1/";
    private static final String PUD = "http://stremor-pud.appspot.com/v1/";

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

    // Class dependencies
    private IRequestHelper requestHelper;

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

        createTimer();

        if (this.locationTracker == null) {
            this.locationTracker = new LocationTracker(context);
        }
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
                choiceList((ResponderModel) data);
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
                reset();
            }
        };
    }

    private void reset() {
        clearContext();
    }

    private void choiceList(ResponderModel response) {
        try {
            DisambiguationCandidate[] list = response.getShow().getSimple().getList();

            if (list != null) {
                // TODO send response to listener
            } else {
                Log.d(TAG, "no list could be found");
            }
        } catch (Exception e) {
            Log.e(TAG, e.getMessage());
        }
    }

    // called from "inprogress" status
    private void show(ResponderModel response) {
        /*
        try
        {
            PhoneApplicationFrame frame = App.Current.RootVisual as PhoneApplicationFrame;

            if (frame.CurrentSource.Equals(ViewModelLocator.ConversationPageUri))
            {
                Show(response.show, response.speak);
            }
            else
            {
                //ConversationViewModel vm = ViewModelLocator.GetViewModelInstance<ConversationViewModel>();

                // this won't have any speak
                //vm.AddDialog("please", (string)response.show.simple["text"]);

                Show(response.show, response.speak);

                // navigate to conversation.xaml
                this.navigationService.NavigateTo(ViewModelLocator.ConversationPageUri);
            }
        }
        catch (Exception err)
        {
            Debug.WriteLine("Show:inprogress - " + err.Message);
        }
        */
        show(response.getShow(), response.getSpeak());
    }

    // called from actor
    private void show(ShowModel model, String speak) {
        notifyListeners(PublicEvent.SHOW, model, speak);

//        JsonObject simple = model.getSimple();
//        if (simple.has("text")) {
//            String show = simple.get("text").getAsString();
//
//            String link = simple.has("link")
//                    ? link = simple.get("link").getAsString()
//                    : null;
//
//            show(speak, show, link);
//        }
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
                    URLEncoder.encode(query, "utf-8"), RequestTask.HttpMethod.GET, this);
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
        requestHelper.doRequest(JsonObject.class, DISAMBIGUATOR + "/active",
                RequestTask.HttpMethod.POST, postData, true, this);
    }

    private void disambiguateCandidate(String data) {
        DisambiguationCandidate[] candidates = tempContext.getShow().getSimple().getList();
        // String field = tempContext.field;

        candidates = candidates == null ? new DisambiguationCandidate[]{} : candidates;
        DisambiguatorModel postData = new DisambiguatorModel(data, tempContext.getType(),
                candidates);
        requestHelper.doRequest(JsonObject.class, DISAMBIGUATOR + "/candidate",
                RequestTask.HttpMethod.POST, postData, true, this);
    }

    private void disambiguatePassive(ResponderModel data) {
        String field = data.getField();
        String type = data.getType();

        Object payload = JsonObjectUtil.find(mainContext.getPayload(), field);

        DisambiguatorModel postData = new DisambiguatorModel(payload, type, getDeviceInfo());
        requestHelper.doRequest(JsonObject.class, DISAMBIGUATOR + "/passive",
                RequestTask.HttpMethod.POST, postData, true, this);
    }

    private void disambiguatePersonal(ResponderModel data) {
        String field = data.getField();
        String type = data.getType();

        Object payload = JsonObjectUtil.find(mainContext.getPayload(), field);

        DisambiguatorModel postData = new DisambiguatorModel(payload, type);
        requestHelper.doRequest(JsonObject.class, PUD + "disambiguate",
                RequestTask.HttpMethod.POST, postData, true, this);
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
                    /* pass */
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

    public void choice(ChoiceModel choice) {
        // send message to update conversation with choice.text

        String field = tempContext.getField();
        JsonObjectUtil.replace(mainContext.getPayload(), field, choice.data);
        changeState(State.AUDIT, mainContext);
    }

    /**
     * Auditor methods
     */

    private void auditor(ClassifierModel context) {
        if (!context.equals(mainContext)) {
            this.mainContext = context;

            requestHelper.doRequest(ResponderModel.class, RESPONDER + "audit",
                    RequestTask.HttpMethod.POST, context, false, this);
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

            requestHelper.doRequest(ActorModel.class, endpoint, RequestTask.HttpMethod.POST,
                    this.mainContext, false, this);
        } else {
            show(data);
        }
    }

    public void handleActorResponse(ActorModel response) {
        this.clearContext();

        if (response.error != null) {
            changeState(State.EXCEPTION, response.error.getMessage());
        } else {
            notifyListeners(PublicEvent.SHOW, response.show, response.speak);

//            TODO will be possible when ShowModel is more strongly typed
//            Intent intent = new Intent("plexiActor");
//
//            intent.putExtra("response", response);
//
//            LocalBroadcastManager.getInstance(this.context).sendBroadcast(intent);
        }
    }

    // helpers

    /**
     * Perform client-side operations in-place on a server response.
     *
     * @param response
     */
    private void doClientOperations(JsonObject response) {
        response = replaceLocation(response);
        response = buildDateTime(response);
//        prependTo(context, response);
    }

    private void prependTo(ClassifierModel context, JsonObject data) {
        if (!data.has("unused_tokens") || data.getAsJsonArray("unused_tokens").size() == 0)
            return;

        // TODO
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

    private JsonObject replaceLocation(JsonObject payload) {
        // TODO
//        if (payload.get("location") instanceof String) {
//            String location = (String) payload.get("location");
//
//            if (location.contains("current_location")) {
//                // TODO get device info
//                // payload.put("location", deviceInfo);
//            }
//        }

        return payload;
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

    private HashMap<String, Object> getDeviceInfo() {
        HashMap<String, Object> deviceInfo = new HashMap<String, Object>();

        long time = System.currentTimeMillis();
        int offset = DateTimeZone.getDefault().getOffset(time) / 1000;

        int hours = offset / 3600;
        int minutes = (offset % 3600) / 60;
        String timeOffset = String.format("%d:%02d", hours, minutes);

        deviceInfo.put("timestamp", System.currentTimeMillis() / 1000L);
        deviceInfo.put("timeoffset", timeOffset);

        HashMap<String, Object> geolocation = this.locationTracker.getCurrentPosition();

        if (geolocation == null) {
            geolocation = this.locationTracker.getGeolocation();
        }

        if (!geolocation.containsKey("error") && geolocation.size() > 1) {
            deviceInfo.put("latitude", geolocation.get("latitude"));
            deviceInfo.put("longitude", geolocation.get("longitude"));
        } else if (geolocation.containsKey("error")) {
            Log.e(TAG, (String) geolocation.get("error"));
        }

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
        else if (response instanceof JsonObject)
            handleDisambiguationResponse((JsonObject) response);
        else
            Log.e(TAG, "unhandled query response");
    }

    private enum PublicEvent {
        SHOW, ERROR
    };

    public void addListener(IPlexiListener listener) {
        listeners.add(listener);
    }

    public void removeListener(IPlexiListener listener) {
        listeners.remove(listener);
    }

    private void notifyListeners(PublicEvent event, Object... data) {
        switch (event) {
            case SHOW:
                for (IPlexiListener listener : listeners)
                    listener.show((ShowModel) data[0], (String) data[1]);
                break;
            case ERROR:
                for (IPlexiListener listener : listeners)
                    listener.error((String) data[0]);
                break;
            default:
                throw new IllegalArgumentException(
                        "PublicEvent value missing dispatch implementation");
        }
    }
}