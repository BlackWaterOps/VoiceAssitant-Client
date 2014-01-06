package com.stremor.plexi.client;

import android.app.ListActivity;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.ListView;

public class AuthProvidersActivity extends ListActivity {

    public static final String EXTRA_AUTH_DEVICE = "authDevice";
    public static final String EXTRA_AUTH_TOKEN = "authToken";

    private static final int REQ_DO_AUTH = 1;

    private static final String AUTH_URL = "http://stremor-pud.appspot.com/auth";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_auth_providers);
    }

    /**
     * Store the fact that the given provider has been authenticated.
     *
     * @param providerId
     */
    private void saveProvider(String providerId) {
        getPreferences(Context.MODE_PRIVATE).edit()
                .putBoolean(MainActivity.PREF_PREFIX_AUTH_PROVIDER + providerId, true).commit();

        // TODO alert user somehow
    }

    @Override
    protected void onListItemClick(ListView i, View v, int position, long id) {
        String providerId = getResources().getStringArray(R.array.auth_provider_ids)[position];
        String url = AUTH_URL + "/" + providerId;

        Intent intent = new Intent(this, WebAuthActivity.class);
        intent.putExtra(WebAuthActivity.EXTRA_URL, url);
        intent.putExtra(WebAuthActivity.EXTRA_AUTH_PROVIDER, providerId);
        intent.putExtra(WebAuthActivity.EXTRA_AUTH_TOKEN,
                getIntent().getStringExtra(EXTRA_AUTH_TOKEN));
        intent.putExtra(WebAuthActivity.EXTRA_AUTH_DEVICE,
                getIntent().getStringExtra(EXTRA_AUTH_DEVICE));
        startActivityForResult(intent, REQ_DO_AUTH);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == REQ_DO_AUTH) {
            String authProvider = data.getStringExtra(WebAuthActivity.EXTRA_AUTH_PROVIDER);
            saveProvider(authProvider);
        }
    }
}
