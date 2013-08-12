// Generated by CoffeeScript 1.6.3
(function() {
  var Please,
    __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };

  Please = (function() {
    var operators;

    function Please(options) {
      this.error = __bind(this.error, this);
      this.log = __bind(this.log, this);
      this.datetimeHelper = __bind(this.datetimeHelper, this);
      this.newDateHelper = __bind(this.newDateHelper, this);
      this.fuzzyHelper = __bind(this.fuzzyHelper, this);
      this.weekdayHelper = __bind(this.weekdayHelper, this);
      this.buildDatetime = __bind(this.buildDatetime, this);
      this.replaceDates = __bind(this.replaceDates, this);
      this.toISOString = __bind(this.toISOString, this);
      this.requestHelper = __bind(this.requestHelper, this);
      this.buildDeviceInfo = __bind(this.buildDeviceInfo, this);
      this.nameMap = __bind(this.nameMap, this);
      this.findOrReplace = __bind(this.findOrReplace, this);
      this.updatePosition = __bind(this.updatePosition, this);
      this.getLocation = __bind(this.getLocation, this);
      this.show = __bind(this.show, this);
      this.addDebug = __bind(this.addDebug, this);
      this.actor = __bind(this.actor, this);
      this.responderSuccessHandler = __bind(this.responderSuccessHandler, this);
      this.auditor = __bind(this.auditor, this);
      this.disambiguatePassive = __bind(this.disambiguatePassive, this);
      this.disambiguatePersonal = __bind(this.disambiguatePersonal, this);
      this.disambiguateActive = __bind(this.disambiguateActive, this);
      this.disambiguateSuccessHandler = __bind(this.disambiguateSuccessHandler, this);
      this.cancel = __bind(this.cancel, this);
      this.expand = __bind(this.expand, this);
      this.keyup = __bind(this.keyup, this);
      this.ask = __bind(this.ask, this);
      this.registerListeners = __bind(this.registerListeners, this);
      this.init = __bind(this.init, this);
      this.debug = true;
      this.debugData = {};
      this.classifier = 'http://casper-cached.stremor-nli.appspot.com/v1';
      this.disambiguator = 'http://casper-cached.stremor-nli.appspot.com/v1/disambiguate';
      this.personal = 'http://stremor-pud.appspot.com/disambiguate';
      this.responder = 'http://rez.stremor-apier.appspot.com/v1/';
      this.lat = 0.00;
      this.lon = 0.00;
      this.mainContext = {};
      this.disambigContext = {};
      this.history = [];
      this.pos = this.history.length;
      this.loader = $('#loader');
      this.board = $('#board');
      this.input = $('#main-input');
      this.dateRegex = /\d{2,4}[-]\d{2}[-]\d{2}/i;
      this.timeRegex = /\d{1,2}[:]\d{2}[:]\d{2}/i;
      this.counter = 0;
      this.currentState = 'init';
      this.presets = {
        'after work': '18:00:00',
        'breakfast': '7:30:00',
        'lunch': '12:00:00'
      };
      this.init();
    }

    Please.prototype.init = function() {
      var init;
      this.input.focus().on('webkitspeechchange', this.ask).on('keyup', this.keyup);
      $('body').on('click', '.expand', this.expand);
      $('#cancel').on('click', this.cancel);
      if (this.board.is(':empty')) {
        init = $('#init');
        init.fadeIn('slow');
        setTimeout((function() {
          return init.fadeOut('slow');
        }), 1000);
      }
      this.getLocation();
      return this.registerListeners();
    };

    Please.prototype.registerListeners = function() {
      return $(document).on('init', this.auditor).on('disambiguate', this.disambiguatePassive).on('disambiguate:personal', this.disambiguatePersonal).on('disambiguate:active', this.disambiguateActive).on('inprogress', this.show).on('completed', this.actor).on('error', this.show).on('debug', this.addDebug);
    };

    Please.prototype.ask = function(input) {
      var data, template, text,
        _this = this;
      input = $(input);
      text = input.val();
      input.val('');
      template = Handlebars.compile($('#bubblein-template').html());
      this.board.append(template(text)).scrollTop(this.board.find('.bubble:last').offset().top);
      $('#input-form').addClass('cancel');
      console.log(this.currentState);
      if (this.currentState === 'inprogress') {
        return $(document).trigger({
          type: 'disambiguate:active',
          response: text
        });
      } else {
        data = {
          query: text
        };
        return this.requestHelper(this.classifier, "GET", data, function(response) {
          return $(document).trigger($.Event('debug')).trigger({
            type: 'init',
            response: response
          });
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
      this.mainContext = {};
      this.disambigContext = {};
      this.history = [];
      $('#input-form').removeClass('cancel');
      this.loader.hide();
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

    Please.prototype.disambiguateSuccessHandler = function(response, field, type) {
      var request;
      if (this.currentState === 'inprogress') {
        this.addDebug();
      }
      if (response != null) {
        this.replaceDates(response);
        if (field.indexOf('.') !== -1) {
          this.findOrReplace(field, response[type]);
        } else {
          this.mainContext.payload[field] = response[type];
        }
        request = $.extend({}, this.mainContext);
        if (response.unused_tokens != null) {
          request.unused_tokens = response.unused_tokens;
        }
        return this.auditor(request);
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
        types: [type]
      };
      return this.requestHelper(this.disambiguator + '/active', 'POST', postData, function(response) {
        return _this.disambiguateSuccessHandler(response, field, type);
      });
    };

    Please.prototype.disambiguatePersonal = function(data) {
      var field, postData, text, type,
        _this = this;
      field = data.field;
      type = data.type;
      if (field.indexOf('.') !== -1) {
        text = this.findOrReplace(field);
      } else {
        text = this.mainContext.payload[field];
      }
      postData = {
        types: [type],
        type: type,
        payload: text
      };
      return this.requestHelper(this.personal, 'POST', postData, function(response) {
        return _this.disambiguateSuccessHandler(response, field, type);
      });
    };

    Please.prototype.disambiguatePassive = function(data) {
      var field, postData, text, type,
        _this = this;
      field = data.field;
      type = data.type;
      if (field.indexOf('.') !== -1) {
        text = this.findOrReplace(field);
      } else {
        text = this.mainContext.payload[field];
      }
      postData = {
        types: [type],
        type: type,
        payload: text
      };
      return this.requestHelper(this.disambiguator + '/passive', 'POST', postData, function(response) {
        return _this.disambiguateSuccessHandler(response, field, type);
      });
    };

    Please.prototype.auditor = function(data) {
      var payload, response;
      response = data instanceof $.Event ? data.response : data;
      console.log('auditor', response);
      this.mainContext = response;
      payload = response.payload;
      if (payload != null) {
        this.replaceDates(payload);
      }
      this.mainContext = response;
      this.counter++;
      if (this.counter < 3) {
        return this.requestHelper(this.responder + 'audit', 'POST', response, this.responderSuccessHandler);
      }
    };

    Please.prototype.responderSuccessHandler = function(response) {
      this.counter = 0;
      this.currentState = response.status.replace(' ', '');
      if (this.currentState === 'inprogress') {
        this.disambigContext = response;
      }
      return $(document).trigger({
        type: this.currentState,
        response: response
      });
    };

    Please.prototype.actor = function(data) {
      this.disambigContext = {};
      if (data.actor === null || data.actor === void 0) {
        return this.show(data);
      } else {
        return this.requestHelper(this.responder + 'actors/' + data.actor, 'POST', this.mainContext, this.show);
      }
    };

    Please.prototype.addDebug = function(results) {
      var template;
      if (this.debug === false) {
        return;
      }
      if (this.debugData.request != null) {
        this.debugData.request = JSON.stringify(this.debugData.request, null, 4);
      }
      if (this.debugData.response != null) {
        this.debugData.response = JSON.stringify(this.debugData.response, null, 4);
      }
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
      console.log('show', results);
      templateName = 'bubbleout';
      templateData = results.show.simple;
      template = $('#' + templateName + '-template');
      template = Handlebars.compile(template.html());
      this.board.append(template(templateData)).scrollTop(this.board.find('.bubble:last').offset().top);
      this.addDebug(results);
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
          this.board.append(template(templateData)).scrollTop(this.board.find('.bubble:last').offset().top);
        }
      }
      return this.loader.hide();
    };

    Please.prototype.getLocation = function() {
      return navigator.geolocation.getCurrentPosition(this.updatePosition);
    };

    Please.prototype.updatePosition = function(position) {
      this.lat = position.coords.latitude;
      return this.lon = position.coords.longitude;
    };

    Please.prototype.findOrReplace = function(field, type) {
      var cursive, fields, found;
      if (type == null) {
        type = null;
      }
      fields = field.split('.');
      found = null;
      cursive = function(obj) {
        var key, val;
        for (key in obj) {
          val = obj[key];
          if (fields.length > 1 && key === fields[0]) {
            fields.shift();
            cursive(val);
          } else if (key === fields[0]) {
            if (type === null) {
              found = obj[key];
            } else {
              obj[key] = type;
            }
            return;
          }
        }
      };
      cursive(this.mainContext);
      return found;
    };

    Please.prototype.nameMap = function(key) {
      var map;
      map = false;
      if (key.indexOf(this.classifier) !== -1) {
        map = "Casper";
      } else if (key.indexOf(this.disambiguator) !== -1) {
        map = "Disambiguator";
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
      if (this.currentState === 'disambiguate') {
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
      console.log(data);
      return $.ajax({
        url: endpoint,
        type: type,
        data: type === "POST" ? JSON.stringify(data) : data,
        dataType: "json",
        timeout: 10000,
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
        return _this.loader.hide();
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

    Please.prototype.replaceDates = function(payload) {
      var datetime;
      if ((payload.start_date != null) || (payload.start_time != null)) {
        datetime = this.buildDatetime(payload.start_date, payload.start_time);
        if (payload.start_date != null) {
          payload.start_date = datetime.date;
        }
        if (payload.start_time != null) {
          payload.start_time = datetime.time;
        }
      }
      if ((payload.end_date != null) || (payload.end_time != null)) {
        datetime = this.buildDatetime(payload.end_date, payload.end_time);
        if (payload.end_date != null) {
          payload.end_date = datetime.date;
        }
        if (payload.end_time != null) {
          payload.end_time = datetime.time;
        }
      }
      if ((payload.date != null) || (payload.time != null)) {
        datetime = this.buildDatetime(payload.date, payload.time);
        if (payload.date != null) {
          payload.date = datetime.date;
        }
        if (payload.time != null) {
          return payload.time = datetime.time;
        }
      }
    };

    Please.prototype.buildDatetime = function(date, time) {
      var dateString, newDate;
      newDate = null;
      if (date !== null && date !== void 0 && dateRegex.test(date) === false) {
        newDate = this.datetimeHelper(date);
      }
      if (time !== null && time !== void 0 && timeRegex.test(time) === false) {
        newDate = this.datetimeHelper(time, newDate);
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

  new Please();

}).call(this);
