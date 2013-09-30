/*
       Licensed to the Apache Software Foundation (ASF) under one
       or more contributor license agreements.  See the NOTICE file
       distributed with this work for additional information
       regarding copyright ownership.  The ASF licenses this file
       to you under the Apache License, Version 2.0 (the
       "License"); you may not use this file except in compliance
       with the License.  You may obtain a copy of the License at

         http://www.apache.org/licenses/LICENSE-2.0

       Unless required by applicable law or agreed to in writing,
       software distributed under the License is distributed on an
       "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
       KIND, either express or implied.  See the License for the
       specific language governing permissions and limitations
       under the License.
 */

package com.stremor.please;

import org.apache.cordova.Config;
import org.apache.cordova.DroidGap;

import android.content.ComponentName;
import android.content.Context;
import android.media.AudioManager;
import android.os.Bundle;
import android.webkit.WebView;

public class please extends DroidGap
{
    public static WebView webView;
    
	@Override
    public void onCreate(Bundle savedInstanceState)
    {
        AudioManager am = (AudioManager) getContext().getSystemService(
        		Context.AUDIO_SERVICE);
        
        am.registerMediaButtonEventReceiver(
        		new ComponentName(getContext(), AudioButtonReceiver.class));
		
        super.onCreate(savedInstanceState);
        // Set by <content src="index.html" /> in config.xml
        super.loadUrl(Config.getStartUrl());
        //super.loadUrl("file:///android_asset/www/index.html")
        
        webView = this.appView;
    }
}

