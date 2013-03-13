$(function () {
    var capture,
    activeContact,
    activeContactTel;

    document.addEventListener("deviceready", onDeviceReady, false);

    var myScroll = new iScroll('wrapper', {checkDOMChanges: true});
    var hite = $('#wrapper').innerHeight();
    function refreshiScroll () {
        setTimeout(function () {
            myScroll.refresh();
            if($('.console').innerHeight() > hite) {
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
                var matches = respObj.speechMatches.speechMatch;

                for (x in matches) {
                    $('.console').append('<p class="bubble owner">' + matches[x] + '</p>');
                    
                    arg = matches[x];
                    switch (true) {
                        case arg == "what is my name":
                            reply = "why don't you know what your name is?";
                            conversation();
                            break;
                        case arg.indexOf('call') > -1:
                            var lookup = arg.split(" ");
                            var person = lookup[1];
                            console.log(person + " is here")
                            contactLookup(person);
                            break;
                        case arg.indexOf('yes') != -1 && activeContact != null:
                            var numbers = activeContact.phoneNumbers;
                            console.log(numbers);
                            if ( numbers.length == 0 ) {
                                reply = "I couldn't find any numbers for " + activeContact.displayName;
                                conversation();
                            } else {
                                window.plugins.PhoneCall.call(numbers[0].value);
                            }
                            break;
                        case arg.indexOf('f***') > -1:
                            reply = "How shall I fuck off?";
                            conversation();
                            break;
                        case arg == "clear the conversation":
                            $('.console').html("");
                            reply = "Okay. Your wish is my command.";
                            conversation();
                            break;
                        default:
                            reply = "I didn't understand. I am such an idiot.";
                            conversation();
                            break;
                    }
                }
            }        
        }
    }
    function conversation (e) {
        $('.console').append('<p class="bubble please">' + reply + '</p>');
        refreshiScroll();
        window.plugins.tts.speak(reply);
    }

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