var echo, say, performAction, refreshiScroll, isDebugging = false;

var shouldSpeak = true;

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
        shows = cordova.require('please/shows'),
        capture,
        activeContact,
        activeContactTel;

    var MONTH_NAMES = ['January', 'February', 'March', 'April', 'May', 'June',
                       'July', 'August', 'September', 'October', 'November',
                       'December'];

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
                myScroll.scrollToElement('.console div:last-child', 250);
            }
        }, 0);
        $('.spinner').remove();
    }

    function startDebugging() {
        $('.container').addClass("enableForm");
        isDebugging = true;
    }

    function stopDebugging() {
        $('.container').removeClass("enableForm");
        isDebugging = false;
    }

    function createNewEventPayload() {
        var payload = { };

        var date = new Date();
        var month = (date.getMonth() + 1);
        var day = date.getDate();

        if (month < 10) {
            month = "0" + month;
        }

        if (day < 10) {
            day = "0" + day;
        }

        dateString = [date.getFullYear(), month, day].join("-");
        payload.duration = 0.5;
        payload.date = dateString;
        payload.time = '20:18:00';
        payload.location = null;
        payload.person = "jeff";
        payload.subject = "meeting";

        return payload;
    }

    //console.log(createNewEventPayload());

    function onDeviceReady() {
        window.plugins.tts.startup(startupWin, startupFail);
        window.plugins.speechrecognizer.init(speechInitOk, speechInitFail);

        $(document).on('mediaButton', function() {
            initQuery();
        });

        $(document).on('backbutton', function () {
            var l = $('.listSlider');

            if ( l.hasClass('active') )
                l.addClass('suspended').removeClass('active');
            else
                navigator.app.exitApp();
        });

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
            $('.console').append('<div class="bubble please">Tap the Please logo to get started</div>');
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
            shouldSpeak = true;
            respObj = JSON.parse(result);
            if (respObj) {
                // TODO: Send multiple matches with probabilities (will need to
                // mod plugin)
                matches = respObj.speechMatches.speechMatch;

                if ( matches.length > 0 ) {
                    $(document).trigger('sendQuery');

                    var query = matches[0];
                    query = cleanQuery(query);
                    echo(query);

                    switch (query) {
                        case "can you find me":
                        performAction('locate', "");
                        break;

                        case "it takes two to tango":
                        startDebugging();
                        break;

                        default:
                        sendQuery(query);
                        break;
                    }

                    /*
                    if (query == "can you find me") {
                        performAction('locate', "");
                    } else {
                        sendQuery(query);
                    }
                    */
                } else {
                    say("I didn't understand. I am such an idiot.", null);
                }
            }
        }
    }

    var initQuery = function () {
        $('.spinner').remove();
        $('.control').addClass('fixIt').removeClass('fixIt');
        window.plugins.tts.stop();
        recognizeSpeech();
    };

    /**
     * Do some very basic cleaning on a query.
     */
    var cleanQuery = function (query) {
        query = query.replace('needa', 'need a');
        return query;
    };

    /**
     * Send a voice query to the server.
     *
     * @param query The raw text of a voice query, as returned by the speech
     *     recognition engine.
     */
    var sendQuery = function (query, cb) {
        clientDate = new Date();
        deviceInfo = {
            "device": {
                "platform": "android",
                "device": "android-client",
                "lat": latitude,
                "lon": longitude,
                "timestamp": clientDate.getTime() / 1000,
                "timeoffset": - clientDate.getTimezoneOffset() / 60
            }
        };

        if (!window.context) {
            window.context = deviceInfo;
            console.log(context)
        } else {
            context.device = deviceInfo.device;
            console.log("sending: ", context)
        }

        api.ask(query, context, function(response) {
            window.context = response.context;

            if (response.show !== undefined && response.show !== null) {
                switch (response.show.type) {
                    case "string":
                    say(response.speak, response.show.text);
                    break;

                    default:
                    performShow(response);
                    break;
                }
            } else if (( response.speak != null ) && (response.speak !== "REPLACE_WITH_DEVICE_TIME")) {
                say(response.speak, null);
            }

            /*
            if ( response.show !== undefined && response.show.type == 'string' ) {
                say(response.speak, response.show.text);
            } else if (( response.speak != null ) && (response.speak !== "REPLACE_WITH_DEVICE_TIME")) {
                say(response.speak, null);
            }

            if ( response.show !== undefined && response.show.type != 'string' ) {
                performShow(response.show);
            }
            */
            
            if ( response.trigger.action != null ) {
                performAction(response.trigger.action, response.trigger.payload);
            }

            if ( cb )
                cb();
        });
    };

    /**
     * Display text spoken by the user.
     * @param message
     */
    echo = function (message) {
        $('.console').append('<div class="bubble owner">' + message + '</div>');
        refreshiScroll();
        // window.plugins.tts.speak(message);
        $('#wrapper').spin(opts);
        $('#wrapper>div:first').addClass('spinner');
    };

    /**
     * Continue the conversation with a message.
     * @param message
     */
    say = function (speak, show) {
        if (show === undefined || show === null)
            show = speak;

        $('.console').append('<div class="bubble please">' + show + '<span class="helperButtons"></span></div>');
        refreshiScroll();

        if (shouldSpeak === true)
            window.plugins.tts.speak(speak);

        // clear out test input field if it's visible
        if ($('.container').hasClass('enableForm'))
            $('.testInput').val("");
    };

    var performShow = function (response) {
        shows[response.show.type] (response);
    };

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

    function clearCookie(name, domain, path){
        var domain = domain || document.domain;
        var path = path || "/";
        document.cookie = name + "=; expires=" + +new Date + "; domain=" + domain + "; path=" + path;
    };

    function contactLookup(e) {
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
    function contactLookupError() {
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

    $('.listView').on('click', 'li', function () {
        var query = $(this).text();

        echo(query);
        sendQuery(query);

        $('.listSlider').removeClass('active suspended');
    });

    $(document).on('swipeRight', function() {
        if ( $('.listSlider').hasClass('active') ) {
            $('.listSlider').addClass('suspended').removeClass('active');
        }
    });

    $(document).on('swipeLeft', function() {
        if ( $('.listSlider').hasClass('suspended') ) {
            $('.listSlider').removeClass('suspended').addClass('active');
        }
    });

    $('.console').on('click', '.calendarLink', function() {
        window.plugins.CalendarDialog.getDate(function (isoDate) {
            if ( isoDate == undefined ) return;

            var date = new Date(isoDate);
            var dateString = MONTH_NAMES[date.getMonth()] + ' ' + date.getDate()
                    + ' ' + date.getFullYear();
            var dateQueryString = date.getFullYear() + '-'
                    + (date.getMonth + 1) + '-' + date.getDate();

            echo(dateString);
            sendQuery(dateQueryString);
        }, null);
    });

    $('.console').on('click', 'a', function() {
        e = event.target;
        window.open($(e).attr('href'), '_blank', 'location=yes');
        return false;
    });

    $('.control').on('fastClick', '.micbutton', function () {
        initQuery();

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
    
    // accept typed text in place of spoken text.
    // Remove for Demos
    $('#testForm').submit(function(e) {
        e.preventDefault();

        var query = $(this).find('#testInput').val();

        if ( query.length > 0 ) {
            shouldSpeak = false;
            query = cleanQuery(query);
            echo(query);

            if (query == "can you find me") {
                performAction('locate', "");
            } else {
                sendQuery(query);
            }
        }
    });
});
