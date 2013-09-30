package com.stremor.plugins;

//import java.util.Calendar;

import org.apache.cordova.api.CallbackContext;
import org.apache.cordova.api.CordovaPlugin;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.provider.AlarmClock;
import android.util.Log;

public class AlarmClockPlugin extends CordovaPlugin {
	
	private Context ctx;
	
	public AlarmClockPlugin() {
		if (Build.VERSION.SDK_INT < 9) {
			// set message
			return;
		}
	}
	
	@Override
	public boolean execute(String action, JSONArray args, CallbackContext callbackContext) throws JSONException {    	    	
		Log.i("StremorPlugin", "alarm clock intent");
		
		try {
	    	this.ctx = this.cordova.getActivity();
	    	
	    	JSONObject arg_object = args.getJSONObject(0);
    		
    		if (action.equals("add")) {
    			this.add(arg_object);
    		} else if (action.equals("cancel")) {
    			this.cancel(arg_object);
    		}
       	
    		callbackContext.success();
    		
    		return true;
    	} catch (Exception e) {
			callbackContext.error(e.getMessage());
    		return false;
    	}    	
    }
	
	protected void add(JSONObject args) throws JSONException {
		try {
			Log.i("StremorPlugin", "add new alarm");
				
			JSONObject date = args.getJSONObject("date");			
			
			Log.i("StremorPlugin", date.getString("hour"));
			Log.i("StremorPlugin", date.getString("minutes"));
			
			Intent i = new Intent(AlarmClock.ACTION_SET_ALARM); 
			i.putExtra(AlarmClock.EXTRA_MESSAGE, args.optString("message")); 
			i.putExtra(AlarmClock.EXTRA_HOUR, date.getInt("hour")); 
			i.putExtra(AlarmClock.EXTRA_MINUTES, date.getInt("minutes"));

			if (Build.VERSION.SDK_INT >= 11) {
				i.putExtra(AlarmClock.EXTRA_SKIP_UI, args.optBoolean("skip"));
			}
			
			this.ctx.startActivity(i);			
		} catch (Exception e) {
			Log.i("StremorPlugin", e.getMessage());			
		}
	}
	
	protected void cancel(JSONObject args) {
		Log.i("StremorPlugin", "cancel existing alarm");
		
	}
}