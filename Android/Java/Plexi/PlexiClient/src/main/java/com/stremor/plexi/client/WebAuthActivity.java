package com.stremor.plexi.client;

import android.content.Intent;
import android.webkit.WebView;
import android.webkit.WebViewClient;

/**
 * Created by jon on 02.01.2014.
 */
public class WebAuthActivity extends WebActivity {
    public static final String EXTRA_AUTH_SERVICE = "authService";

    @Override
    protected WebViewClient getClient() {
        return new WebViewClient() {
            @Override
            public void onPageFinished(WebView webView, String url) {
                if (url.contains("auth/success")) {
                    Intent result = new Intent();
                    result.getExtras().putString(EXTRA_AUTH_SERVICE,
                            getIntent().getStringExtra(EXTRA_AUTH_SERVICE));
                    setResult(RESULT_OK, result);
                    finish();
                }
            }
        };
    }
}
