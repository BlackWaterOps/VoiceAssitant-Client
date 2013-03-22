/**
 * Actions triggered by the Butler API
 */
cordova.define('please/actions', function(require, exports, module) {
    var PhoneCall = require('cordova/plugin/phonecall');

    exports.calendar = function(payload) {
        var temp, sDate, eDate;
            if (payload.time == null) {
                payload.time = "12:00:00 PM";
            }
            sDate = new Date(payload.date + " " + payload.time);
            temp = sDate.getTime();
            temp += (payload.duration * 3600000);
            eDate = new Date(temp);
            if (payload.location == null) {
                payload.location = "";
            }
            memo = payload.subject;
            if (payload.person !== null) {
                memo += " with " + payload.person;
            }
            if ((payload.location !== null) && (payload.location !== "")) {
                memo += " at " + payload.location;
            }

            function success () {}
            function error () {}

        window.plugins.calendarPlugin.createEvent(
                payload.subject, 
                payload.location, 
                memo,
                sDate, 
                eDate,
                success,
                error
            );
    };

    exports.call = function(payload) {
        PhoneCall.call(payload.phone);
    };

    exports.web = function(payload) {
        window.open(payload.url, '_blank', 'location=yes');
    };

    exports.reminder = function (payload) {
        var now = new Date();
        now.setSeconds(now.getSeconds() + payload.seconds);
        window.plugins.localNotification.add({
            date: now,
            ticker: payload.ticker,
            message: payload.message,
            repeatDaily: false,
            id: 999
        })
    }

    exports.sms = function (payload) {
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

    exports.email = function (payload) {
            window.plugins.emailComposer.showEmailComposer(
                payload.subject,
                payload.message,
                [payload.address],
                [], // ccRecipients
                [], // bccRecipients
                false, // isHtml
                [] //attachments
            );
    }

    var capture_image = function (payload) {
        navigator.device.capture.captureImage(onSuccess, onFail, {limit:1});
        function onSuccess (imageData) {
            path = imageData[0].fullPath;
            $('.console').append('<p class="bubble please">' + '<img src="' +path + '" />' + '</p>');
            say('That is a beautiful picture!')
        }
        function onFail () {}
    }
    exports.capture_image = capture_image;

    var capture_audio = function (payload) {
        navigator.device.capture.captureAudio(captureSuccess, captureError, {limit:1});
        function captureSuccess (media) {
            path = media[0].fullPath;
            $('.console').append('<p class="bubble please">' + '<audio src="' +path + '"controls ></audio>' + '</p>');
            say('That sounded great!')
        }
        function captureError () {}
    }
    exports.capture_audio = capture_audio;

    var capture_video = function (payload) {
        navigator.device.capture.captureVideo(captureSuccess, captureError, {limit:1});
        function captureSuccess (media) {
            path = media[0].fullPath;
            $('.console').append('<p class="bubble please">' + '<video src="' +path + '" controls ></video>' + '</p>');
            say('That was a funny video!')
        }
        function captureError () {}
    }
    exports.capture_video = capture_video;

    exports.clear_log = function (payload) {
        $('.console').html("");
        say('Let me know how I can be of assistance.');
    }

    exports.locate = function (payload) {
        console.log(payload)
        navigator.geolocation.getCurrentPosition(geoSuccess, geoFail, {timeout:5000, enableHighAccuracy:true});
        
        function geoSuccess(pos) {
            var lat = pos.coords.latitude;
            var lng = pos.coords.longitude;
            say("I found you!");
            $('#gMapPos').attr('id', '');
            $('.bubble:last').append('<div id="gMapPos"></div>');
            var latlng = new google.maps.LatLng(lat,lng);   
            var options = { zoom: 16, center: latlng, disableDefaultUI: true, mapTypeId: google.maps.MapTypeId.ROADMAP };
            var map = new google.maps.Map(document.getElementById('gMapPos'), options);
            var uRHere = new google.maps.Marker({ position: latlng, map: map, title: 'You are here' });
        }
        function geoFail () {
           say("You must be hiding, because I can't find you.");
        }
    }

    var directions = function (payload) {
        var directionsDisplay;
        var directionsService = new google.maps.DirectionsService();
        var map;
        var myDestination = payload.location;

        navigator.geolocation.getCurrentPosition(navSuccess, navFail, {timeout:5000, enableHighAccuracy:true});

        function navSuccess(pos) {
          directionsDisplay = new google.maps.DirectionsRenderer();
          var lat = pos.coords.latitude;
          var lng = pos.coords.longitude;
          var originPoint = new google.maps.LatLng(lat, lng);
          var mapOptions = {
            zoom:7,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            center: originPoint
          }
          $('#gMap').attr('id', '');
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
        }
        function navFail () {console.log('foo')}
    }
    exports.directions = directions;
    
});