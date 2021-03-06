// Generated by CoffeeScript 1.6.3
(function() {
  var Actions, calcRoute, navFail;

  Actions = (function() {
    function Actions() {
      this.monthsOfTheYear = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
      this.dayOfTheWeek = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
      this.phoneCall = null;
      /*
      cordova.define('please/actions', (require, exports, module) =>
          @phoneCall = require('cordova/plugin/phonecall')
          exports.calendar = @calendar
          exports.time = @time
          exports.call = @call
          exports.web = @web
          exports.link = @link
          exports.reminder = @reminder
          exports.alarm = @alarm
          exports.images = @images
          exports.sms = @sms
          exports.email = @email
          exports.capture_image = @capture_image
          exports["capture-image"] = @capture_image
          exports.capture_audio = @capture_audio
          exports["capture-audio"] = @capture_audio
          exports.capture_video = @capture_video
          exports["capture-video"] = @capture_video
          exports.clear_log = @clear_log
          exports.locate = @locate
          exports.directions = @directions
          exports.app_launch = @app_launch
          exports.app_view = @app_view
          exports.contacts = @contacts
      */

    }

    Actions.prototype.dateFromString = function(date, time) {
      if (time == null) {
        time = "12:00:00 PM";
      }
      return new Date(date + " " + time);
    };

    Actions.prototype.createEventSubject = function(subject, person, location) {
      if (subject == null) {
        subject = "";
      }
      if (person(!null)) {
        subject += " with " + person;
      }
      if (location(!null && location(!""))) {
        subject += " at " + location;
      }
      return subject;
    };

    Actions.prototype.calendar = function(payload) {
      var eDate, errorHandler, memo, sDate, successHandler, temp;
      sDate = dateFromString(payload.date, payload.time);
      /*
      if (payload.time == null) {
          payload.time = "12:00:00 PM"
      }
      sDate = new Date(payload.date + " " + payload.time)
      */

      temp = sDate.getTime();
      temp += payload.duration * 3600000;
      eDate = new Date(temp);
      if ((payload.location == null) === null) {
        payload.location = "";
      }
      memo = createEventSubject(payload.subject, payload.person, payload.location);
      /*
      memo = payload.subject
      if (payload.person !== null) {
          memo += " with " + payload.person
      }
      if ((payload.location !== null) && (payload.location !== "")) {
          memo += " at " + payload.location
      }
      */

      successHandler = function(success) {};
      errorHandler = function(error) {};
      return window.plugins.calendarPlugin.createEvent(memo, payload.location, "", sDate, eDate, successHandler, errorHandler);
    };

    Actions.prototype.time = function() {};

    Actions.prototype.call = function(payload) {
      return PhoneCall.call(payload.phone);
    };

    Actions.prototype.web = function(payload) {
      return window.open(payload.url, '_blank', 'location=yes');
    };

    Actions.prototype.link = function(payload) {
      if (payload.url != null) {
        return $('.bubble:last').append('<a href="' + payload.url + '" class="showMore extLink">Click for more</a>');
      }
    };

    Actions.prototype.reminder = function(payload) {
      var date, regex;
      regex = /alarm/i;
      if (payload.seconds != null) {
        date = new Date();
        date.setSeconds(date.getSeconds() + payload.seconds);
      } else if (payload.datetime != null) {
        date = new Date(payload.datetime);
      } else {
        date = new Date();
      }
      if (payload.ticker == null) {
        payload.ticker = "";
      }
      if ((payload.message == null) || (payload.ticker == null)) {
        payload.message = "";
      }
      window.plugins.localNotification.add({
        date: date,
        ticker: payload.ticker,
        message: payload.message,
        repeatDaily: false,
        id: 999
      });
      if (payload.query(!void 0 && regex.test("alarm"))) {
        return say("a reminder alarm has been set for you");
      } else {
        return say("a reminder has been set for you");
      }
    };

    Actions.prototype.alarm = function(payload) {
      var date, errorHandler, now, successHandler;
      payload.datetime = payload.datetime.replace("T", " ");
      date = new Date(payload.datetime);
      now = new Date();
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
      successHandler = function() {
        return say("an alarm has been set for you");
      };
      errorHandler = function(msg) {
        console.log(msg);
        return say("Sorry, I could not set your alarm");
      };
      return window.plugins.alarmClockPlugin.addAlarm(date, "Please Alarm", false, successHandler, errorHandler);
    };

    Actions.prototype.images = function(payload) {
      var path, _i, _len, _ref;
      _ref = payload.url;
      for (_i = 0, _len = _ref.length; _i < _len; _i++) {
        path = _ref[_i];
        $('.bubble:last').append('<img class="galleryImg" src="' + path + '" />');
      }
      return refreshiScroll();
    };

    Actions.prototype.sms = function(payload) {
      var errorHandler, successHandler;
      successHandler = function(success) {};
      errorHandler = function(error) {};
      return cordova.exec(successHandler, errorHandler, "SendSms", "SendSms", [payload.phone, payload.message]);
    };

    Actions.prototype.email = function(payload) {
      var address, message, subject;
      subject = payload.subject;
      message = payload.message;
      address = payload.address;
      return window.plugins.emailComposer.showEmailComposer(subject, message, [address], [], [], false, []);
    };

    return Actions;

  })();

  ({
    capture_image: function(payload) {
      var errorHandler, successHandler;
      successHandler = function(imageData) {
        var img, path;
        path = imageData[0].fullPath;
        img = "<img src=\"" + path + "\" />";
        return say('That is a beautiful picture!', img);
      };
      errorHandler = function(error) {};
      return navigator.device.capture.captureImage(successHandler, errorHandler, {
        limit: 1
      });
    },
    capture_audio: function(payload) {
      var captureSuccess;
      captureSuccess = function(media) {
        var audio, path;
        path = media[0].fullPath;
        audio = "<audio src=\"" + path + "\" controls></audio>";
        return say('That sounded great!', audio);
      };
      captureError(function() {
        return say('Sorry, I could not launch the audio recorder');
      });
      return navigator.device.capture.captureAudio(captureSuccess, captureError, {
        limit: 1
      });
    },
    capture_video: function(payload) {
      var captureError, captureSuccess;
      captureSuccess = function(media) {
        var path, video;
        path = media[0].fullPath;
        video = "<video src=\"" + path + "\" controls></video>";
        return say('That was a funny video!', video);
      };
      captureError = function() {
        return say('Sorry, I could not launch the video recorder');
      };
      return navigator.device.capture.captureVideo(captureSuccess, captureError, {
        limit: 1
      });
    },
    clear_log: function(payload) {
      $('.console').html("");
      return $('.listSlider').removeClass('active suspended').find('.prompt').empty().end().find('ul').empty();
    },
    locate: function(payload) {
      var defaultLocation, geoFail, geoSuccess, oops;
      oops = false;
      defaultLocation = {
        coords: {
          latitude: 33.620632,
          longitude: -111.92565
        }
      };
      navigator.geolocation.getCurrentPosition(geoSuccess, geoFail, {
        timeout: 10000,
        enableHighAccuracy: true
      });
      geoSuccess = function(pos) {
        var lat, latlng, lng, map, options, uRHere;
        lat = pos.coords.latitude;
        lng = pos.coords.longitude;
        if (!oops) {
          say("I found you!");
        }
        $('#gMapPos').attr('id', '');
        $('.bubble:last').append('<div id="gMapPos"></div>');
        latlng = new google.maps.LatLng(lat, lng);
        options = {
          zoom: 16,
          center: latlng,
          disableDefaultUI: true,
          mapTypeId: google.maps.MapTypeId.ROADMAP,
          draggable: false
        };
        map = new google.maps.Map(document.getElementById('gMapPos'), options);
        return uRHere = new google.maps.Marker({
          position: latlng,
          map: map,
          title: 'You are here'
        });
      };
      return geoFail = function() {
        say("You must be hiding, because I can't find you. But I did find Stremor headquarters in Scottsdale.");
        oops = true;
        return geoSuccess(defaultLocation);
      };
    },
    directions: function(payload) {
      directionsDisplay;
      var defaultLocation, directionsService, map, myDestination, navSuccess, oops;
      directionsService = new google.maps.DirectionsService();
      myDestination = payload.location;
      defaultLocation = {
        coords: {
          latitude: 33.620632,
          longitude: -111.92565
        }
      };
      oops = false;
      navigator.geolocation.getCurrentPosition(navSuccess, navFail, {
        timeout: 10000,
        enableHighAccuracy: true
      });
      $('#gMap').attr('id', '');
      $('#wrapper').spin(opts);
      navSuccess = function(pos) {
        var directionsDisplay, lat, lng, mapOptions, originPoint;
        if (!oops) {
          say("Here is the route that I found: ");
        }
        directionsDisplay = new google.maps.DirectionsRenderer();
        lat = pos.coords.latitude;
        lng = pos.coords.longitude;
        originPoint = new google.maps.LatLng(lat, lng);
        return mapOptions = {
          zoom: 7,
          mapTypeId: google.maps.MapTypeId.ROADMAP,
          center: originPoint,
          draggable: false
        };
      };
      $('.bubble:last').append('<div id="gMap"></div>');
      map = new google.maps.Map(document.getElementById("gMap"), mapOptions);
      directionsDisplay.setMap(map);
      return calcRoute(originPoint, myDestination);
    }
  });

  calcRoute = function(startPt, endPt) {
    var end, request, start;
    start = startPt;
    end = endPt;
    request = {
      origin: start,
      destination: end,
      travelMode: google.maps.TravelMode.DRIVING
    };
    directionsService.route(request, function(result, status) {
      if (status === google.maps.DirectionsStatus.OK) {
        return directionsDisplay.setDirections(result);
      }
    });
    return refreshiScroll();
  };

  navFail = function() {
    var oops;
    say("This is sad, but I'm having trouble with my GPS. Here's how to get there from Scottsdale.");
    oops = true;
    return navSuccess(defaultLocation);
  };

  ({
    app_launch: function(payload) {
      return window.plugins.AndroidLauncher.launch(payload["package"], null, null);
    },
    app_view: function(payload) {
      return window.plugins.AndroidLauncher.view(payload.uri, null, function(b) {
        /*
        if ( payload.uri.indexOf('waze') != -1 ) {
            window.plugins.AppInstallOffer.prompt('Waze not found',
                                                  "We couldn't find Waze on your phone. Please install it.",
                                                  "com.waze")
        }
        */

      });
    },
    contacts: function() {
      var error, find, success;
      console.log("contacts");
      find = ["id", "displayName", "name", "nickname", "phoneNumbers", "emails", "addresses", "ims", "birthday", "note", "categories", "urls"];
      success = function(contacts) {
        var contact, _i, _len, _results;
        _results = [];
        for (_i = 0, _len = contacts.length; _i < _len; _i++) {
          contact = contacts[_i];
          _results.push(console.log(JSON.stringify(contact)));
        }
        return _results;
      };
      error = function(msg) {
        return console.log(msg);
      };
      return navigator.contacts.find(["*"], success, error, {});
    }
  });

}).call(this);
