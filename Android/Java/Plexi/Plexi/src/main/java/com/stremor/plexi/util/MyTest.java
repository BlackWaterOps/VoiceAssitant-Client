package com.stremor.plexi.util;

import android.app.Service;
import android.content.Intent;
import android.os.Binder;
import android.os.IBinder;

import com.google.gson.JsonObject;
import com.stremor.plexi.interfaces.IPlexiResponse;
import com.stremor.plexi.interfaces.IPlexiService;
import com.stremor.plexi.models.ActorModel;
import com.stremor.plexi.models.ChoiceModel;
import com.stremor.plexi.models.ClassifierModel;
import com.stremor.plexi.models.ResponderModel;

import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;

/**
 * Created by jeffschifano on 11/1/13.
 */
public class MyTest extends Service {
    private final IBinder mBinder = new LocalBinder();

    @Override
    public IBinder onBind(Intent intent) {
        return mBinder;
    }

    public class LocalBinder extends Binder {
        public MyTest getService() {
            return MyTest.this;
        }
    }

    public class PlexiCore implements IPlexiService, IPlexiResponse, PropertyChangeListener {

        @Override
        public String getOriginalQuery() {
            return null;
        }

        @Override
        public void clearContext() {

        }

        @Override
        public void resetTimer() {

        }

        @Override
        public void query(String query) {

        }

        @Override
        public void choice(ChoiceModel choice) {

        }

        @Override
        public void propertyChange(PropertyChangeEvent propertyChangeEvent) {

        }

        @Override
        public void onQueryResponse(Object queryResponse) {

        }

        @Override
        public void onQueryResponse(ClassifierModel queryResponse) {

        }

        @Override
        public void onQueryResponse(JsonObject queryResponse) {

        }

        @Override
        public void onQueryResponse(ResponderModel queryResponse) {

        }

        @Override
        public void onQueryResponse(ActorModel queryResponse) {

        }
    }
}
