package com.stremor.plexi.util.test;

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParser;
import com.stremor.plexi.interfaces.IRequestHelper;
import com.stremor.plexi.interfaces.IResponseListener;
import com.stremor.plexi.models.ActorModel;
import com.stremor.plexi.models.ClassifierModel;
import com.stremor.plexi.models.DisambiguatorModel;
import com.stremor.plexi.models.ResponderModel;
import com.stremor.plexi.models.ShowModel;
import com.stremor.plexi.util.RequestTask;

/**
 * An IRequestHelper stub implementation which simulates the following scenario:
 *
 * - Fake classification immediately returned.
 * - Auditor requests passive disambiguation on string field.
 * - Auditor accepts after passive disambiguation completed.
 * - Actor is a stub.
 *
 * Created by jon on 20.12.2013.
 */
public class RequestHelperStub4 implements IRequestHelper {
    private static JsonParser parser = new JsonParser();

    private static JsonObject locationObject = parser.parse("{" +
            "\"city\":\"san francisco\"," +
            "\"dst\": true," +
            "\"latitude\": 37.784827," +
            "\"longitude\": -122.727802," +
            "\"state\": \"ca\"," +
            "\"time_offset\": -8," +
            "\"zipcode\": 94188}").getAsJsonObject();

    public static ClassifierModel MODEL_START = new ClassifierModel("location", "query",
            parser.parse("{\"location\":null}").getAsJsonObject());

    public static ClassifierModel MODEL_COMPLETE = new ClassifierModel("location", "query",
            parser.parse("{\"location\":" + locationObject.toString() + "}").getAsJsonObject());

    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method,
                              IResponseListener listener) {
        if (type == ClassifierModel.class) {
            ClassifierModel response = null;
            try {
                response = MODEL_START.clone();
            } catch (CloneNotSupportedException e) { /* pass */ }

            listener.onQueryResponse(response);
        }
    }

    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method,
                              Object data, boolean includeNulls, IResponseListener listener) {
        if (type == JsonObject.class && endpoint.contains("disambiguate")) {
            // This is a disambiguation call.
            assert data instanceof DisambiguatorModel;
            assert ((DisambiguatorModel) data).getType().equals("string");

            JsonObject response = parser.parse(
                    "{\"location\": "
                        + locationObject.toString()
                      + "}")
                    .getAsJsonObject();

            listener.onQueryResponse(response);
        } else if (type == ResponderModel.class) {
            // This is an auditor call.
            ClassifierModel pkg = (ClassifierModel) data;

            JsonElement location = pkg.getPayload().get("location");
            ResponderModel response = null;

            if (location.isJsonNull()) {
                // Needs disambiguation!
                response = new ResponderModel("disambiguate", "location", "location");
            } else if (location.isJsonObject() && location.equals(locationObject)) {
                response = new ResponderModel("completed", "foobar", null, pkg);
            } else {
                assert false;
            }

            listener.onQueryResponse(response);
        } else if (type == ActorModel.class) {
            // This is an actor call.
            ActorModel response = new ActorModel();
            response.speak = "Hello world";

            ShowModel show = new ShowModel(parser.parse("{\"text\":\"Hello world\"}")
                    .getAsJsonObject(), null);
            response.show = show;

            listener.onQueryResponse(response);
        }
    }
}
