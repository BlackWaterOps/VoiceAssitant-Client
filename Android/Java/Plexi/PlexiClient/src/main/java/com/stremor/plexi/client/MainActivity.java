package com.stremor.plexi.client;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.os.Bundle;
import android.speech.RecognizerIntent;
import android.speech.tts.TextToSpeech;
import android.util.Base64;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;

import com.stremor.plexi.PlexiService;
import com.stremor.plexi.client.models.ConversationItem;
import com.stremor.plexi.client.views.MainView;
import com.stremor.plexi.interfaces.IPlexiListener;
import com.stremor.plexi.models.Choice;
import com.stremor.plexi.models.LoginResponse;
import com.stremor.plexi.models.ShowModel;
import com.stremor.plexi.models.SignupResponse;
import com.stremor.plexi.util.Installation;

import java.io.UnsupportedEncodingException;
import java.util.List;
import java.util.Locale;

public class MainActivity extends Activity implements MainView.ViewListener, IPlexiListener {

    private static final String TAG = "MainActivity";
    private static final String RECOGNIZER_LANGUAGE_MODEL = RecognizerIntent.LANGUAGE_MODEL_FREE_FORM;
    private static final String RECOGNIZER_LANGUAGE = "en";

    private static final String PREFS_NAME = "PlexiClientPrefs";
    private static final String PREF_AUTH_TOKEN = "authToken";
    static final String PREF_PREFIX_AUTH_PROVIDER = "authProvider_";

    private static final int REQ_RECOGNIZE_SPEECH = 41;
    private static final int REQ_CHECK_TTS = 42;
    private static final int REQ_LOGIN = 1;

    private MainView mView;
    private TextToSpeech mTts;
    private TextToSpeech.OnInitListener ttsInitListener = new TextToSpeech.OnInitListener() {
        @Override
        public void onInit(int status) {
            if (status == TextToSpeech.SUCCESS) {
                mTts.setLanguage(Locale.ENGLISH);
            }
        }
    };

    public static PlexiService sPlexi;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        mView = (MainView) findViewById(R.id.mainView);
        mView.setViewListener(this);

        sPlexi = new PlexiService(this);
        sPlexi.addListener(this);
        sPlexi.setAuthToken(getAuthToken());

        // Check for TTS support
        Intent checkIntent = new Intent(TextToSpeech.Engine.ACTION_CHECK_TTS_DATA);
        startActivityForResult(checkIntent, REQ_CHECK_TTS);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        Intent intent;

        if (item.getItemId() == R.id.plexi_login_or_signup) {
            intent = new Intent(this, LoginActivity.class);
            startActivityForResult(intent, REQ_LOGIN);
            return true;
        } else if (item.getItemId() == R.id.action_services) {
            String authToken = getAuthToken();
            if (authToken == null) {
                // Auth token needed before providers can be added
                new AlertDialog.Builder(this)
                        .setIcon(android.R.drawable.ic_dialog_alert)
                        .setTitle("Cannot add services")
                        .setMessage("Please log in before adding services.")
                        .show();
            } else {
                intent = new Intent(this, AuthProvidersActivity.class);
                intent.putExtra(AuthProvidersActivity.EXTRA_AUTH_TOKEN, authToken);
                intent.putExtra(AuthProvidersActivity.EXTRA_AUTH_DEVICE, Installation.id(this));
                startActivity(intent);
            }
        }

        return super.onOptionsItemSelected(item);
    }

    /**
     * Attempt to retrieve a saved auth token.
     *
     * @return A saved auth token or null if none has been saved.
     */
    private String getAuthToken() {
        try {
            String encoded = getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
                    .getString(PREF_AUTH_TOKEN, null);

            return encoded == null
                    ? null
                    : new String(Base64.decode(encoded, Base64.DEFAULT), "UTF-8");
        } catch (UnsupportedEncodingException e) {
            /* pass */
        }

        return null;
    }

    /**
     * Handle the receipt of a new auth token from the login process.
     *
     * @param newAuthToken
     */
    private void handleAuthToken(String newAuthToken) {
        try {
            // Store auth token, overwriting old copy
            getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE).edit()
                    .putString(PREF_AUTH_TOKEN,
                            Base64.encodeToString(newAuthToken.getBytes("UTF-8"), Base64.DEFAULT))
                    .commit();
        } catch (UnsupportedEncodingException e) {
            String here = "5";
            /* pass */
        }

        sPlexi.setAuthToken(newAuthToken);
    }

    /**
     * Checks if this device supports speech recognition.
     */
    private boolean isSpeechRecognizerPresent() {
        PackageManager pm = getPackageManager();
        List<ResolveInfo> activities = pm.queryIntentActivities(new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH), 0);
        return activities != null && !activities.isEmpty();
    }

    /**
     * Prompt the user to speak.
     *
     * @param prompt        Text prompt to show the user when asking them to speak.
     * @param languageModel Informs the recognizer which speech model to prefer.
     * @param language      Language of speech to expect.
     */
    private void recognizeSpeech(String prompt, String languageModel, String language) {
        Intent i = new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
        i.putExtra(RecognizerIntent.EXTRA_PROMPT, prompt);
        i.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL, languageModel);
        i.putExtra(RecognizerIntent.EXTRA_LANGUAGE, language);

        startActivityForResult(i, REQ_RECOGNIZE_SPEECH);
    }

    /**
     * Speaks via TTS.
     *
     * @param text
     */
    private void speak(String text) {
        mTts.speak(text, TextToSpeech.QUEUE_FLUSH, null);
    }

    /**
     * Show a text response.
     *
     * @param text
     */
    private void showText(String text) {
        mView.addConversationItem(new ConversationItem(text));
    }

    /**
     * Handles a query spoken by the user.
     *
     * Updates the view and calls subsequent processing routines.
     *
     * @param query
     */
    private void handleQuery(String query) {
        mView.addConversationItem(new ConversationItem(query));
        sPlexi.query(query);
    }

    @Override
    public void show(ShowModel showModel, String speakText) {
        speak(speakText);
        showText(showModel.getSimple().getText());
    }

    @Override
    public void requestChoice(final Choice[] choices) {
        String[] choiceStrings = new String[choices.length];
        for (int i = 0; i < choices.length; i++)
            choiceStrings[i] = choices[i].getText();

        DialogInterface.OnClickListener listener = new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                sPlexi.choice(choices[which]);
            }
        };

        new AlertDialog.Builder(this)
                .setItems(choiceStrings, listener)
                .show();
    }

    @Override
    public void onLoginResponse(LoginResponse response) {
        // TODO
    }

    @Override
    public void onSignupResponse(SignupResponse response) {
        // TODO
    }

    @Override
    public void onError(String message) {
        // TODO Special error formatting
        showText(message);
    }

    @Override
    public void onInternalError() {
        // TODO handle? Do we need to pass some kind of error info to be useful to an external
        // client, or is it better to have Plexi act like a black box?
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == REQ_RECOGNIZE_SPEECH) {
            if (resultCode == RESULT_OK) {
                List<String> matches = data.getStringArrayListExtra(RecognizerIntent.EXTRA_RESULTS);
                float[] confidences = data.getFloatArrayExtra(RecognizerIntent.EXTRA_CONFIDENCE_SCORES);

                if (matches.isEmpty())
                    return;

                // TODO: Inspect other matches?
                handleQuery(matches.get(0));

            } else if (resultCode == RecognizerIntent.RESULT_AUDIO_ERROR) {
                Log.e(TAG, "Speech recognition: audio error");
            } else if (resultCode == RecognizerIntent.RESULT_CLIENT_ERROR) {
                Log.e(TAG, "Speech recognition: client error");
            } else if (resultCode == RecognizerIntent.RESULT_NETWORK_ERROR) {
                Log.e(TAG, "Speech recognition: network error");
            } else if (resultCode == RecognizerIntent.RESULT_SERVER_ERROR) {
                Log.e(TAG, "Speech recognition: server error");
            }
        } else if (requestCode == REQ_CHECK_TTS) {
            if (resultCode == TextToSpeech.Engine.CHECK_VOICE_DATA_PASS) {
                mTts = new TextToSpeech(this, ttsInitListener);
            } else {
                // Install TTS data
                Intent installIntent = new Intent(TextToSpeech.Engine.ACTION_INSTALL_TTS_DATA);
                startActivity(installIntent);
            }
        } else if (requestCode == REQ_LOGIN && resultCode == RESULT_OK) {
            String authToken = data.getExtras().getString(LoginActivity.EXTRA_FIELD_AUTH_TOKEN);
            handleAuthToken(authToken);
        }
    }

    public void onPleaseClick(View v) {
        recognizeSpeech("Please say a command.", RECOGNIZER_LANGUAGE_MODEL, RECOGNIZER_LANGUAGE);
    }
}