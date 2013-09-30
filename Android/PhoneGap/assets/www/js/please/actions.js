/**
 * Actions triggered by the Butler API
 */
cordova.define('please/actions', function(require, exports, module) {
    var PhoneCall = require('cordova/plugin/phonecall');

    var dateFromString = function (date, time) {
        if (time === undefined || time === null) {
            time = "12:00:00 PM";
        }
        
        return new Date(date + " " + time);
    }

    var monthsOfTheYear = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

    var dayOfTheWeek = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

    /*
    {'action': 'calendar', 'payload': {'date': '2013-06-20', 'person': u'bob', 'location': None, 'time': '02:18:00', 'duration': 0.5, 'query': u'Dinner at 2:18 a m with bob', 'subject': u'Dinner'}}
    */

    var createEventSubject = function (subject, person, location) {
        if (subject === undefined || subject === null) {
            subject = "";
        }

        if (person !== null) {
            subject += " with " + person;
        }
        if ((location !== null) && (location !== "")) {
            subject += " at " + location;
        }

        return subject;
    }

    var calendar = function (payload) {
        var temp, sDate, eDate, memo;
        
        sDate = dateFromString(payload.date, payload.time);
        /*
        if (payload.time == null) {
            payload.time = "12:00:00 PM";
        }
        sDate = new Date(payload.date + " " + payload.time);
        */
        temp = sDate.getTime();
        temp += (payload.duration * 3600000);
        eDate = new Date(temp);
        
        if (payload.location == null) {
            payload.location = "";
        }

        memo = createEventSubject(payload.subject, payload.person, payload.location);
        
        /*
        memo = payload.subject;
        if (payload.person !== null) {
            memo += " with " + payload.person;
        }
        if ((payload.location !== null) && (payload.location !== "")) {
            memo += " at " + payload.location;
        }
        */
        function success() {}
        function error() {}

        window.plugins.calendarPlugin.createEvent(
                memo, 
                payload.location, 
                "",
                sDate, 
                eDate,
                success,
                error
            );
    };
    exports.calendar = calendar;

    var time = function () {
        // var theDate = new Date();
        // var hours = theDate.getHours();
        // if (hours < 12) {
        //     var ampm = "AM";
        // } else {
        //     ampm = "PM";
        // }
        // var mins = theDate.getMinutes();
        // if (mins < 10) {
        //     mins = "0" + mins;
        // }
        // var seconds = theDate.getSeconds();
        // if (seconds < 10) {
        //     seconds = "0" + seconds;
        // }
        // var dayOfTheWeek = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
        // var day = dayOfTheWeek[theDate.getDay()];
        // var monthsOfTheYear = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
        // var month = monthsOfTheYear[theDate.getMonth()];
        // var date = theDate.getDate();
        // var year = theDate.getFullYear();
        // say("It is now " + hours + ":" + mins + " " + ampm + " on " + day + ", " + month + " " +  date + ", " + year + ".");
    };
    exports.time = time;

    var call = function (payload) {
        PhoneCall.call(payload.phone);
    };
    exports.call = call;

    var web = function (payload) {
        window.open(payload.url, '_blank', 'location=yes');
    };
    exports.web = web;

    var link = function (payload) {
        if (payload.url !== undefined && payload.url !== null)
            $('.bubble:last').append('<a href="' + payload.url + '" class="showMore extLink">Click for more</a>');
    };
    exports.link = link;

    var reminder = function (payload) {
        var date;
        var regex = /alarm/i;

        if (payload.seconds !== undefined && payload.seconds !== null) {
           date = new Date();
           date.setSeconds(date.getSeconds() + payload.seconds);
        } else if (payload.datetime !== undefined && payload.datetime !== null) {   
            date = new Date(payload.datetime);
        } else {
            date = new Date();
        }  

        if (payload.ticker === undefined || payload.ticker === null) {
            payload.ticker = "";
        }

        if (payload.message === undefined || payload.ticker === null) {
            payload.message = "";
        }

        window.plugins.localNotification.add({
            date: date,
            ticker: payload.ticker,
            message: payload.message,
            repeatDaily: false,
            id: 999
        });

        if (payload.query !== undefined && regex.test("alarm")) {
            say("a reminder alarm has been set for you");
        } else {
            say("a reminder has been set for you");
        }
    };
    exports.reminder = reminder;

    var alarm = function (payload) {
        payload.datetime = payload.datetime.replace("T", " ");
        
        var date = new Date(payload.datetime);
        var now = new Date();

        // if date is next day, pass off to reminder.
        if (date.getFullYear() > now.getFullYear()) {
            reminder(payload);
            return;
        } else if (date.getMonth() > now.getMonth()) {
            reminder(payload);
            return;
        } else if (date.getDate() > now.getDate()) {
            reminder(payload);
            return;
        }

        var successHandler = function () {
            say("an alarm has been set for you");
        };
        var errorHandler = function (msg) {
            console.log(msg);
            say("Sorry, I could not set your alarm");
        };

        window.plugins.alarmClockPlugin.addAlarm(
            date,
            "Please Alarm", 
            false, 
            successHandler,
            errorHandler
        );
    };
    exports.alarm = alarm;

    var images = function (payload) {
        for (var i=0; i<payload.url.length; i++) {
            path = payload.url[i];
            $('.bubble:last').append('<img class="galleryImg" src="' +path + '" />');
        }
        refreshiScroll();
    };
    exports.images = images;

    var sms = function (payload) {
         cordova.exec(
            function(success) {}, 
            function(error) {}, 
            "SendSms", 
            "SendSms", 
            [
                payload.phone, 
                payload.message
            ]);
    };
    exports.sms = sms;

    var email = function (payload) {
            window.plugins.emailComposer.showEmailComposer(
                payload.subject,
                payload.message,
                [payload.address],
                [], // ccRecipients
                [], // bccRecipients
                false, // isHtml
                [] //attachments
            );
    };
    exports.email = email;

    var capture_image = function (payload) {
        function captureSuccess(imageData) {
            var path = imageData[0].fullPath;
            var img = "<img src=\"" + path + "\" />";
            /*
            $('.console').append('<div class="bubble please">' + '<img src="' + path + '" />' + '</div>');
            */
            say('That is a beautiful picture!', img);
        }

        function captureError() {}

        navigator.device.capture.captureImage(captureSuccess, captureError, {limit:1});
    };
    exports.capture_image = capture_image;
    exports["capture-image"] = capture_image;

    var capture_audio = function (payload) {
        function captureSuccess(media) {
            var path = media[0].fullPath;
            var audio = "<audio src=\"" + path + "\" controls></audio>";
            /*
            $('.console').append('<div class="bubble please">' + '<audio src="' +path + '"controls ></audio>' + '</div>');
            */
            say('That sounded great!', audio);
        }
        function captureError() {
            say('Sorry, I could not launch the audio recorder');
        }

        navigator.device.capture.captureAudio(captureSuccess, captureError, {limit:1});
    };
    exports.capture_audio = capture_audio;
    exports["capture-audio"] = capture_audio;

    var capture_video = function (payload) {
        function captureSuccess(media) {
            var path = media[0].fullPath;
            var video = "<video src=\"" + path + "\" controls></video>";
            /*
            $('.console').append('<div class="bubble please">' + '<video src="' +path + '" controls ></video>' + '</div>');*/

            say('That was a funny video!', video);
        }
        function captureError() {
            say('Sorry, I could not launch the video recorder');
        }

        navigator.device.capture.captureVideo(captureSuccess, captureError, {limit:1});
    };
    exports.capture_video = capture_video;
    exports["capture-video"] = capture_video;

    var clear_log = function (payload) {
        $('.console').html("");
        $('.listSlider').removeClass('active suspended')
        .find('.prompt').empty().end()
        .find('ul').empty();
        // say('Let me know how I can be of assistance.');
    };
    exports.clear_log = clear_log;

    var locate = function (payload) {
        var oops = false;
         var defaultLocation = {"coords":{"latitude":33.620632, "longitude":-111.92565}};
        navigator.geolocation.getCurrentPosition(geoSuccess, geoFail, {timeout:10000, enableHighAccuracy:true});
        
        function geoSuccess(pos) {
            var lat = pos.coords.latitude;
            var lng = pos.coords.longitude;
            if (!oops) {
                say("I found you!");
            }
            $('#gMapPos').attr('id', '');
            $('.bubble:last').append('<div id="gMapPos"></div>');
            var latlng = new google.maps.LatLng(lat,lng);   
            var options = { zoom: 16, center: latlng, disableDefaultUI: true, mapTypeId: google.maps.MapTypeId.ROADMAP, draggable: false };
            var map = new google.maps.Map(document.getElementById('gMapPos'), options);
            var uRHere = new google.maps.Marker({ position: latlng, map: map, title: 'You are here' });
        }

        function geoFail() {
           say("You must be hiding, because I can't find you. But I did find Stremor headquarters in Scottsdale.");
           oops = true;
           geoSuccess(defaultLocation);
        }
    };
    exports.locate = locate;

    var directions = function (payload) {
        var directionsDisplay;
        var directionsService = new google.maps.DirectionsService();
        var map;
        var myDestination = payload.location;
        var defaultLocation = {"coords":{"latitude":33.620632, "longitude":-111.92565}};
        var oops = false;

        navigator.geolocation.getCurrentPosition(navSuccess, navFail, {timeout:10000, enableHighAccuracy:true});
        $('#gMap').attr('id', '');
        $('#wrapper').spin(opts);

        function navSuccess(pos) {
            if (!oops) {
                say("Here is the route that I found: ")
            }
          directionsDisplay = new google.maps.DirectionsRenderer();
          var lat = pos.coords.latitude;
          var lng = pos.coords.longitude;
          var originPoint = new google.maps.LatLng(lat, lng);
          var mapOptions = {
            zoom:7,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            center: originPoint,
            draggable: false
          }
          $('.bubble:last').append('<div id="gMap"></div>');
          map = new google.maps.Map(document.getElementById("gMap"), mapOptions);
          directionsDisplay.setMap(map);
          calcRoute(originPoint, myDestination);
        }

        function calcRoute(startPt, endPt) {
          var start = startPt;
          var end = endPt;
          var request = {
            origin:start,
            destination:end,
            travelMode: google.maps.TravelMode.DRIVING
          };
          directionsService.route(request, function(result, status) {
            if (status == google.maps.DirectionsStatus.OK) {
              directionsDisplay.setDirections(result);
            }
          });
          refreshiScroll();
        }
        function navFail() {
            say("This is sad, but I'm having trouble with my GPS. Here's how to get there from Scottsdale.");
            oops = true;
            navSuccess(defaultLocation);
        }
    };
    exports.directions = directions;

    var app_launch = function (payload) {
        window.plugins.AndroidLauncher.launch(payload.package, null, null);
    };
    exports.app_launch = app_launch;

    var app_view = function (payload) {
        window.plugins.AndroidLauncher.view(payload.uri, null, function(b){
            /*
            if ( payload.uri.indexOf('waze') != -1 ) {
                window.plugins.AppInstallOffer.prompt('Waze not found',
                                                      "We couldn't find Waze on your phone. Please install it.",
                                                      "com.waze");
            }
            */
        });
    };
    exports.app_view = app_view;

    var contacts = function () {
        console.log("contacts");

        var find = ["id", "displayName", "name", "nickname", "phoneNumbers", "emails", "addresses", "ims", "birthday", "note", "categories", "urls"];

        var success = function (contacts) {
            var i, len, contact, contactItem;
            for (i=0, len=contacts.length; i < len; i++) {
                contact = contacts[i];

                console.log(JSON.stringify(contact));
            }
        };

        var error = function (msg) {
            console.log(msg);
        };

        navigator.contacts.find(["*"], success, error, {});
    };
    exports.contacts = contacts;
});
