package com.stremor.plexi.util;

import android.content.Context;
import android.location.Criteria;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.location.LocationProvider;
import android.os.Bundle;
import android.util.Log;

import java.util.HashMap;

/**
 * Created by jeffschifano on 10/30/13.
 */
public final class LocationTracker implements LocationListener {
    /*
    private static LocationTracker ourInstance = new LocationTracker();

    public static LocationTracker getInstance() {
        return ourInstance;
    }
    */

    private String TAG = "LocationTracker";

    private LocationManager locationManager;

    private Context context;

    private boolean isTracking = false;

    private HashMap<String, Object> currentPosition;
    public HashMap<String, Object> getCurrentPosition() { return currentPosition; }

    public LocationTracker(Context context) {
        this.context = context;

        this.locationManager = (LocationManager) context.getSystemService(Context.LOCATION_SERVICE);
    }

    @Override
    public void onLocationChanged(Location location) {
        try {
            double lat = location.getLatitude();
            double lon = location.getLongitude();


        } catch (Exception e) {
            Log.e(TAG, "on location changed failure");
        }
    }

    @Override
    public void onStatusChanged(String provider, int status, Bundle extras) {
        String statusText;

        switch (status) {
            case LocationProvider.AVAILABLE:
                statusText = "";
                break;

            case LocationProvider.TEMPORARILY_UNAVAILABLE:
                statusText = "";
                break;

            case LocationProvider.OUT_OF_SERVICE:
                statusText = "";
                break;

            default:
                statusText = "Unknown status";
                break;
        }

        if (currentPosition == null) {
            currentPosition = new HashMap<String, Object>();
        }

        currentPosition.put("status", status);
        currentPosition.put("statusText", statusText);
    }

    @Override
    public void onProviderEnabled(String provider) {

    }

    @Override
    public void onProviderDisabled(String provider) {

    }

    public HashMap<String, Object> getGeolocation() {
        Criteria criteria = new Criteria();

        String best = locationManager.getBestProvider(criteria, true);

        Location location = locationManager.getLastKnownLocation(best);

        setCurrentLocation(location.getLatitude(), location.getLongitude());

        return currentPosition;
    }

    public void cancelGetGeolocation() {
        this.locationManager.removeUpdates(this);
    }

    public void startTrackingGeolocation(Criteria criteria, long minTime, float minDistance) {
        if (this.isTracking == false) {
            String best = this.locationManager.getBestProvider(criteria, true);

            Location location = this.locationManager.getLastKnownLocation(best);

            setCurrentLocation(location.getLatitude(), location.getLongitude());

            //this.locationManager.requestLocationUpdates(best, 300000, 50, this);
            this.locationManager.requestLocationUpdates(best, minTime, minDistance ,this);

            this.isTracking = true;
        }
    }

    public boolean isLocationEnabled() {
        return true;
    }

    private void setCurrentLocation(double lat, double lon) {
        if (currentPosition == null) {
            currentPosition = new HashMap<String, Object>();
        }

        currentPosition.put("latitude", lat);
        currentPosition.put("longitude", lon);
    }
}