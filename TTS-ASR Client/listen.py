import android
droid = android.Android()
(id, result, error) = droid.recognizeSpeech("Say something")
print result, error