$(function () {
    var api = cordova.require('please/api'),
        actions = cordova.require('please/actions'),
        capture,
        activeContact,
        activeContactTel;

    document.addEventListener("deviceready", onDeviceReady, false);

    var myScroll = new iScroll('wrapper', {checkDOMChanges: true});
    var hite = $('#wrapper').innerHeight();
    function refreshiScroll () {
        setTimeout(function () {
            myScroll.refresh();
            if ($('.console').innerHeight() > hite) {
                myScroll.scrollToElement('.console p:last-child', 250);
            }
        }, 0);
    }



    function onDeviceReady() {
        window.plugins.tts.startup(startupWin, startupFail);
        window.plugins.speechrecognizer.init(speechInitOk, speechInitFail);       
    }

    function startupWin(result) {
        // When result is equal to STARTED we are ready to play
        if (result == TTS.STARTED) {
            $('.console').append('<p class="bubble please">How can I be of assistance?</p>');
            refreshiScroll();
            window.plugins.tts.speak("How can I be of assistance?");
        }
    }

    function recognizeSpeech() {
        var requestCode = 1234;
        var maxMatches = 1;
        var promptString = "Please say a command";  // optional
        var language = "en-US";                     // optional
        window.plugins.speechrecognizer.startRecognize(speechOk, speechFail, requestCode, maxMatches, promptString, language);
    }

    function speechOk(result) {
        var respObj, requestCode, matches;
        if (result) {
            respObj = JSON.parse(result);
            if (respObj) {
                // TODO: Send multiple matches with probabilities (will need to
                // mod plugin)
                var matches = respObj.speechMatches.speechMatch;

                if ( matches.length > 0 ) {
                    reply = matches[0];
                    echo(reply);

                    api.ask(matches[0], function(response) {
                        console.log("RESPONSE: " + JSON.stringify(response));

                        if ( response.response != null ) {
                            say(response.response);
                        }

                        if ( response.action != null ) {
                            performAction(response.action, response.payload);
                        }
                    });
                } else {
                    say("I didn't understand. I am such an idiot.");
                }
            }
        }
    }

    function echo (message) {
        $('.console').append('<p class="bubble owner">' + message + '</p>');
        refreshiScroll();
        window.plugins.tts.speak(message);
    }

    function say(message) {
        $('.console').append('<p class="bubble please">' + message + '</p>');
        refreshiScroll();
        window.plugins.tts.speak(message);
    }

    var performAction = function(action, payload) {
        actions[action](payload);
    };

    function contactLookup (e) {
        var options = new ContactFindOptions();
        options.filter = e;
        var fields = ["displayName", "name", "phoneNumbers"];
        navigator.contacts.find(fields, contactLookupSuccess, contactLookupError, options);
    }
    function contactLookupSuccess(contacts) {
        console.log(contacts)
        if (contacts.length > 1) {
            reply = "I found" + contacts.length;
        } else if ( contacts.length == 1 ) {
            reply = "Shall I call " + contacts[0].displayName + "?";
            activeContact = contacts[0];
        } else {
            reply = "I couldn't find anyone in your address book with that name.";
        }
        conversation();
    }
    function contactLookupError () {
        reply = "Oh, snap";
        conversation();
    }

    function speechInitOk() {
        console.log('speech recognizer is ready');
    }
    function speechInitFail(m) {
        console.log('speech recognizer failed');
    }
    function speechFail(message) {
        console.log("speechFail: " + message);
    }
    function startupFail(result) {
        console.log("Startup failure = " + result);
    }

    // $('.micbutton').click(function () {
    //     capture = "Go fuck yourself";
    //     $('.console').append('<p class="bubble please" id="last">' + capture + '</p>');
    //     window.location='#last';
    //     // window.plugins.tts.speak(capture);
    // })

    $('.micbutton').click(function () {
        recognizeSpeech();
    })

})