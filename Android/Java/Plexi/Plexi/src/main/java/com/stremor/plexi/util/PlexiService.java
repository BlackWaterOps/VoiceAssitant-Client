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
import com.stremor.plexi.interfaces.IPlexiResponse;
import com.stremor.plexi.interfaces.IPlexiService;
import com.stremor.plexi.models.ActorModel;
import com.stremor.plexi.models.ChoiceModel;
import com.stremor.plexi.models.ClassifierModel;
import com.stremor.plexi.models.DisambiguatorModel;
import com.stremor.plexi.models.ResponderModel;
import com.stremor.plexi.models.ShowModel;
import com.stremor.plexi.models.StateModel;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Calendar;
import java.util.HashMap;
import java.util.List;
import java.util.concurrent.TimeUnit;

/**
 * Created by jeffschifano on 10/28/13.
 */
public final class PlexiService extends Service implements IPlexiService, IPlexiResponse, PropertyChangeListener {
    private final IBinder mBinder = new LocalBinder();

    @Override
    public IBinder onBind(Intent intent) {
        return mBinder;
    }

    public class LocalBinder extends Binder {
        public PlexiService getService() { return PlexiService.this; }
    }

    // endpoints
    private static final String CLASSIFIER = "http://casper-cached.stremor-nli.appspot.com/v1";
    private static final String DISAMBIGUATOR = CLASSIFIER + "/disambiguate";
    private static final String RESPONDER = "http://rez.stremor-apier.appspot.com/v1/";
    private static final String PUD = "http://stremor-pud.appspot.com/v1/";

    // tag for logging
    private static final String TAG = "PlexiService";

    private ArrayList<String> auditorStates = new ArrayList<String>() {{ add("disambiguate"); add("inprogress"); add("choice"); }};

    // gets completed with all the necessary fields in order to fulfill an action
    private ClassifierModel mainContext = null;

    // indicates fields that need to be completed in the main context
    private ResponderModel tempContext = null;

    private StateModel currentState = new StateModel();

    private CountDownTimer contextTimer;

    private String originalQuery;

    public String getOriginalQuery() { return this.originalQuery; }

    private Context context;

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
    public void propertyChange(PropertyChangeEvent e) {
        if (e.getPropertyName().equals("State")) {
            String state = (String) e.getNewValue();

            Object response = this.currentState.getResponse();

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

    public void query(String query) {
        this.originalQuery = query;

        String currState = this.currentState.getState();

        String newState;

        if ( currState.equals("inprogress") || currState.equals("error") )
            newState = "disambiguate:active";
        else if ( currState.equals("choice") )
            newState = "disambiguate:candidate";
        else
            newState = "init";

        this.currentState.set(newState, query);
    }

    public void clearContext()
    {
        this.mainContext = null;
        this.tempContext = null;
        this.currentState.reset();

        resetTimer();
    }

    public void resetTimer()
    {
        this.contextTimer.cancel();
    }

    private void createTimer() {
        this.contextTimer = new CountDownTimer(2000, 0) {
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

    private void classify(String query) {
        requestHelper(ClassifierModel.class, CLASSIFIER + "query=" + query, "GET", null, false);
    }

    // classifier response handler
    @Override
    public void onQueryResponse(ClassifierModel response) throws JSONException {
        if (response.error != null) {
            this.currentState.set("exception", response.error.message);
        } else {
            ClassifierModel context = doClientOperations(response, response.payload);

            this.currentState.set("audit", context);
        }
    }

    private void disambiguateActive(String data) {
        // String field = tempContext.field;

        String type = tempContext.type;

        String payload = data;

        DisambiguatorModel postData = new DisambiguatorModel();

        postData.payload = payload;
        postData.type = type;

        this.requestHelper(HashMap.class, DISAMBIGUATOR + "/active", "POST", postData, true);
    }

    private void disambiguateCandidate(String data) {
        HashMap simple = tempContext.show.simple;

        // String field = tempContext.field;

        String type = tempContext.type;

        JSONArray list = (simple.containsKey("list")) ? (JSONArray) simple.get("list") : new JSONArray();

        String payload = data;

        DisambiguatorModel postData = new DisambiguatorModel();

        postData.payload = payload;
        postData.type = type;
        postData.candidates = list;

        this.requestHelper(HashMap.class, DISAMBIGUATOR + "/candidate", "POST", postData, true);
    }

    private void disambiguatePassive(ResponderModel data) {
        String field = data.field;

        String type = data.type;

        Object payload;

        if (field.contains(".")) {
            payload = find(field);
        } else {
            payload = this.mainContext.payload.get(field);
        }

        HashMap<String, Object> deviceInfo = new HashMap<String, Object>();

        deviceInfo = this.getDeviceInfo();

        DisambiguatorModel postData = new DisambiguatorModel();

        postData.payload = payload;
        postData.type = type;
        postData.device_info = deviceInfo;

        this.requestHelper(HashMap.class, DISAMBIGUATOR + "/passive", "POST", postData, true);
    }

    private void disambiguatePersonal(ResponderModel data) {
        String field = data.field;

        String type = data.type;

        Object payload;

        if (field.contains(".")) {
            payload = find(field);
        } else {
            payload = this.mainContext.payload.get(field);
        }

        DisambiguatorModel postData = new DisambiguatorModel();

        postData.payload = payload;
        postData.type = type;

        this.requestHelper(HashMap.class, PUD + "disambiguate", "POST", postData, true);
    }

    // disambiguator response handler
    @Override
    public void onQueryResponse(HashMap<String, Object> response) throws JSONException {
        if (response != null) {
            if (response.containsKey("error")) {
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
                context = doClientOperations(context, response);

                String field = this.tempContext.field;

                String type = this.tempContext.type;

                // replace fields
                if (response.containsKey(type)) {
                    if (field.contains(".")) {
                        context = replace(context, field, response.get(type));
                    } else {
                        context.payload.put(field, response.get(type));
                    }
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

        ClassifierModel context;

        try {
            context = this.mainContext.clone();

        } catch (CloneNotSupportedException e) {
            Log.e(TAG, "Auditor choice cloning failed");
            return;
        }

        if (field.contains(".")) {
            context = replace(this.mainContext, field, choice.data);
        } else {
            context.payload.put(field, choice.data);
        }

        currentState.set("audit", context);
    }

    private void auditor(ClassifierModel context) {
        if (!context.equals(mainContext)) {
            this.mainContext = context;

            this.requestHelper(ResponderModel.class, RESPONDER + "audit", "POST", context, false);
        } else {
            Log.e(TAG, "potential request loop detected");
        }
    }

    // auditor response handler
    @Override
    public void onQueryResponse(ResponderModel response) throws JSONException {
        if (response.error != null) {
            currentState.set("exception", response.error.message);
        } else {
            String state = response.status.replace(" ", "");

            String crossCheck = state.split(":")[0];

            if (this.auditorStates.contains(crossCheck)) {
                this.tempContext = response;
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

    private void actor(ResponderModel data) {
        String actor = data.actor;

        if (actor != null) {
            String endpoint = RESPONDER + "actor/" + actor;

            if (actor.contains("private:")) {
                endpoint = PUD + "actor/" + actor.replace("private:", "");
            }

            requestHelper(ActorModel.class, endpoint, "POST", this.mainContext, false);
        } else {
            show(data);
        }
    }

    @Override
    public void onQueryResponse(ActorModel response) throws JSONException {
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
    private void requestHelper(Class<?> type, String endpoint, String method, Object data, Boolean includeNulls) {
        RequestTask req = new RequestTask(type.getClass(), this);

        req.Method = method;

        if (method.equalsIgnoreCase("get")) {
            req.execute(endpoint, null);
        } else {
            req.ContentType = "application/json";

            req.execute(endpoint, serializeData(data, includeNulls));
        }
    }

    private ClassifierModel doClientOperations(ClassifierModel context, HashMap<String, Object> response) {
        response = replaceLocation(response);
        response = buildDateTime(response);
        context = prependTo(context, response);

        return context;
    }

    private ClassifierModel prependTo(ClassifierModel context, HashMap<String, Object> data) {
        try {
            if (!data.containsKey("unused_tokens")) {
                return context;
            }

            if (((JSONArray) data.get("unused_tokens")).length() <= 0) {
                return context;
            }

            String prepend = "";
            //String prepend (String) ((JSONArray) data.get("unused_tokens")).

            String field = (String) data.get("prepend_to");

            String payloadField = "";

            if (context.payload.containsKey("field") && context.payload.get(field) != null) {
                payloadField = " " + (String) context.payload.get(field);
            }

            context.payload.put(field, (prepend + payloadField));
        } catch (Exception e) {
            Log.e(TAG, e.getMessage());
        }

        return context;
    }

    private ClassifierModel replace(ClassifierModel context, String field, final Object type) {
        List<String> fields = Arrays.asList(field.split("."));

        final String last = fields.remove(fields.size() - 1);

        // convert to generic object
        Object obj = deepCopy(Object.class, context);

        Object t = new Reduce<String, Object>() {
            public Object function(Object a, String b) {
              try {
                    if (b == last) {
                        return ((JSONObject) a).put(b, type);
                    } else {
                        return ((JSONObject) a).get(b);
                    }
              } catch (JSONException e) {
                  Log.e(TAG, "replace reduce error");
                  Log.e(TAG, e.getMessage());
                  return null;
              }
            }
        }.apply(fields, context);

        return deepCopy(ClassifierModel.class, obj);
    }

    private Object find(String field) {
        List<String> fields = Arrays.asList(field.split("."));

        // convert to generic object
        Object context = deepCopy(Object.class, this.mainContext);

        return new Reduce<String, Object>() {
            public Object function(Object a, String b) {
                try {
                    return ((JSONObject) a).get(b);
                } catch (JSONException e) {
                    Log.e(TAG, "find reduce error");
                    Log.e(TAG, e.getMessage());
                    return null;
                }
            }
        }.apply(fields, context);
    }

    private HashMap<String, Object> replaceLocation(HashMap<String, Object> payload) {
        try {
            if (payload.containsKey("location") && payload.get("location") != null) {
                if (payload.get("location").getClass() == String.class) {
                    String location = (String) payload.get("location");

                    if (location.contains("current_location")) {
                        // get device info

                        // payload.put("location", deviceInfo);
                    }
                }
            }
        } catch (Exception e) {
            Log.e(TAG, e.getMessage());
        }

        return payload;
    }

    private HashMap<String, Object> buildDateTime(HashMap<String, Object> data) {
        try {
            if (data != null) {
                ArrayList<Pair<String, String>> datetimes = new ArrayList<Pair<String, String>>();

                datetimes.add(new Pair<String, String>("date", "time"));
                datetimes.add(new Pair<String, String>("start_date", "start_time"));
                datetimes.add(new Pair<String, String>("end_date", "end_time"));

                for(Pair<String, String> datetime : datetimes) {
                    String first = datetime.first;
                    String second = datetime.second;

                    if (data.containsKey(first) || data.containsKey(second)) {
                        Boolean removeDate = null;
                        Boolean removeTime = null;

                        if (!data.containsKey(first)) {
                            data.put(first, null);
                            removeDate = true;
                        }

                        if (!data.containsKey(second)) {
                            data.put(second, null);
                            removeTime = true;
                        }

                        if (data.get(first) != null || data.get(second) != null) {
                            Pair<String, String> build = Datetime.BuildDatetimeFromJson(data.get(first),
                                    data.get(second));

                            if (data.get(first) != null) {
                                data.put(first, build.first);
                            }

                            if (data.get(second) != null) {
                                data.put(second, build.second);
                            }
                        }

                        if (removeDate == true) {
                            data.remove(first);
                        }

                        if (removeTime == true) {
                            data.remove(second);
                        }
                    }
                }
            }
        } catch (Exception e) {
            Log.e(TAG, e.getMessage());
        }

        return data;
    }

    private HashMap<String, Object> getDeviceInfo() {
        HashMap<String, Object> deviceInfo = new HashMap<String, Object>();

        Calendar cal = Calendar.getInstance();

        int raw = cal.getTimeZone().getRawOffset();

        long hours = TimeUnit.MILLISECONDS.toHours(raw);
        long minutes = TimeUnit.MILLISECONDS.toMinutes(raw) - TimeUnit.HOURS.toMinutes(hours);

        String timeOffset = String.format("%d:%02d", hours, minutes);

        deviceInfo.put("timestamp", Datetime.ConvertToUnixTimestamp(cal));
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

    // hacky clone method !!
    private <T> T deepCopy(Class<T> type, Object obj) {
        Gson gson = new GsonBuilder().serializeNulls().create();

        String json = gson.toJson(obj);

        return (T) gson.fromJson(json, type.getClass());
    }

    private String serializeData(Object data, Boolean includeNulls) {
        Gson gson;

        if (includeNulls == true) {
            gson = new GsonBuilder().serializeNulls().create();
        } else {
            gson = new Gson();

        }

        return gson.toJson(data);
    }

    @Override
    public void onQueryResponse(Object response) throws JSONException {
        Log.e(TAG, "unhandled query response");
    }
}
