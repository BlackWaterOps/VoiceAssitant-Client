package com.stremor.plexi.util;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.stremor.plexi.models.ShowModel;
import com.stremor.plexi.models.ShowSimpleModel;

import java.lang.reflect.Type;

/**
 * Created by jon on 27.12.2013.
 */
public class ShowModelTypeConverter implements JsonDeserializer<ShowModel> {
    @Override
    public ShowModel deserialize(JsonElement json, Type type, JsonDeserializationContext ctx) {
        JsonObject obj = json.getAsJsonObject();

        JsonElement simpleEl = obj.get("simple");
        ShowSimpleModel simple = (ShowSimpleModel) (simpleEl.isJsonNull()
                ? null : ctx.deserialize(simpleEl, ShowSimpleModel.class));
        JsonObject structured = obj.get("structured").isJsonNull()
                ? null : obj.getAsJsonObject("structured");

        return new ShowModel(simple, structured);
    }
}
