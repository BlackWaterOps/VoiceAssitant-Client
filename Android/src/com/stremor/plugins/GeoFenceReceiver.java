package com.stremor.plugins;

import com.stremor.pgtest.R;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

public class GeoFenceReceiver extends BroadcastReceiver {
	
	@Override
	public void onReceive(Context ctx, Intent i) {
		Log.i("StremorPlugin", "GeoFenceReceiver Invoked");
		
		final Bundle bundle = i.getExtras();
		
		int notificationId = 0;

        try {
            notificationId = Integer.parseInt(bundle.getString("NOTIFICATION_ID"));
        } catch (Exception e) {
            Log.d("AlarmReceiver", "Unable to process alarm with id: " + bundle.getString("NOTIFICATION_ID"));
        }
		
		// Construct the notification and notificationManager objects
		final NotificationManager notificationMgr = (NotificationManager)ctx.getSystemService(Context.NOTIFICATION_SERVICE);
        final Notification notification = new Notification(R.drawable.ic_launcher, bundle.getString("ticker"),
                System.currentTimeMillis());
        final PendingIntent contentIntent = PendingIntent.getActivity(ctx, 0, new Intent(), 0);
        notification.defaults |= Notification.DEFAULT_SOUND;
        notification.vibrate = new long[] { 0, 100, 200, 300 };
        notification.setLatestEventInfo(ctx, bundle.getString("title"), bundle.getString("message"), contentIntent);
	}
}
