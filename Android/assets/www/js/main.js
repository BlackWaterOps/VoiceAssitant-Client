var echo, say, performAction, refreshiScroll;

var opts = {
      lines: 12, // The number of lines to draw
      length: 7, // The length of each line
      width: 5, // The line thickness
      radius: 10, // The radius of the inner circle
      color: '#999', // #rbg or #rrggbb
      speed: 1, // Rounds per second
      trail: 100, // Afterglow percentage
      shadow: true // Whether to render a shadow
    };

var latitude, longitude, clientDate, positionCheck, test;

$(function () {
    var api = cordova.require('please/api'),
        actions = cordova.require('please/actions'),
        capture,
        activeContact,
        activeContactTel;

    $.fn.spin = function(opts) {
            this.each(function() {
            var $this = $(this),
            spinner = $this.data('spinner');

            if (spinner) spinner.stop();
            if (opts !== false) {
                  opts = $.extend({color: $this.css('color')}, opts);
                  spinner = new Spinner(opts).spin(this);
                  $this.data('spinner', spinner);
                }
              });
          return this;
    };

    document.addEventListener("deviceready", onDeviceReady, false);

    var myScroll = new iScroll('wrapper', {checkDOMChanges: true});
    var hite = $('#wrapper').innerHeight();
    refreshiScroll = function () {
        setTimeout(function () {
            myScroll.refresh();
            if ($('.console').innerHeight() > hite) {
                myScroll.scrollToElement('.console p:last-child', 250);
            }
        }, 0);
        $('.spinner').remove();
    }

    function onDeviceReady() {
        window.plugins.tts.startup(startupWin, startupFail);
        window.plugins.speechrecognizer.init(speechInitOk, speechInitFail);
        document.getElementById('wrapper').offsetWidth; 
        playAudio('file:///android_asset/www/snd/happyalert.wav');
        positionCheck = setInterval(get_location, 60000);
    }

    function playAudio(url) {
    // Play the audio file at url
    var my_media = new Media(url,
        // success callback
        function() {
            console.log("playAudio():Audio Success");
        },
        // error callback
        function(err) {
            console.log("playAudio():Audio Error: "+err);
    });

    // Play audio
    my_media.play();
}

    function startupWin(result) {
        // When result is equal to STARTED we are ready to play
        if (result == TTS.STARTED) {
            $('.console').append('<p class="bubble please">Tap the Please logo to get started</p>');
            refreshiScroll();
            // window.plugins.tts.speak("How can I be of assistance?");
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
                    if (reply == "can you find me") {
                        performAction('locate', "");
                    } 
                     else {
                        clientDate = new Date();
                        deviceInfo = {
                                "device":{
                                        "lat": latitude,
                                        "lon": longitude,
                                        "timestamp": clientDate.getTime(),
                                        "timeoffset": clientDate.getTimezoneOffset() / 60
                                    }
                            }
                        if (!window.context) {
                            window.context = deviceInfo;
                            console.log(context)
                        } else {
                            context.device = deviceInfo.device;
                            console.log("sending: ", context)
                        }

                         window.context = JSON.stringify(context);

                        api.ask(matches[0], context, function(response) {
                            window.context = response.context;
                            if (( response.speak != null ) && (response.speak !== "REPLACE_WITH_DEVICE_TIME")) {
                                    say(response.speak);
                            }
                            if ( response.trigger.action != null ) {
                                performAction(response.trigger.action, response.trigger.payload);
                            }
                        });
                    }  
                } else {
                    say("I didn't understand. I am such an idiot.");
                }
            }
        }
    }

    // changed to a variable so that it can be called from the actions plugin
    echo = function (message) {
        $('.console').append('<p class="bubble owner">' + message + '</p>');
        refreshiScroll();
        // window.plugins.tts.speak(message);
        $('#wrapper').spin(opts);
        $('#wrapper>div:first').addClass('spinner');
    }

    say = function (message) {
        $('.console').append('<p class="bubble please">' + message + '</p>');
        refreshiScroll();
        window.plugins.tts.speak(message);
    }

    performAction = function (action, payload) {
        actions[action] (payload);
    };

    function get_location() {
      navigator.geolocation.getCurrentPosition(update_position);
    }
    function update_position (position) {
      latitude = position.coords.latitude;
      longitude = position.coords.longitude;
    }

    get_location();

    test = "send Anghel a LONG text";
    var deviceInfo = {
        "device":{
                "lat":latitude,
                "lon":longitude,
                "time":clientDate
            }
    }

    function clearCookie(name, domain, path){
        var domain = domain || document.domain;
        var path = path || "/";
        document.cookie = name + "=; expires=" + +new Date + "; domain=" + domain + "; path=" + path;
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

    $('.console').on('click', '.extLink', function () {
        e = event.target;
        window.open($(e).attr('href'), '_blank', 'location=yes');
        return false;
    });

    $('.control').on('click', '.micbutton', function () {
            $('.spinner').remove();
            $('.control').addClass('fixIt').removeClass('fixIt');
            window.plugins.tts.stop();
            recognizeSpeech();
                        // clientDate = new Date();
                        // // clientDate = clientDate.toString();
                        // deviceInfo = {
                        //         "device":{
                        //                 "lat": latitude,
                        //                 "lon": longitude,
                        //                 "timestamp": clientDate.getTime(),
                        //                 "timeoffset": clientDate.getTimezoneOffset() / 60
                        //             }
                        //     }
                        // if (!window.context) {
                        //     window.context = deviceInfo;
                        //     console.log(context)
                        // } else {
                        //     context.device = deviceInfo.device;
                        //     // context = {context}
                        //     console.log("sending: ", context)
                        // }

                        //  window.context = JSON.stringify(context);

                        // api.ask(test, context, function(response) {
                        //     console.log("received: ", response);
                        //     window.context = response.context;
                        //     console.log('new context: ', context)
                        // });
        });
})