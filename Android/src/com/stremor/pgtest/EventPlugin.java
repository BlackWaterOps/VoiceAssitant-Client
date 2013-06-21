package com.stremor.pgtest;

import org.apache.cordova.api.CallbackContext;
import org.apache.cordova.api.CordovaPlugin;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.database.Cursor;
import android.net.Uri;
import android.content.*;

public class EventPlugin extends CordovaPlugin {
	Context context;
	
	public EventPlugin() {
		context = super.cordova.getActivity().getApplicationContext();
	}
	
	@Override
	public boolean execute(String action, JSONArray args, CallbackContext callbackContext) throws JSONException {
		try {
			if (action.equals("addEvent")) {
				JSONObject arg_object = args.getJSONObject(0);
								
				Cursor cursor = context.getContentResolver().query(Uri.parse("content://com.android.calendar/calendars"), new String[]{"_id", "displayname"}, null, null, null);

				cursor.moveToFirst();
				// Get calendars name
				String calendarNames[] = new String[cursor.getCount()];
				// Get calendars id
				int[] calendarId = new int[cursor.getCount()];
				for (int i = 0; i < calendarNames.length; i++)
				{
				         calendarId[i] = cursor.getInt(0);
				         calendarNames[i] = cursor.getString(1);
				         cursor.moveToNext();
				}
				cursor.close();

				ContentValues contentEvent = new ContentValues();
				contentEvent.put("calendar_id", 1);
				contentEvent.put("title", arg_object.getString("title"));
				contentEvent.put("eventLocation", arg_object.getString("eventLocation"));                
				contentEvent.put("dtstart", arg_object.getLong("startTimeMillis"));
				contentEvent.put("dtend", arg_object.getLong("endTimeMillis"));
				contentEvent.put("description", arg_object.getString("description"));
				 
				 /*
				  *.setType("vnd.android.cursor.item/event")
					.putExtra("beginTime", arg_object.getLong("startTimeMillis"))
					.putExtra("endTime", arg_object.getLong("endTimeMillis"))
					.putExtra("title", arg_object.getString("title"))
					.putExtra("description", arg_object.getString("description"))
					.putExtra("eventLocation", arg_object.getString("eventLocation"));
 
				  */

				 Uri eventsUri = Uri.parse("content://com.android.calendar/events");
				 context.getContentResolver().insert(eventsUri, contentEvent);
				 
				 callbackContext.success();
				 
				 return true;
			}
		} catch (Exception e) {
			return false;
		}
		
		return false;
	}
}
