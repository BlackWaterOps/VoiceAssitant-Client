package com.stremor.plugins;

import org.apache.cordova.api.CallbackContext;
import org.apache.cordova.api.CordovaPlugin;
import org.json.JSONArray;
import org.json.JSONException;

import android.content.Intent;
import android.net.Uri;

public class PhoneCall extends CordovaPlugin {

	@Override
	public boolean execute(String action, JSONArray args,
			CallbackContext callbackContext) throws JSONException {
		if ( action.equals("call") ) {
			final String number = args.getString(0);
			
			cordova.getActivity().runOnUiThread(new Runnable() {
				public void run() {
					Intent callIntent = new Intent(Intent.ACTION_CALL);
					callIntent.setData(Uri.parse("tel:" + number));
					cordova.startActivityForResult(PhoneCall.this, callIntent, 0);
				}
			});
			
			
			callbackContext.success();
			return true;
		}
		
		return false;
	}

}
