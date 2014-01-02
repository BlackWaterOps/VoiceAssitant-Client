package com.stremor.plexi.client;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.webkit.WebView;
import android.webkit.WebViewClient;

import java.util.HashMap;
import java.util.Map;

/**
 * Created by jon on 02.01.2014.
 */
public class WebAuthActivity extends Activity {
    public static final String EXTRA_URL = "url";
    public static final String EXTRA_AUTH_PROVIDER = "authProvider";
    public static final String EXTRA_AUTH_TOKEN = "authToken";
    public static final String EXTRA_AUTH_DEVICE = "authDevice";

    private static final String HEADER_AUTH_TOKEN = "Stremor-Auth-Token";
    private static final String HEADER_AUTH_DEVICE = "Stremor-Auth-Device";

    private String authToken;
    private String authDevice;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_web_auth);

        WebView webView = (WebView) findViewById(R.id.webview);
        webView.getSettings().setJavaScriptEnabled(true);

        webView.setWebViewClient(new AuthWebViewClient());

        String url = getIntent().getExtras().getString(EXTRA_URL);

        Map<String, String> headers = new HashMap<String, String>();
        headers.put(HEADER_AUTH_TOKEN, getIntent().getStringExtra(EXTRA_AUTH_TOKEN));
        headers.put(HEADER_AUTH_DEVICE, getIntent().getStringExtra(EXTRA_AUTH_DEVICE));

        webView.loadUrl(url, headers);
    }

    private class AuthWebViewClient extends WebViewClient {
        @Override
        public void onPageFinished(WebView webView, String url) {
            if (url.contains("auth/success")) {
                Intent result = new Intent();
                result.putExtra(EXTRA_AUTH_PROVIDER,
                        getIntent().getStringExtra(EXTRA_AUTH_PROVIDER));
                setResult(RESULT_OK, result);
                finish();
            }
        }

    }
}
