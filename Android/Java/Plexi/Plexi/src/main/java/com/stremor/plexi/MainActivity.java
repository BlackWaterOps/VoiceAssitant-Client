package com.stremor.plexi;

import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.os.Bundle;
import android.app.Activity;
import android.os.IBinder;
import android.view.Menu;

import com.stremor.plexi.interfaces.IPlexiService;
import com.stremor.plexi.util.MyTest;
import com.stremor.plexi.util.PlexiService;

public class MainActivity extends Activity {
    MyTest test;

    IPlexiService plexiService;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);


    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main, menu);
        return true;
    }

    @Override
    protected void onStart() {
        super.onStart();

        Intent intent = new Intent(this, MyTest.class);
        bindService(intent, mConnection, Context.BIND_AUTO_CREATE);
    }


    private ServiceConnection mConnection = new ServiceConnection() {
        @Override
        public void onServiceConnected(ComponentName componentName, IBinder iBinder) {
            // MyTest.LocalBinder binder = (MyTest.LocalBinder) iBinder;

            // test = binder.getService();

            // plexiService = test.new PlexiCore();

            PlexiService.LocalBinder binder = (PlexiService.LocalBinder) iBinder;

            plexiService = binder.getService();
        }

        @Override
        public void onServiceDisconnected(ComponentName componentName) {

        }
    };
}
