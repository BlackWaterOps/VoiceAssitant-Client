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
 * - Auditor requests active disambiguation on string field.
 * - Auditor accepts after active disambiguation completed.
 * - Actor is a stub.
 *
 * Created by jon on 20.12.2013.
 */
public class RequestHelperStub3 implements IRequestHelper {
    private static JsonParser parser = new JsonParser();

    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method,
                              IResponseListener listener) {
        if (type == ClassifierModel.class) {
            ClassifierModel response = new ClassifierModel();
            response.model = "calendar";
            response.action = "create";
            response.payload = parser.parse("{\"name\":null}").getAsJsonObject();

            listener.onQueryResponse(response);
        }
    }

    public <T> void doRequest(Class<T> type, String endpoint, RequestTask.HttpMethod method,
                              Object data, boolean includeNulls, IResponseListener listener) {
        if (type == JsonObject.class && endpoint.contains("disambiguate")) {
            // This is a disambiguation call.
            assert data instanceof DisambiguatorModel;
            assert ((DisambiguatorModel) data).type.equals("string");

            JsonObject response = parser.parse("{\"string\": \"Party\"}").getAsJsonObject();
            listener.onQueryResponse(response);
        } else if (type == ResponderModel.class) {
            // This is an auditor call.
            ClassifierModel pkg = (ClassifierModel) data;

            JsonElement name = pkg.payload.get("name");
            ResponderModel response = new ResponderModel();

            if (name.isJsonNull()) {
                // Needs disambiguation!
                response.status = "in progress";
                response.field = "name";
                response.type = "string";

                ShowModel show = new ShowModel();
                show.simple = parser.parse("{\"text\":\"What is the name of the event?\"}")
                        .getAsJsonObject();
                response.show = show;
                response.speak = "What is the name of the event?";
            } else if (name.isJsonPrimitive() && name.getAsJsonPrimitive().isString()) {
                response.status = "completed";
                response.actor = "foobar";
                response.data = pkg;
            } else {
                assert false;
            }

            listener.onQueryResponse(response);
        } else if (type == ActorModel.class) {
            // This is an actor call.
            ActorModel response = new ActorModel();
            response.speak = "Hello world";

            ShowModel show = new ShowModel();
            show.simple = parser.parse("{\"text\":\"Hello world\"}").getAsJsonObject();
            response.show = show;

            listener.onQueryResponse(response);
        }
    }
}
