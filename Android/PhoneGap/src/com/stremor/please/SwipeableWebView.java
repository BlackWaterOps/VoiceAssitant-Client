package com.stremor.please;

import org.apache.cordova.CordovaWebView;

import android.content.Context;
import android.util.AttributeSet;
import android.util.Log;
import android.view.GestureDetector;
import android.view.GestureDetector.SimpleOnGestureListener;
import android.view.MotionEvent;

public class SwipeableWebView extends CordovaWebView {
	private GestureDetector gestureDetector;
	private GestureListener gestureListener;

	private static final int SWIPE_MIN_X_DISTANCE = 120;
	private static final int SWIPE_MAX_Y_DISTANCE = 100;
	private static final int SWIPE_THRESHOLD_VELOCITY = 200;

	public SwipeableWebView(Context context) {
		super(context);
	}

	public SwipeableWebView(Context context, AttributeSet attrs) {
		super(context, attrs);
	}

	public SwipeableWebView(Context context, AttributeSet attrs, int defStyle) {
		super(context, attrs, defStyle);
	}

	public SwipeableWebView(Context context, AttributeSet attrs, int defStyle,
			boolean privateBrowsing) {
		super(context, attrs, defStyle, privateBrowsing);
	}

	@Override
	public void setup() {
		super.setup();
		
		setupGestureDetector();
	}

	public interface GestureListener {
		public void onLeftSwipe(MotionEvent e1, MotionEvent e2);

		public void onRightSwipe(MotionEvent e1, MotionEvent e2);
	}

	public void setGestureListener(GestureListener l) {
		gestureListener = l;
	}

	public GestureListener getGestureListener() {
		return gestureListener;
	}

	private void setupGestureDetector() {
		SimpleOnGestureListener gestureListener = new SimpleOnGestureListener() {
			public boolean onDown(MotionEvent event) {
				return true;
			}

			public boolean onFling(MotionEvent e1, MotionEvent e2,
					float velocityX, float velocityY) {
				if (Math.abs(velocityX) < SWIPE_THRESHOLD_VELOCITY
						|| Math.abs(e2.getY() - e1.getY()) > SWIPE_MAX_Y_DISTANCE) {
					return false;
				}

				if (e1.getX() - e2.getX() > SWIPE_MIN_X_DISTANCE) {
					Log.d("TLDRWebView", "Left swipe");
					onLeftSwipe(e1, e2);
					return true;
				} else if (e2.getX() - e1.getX() > SWIPE_MIN_X_DISTANCE) {
					Log.d("TLDRWebView", "Right swipe");
					onRightSwipe(e1, e2);
					return true;
				}

				return false;
			}
		};

		gestureDetector = new GestureDetector(getContext(), gestureListener);
	}

	@Override
	public boolean onTouchEvent(MotionEvent e) {
		super.onTouchEvent(e);
		return gestureDetector.onTouchEvent(e);
	}

	public void onLeftSwipe(MotionEvent e1, MotionEvent e2) {
		loadUrl("javascript: $(document).trigger('swipeLeft');");

		if (gestureListener != null)
			gestureListener.onLeftSwipe(e1, e2);
	}

	public void onRightSwipe(MotionEvent e1, MotionEvent e2) {
		loadUrl("javascript: $(document).trigger('swipeRight');");

		if (gestureListener != null)
			gestureListener.onRightSwipe(e1, e2);
	}
}
