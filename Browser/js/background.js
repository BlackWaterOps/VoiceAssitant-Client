/**
 * Listens for the app launching then creates the window
 *
 * @see http://developer.chrome.com/trunk/apps/app.runtime.html
 * @see http://developer.chrome.com/trunk/apps/app.window.html
 */
chrome.app.runtime.onLaunched.addListener(function() {
	chrome.app.window.create('../main.html', {
		//width: 768, 
		//height: 1024, 
		minWidth: 768, 
		minHeight: 1024, 
		//maxWidth: 768, 
		//maxHeight: 1024
	}, function(win) {
		win.contentWindow.addEventListener('message', function(e) {
			var data = e.data;
			if (e.data.action !== null && e.data.action == 'speak') {
				speak(e.data.speak, e.data.options);
			}
		});
	});
});

/*
chrome.app.runtime.onRestart.addListener(function() {
	// Do stuff on restart
});

chrome.runtime.onSuspend.addListener(function() { 
  // Do some simple clean-up tasks.
});
*/

function speak(str, options) {
	if (options == null) {
		options = {};
	}

	if (options.rate == null) {
		options.rate = 1.0;
	}

	if (options.pitch == null) {
		options.pitch = 1.0;
	}

	if (options.volume == null) {
		options.volume = 1.0;
	}

	if ((typeof chrome !== "undefined" && chrome !== null) && chrome.tts !== null && chrome.tts.speak !== null) {
		console.log(str);
		return chrome.tts.speak(str, options);
	}
}