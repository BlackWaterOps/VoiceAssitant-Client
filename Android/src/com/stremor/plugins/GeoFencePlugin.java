package com.stremor.plugins;

import org.apache.cordova.api.CallbackContext;
import org.apache.cordova.api.CordovaPlugin;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.location.LocationManager;
import android.util.Log;

public class GeoFencePlugin extends CordovaPlugin {
	Context ctx;
	
	@Override
	public boolean execute(String action, JSONArray args, CallbackContext callbackContext) throws JSONException {
		try {
			this.ctx = this.cordova.getActivity();
	    	
	    	JSONObject arg_object = args.getJSONObject(0);
			
	    	if (action.equals("add")) {
				this.add(arg_object);
			}
			
			return true;
		} catch (Exception e) {
			return false;
		}
	}

	protected boolean add(JSONObject args) {
		try {
			Log.i("StremorPlugin", "add new proximity alert");
				
			LocationManager lm = (LocationManager)this.ctx.getSystemService(Context.LOCATION_SERVICE);
			
			Intent i = new Intent(this.ctx, GeoFenceReceiver.class);
			
			i.setAction("" + args.getInt("id"));
			i.putExtra("message", args.getString("message"));
			i.putExtra("title", args.getString("title"));
			
			final PendingIntent pi = PendingIntent.getBroadcast(this.ctx, 0, i, 0);
			
			lm.addProximityAlert(args.getDouble("lat"), args.getDouble("lon"), args.getInt("radius"), -1, pi);
			
			return true;			
		} catch (Exception e) {
			Log.i("StremorPlugin", e.getMessage());
			return false;
		}
	}
}