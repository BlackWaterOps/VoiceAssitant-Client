// Generated by CoffeeScript 1.6.3
(function() {
  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };

  window.Please = (function() {
    var operators;

    function Please(options) {
      this.error = __bind(this.error, this);
      this.log = __bind(this.log, this);
      this.formatDate = __bind(this.formatDate, this);
      this.elapsedTimeHelper = __bind(this.elapsedTimeHelper, this);
      this.datetimeHelper = __bind(this.datetimeHelper, this);
      this.newDateHelper = __bind(this.newDateHelper, this);
      this.fuzzyHelper = __bind(this.fuzzyHelper, this);
      this.weekdayHelper = __bind(this.weekdayHelper, this);
      this.buildDatetime = __bind(this.buildDatetime, this);
      this.replaceDates = __bind(this.replaceDates, this);
      this.replaceLocation = __bind(this.replaceLocation, this);
      this.prependTo = __bind(this.prependTo, this);
      this.clientOperations = __bind(this.clientOperations, this);
      this.toISOString = __bind(this.toISOString, this);
      this.requestHelper = __bind(this.requestHelper, this);
      this.buildDeviceInfo = __bind(this.buildDeviceInfo, this);
      this.nameMap = __bind(this.nameMap, this);
      this.replace = __bind(this.replace, this);
      this.find = __bind(this.find, this);
      this.reduce = __bind(this.reduce, this);
      this.updatePosition = __bind(this.updatePosition, this);
      this.getLocation = __bind(this.getLocation, this);
      this.show = __bind(this.show, this);
      this.addDebug = __bind(this.addDebug, this);
      this.actorResponseHandler = __bind(this.actorResponseHandler, this);
      this.actor = __bind(this.actor, this);
      this.auditorSuccessHandler = __bind(this.auditorSuccessHandler, this);
      this.auditor = __bind(this.auditor, this);
      this.handleChoice = __bind(this.handleChoice, this);
      this.choose = __bind(this.choose, this);
      this.disambiguatePassive = __bind(this.disambiguatePassive, this);
      this.disambiguatePersonal = __bind(this.disambiguatePersonal, this);
      this.disambiguateActive = __bind(this.disambiguateActive, this);
      this.disambiguateSuccessHandler = __bind(this.disambiguateSuccessHandler, this);
      this.classify = __bind(this.classify, this);
      this.replaceContext = __bind(this.replaceContext, this);
      this.cancel = __bind(this.cancel, this);
      this.expand = __bind(this.expand, this);
      this.keyup = __bind(this.keyup, this);
      this.ask = __bind(this.ask, this);
      this.registerHandlebarHelpers = __bind(this.registerHandlebarHelpers, this);
      this.registerListeners = __bind(this.registerListeners, this);
      this.init = __bind(this.init, this);
      this.debug = true;
      this.debugData = {};
      this.classifier = 'http://casper-cached.stremor-nli.appspot.com/v1';
      this.disambiguator = 'http://casper-cached.stremor-nli.appspot.com/v1/disambiguate';
      this.personal = 'http://stremor-pud.appspot.com/v1/';
      this.responder = 'http://rez.stremor-apier.appspot.com/v1/';
      this.lat = 33.4930947;
      this.lon = -111.928558;
      this.mainContext = null;
      this.disambigContext = null;
      this.history = [];
      this.pos = this.history.length;
      this.loader = $('.loader');
      this.board = $('.board');
      this.input = $('.main-input');
      this.dateRegex = /\d{2,4}[-]\d{2}[-]\d{2}/i;
      this.timeRegex = /\d{1,2}[:]\d{2}[:]\d{2}/i;
      this.counter = 0;
      this.disableSpeech = false;
      this.currentState = {
        status: null,
        origin: null
      };
      this.presets = {
        'after work': '18:00:00',
        'breakfast': '7:30:00',
        'lunch': '12:00:00'
      };
      this.init();
    }

    Please.prototype.init = function() {
      var init;
      this.getLocation();
      this.registerHandlebarHelpers();
      this.registerListeners();
      this.input.focus().on('webkitspeechchange', this.ask).on('keyup', this.keyup);
      $('body').on('click', '.expand', this.expand).on('click', '.choice-item', this.handleChoice);
      $('#cancel').on('click', this.cancel);
      if (this.board.is(':empty')) {
        init = $('.init');
        init.fadeIn('slow');
        return setTimeout((function() {
          return init.fadeOut('slow');
        }), 1000);
      }
    };

    Please.prototype.registerListeners = function() {
      return $(document).on('init', this.classify).on('audit', this.auditor).on('disambiguate', this.disambiguatePassive).on('disambiguate:personal', this.disambiguatePersonal).on('disambiguate:active', this.disambiguateActive).on('restart', this.replaceContext).on('choice', this.choose).on('inprogress', this.show).on('completed', this.actor).on('error', this.show).on('debug', this.addDebug);
    };

    Please.prototype.registerHandlebarHelpers = function() {
      var _this = this;
      Handlebars.registerHelper('elapsedTime', function(dateString) {
        var results;
        results = _this.elapsedTimeHelper(dateString);
        return results.newDate + ' ' + results.newTime;
      });
      Handlebars.registerHelper('flightDates', function(dateString, options) {
        var am, day, dd, formatted, hh, min, mm, mon, result, yy;
        if (dateString == null) {
          return "--";
        }
        formatted = _this.formatDate(dateString);
        am = formatted.am;
        mm = formatted.month;
        dd = formatted.date;
        yy = formatted.year;
        hh = formatted.hours;
        min = formatted.minutes;
        day = formatted.dayOfWeek;
        mon = formatted.monthOfYear;
        result = "<span class=\"formatted-time\">" + hh + ":" + min + "</span><span class=\"formatted-date\">" + day + ", " + mon + " " + dd + ", " + yy + "</span>";
        return new Handlebars.SafeString(result);
      });
      return Handlebars.registerHelper('eventDates', function(dateString) {
        var day, dd, formatted, mm, mon, yy;
        formatted = _this.formatDate(dateString);
        mm = formatted.month;
        dd = formatted.date;
        yy = formatted.year;
        day = formatted.dayOfWeek;
        mon = formatted.monthOfYear;
        return day.substr(0, 3) + ", " + mon.substr(0, 3) + " " + dd + ", " + yy;
      });
    };

    Please.prototype.ask = function(input) {
      var template, text;
      if (typeof input === 'string') {
        text = input;
      } else {
        input = $(input);
        text = input.val();
        input.val('');
      }
      template = Handlebars.compile($('#bubblein-template').html());
      this.board.append(template(text)).scrollTop(this.board.find('.bubble:last').offset().top);
      $('.input-form').addClass('cancel');
      if (this.currentState.state === 'inprogress' || (this.currentState.state === 'error' && (this.disambigContext != null))) {
        if (this.currentState.origin === 'actor') {
          return $(document).trigger({
            type: 'completed',
            response: null
          });
        } else {
          return $(document).trigger({
            type: 'disambiguate:active',
            response: text
          });
        }
      } else {
        return $(document).trigger({
          type: 'init',
          response: text
        });
      }
    };

    Please.prototype.keyup = function(e) {
      var target, value;
      value = $(e.target).val();
      target = $(e.target);
      switch (e.which) {
        case 13:
          if (value) {
            this.ask(target);
            this.history.push(value);
            return this.pos = this.history.length;
          }
          break;
        case 38:
          if (this.pos > 0) {
            this.pos -= 1;
          }
          return target.val(this.history[this.pos]);
        case 40:
          if (this.pos < this.history.length) {
            this.pos += 1;
          }
          return target.val(this.history[this.pos]);
      }
    };

    Please.prototype.expand = function(e) {
      e.preventDefault();
      return $(e.target).parent().next().toggle();
    };

    Please.prototype.cancel = function(e) {
      this.board.empty();
      this.mainContext = null;
      this.disambigContext = null;
      this.history = [];
      this.currentState = {
        state: null,
        origin: null
      };
      $('.input-form').removeClass('cancel');
      this.loader.hide();
      this.counter = 0;
      return this.input.focus();
    };

    Please.prototype.store = {
      createCookie: function(k, v, d) {
        var exp, val, _ref;
        exp = new Date();
        exp.setDate(exp.getDate() + d);
        val = escape(v) + ((_ref = exdays === null) != null ? _ref : {
          "": "; expires=" + exp.toUTCString()
        });
        document.cookie = k + "=" + v;
        return true;
      },
      get: function(k) {
        var i, x, y;
        if (window.Modernizr.localstorage) {
          return $.parseJSON(localStorage.getItem(k));
        } else {
          i = 0;
          while (i < c.length) {
            x = c[i].substr(0, c[i].indexOf("="));
            y = c[i].substr(c[i].indexOf("=") + 1);
            x = x.replace(/^\s+|\s+$/g, "");
            if (x === k) {
              return unescape(y);
            }
            c++;
          }
        }
      },
      set: function(k, v) {
        if (window.Modernizr.localstorage) {
          return localStorage.setItem(k, JSON.stringify(v));
        } else {
          return createCookie(k, v, 365);
        }
      },
      remove: function(k) {
        if (window.Modernizr.localstorage) {
          return localStorage.removeItem(k);
        } else {
          return createCookie(k, v, -1);
        }
      },
      clear: function() {
        if (window.Modernizr.localstorage) {
          return localStorage.clear();
        } else {
          return createCookie(k, v, -1);
        }
      }
    };

    Please.prototype.replaceContext = function(e) {
      var response;
      this.counter = 0;
      response = e.response;
      this.mainContext = response.data;
      return this.auditor(this.mainContext);
    };

    Please.prototype.classify = function(e) {
      var data, query,
        _this = this;
      query = e instanceof $.Event ? e.response : e;
      data = {
        query: query
      };
      return this.requestHelper(this.classifier, "GET", data, function(response) {
        return $(document).trigger($.Event('debug')).trigger({
          type: 'audit',
          response: response
        });
      });
    };

    Please.prototype.disambiguateSuccessHandler = function(response, field, type) {
      var request;
      if (this.currentState.state === 'inprogress' || this.currentState.state === 'error') {
        $(document).trigger($.Event('debug'));
      }
      if (response != null) {
        this.clientOperations(response);
        if (field.indexOf('.') !== -1) {
          this.replace(field, response[type]);
        } else {
          this.mainContext.payload[field] = response[type];
        }
        request = $.extend({}, this.mainContext);
        return this.auditor(request);
        /*
        			$(document).trigger(
        				type: 'audit'
        				response: request
        			)
        */

      } else {
        return console.log('oops no responder response', results);
      }
    };

    Please.prototype.disambiguateActive = function(e) {
      var field, postData, text, type,
        _this = this;
      field = this.disambigContext.field;
      type = this.disambigContext.type;
      text = e.response;
      postData = {
        payload: text,
        type: type
      };
      return this.requestHelper(this.disambiguator + '/active', 'POST', postData, function(response) {
        return _this.disambiguateSuccessHandler(response, field, type);
      });
    };

    Please.prototype.disambiguatePersonal = function(e) {
      var data, field, postData, text, type,
        _this = this;
      data = e.response;
      field = data.field;
      type = data.type;
      if (field.indexOf('.') !== -1) {
        text = this.find(field);
      } else {
        text = this.mainContext.payload[field];
      }
      postData = {
        type: type,
        payload: text
      };
      return this.requestHelper(this.personal + 'disambiguate', 'POST', postData, function(response) {
        return _this.disambiguateSuccessHandler(response, field, type);
      });
    };

    Please.prototype.disambiguatePassive = function(e) {
      var data, field, postData, text, type,
        _this = this;
      data = e.response;
      field = data.field;
      type = data.type;
      if (field.indexOf('.') !== -1) {
        text = this.find(field);
      } else {
        text = this.mainContext.payload[field];
      }
      postData = {
        type: type,
        payload: text
      };
      return this.requestHelper(this.disambiguator + '/passive', 'POST', postData, function(response) {
        return _this.disambiguateSuccessHandler(response, field, type);
      });
    };

    Please.prototype.choose = function(e) {
      var data, item, list, listItem, _i, _len, _ref;
      data = e.response;
      list = $('<ul/>');
      _ref = data.show.simple.list;
      for (_i = 0, _len = _ref.length; _i < _len; _i++) {
        item = _ref[_i];
        listItem = $('<li/>').addClass('choice-item').data('choice', item).append($('<a/>').text(item.text));
        list.append(listItem);
      }
      $('.list-slider').html(list);
      $('body').addClass('choice');
      return this.show(e);
    };

    Please.prototype.handleChoice = function(e) {
      var choice, field, template;
      $('body').removeClass('choice');
      choice = $(e.currentTarget).data('choice');
      field = this.disambigContext.field;
      template = Handlebars.compile($('#bubblein-template').html());
      this.board.append(template(choice.text)).scrollTop(this.board.find('.bubble:last').offset().top);
      if (field.indexOf('.') !== -1) {
        this.replace(field, choice);
      } else {
        this.mainContext.payload[field] = choice;
      }
      return $(document).trigger({
        type: 'audit',
        response: this.mainContext
      });
    };

    Please.prototype.auditor = function(data) {
      var payload, response;
      response = data instanceof $.Event ? data.response : data;
      payload = response.payload;
      if (payload != null) {
        this.clientOperations(payload);
      }
      this.mainContext = response;
      this.counter++;
      if (this.counter < 3) {
        return this.requestHelper(this.responder + 'audit', 'POST', response, this.auditorSuccessHandler);
      }
    };

    Please.prototype.auditorSuccessHandler = function(response) {
      this.currentState = {
        state: response.status.replace(' ', ''),
        origin: 'auditor'
      };
      if (this.currentState.state === 'inprogress' || this.currentState.state === 'choice') {
        this.disambigContext = response;
      }
      return $(document).trigger({
        type: this.currentState.state,
        response: response
      });
    };

    Please.prototype.actor = function(e) {
      var data, endpoint;
      this.disambigContext = {};
      data = e.response;
      if (data.actor === null || data.actor === void 0) {
        return this.show(data);
      } else {
        endpoint = this.responder + 'actors/' + data.actor;
        if (data.actor.indexOf('private') !== -1) {
          data.actor = data.actor.replace('private:', '');
          endpoint = this.personal + 'actors/' + data.actor;
        }
        return this.requestHelper(endpoint, 'POST', this.mainContext, this.actorResponseHandler);
      }
    };

    Please.prototype.actorResponseHandler = function(response) {
      if (response.status != null) {
        this.currentState = {
          state: response.status.replace(' ', ''),
          origin: 'actor'
        };
        return $(document).trigger({
          type: this.currentState.state,
          response: response
        });
      } else {
        return this.show(response);
      }
    };

    Please.prototype.addDebug = function(e) {
      var results, template;
      if (this.debug === false) {
        return;
      }
      if (this.debugData.request != null) {
        this.debugData.request = JSON.stringify(this.debugData.request, null, 4);
      }
      if (this.debugData.response != null) {
        this.debugData.response = JSON.stringify(this.debugData.response, null, 4);
      }
      results = e.response;
      if (!results) {
        results = {
          debug: this.debugData
        };
      } else {
        results.debug = this.debugData;
      }
      template = Handlebars.compile($('#debug-template').html());
      return this.board.find('.bubble:last').append(template(results));
    };

    Please.prototype.show = function(results) {
      var template, templateBase, templateData, templateName, templateType;
      if (results instanceof $.Event) {
        results = results.response;
      }
      templateData = results.show.simple;
      if (templateData.link != null) {
        templateName = 'link';
      } else if (templateData.image != null) {
        templateName = 'image';
      } else {
        templateName = 'bubbleout';
      }
      template = $('#' + templateName + '-template');
      template = Handlebars.compile(template.html());
      this.board.append(template(templateData)).scrollTop(this.board.find('.bubble:last').offset().top);
      $(document).trigger({
        type: 'debug',
        response: results
      });
      if ((results.show != null) && (results.show.structured != null) && (results.show.structured.template != null)) {
        templateData = results.show.structured.items;
        template = results.show.structured.template.split(':');
        templateBase = template[0];
        templateType = template[1];
        templateName = template[2] != null ? template[2] : templateType;
        template = $('#' + templateType + '-template');
        if (template.length === 0) {
          template = $('#' + templateBase + '-template');
        }
        if (template.length > 0) {
          template = Handlebars.compile(template.html());
          this.board.find('.bubble:last').append(template(templateData)).scrollTop(this.board.find('.bubble:last').offset().top);
        }
      }
      this.loader.hide();
      this.counter = 0;
      if (window !== window.top) {
        return window.top.postMessage({
          action: 'speak',
          speak: results.speak,
          options: {}
        }, '*');
      } else {
        return $(document).trigger({
          type: 'speak',
          response: results.speak
        });
      }
    };

    Please.prototype.getLocation = function() {
      return navigator.geolocation.getCurrentPosition(this.updatePosition);
    };

    Please.prototype.updatePosition = function(position) {
      this.lat = position.coords.latitude;
      return this.lon = position.coords.longitude;
    };

    Please.prototype.reduce = function(fun, iterable, initial) {
      if (iterable.length > 0) {
        initial = fun(initial, iterable[0]);
        return this.reduce(fun, iterable.slice(1), initial);
      } else {
        return initial;
      }
    };

    Please.prototype.find = function(field) {
      var fields,
        _this = this;
      fields = field.split('.');
      if ('function' === typeof Array.prototype.reduce) {
        return fields.reduce(function(prevVal, currVal, index, array) {
          return prevVal[currVal] || null;
        }, this.mainContext);
      } else {
        return this.reduce(function(obj, key) {
          return obj[key] || null;
        }, fields, this.mainContext);
      }
    };

    Please.prototype.replace = function(field, type) {
      var fields, last, obj;
      fields = field.split('.');
      last = fields.pop();
      field = fields.join('.');
      obj = this.find(field);
      return obj[last] = type;
    };

    Please.prototype.nameMap = function(key) {
      var map;
      map = false;
      if (key.indexOf(this.disambiguator) !== -1) {
        map = "Disambiguator";
      } else if (key.indexOf(this.classifier) !== -1) {
        map = "Casper";
      } else if (key.indexOf(this.responder) !== -1) {
        map = "Rez";
      } else if (key.indexOf(this.personal) !== -1) {
        map = "Pud";
      }
      return map;
    };

    Please.prototype.buildDeviceInfo = function() {
      var clientDate, deviceInfo;
      clientDate = new Date();
      return deviceInfo = {
        "latitude": this.lat,
        "longitude": this.lon,
        "timestamp": clientDate.getTime() / 1000,
        "timeoffset": -clientDate.getTimezoneOffset() / 60
      };
    };

    Please.prototype.requestHelper = function(endpoint, type, data, doneHandler) {
      var endpointName,
        _this = this;
      if (endpoint.indexOf(this.disambiguator) !== -1 && endpoint.indexOf('passive') !== -1) {
        data = $.extend({}, data);
        data.device_info = this.buildDeviceInfo();
      }
      if (this.debug === true) {
        this.debugData = {
          endpoint: endpoint,
          type: type,
          request: data
        };
      }
      endpointName = this.nameMap(endpoint);
      return $.ajax({
        url: endpoint,
        type: type,
        data: type === "POST" ? JSON.stringify(data) : data,
        dataType: "json",
        timeout: 20000,
        beforeSend: function() {
          _this.log(endpointName, ">", data);
          return _this.loader.show();
        }
      }).done(function(response, status) {
        _this.log(endpointName, "<", response);
        if (_this.debug === true) {
          _this.debugData.status = status;
          _this.debugData.response = response;
        }
        if (doneHandler != null) {
          return doneHandler(response);
        }
      }).fail(function(response, status) {
        _this.error(endpointName, "<", (response.responseJSON != null ? response.responseJSON : response));
        if (_this.debug === true) {
          _this.debugData.status = status;
          _this.debugData.response = response;
        }
        _this.loader.hide();
        return _this.counter = 0;
      });
    };

    operators = {
      "+": function(left, right) {
        return parseInt(left, 10) + parseInt(right, 10);
      },
      "-": function(left, right) {
        return parseInt(left, 10) - parseInt(right, 10);
      }
    };

    Please.prototype.toISOString = function(dateObj) {
      var pad;
      pad = function(number) {
        var r;
        r = String(number);
        if (r.length === 1) {
          r = '0' + r;
        }
        return r;
      };
      return dateObj.getFullYear() + '-' + pad(dateObj.getMonth() + 1) + '-' + pad(dateObj.getDate()) + 'T' + pad(dateObj.getHours()) + ':' + pad(dateObj.getMinutes()) + ':' + pad(dateObj.getSeconds());
    };

    Please.prototype.clientOperations = function(data) {
      this.replaceLocation(data);
      this.replaceDates(data);
      if (data.unused_tokens != null) {
        return this.prependTo(data);
      }
    };

    Please.prototype.prependTo = function(data) {
      var field, payloadField, prepend;
      prepend = data.unused_tokens.join(" ");
      field = data.prepend_to;
      payloadField = this.mainContext.payload[field];
      payloadField = payloadField == null ? "" : " " + payloadField;
      return this.mainContext.payload[field] = prepend + payloadField;
    };

    Please.prototype.replaceLocation = function(payload) {
      if ((payload != null) && (payload.location != null)) {
        switch (payload.location) {
          case '#current_location':
            payload.location = this.buildDeviceInfo();
        }
      }
    };

    Please.prototype.replaceDates = function(payload) {
      var date, datetime, datetimes, pair, time, _i, _len;
      datetimes = [['date', 'time'], ['start_date', 'start_time'], ['end_date', 'end_time']];
      for (_i = 0, _len = datetimes.length; _i < _len; _i++) {
        pair = datetimes[_i];
        date = pair[0];
        time = pair[1];
        if ((payload[date] != null) || (payload[time] != null)) {
          datetime = this.buildDatetime(payload[date], payload[time]);
          if (datetime != null) {
            if (payload[date] != null) {
              payload[date] = datetime.date;
            }
            if (payload[time] != null) {
              payload[time] = datetime.time;
            }
          }
        }
      }
    };

    Please.prototype.buildDatetime = function(date, time) {
      var dateString, newDate;
      newDate = null;
      if (date !== null && date !== void 0 && this.dateRegex.test(date) === false) {
        newDate = this.datetimeHelper(date);
      }
      if (time !== null && time !== void 0 && this.timeRegex.test(time) === false) {
        newDate = this.datetimeHelper(time, newDate);
      }
      if (newDate == null) {
        return;
      }
      dateString = this.toISOString(newDate).split('T');
      return {
        date: dateString[0],
        time: dateString[1]
      };
    };

    Please.prototype.weekdayHelper = function(dayOfWeek) {
      var currentDate, currentDay, date, offset;
      date = new Date();
      currentDay = date.getDay();
      currentDate = date.getDate();
      offset = currentDay < dayOfWeek ? dayOfWeek - currentDay : 7 - (currentDay - dayOfWeek);
      date.setDate(currentDate + offset);
      return date;
    };

    Please.prototype.fuzzyHelper = function(datetime, isDate) {
      var date, datetimeArr, def, key, label, preference, presetLabel, splitSym, val;
      date = new Date();
      label = null;
      def = null;
      for (key in datetime) {
        val = datetime[key];
        if (key === 'label') {
          label = val;
        }
        if (key === 'default') {
          def = val;
        }
      }
      presetLabel = this.presets[label];
      preference = presetLabel != null ? presetLabel : def;
      if (preference === null) {
        return;
      }
      splitSym = isDate === true ? '-' : ':';
      datetimeArr = preference.trim().split(splitSym);
      if (isDate === true) {
        date.setFullYear(datetimeArr[0]);
        date.setMonth(datetimeArr[1] - 1);
        date.setDate(datetimeArr[2]);
      } else {
        date.setHours(datetimeArr[0]);
        date.setMinutes(datetimeArr[1]);
        date.setSeconds(datetimeArr[2]);
      }
      return date;
    };

    Please.prototype.newDateHelper = function(datetime) {
      var hours, minutes, newDate, seconds, split;
      if (datetime.indexOf('now') !== -1) {
        newDate = new Date();
      } else if (this.dateRegex.test(datetime) === true) {
        split = datetime.split('-');
        newDate = new Date(split[0], split[1] - 1, split[2]);
      } else if (this.timeRegex.test(datetime) === true) {
        newDate = new Date();
        split = datetime.split(':');
        hours = newDate.getHours();
        minutes = newDate.getMinutes();
        seconds = newDate.getSeconds();
        if ((hours > split[0]) || (hours === split[0] && minutes > split[1])) {
          newDate.setDate(newDate.getDate() + 1);
        }
        newDate.setHours(split[0]);
        newDate.setMinutes(split[1]);
        newDate.setSeconds(split[2]);
      }
      if (newDate === null || newDate === void 0) {
        return new Date();
      } else {
        return newDate;
      }
    };

    Please.prototype.datetimeHelper = function(dateOrTime, newDate) {
      var action, curr, date, dateOrTimeType, interval, isDate, item, itemKey, itemType, itemValue, operator, parsable, parsableType, time, _i, _len;
      if (newDate == null) {
        newDate = null;
      }
      dateOrTimeType = Object.prototype.toString.call(dateOrTime);
      switch (dateOrTimeType) {
        case '[object String]':
          if (newDate === null) {
            newDate = this.newDateHelper(dateOrTime);
          }
          break;
        case '[object Object]':
          for (action in dateOrTime) {
            parsable = dateOrTime[action];
            if (action.indexOf('weekday') !== -1) {
              return this.weekdayHelper(parsable);
            } else if (action.indexOf('fuzzy') !== -1) {
              isDate = action.indexOf('date') !== -1 ? true : false;
              return this.fuzzyHelper(parsable, isDate);
            } else {
              operator = action.indexOf('add') !== -1 ? '+' : '-';
              parsableType = Object.prototype.toString.call(parsable);
              if (parsableType === '[object Array]') {
                for (_i = 0, _len = parsable.length; _i < _len; _i++) {
                  item = parsable[_i];
                  itemType = Object.prototype.toString.call(item);
                  if (newDate === null) {
                    switch (itemType) {
                      case '[object String]':
                        newDate = this.newDateHelper(item);
                        break;
                      case '[object Object]':
                        for (itemKey in item) {
                          itemValue = item[itemKey];
                          if (newDate === null) {
                            if (itemKey.indexOf('weekday') !== -1) {
                              newDate = this.weekdayHelper(itemValue);
                            } else if (itemKey.indexOf('fuzzy') !== -1) {
                              isDate = itemKey.indexOf('date') !== -1 ? true : false;
                              newDate = this.fuzzyHelper(itemValue, isDate);
                            }
                          }
                        }
                    }
                  } else if (itemType === '[object Number]') {
                    interval = item;
                    if (interval === null) {
                      return;
                    }
                    if (action.indexOf('time') !== -1) {
                      curr = newDate.getSeconds();
                      time = operators[operator](curr, interval);
                      newDate.setSeconds(time);
                    } else if (action.indexOf('date') !== -1) {
                      curr = newDate.getDate();
                      date = operators[operator](curr, interval);
                      newDate.setDate(date);
                    }
                  }
                }
              }
            }
          }
      }
      return newDate;
    };

    Please.prototype.elapsedTimeHelper = function(dateString) {
      var dTime, dt, formatted, origPubdate, origPubtime, pTime, pubdate, pubtime, uTime, ut;
      formatted = this.formatDate(dateString);
      pubdate = formatted.month + "/" + formatted.date + "/" + formatted.year;
      pubtime = formatted.hours + ":" + formatted.minutes;
      origPubdate = pubdate;
      origPubtime = pubtime;
      dt = new Date(dateString);
      ut = new Date();
      dTime = dt.getTime();
      uTime = ut.getTime();
      if ((uTime - dTime) < 86400000) {
        pubdate = "";
        pTime = Math.round(((uTime - dTime) / 1000) / 60);
        if (pTime < 60) {
          pubtime = "About " + pTime + " minutes ago";
        } else {
          pTime = Math.round(pTime / 60);
          if (pTime === 1) {
            pubtime = "About " + pTime + " hour ago";
          } else {
            pubtime = "About " + pTime + " hours ago";
          }
        }
      }
      return {
        oldDate: origPubtime,
        oldTime: origPubdate,
        newDate: pubdate,
        newTime: pubtime
      };
    };

    Please.prototype.formatDate = function(dateString) {
      var am, date, day, daysOfTheWeek, dd, dt, hh, min, mm, monthsOfTheYear, yy;
      monthsOfTheYear = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
      daysOfTheWeek = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
      dt = new Date(dateString);
      mm = dt.getMonth() + 1;
      dd = dt.getDate();
      yy = dt.getFullYear();
      hh = dt.getHours();
      min = dt.getMinutes();
      day = dt.getDay();
      if (mm < 10) {
        mm = "0" + mm;
      }
      if (dd < 10) {
        dd = "0" + dd;
      }
      am = hh > 12 ? "pm" : "am";
      if (hh > 12) {
        hh = hh - 12;
      }
      if (hh < 10) {
        hh = "0" + hh;
      }
      if (min < 10) {
        min = "0" + min;
      }
      return date = {
        year: yy,
        month: mm,
        monthOfYear: monthsOfTheYear[dt.getMonth()],
        date: dd,
        day: day,
        dayOfWeek: daysOfTheWeek[day],
        hours: hh,
        minutes: min,
        am: am
      };
    };

    Please.prototype.log = function() {
      var args, argument, _i, _len;
      args = [];
      for (_i = 0, _len = arguments.length; _i < _len; _i++) {
        argument = arguments[_i];
        if (typeof argument === 'object') {
          argument = JSON.stringify(argument, null, " ");
        }
        args.push(argument);
      }
      return console.log(args.join(" "));
    };

    Please.prototype.error = function() {
      var args, argument, _i, _len;
      args = [];
      for (_i = 0, _len = arguments.length; _i < _len; _i++) {
        argument = arguments[_i];
        if (typeof argument === 'object') {
          argument = JSON.stringify(argument, null, " ");
        }
        args.push(argument);
      }
      return console.error(args.join(" "));
    };

    return Please;

  })();

}).call(this);
