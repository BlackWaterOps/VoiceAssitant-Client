package com.stremor.plugins.calendar;

import java.text.SimpleDateFormat;
import java.util.Date;

import org.apache.cordova.api.CallbackContext;
import org.apache.cordova.api.CordovaPlugin;
import org.json.JSONArray;
import org.json.JSONException;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.DialogInterface.OnClickListener;
import android.view.LayoutInflater;
import android.widget.CalendarView;
import android.widget.LinearLayout;

import com.stremor.pgtest.R;

public class CalendarDialog extends CordovaPlugin {

	private static final SimpleDateFormat dateFormat = new SimpleDateFormat(
			"yyyy-MM-dd'T'HH:mm");

	public CallbackContext context;

	@Override
	public boolean execute(String action, JSONArray args,
			CallbackContext callbackContext) throws JSONException {
		if ( action.equals("getDate") ) {
			context = callbackContext;

			cordova.getActivity().runOnUiThread(new Runnable() {
				public void run() {
					LayoutInflater inflater = (LayoutInflater) cordova
							.getActivity().getApplicationContext()
							.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
					LinearLayout ll = (LinearLayout) inflater.inflate(
							R.layout.calendar_dialog, null, false);
					final CalendarView calendar = (CalendarView) ll
							.getChildAt(0);

					OnClickListener okListener = new OnClickListener() {
						public void onClick(DialogInterface dialog, int button) {
							String dateString = dateFormat.format(new Date(
									calendar.getDate()));
							context.success(dateString);
						}
					};

					OnClickListener cancelListener = new OnClickListener() {
						public void onClick(DialogInterface dialog, int button) {
							context.success((String) null);
						}
					};

					new AlertDialog.Builder(cordova.getActivity())
							.setTitle("Calendar").setView(ll)
							.setPositiveButton("OK", okListener)
							.setNegativeButton("Cancel", cancelListener).show();
				}
			});

			return true;
		}

		return false;
	}

}
