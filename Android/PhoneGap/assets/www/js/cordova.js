// Generated by CoffeeScript 1.6.3
(function() {
  var Cordova,
    __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };

  Cordova = (function() {
    function Cordova(options) {
      this.startupFail = __bind(this.startupFail, this);
      this.speechFail = __bind(this.speechFail, this);
      this.speechInitFail = __bind(this.speechInitFail, this);
      this.speechInitOk = __bind(this.speechInitOk, this);
      this.cleanQuery = __bind(this.cleanQuery, this);
      this.speak = __bind(this.speak, this);
      this.speechOk = __bind(this.speechOk, this);
      this.recognizeSpeech = __bind(this.recognizeSpeech, this);
      this.initQuery = __bind(this.initQuery, this);
      this.startupWin = __bind(this.startupWin, this);
      this.refreshiScroll = __bind(this.refreshiScroll, this);
      this.deviceReady = __bind(this.deviceReady, this);
      var opts;
      this.please = new Please();
      this.please.debug = false;
      this.myScroll = null;
      this.hite = null;
      opts = {
        lines: 12,
        length: 7,
        width: 5,
        radius: 10,
        color: '#999',
        speed: 1,
        trail: 100,
        shadow: true
      };
      $.fn.spin = function(spinOpts) {
        this.each(function() {
          var $this, spinner;
          $this = $(this);
          spinner = $this.data('spinner');
          if (spinner) {
            spinner.stop();
          }
          if (opts !== false) {
            opts = $.extend({
              color: $this.css('color')
            }, opts);
            spinner = new Spinner(opts).spin(this);
            return $this.data('spinner', spinner);
          }
        });
        return this;
      };
      $('.spinner').remove();
      document.addEventListener("deviceready", this.deviceReady, false);
    }

    Cordova.prototype.deviceReady = function() {
      window.plugins.tts.startup(this.startupWin, this.startupFail);
      window.plugins.speechrecognizer.init(this.speechInitOk, this.speechInitFail);
      $(document).on('speak', this.speak);
      this.myScroll = new iScroll('wrapper', {
        checkDOMChanges: true,
        onBeforeScrollStart: function(e) {
          if (this.absDistY > (this.absDistX + 5)) {
            return e.preventDefault();
          }
        }
      });
      this.hite = $('#wrapper').innerHeight();
      this.refreshiScroll();
    };

    Cordova.prototype.refreshiScroll = function() {
      var _this = this;
      return setTimeout(function() {
        _this.myScroll.refresh();
        if ($('#board').innerHeight() > _this.hite) {
          return _this.myScroll.scrollToElement('#board div:last-child', 250);
        }
      }, 0);
    };

    Cordova.prototype.startupWin = function(result) {
      if (result === TTS.STARTED) {
        console.log('cordova startupWin');
        $('.control').on('fastClick', '.micbutton', this.initQuery);
        return this.refreshiScroll();
      }
    };

    Cordova.prototype.initQuery = function() {
      window.plugins.tts.stop();
      return this.recognizeSpeech();
    };

    Cordova.prototype.recognizeSpeech = function() {
      var language, maxMatches, promptString, requestCode;
      requestCode = 1234;
      maxMatches = 1;
      promptString = "Please say a command";
      language = "en-US";
      return window.plugins.speechrecognizer.startRecognize(this.speechOk, this.speechFail, requestCode, maxMatches, promptString, language);
    };

    Cordova.prototype.speechOk = function(results) {
      var matches, query, respObj;
      if (results) {
        respObj = JSON.parse(results);
        if (respObj) {
          matches = respObj.speechMatches.speechMatch;
          if (matches.length > 0) {
            query = matches[0];
            query = this.cleanQuery(query);
            console.log('your query was ' + query);
            this.please.ask(query);
            return this.refreshiScroll();
          }
        }
      }
    };

    Cordova.prototype.speak = function(e) {
      return window.plugins.tts.speak(e.response);
    };

    Cordova.prototype.cleanQuery = function(query) {
      return query.replace('needa', 'need a');
    };

    Cordova.prototype.speechInitOk = function() {
      return console.log('speech recognizer is ready');
    };

    Cordova.prototype.speechInitFail = function(m) {
      return console.log('speech recognizer failed');
    };

    Cordova.prototype.speechFail = function(message) {
      return console.log("speechFail: " + message);
    };

    Cordova.prototype.startupFail = function(result) {
      return console.log("Startup failure: " + result);
    };

    return Cordova;

  })();

  new Cordova();

}).call(this);