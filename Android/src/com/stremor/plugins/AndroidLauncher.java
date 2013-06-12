package com.stremor.plugins;

import org.apache.cordova.api.CallbackContext;
import org.apache.cordova.api.CordovaPlugin;
import org.json.JSONArray;
import org.json.JSONException;

import android.content.ActivityNotFoundException;
import android.content.Intent;
import android.net.Uri;
import android.util.Log;

public class AndroidLauncher extends CordovaPlugin {

	@Override
	public boolean execute(String action, JSONArray args,
			CallbackContext callbackContext) throws JSONException {
		if ( action.equals("view") ) {
			String uri = args.getString(0);

			try {
				Intent i = new Intent(Intent.ACTION_VIEW, Uri.parse(uri));
				cordova.getActivity().startActivity(i);
			} catch ( ActivityNotFoundException e ) {
				callbackContext.error("Activity not found");
			} finally {
				callbackContext.success();
			}

			return true;
		} else if ( action.equals("launch") ) {
			String pkg = args.getString(0);

			try {
				Intent i = cordova.getActivity().getPackageManager()
						.getLaunchIntentForPackage(pkg);
				cordova.getActivity().startActivity(i);
			} catch ( ActivityNotFoundException e ) {
				callbackContext.error("Package not found");
			} finally {
				callbackContext.success();
			}

			return true;
		}

		return false;
	}
}
