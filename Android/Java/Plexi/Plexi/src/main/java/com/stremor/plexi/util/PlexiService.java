package com.stremor.plexi.util;

import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.os.Binder;
import android.os.CountDownTimer;
import android.os.IBinder;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import android.util.Pair;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.JsonObject;
import com.stremor.plexi.interfaces.IPlexiResponse;
import com.stremor.plexi.interfaces.IPlexiService;
import com.stremor.plexi.models.ActorModel;
import com.stremor.plexi.models.ChoiceModel;
import com.stremor.plexi.models.ClassifierModel;
import com.stremor.plexi.models.DisambiguatorModel;
import com.stremor.plexi.models.ResponderModel;
import com.stremor.plexi.models.ShowModel;
import com.stremor.plexi.models.StateModel;

import org.joda.time.DateTimeZone;
import org.json.JSONArray;

import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by jeffschifano on 10/28/13.
 */
public final class PlexiService extends Service implements IPlexiService, IPlexiResponse, PropertyChangeListener {
    // endpoints
    private static final String CLASSIFIER = "http://casper-cached.stremor-nli.appspot.com/v1";
    private static final String DISAMBIGUATOR = CLASSIFIER + "/disambiguate";
    private static final String RESPONDER = "http://rez.stremor-apier.appspot.com/v1/";
    private static final String PUD = "http://stremor-pud.appspot.com/v1/";

    // tag for logging
    private static final String TAG = "PlexiService";

    private final IBinder mBinder = new LocalBinder();
    private Context context;

    private ArrayList<String> auditorStates = new ArrayList<String>() {{ add("disambiguate"); add("inprogress"); add("choice"); }};

    // gets completed with all the necessary fields in order to fulfill an action
    private ClassifierModel mainContext = null;

    // indicates fields that need to be completed in the main context
    private ResponderModel tempContext = null;

    private StateModel currentState = new StateModel();
    private CountDownTimer contextTimer;
    private String originalQuery;
    private LocationTracker locationTracker;

    public PlexiService(Context context) {
        this.context = context;
        this.currentState.addChangeListener(this);

        createTimer();

        if (this.locationTracker == null) {
            this.locationTracker = new LocationTracker(context);
        }
    }

    @Override
    public IBinder onBind(Intent intent) {
        return mBinder;
    }

    public String getOriginalQuery() { return originalQuery; }

    /**
     * Handles any and all changes in state within the Plexi service. Simply dispatches to other
     * handlers based on state.
     */
    @Override
    public void propertyChange(PropertyChangeEvent e) {
        if (e.getPropertyName().equals("State")) {
            String state = (String) e.getNewValue();

            Object response = currentState.getResponse();

            if ( state.equals("init") )
                classify((String) response);
            else if ( state.equals("audit") )
                auditor((ClassifierModel) response);
            else if ( state.equals("disambiguate") )
                disambiguatePassive((ResponderModel) response);
            else if ( state.equals("disambiguate:personal") )
                //actorInterceptor(mainContext.model);
                disambiguatePersonal((ResponderModel) response);
            else if ( state.equals("disambiguate:active") )
                disambiguateActive((String) response);
            else if ( state.equals("disambiguate:candidate") )
                disambiguateCandidate((String) response);
            else if ( state.equals("inprogress") || state.equals("error") )
                show((ResponderModel) response);
            else if ( state.equals("choice") )
                choiceList((ResponderModel) response);
            else if ( state.equals("restart") )
                restart((ResponderModel) response);
            else if ( state.equals("completed") )
                actor((ResponderModel) response);
            else if ( state.equals("exception") )
                errorMessage((String) response);
        }
    }

    /**
     * Handle arbitrary input provided by the user.
     */
    public void query(String query) {
        originalQuery = query;

        String currState = currentState.getState();
        String newState;

        if ( currState.equals("inprogress") || currState.equals("error") )
            newState = "disambiguate:active";
        else if ( currState.equals("choice") )
            newState = "disambiguate:candidate";
        else
            newState = "init";

        currentState.set(newState, query);
    }

    public void clearContext()
    {
        mainContext = null;
        tempContext = null;
        currentState.reset();

        resetTimer();
    }

    public void resetTimer()
    {
        contextTimer.cancel();
    }

    private void createTimer() {
        contextTimer = new CountDownTimer(2000, 0) {
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
            HashMap<String, Object> simple = response.show.simple;

            if (simple.containsKey("list")) {
                // send response to listener
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
        show(response.show, response.speak);
    }

    // called from actor
    private void show(ShowModel model, String speak) {
        if (model.simple.containsKey("text")) {
            String show = (String) model.simple.get("text");

            String link = null;

            if (model.simple.containsKey("link")) {
                link = (String) model.simple.get("link");
            }

            show(speak, show, link);
        }
    }

    private void show(String speak, String show, String link) {
        Intent intent = new Intent("plexiShow");

        intent.putExtra("speak", speak);
        intent.putExtra("show", show);
        intent.putExtra("link", link);

        LocalBroadcastManager.getInstance(this.context).sendBroadcast(intent);
    }

    private void errorMessage(String message) {
        Intent intent = new Intent("plexiError");

        intent.putExtra("message", message);

        LocalBroadcastManager.getInstance(this.context).sendBroadcast(intent);
    }

    /**
     * Classification methods
     */

    private void classify(String query) {
        requestHelper(ClassifierModel.class, CLASSIFIER + "query=" + query,
                RequestTask.HttpMethod.GET, null, false);
    }

    // classifier response handler
    @Override
    public void onQueryResponse(ClassifierModel response) {
        if (response.error != null) {
            currentState.set("exception", response.error.message);
        } else {
            ClassifierModel context = doClientOperations(response, response.payload);
            currentState.set("audit", context);
        }
    }

    /**
     * Disambiguation methods
     */

    private void disambiguateActive(String data) {
        DisambiguatorModel postData = new DisambiguatorModel();

        postData.payload = data;
        postData.type = tempContext.type;

        requestHelper(HashMap.class, DISAMBIGUATOR + "/active",
                RequestTask.HttpMethod.POST, postData, true);
    }

    private void disambiguateCandidate(String data) {
        HashMap simple = tempContext.show.simple;

        // String field = tempContext.field;

        JSONArray list = (simple.containsKey("list")) ? (JSONArray) simple.get("list") : new JSONArray();

        DisambiguatorModel postData = new DisambiguatorModel();
        postData.payload = data;
        postData.type = tempContext.type;
        postData.candidates = list;

        requestHelper(HashMap.class, DISAMBIGUATOR + "/candidate",
                RequestTask.HttpMethod.POST, postData, true);
    }

    private void disambiguatePassive(ResponderModel data) {
        String field = data.field;
        String type = data.type;

        Object payload = JsonObjectUtil.find(mainContext.payload, field);

        DisambiguatorModel postData = new DisambiguatorModel();

        postData.payload = payload;
        postData.type = type;
        postData.device_info = getDeviceInfo();

        this.requestHelper(HashMap.class, DISAMBIGUATOR + "/passive",
                RequestTask.HttpMethod.POST, postData, true);
    }

    private void disambiguatePersonal(ResponderModel data) {
        String field = data.field;
        String type = data.type;

        Object payload = JsonObjectUtil.find(mainContext.payload, field);

        DisambiguatorModel postData = new DisambiguatorModel();

        postData.payload = payload;
        postData.type = type;

        this.requestHelper(HashMap.class, PUD + "disambiguate",
                RequestTask.HttpMethod.POST, postData, true);
    }

    // disambiguator response handler
    @Override
    public void onQueryResponse(JsonObject response) {
        if (response != null) {
            if (response.has("error")) {
                // TODO
                // currentState.set("exception", error.message);
            } else {
                // make copy of mainContext
                ClassifierModel context;

                try {
                    context = this.mainContext.clone();
                } catch (CloneNotSupportedException e) {
                    Log.e(TAG, "disambiguator response context clone failed");
                    return;
                }

                // do client operations
                doClientOperations(context, response);

                String field = this.tempContext.field;
                String type = this.tempContext.type;

                // replace fields
                if (response.has(type)) {
                    JsonObjectUtil.replace(context.payload, field, response.get(type));
                } else {
                    Log.e(TAG, "disambiguation response is missing type");
                }

                // currentState.set("audit", context);
            }
        }
    }

    public void choice(ChoiceModel choice) {
        // send message to update conversation with choice.text

        String field = tempContext.field;
        JsonObjectUtil.replace(mainContext.payload, field, choice.data);
        currentState.set("audit", mainContext);
    }

    /**
     * Auditor methods
     */

    private void auditor(ClassifierModel context) {
        if (!context.equals(mainContext)) {
            this.mainContext = context;

            this.requestHelper(ResponderModel.class, RESPONDER + "audit",
                    RequestTask.HttpMethod.POST, context, false);
        } else {
            Log.e(TAG, "potential request loop detected");
        }
    }

    // auditor response handler
    @Override
    public void onQueryResponse(ResponderModel response) {
        if (response.error != null) {
            currentState.set("exception", response.error.message);
        } else {
            String state = response.status.replace(" ", "");

            String crossCheck = state.split(":")[0];

            if (auditorStates.contains(crossCheck)) {
                tempContext = response;
                contextTimer.start();
            }

            currentState.set(state, response);
        }
    }

    private void restart(ResponderModel data) {
        if (data.data == null) {
            Log.e(TAG, "missing new replacement context");
        }

        currentState.set("audit", data.data);
    }

    /**
     * Actor methods
     */

    private void actor(ResponderModel data) {
        String actor = data.actor;

        if (actor != null) {
            String endpoint = RESPONDER + "actor/" + actor;

            if (actor.contains("private:")) {
                endpoint = PUD + "actor/" + actor.replace("private:", "");
            }

            requestHelper(ActorModel.class, endpoint, RequestTask.HttpMethod.POST,
                    this.mainContext, false);
        } else {
            show(data);
        }
    }

    @Override
    public void onQueryResponse(ActorModel response) {
        this.clearContext();

        if (response.error != null) {
            this.currentState.set("exception", response.error.msg);
        } else {
            // show(response.show, response.speak);
            // actorResponseHandler(response);

            Intent intent = new Intent("plexiActor");

            intent.putExtra("response", response);

            LocalBroadcastManager.getInstance(this.context).sendBroadcast(intent);
        }
    }

    // helpers
    private void requestHelper(Class<?> type, String endpoint, RequestTask.HttpMethod method,
                               Object data, boolean includeNulls) {
        RequestTask req = new RequestTask(type.getClass(), this, method);

        if (method == RequestTask.HttpMethod.GET) {
            req.execute(endpoint, null);
        } else {
            req.setContentType("application.json");
            req.execute(endpoint, serializeData(data, includeNulls));
        }
    }

    private ClassifierModel doClientOperations(ClassifierModel context, JsonObject response) {
        response = replaceLocation(response);
        response = buildDateTime(response);
        prependTo(context, response);

        return context;
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

    private String serializeData(Object data, boolean includeNulls) {
        Gson gson;

        if (includeNulls) {
            gson = new GsonBuilder().serializeNulls().create();
        } else {
            gson = new Gson();

        }

        return gson.toJson(data);
    }

    @Override
    public void onQueryResponse(Object response) {
        Log.e(TAG, "unhandled query response");
    }

    public class LocalBinder extends Binder {
        public PlexiService getService() { return PlexiService.this; }
    }
}
