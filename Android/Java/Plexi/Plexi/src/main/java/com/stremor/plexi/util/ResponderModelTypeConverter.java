package com.stremor.plexi.util;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;
import com.stremor.plexi.models.ClassifierModel;
import com.stremor.plexi.models.ErrorModel;
import com.stremor.plexi.models.ResponderModel;
import com.stremor.plexi.models.ShowModel;

import java.lang.reflect.Type;

/**
 * Created by jon on 27.12.2013.
 */
public class ResponderModelTypeConverter implements JsonSerializer<ResponderModel>,
        JsonDeserializer<ResponderModel> {
    @Override
    public JsonElement serialize(ResponderModel src, Type srcType, JsonSerializationContext ctx) {
        JsonObject ret = new JsonObject();

        ret.addProperty("status", src.getStatus());
        ret.addProperty("type", src.getType());
        ret.addProperty("field", src.getField());
        ret.add("show", ctx.serialize(src.getShow()));
        ret.addProperty("speak", src.getSpeak());

        if (src.getFollowup() != null)
            ret.addProperty("followup", src.getFollowup());

        if (src.getActor() != null)
            ret.addProperty("actor", src.getActor());

        if (src.getData() != null)
            ret.add("data", ctx.serialize(src.getData()));

        if (src.getError() != null)
            ret.add("error", ctx.serialize(src.getError()));

        return ret;
    }

    @Override
    public ResponderModel deserialize(JsonElement json, Type type, JsonDeserializationContext ctx) {
        JsonObject obj = json.getAsJsonObject();
        ResponderModel model = new ResponderModel(obj.get("status").getAsString());

        // String properties
        if (obj.has("type"))
            model.setType(obj.get("type").getAsString());
        if (obj.has("field"))
            model.setField(obj.get("field").getAsString());
        if (obj.has("speak"))
            model.setSpeak(obj.get("speak").getAsString());
        if (obj.has("followup"))
            model.setFollowup(obj.get("followup").getAsString());
        if (obj.has("actor"))
            model.setActor(obj.get("actor").getAsString());

        // Structured properties
        if (obj.has("show"))
            model.setShow((ShowModel) ctx.deserialize(obj.get("show"), ShowModel.class));
        if (obj.has("data"))
            model.setData((ClassifierModel) ctx.deserialize(obj.get("data"), ClassifierModel.class));
        if (obj.has("error"))
            model.setError((ErrorModel) ctx.deserialize(obj.get("error"), ErrorModel.class));

        return model;
    }
}
