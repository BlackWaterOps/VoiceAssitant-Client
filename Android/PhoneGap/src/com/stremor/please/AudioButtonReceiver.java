package com.stremor.please;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.view.KeyEvent;

public class AudioButtonReceiver extends BroadcastReceiver {

	@Override
	public void onReceive(Context arg0, Intent i) {
		if ( i.getAction().equals(Intent.ACTION_MEDIA_BUTTON) ) {
			KeyEvent event = (KeyEvent) i
					.getParcelableExtra(Intent.EXTRA_KEY_EVENT);

			switch ( event.getKeyCode() ) {
			case KeyEvent.KEYCODE_MEDIA_PLAY:
			case KeyEvent.KEYCODE_MEDIA_PAUSE:
			case KeyEvent.KEYCODE_HEADSETHOOK:
				please.webView.loadUrl("javascript: $(document).trigger('mediaButton');");
				return;
			}
		}
	}

}
