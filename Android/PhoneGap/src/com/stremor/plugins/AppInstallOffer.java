package com.stremor.plugins;

import org.apache.cordova.api.CallbackContext;
import org.apache.cordova.api.CordovaPlugin;
import org.json.JSONArray;
import org.json.JSONException;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.DialogInterface.OnClickListener;
import android.content.Intent;
import android.net.Uri;

public class AppInstallOffer extends CordovaPlugin {

	@Override
	public boolean execute(String action, JSONArray args,
			CallbackContext callbackContext) throws JSONException {
		if ( action.equals("prompt") ) {
			String title = args.getString(0);
			String message = args.getString(1);
			final String appPackage = args.getString(2);
			
			callbackContext.success();

			new AlertDialog.Builder(cordova.getActivity()).setTitle(title)
					.setMessage(message)
					.setPositiveButton("Open in Market", new OnClickListener() {
						@Override
						public void onClick(DialogInterface arg0, int arg1) {
							Intent i = new Intent(Intent.ACTION_VIEW, Uri.parse("market://details?id=" + appPackage));
							cordova.getActivity().startActivity(i);
						}
					}).setNegativeButton("Cancel", new OnClickListener() {
						@Override
						public void onClick(DialogInterface arg0, int arg1) {
							return;
						}
					}).show();

			return true;
		}

		return false;
	}

}
