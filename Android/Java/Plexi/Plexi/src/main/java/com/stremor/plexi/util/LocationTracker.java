package com.stremor.plexi.util;

import android.content.Context;
import android.location.Criteria;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;

/**
 * Created by jeffschifano on 10/30/13.
 */
public final class LocationTracker implements LocationListener {
    private String TAG = "LocationTracker";

    private LocationManager locationManager;
    private boolean isTracking = false;

    public LocationTracker(Context context) {
        locationManager = (LocationManager) context.getSystemService(Context.LOCATION_SERVICE);
    }

    // TODO: Do we need to listen for live updates?

    @Override
    public void onLocationChanged(Location location) {
//        try {
//            double lat = location.getLatitude();
//            double lon = location.getLongitude();
//
//
//        } catch (Exception e) {
//            Log.e(TAG, "on location changed failure");
//        }
    }

    @Override
    public void onStatusChanged(String provider, int status, Bundle extras) {
//        String statusText;
//
//        switch (status) {
//            case LocationProvider.AVAILABLE:
//                statusText = "";
//                break;
//
//            case LocationProvider.TEMPORARILY_UNAVAILABLE:
//                statusText = "";
//                break;
//
//            case LocationProvider.OUT_OF_SERVICE:
//                statusText = "";
//                break;
//
//            default:
//                statusText = "Unknown status";
//                break;
//        }
//
//        if (currentPosition == null) {
//            currentPosition = new HashMap<String, Object>();
//        }
//
//        currentPosition.put("status", status);
//        currentPosition.put("statusText", statusText);
    }

    @Override
    public void onProviderEnabled(String provider) {

    }

    @Override
    public void onProviderDisabled(String provider) {

    }

    public Location getLocation() {
        Criteria criteria = new Criteria();
        String best = locationManager.getBestProvider(criteria, true);
        Location location = locationManager.getLastKnownLocation(best);

        return location;
    }

    public void cancelGetGeolocation() {
        this.locationManager.removeUpdates(this);
    }

    public void startTrackingGeolocation(Criteria criteria, long minTime, float minDistance) {
        if (!isTracking) {
            String best = locationManager.getBestProvider(criteria, true);
            locationManager.requestLocationUpdates(best, minTime, minDistance, this);
            isTracking = true;
        }
    }
}