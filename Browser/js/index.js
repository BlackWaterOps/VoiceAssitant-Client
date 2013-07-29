// Generated by CoffeeScript 1.6.3
(function() {
  var Please,
    __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };

  Please = (function() {
    var operators;

    function Please(options) {
      this.datetimeHelper = __bind(this.datetimeHelper, this);
      this.newDateHelper = __bind(this.newDateHelper, this);
      this.fuzzyHelper = __bind(this.fuzzyHelper, this);
      this.weekdayHelper = __bind(this.weekdayHelper, this);
      this.buildDatetime = __bind(this.buildDatetime, this);
      this.toISOString = __bind(this.toISOString, this);
      this.requestHelper = __bind(this.requestHelper, this);
      this.show = __bind(this.show, this);
      this.expand = __bind(this.expand, this);
      this.keyup = __bind(this.keyup, this);
      this.ask = __bind(this.ask, this);
      this.addDebug = __bind(this.addDebug, this);
      this.resolver = __bind(this.resolver, this);
      this.disambiguate = __bind(this.disambiguate, this);
      this.cancel = __bind(this.cancel, this);
      this.updatePosition = __bind(this.updatePosition, this);
      this.getLocation = __bind(this.getLocation, this);
      this.buildDeviceInfo = __bind(this.buildDeviceInfo, this);
      this.init = __bind(this.init, this);
      this.debug = true;
      this.debugData = {};
      this.classifier = 'http://casper-cached.stremor-nli.appspot.com/';
      this.disambiguator = 'http://casper-cached.stremor-nli.appspot.com/disambiguate';
      this.responder = 'http://rez.stremor-apier.appspot.com/';
      this.lat = 0.00;
      this.lon = 0.00;
      this.sendDeviceInfo = false;
      this.inProgress = false;
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
      return this.getLocation();
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

    Please.prototype.buildDeviceInfo = function() {
      var clientDate, device;
      clientDate = Date();
      return device = {
        "device": {
          "type": "web-client",
          "lat": 0.00,
          "lon": 0.00,
          "timestamp": clientDate.getTime() / 1000,
          "timeoffset": -clientDate.getTimezoneOffset() / 60
        }
      };
    };

    Please.prototype.getLocation = function() {
      return navigator.geolocation.getCurrentPosition(this.updatePosition);
    };

    Please.prototype.updatePosition = function(position) {
      this.lat = position.coords.latitude;
      return this.lon = position.coords.longitude;
    };

    Please.prototype.cancel = function(e) {
      this.board.empty();
      this.mainContext = {};
      this.disambigContext = {};
      this.history = [];
      $('#input-form').removeClass('cancel');
      return this.input.focus();
    };

    Please.prototype.disambiguate = function(payload) {
      var data, endpoint, field, successHandler, text, type,
        _this = this;
      if (this.inProgress === true) {
        endpoint = this.disambiguator + "/active";
        field = this.disambigContext.field;
        type = this.disambigContext.type;
        text = payload;
        data = {
          text: text,
          types: [type]
        };
        console.log('disambiguate user response', data);
      } else {
        console.log('disambiguate rez response');
        endpoint = this.disambiguator + "/passive";
        this.sendDeviceInfo = true;
        field = payload.field;
        type = payload.type;
        text = this.mainContext.payload[field];
        data = {
          text: text,
          types: [type]
        };
      }
      successHandler = function(results) {
        var checkDates, datetime;
        console.log('successHandler', results);
        checkDates = true;
        if (_this.debug === true && _this.inProgress === true) {
          _this.addDebug();
        }
        if (results != null) {
          if ((results.date != null) || (results.time != null)) {
            datetime = _this.buildDatetime(results.date, results.time);
            results[type] = datetime[type];
            checkDates = false;
            console.log('successhandler', results);
          }
        }
        _this.mainContext.payload[field] = results[type];
        return _this.resolver(_this.mainContext, checkDates);
      };
      return this.requestHelper(endpoint, "POST", data, successHandler);
    };

    Please.prototype.resolver = function(response, checkDates) {
      var datetime, payload;
      if (checkDates == null) {
        checkDates = true;
      }
      /*
      		here is where we need to make checks of whether to pass along data
      		to 'REZ' or resolve with the disambiguator
      */

      if (response.status != null) {
        switch (response.status.toLowerCase()) {
          case 'disambiguate':
            console.log('resolver disambiguate', response);
            this.inProgress = false;
            return this.disambiguate(response);
          case 'in progress':
            this.counter = 0;
            this.inProgress = true;
            this.disambigContext = response;
            console.log('resolver progress', response);
            return this.show(response);
          case 'complete':
          case 'completed':
            console.log('resolver complete', response);
            this.counter = 0;
            this.inProgress = false;
            this.disambigContext = {};
            if (response.actor === null || response.actor === void 0) {
              return this.show(response);
            } else {
              return this.requestHelper(this.responder + response.actor, 'POST', this.mainContext, this.show);
            }
        }
      } else {
        console.log('resolver response without status', response);
        payload = response.payload;
        if ((payload != null) && checkDates) {
          if ((payload.start_date != null) || (payload.start_time != null)) {
            datetime = this.buildDatetime(payload.start_date, payload.start_time);
            console.log('datetime no status', datetime);
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
        }
        this.mainContext = response;
        this.counter++;
        if (this.counter < 3) {
          return this.requestHelper(this.responder, "POST", response, this.resolver);
        }
      }
    };

    Please.prototype.addDebug = function(results) {
      var template;
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
      return this.board.find(':last').append(template(results));
    };

    Please.prototype.ask = function(input) {
      var data, template, text,
        _this = this;
      console.log('ask');
      input = $(input);
      text = input.val();
      input.val('');
      template = Handlebars.compile($('#bubblein-template').html());
      this.board.append(template(text)).scrollTop(this.board.find(':last').offset().top);
      $('#input-form').addClass('cancel');
      if (this.inProgress === true) {
        console.log('should disambiguate');
        return this.disambiguate(text);
      } else {
        data = {
          query: text
        };
        return this.requestHelper(this.classifier, "GET", data, function(response) {
          if (_this.debug === true) {
            _this.addDebug();
          }
          return _this.resolver(response);
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
            return this.pos = history.length;
          }
          break;
        case 38:
          if (pos > 0) {
            pos -= 1;
          }
          return target.val(history[pos]);
        case 40:
          if (pos < history.length) {
            pos += 1;
          }
          return target.val(history[pos]);
      }
    };

    Please.prototype.expand = function(e) {
      e.preventDefault();
      return $(e.target).parent().next().toggle();
    };

    Please.prototype.show = function(results) {
      var template, templateName;
      templateName = results.action != null ? results.action : 'bubbleout';
      template = $('#' + templateName + '-template').html();
      template = Handlebars.compile(template);
      this.board.append(template(results)).scrollTop(this.board.find(':last').offset().top);
      if (this.debug === true) {
        this.addDebug(results);
      }
      return this.loader.hide();
    };

    Please.prototype.requestHelper = function(endpoint, type, data, doneHandler) {
      var clientDate,
        _this = this;
      if (this.sendDeviceInfo === true) {
        clientDate = new Date();
        data.device_info = {
          "latitude": this.lat,
          "longitude": this.lon,
          "timestamp": clientDate.getTime() / 1000,
          "timeoffset": -clientDate.getTimezoneOffset() / 60
        };
        this.sendDeviceInfo = false;
      }
      if (this.debug === true) {
        this.debugData = {
          endpoint: endpoint,
          type: type,
          request: data
        };
      }
      return $.ajax({
        url: endpoint,
        type: type,
        data: type === "POST" ? JSON.stringify(data) : data,
        dataType: "json",
        timeout: 10000,
        beforeSend: function() {
          console.log(endpoint, type, data);
          return _this.loader.show();
        }
      }).done(function(response, status) {
        if (_this.debug === true) {
          _this.debugData.status = status;
          _this.debugData.response = response;
        }
        if (doneHandler != null) {
          return doneHandler(response);
        }
      }).fail(function(response, status) {
        console.log('* POST fail', response, response.getResponseHeader());
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

    Please.prototype.buildDatetime = function(date, time) {
      var dateString, newDate;
      newDate = null;
      console.log('start date parsing', date);
      if (date !== null && date !== void 0) {
        newDate = this.datetimeHelper(date);
      }
      console.log('start time parsing', time);
      if (time !== null && time !== void 0) {
        newDate = this.datetimeHelper(time, newDate);
      }
      console.log('buildDatetime datestring', newDate);
      dateString = this.toISOString(newDate).split('T');
      console.log('buildDatetime datestring', dateString);
      return {
        date: dateString[0],
        time: dateString[1]
      };
    };

    Please.prototype.weekdayHelper = function(dayOfWeek) {
      var currentDate, currentDay, date, offset;
      console.log('weekday helper', dayOfWeek);
      date = new Date();
      currentDay = date.getDay();
      currentDate = date.getDate();
      offset = currentDay < dayOfWeek ? dayOfWeek - currentDay : 7 - (currentDay - dayOfWeek);
      date.setDate(currentDate + offset);
      return date;
    };

    Please.prototype.fuzzyHelper = function(datetime, isDate) {
      var date, datetimeArr, def, key, label, preference, presetLabel, splitSym, val;
      console.log('fuzzy helper', datetime, isDate);
      date = new Date();
      console.log('handle fuzzy date or time');
      label = null;
      def = null;
      for (key in datetime) {
        val = datetime[key];
        console.log(key, val);
        if (key === 'label') {
          label = val;
        }
        if (key === 'default') {
          def = val;
        }
      }
      presetLabel = this.presets[label];
      console.log('presetLabel', presetLabel);
      preference = presetLabel != null ? presetLabel : def;
      console.log('use', preference);
      if (preference === null) {
        console.log('useTime error');
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
      console.log('newDate', datetime);
      if (datetime.indexOf('now') !== -1) {
        console.log('is now');
        newDate = new Date();
      } else if (this.dateRegex.test(datetime) === true) {
        console.log('is date string');
        split = datetime.split('-');
        newDate = new Date(split[0], split[1] - 1, split[2]);
      } else if (this.timeRegex.test(datetime) === true) {
        console.log('is time');
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
      console.log('newDate bottom', newDate);
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
      console.log(dateOrTime);
      dateOrTimeType = Object.prototype.toString.call(dateOrTime);
      switch (dateOrTimeType) {
        case '[object String]':
          console.log('is string');
          if (newDate === null) {
            newDate = this.newDateHelper(dateOrTime);
          }
          break;
        case '[object Object]':
          console.log('is object');
          for (action in dateOrTime) {
            parsable = dateOrTime[action];
            console.log('step 1', action, parsable);
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
                    console.log('step 2', 'set datetime');
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
                    console.log('step 3', 'parse array group');
                    interval = item;
                    if (interval === null) {
                      console.log('frag error', interval);
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

    /*   
    	datetimeHelper: (dateOrTime, newDate = null) =>
    		console.log dateOrTime
    
    		for action, parsable of dateOrTime
    			console.log 'step 1', action, parsable
    
    			if action.indexOf('fuzzy') is -1
    				operator = if action.indexOf('add') isnt -1 then '+' else '-'
    
    			parsableType = Object.prototype.toString.call(parsable)
    
    			if parsableType is '[object Array]' # date partials
    				for item in parsable
    					itemType = Object.prototype.toString.call(item);
    
    					if newDate is null 
    						console.log 'step 2', 'set datetime'
    
    						switch itemType
    							when '[object String]' # 'now' or '2013-07-01'
    								newDate = if item is "now" then new Date() else new Date(item)
    							when '[object Object]' # weekday operator {#weekday:1}
    								for itemKey, itemValue of item
    									newDate = @weekdayHelper itemValue if itemKey.indexOf("weekday") isnt -1 and newDate is null
    					else if itemType is '[object Array]' # dates to add to Date object 
    						console.log 'step 3', 'parse array group'
    
    						interval = null
    						type = null
    
    						for frag in item
    							interval = frag if typeof frag is 'number' and not interval?
    							type = frag if typeof frag is 'string' and not type?
    
    						if interval is null or type is null
    							console.log 'frag error', interval, type
    							return
    
    						if type.indexOf('second') isnt -1
    							curr = newDate.getSeconds()
    							time = operators[operator](curr, interval)
    							newDate.setSeconds(time)
    						else if type.indexOf('minute') isnt -1
    							curr = newDate.getMinutes()
    							time = operators[operator](curr, interval)
    							newDate.setMinutes(time)
    						else if type.indexOf('hour') isnt -1
    							curr = newDate.getHours()
    							time = operators[operator](curr, interval)
    							newDate.setHours(time)
    						else if type.indexOf('day') isnt -1
    							curr = newDate.getDate()
    							date = operators[operator](curr, interval)
    							newDate.setDate(date)
    
    			else if parsableType is '[object Object]' # if item is an object we must have a 'special' time
    				console.log 'step 2', 'handle fuzzy date or time'
    				label = null
    				def = null
    
    				for key, val of parsable
    					console.log key, val
    					label = val if key is 'label'
    					def = val if key is 'default'
    
    				presetLabel = @presets[label]
    				console.log 'presetLabel', presetLabel
    				useTime = if presetLabel? then presetLabel else def
    				console.log 'useTime', useTime
    				if useTime is null
    					console.log 'useTime error' 
    					return
    
    				useTimeArr = useTime.trim().split(':')
    
    				newDate.setHours useTimeArr[0]
    				newDate.setMinutes useTimeArr[1]
    				newDate.setSeconds useTimeArr[2]
    
    		return newDate
    */


    return Please;

  })();

  new Please();

}).call(this);
