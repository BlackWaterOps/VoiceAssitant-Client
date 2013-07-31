// Generated by CoffeeScript 1.6.3
(function() {
  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; },
    __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'util', 'models/appState', 'models/classifier', 'models/responder', 'models/disambiguator', 'handlebars'], function(_, Backbone, Util, AppState, Classifier, Responder, Disambiguator) {
    var IndexView, _ref;
    return IndexView = (function(_super) {
      __extends(IndexView, _super);

      function IndexView() {
        this.resolver = __bind(this.resolver, this);
        this.disambiguateResults = __bind(this.disambiguateResults, this);
        this.disambiguate = __bind(this.disambiguate, this);
        this.keyup = __bind(this.keyup, this);
        this.ask = __bind(this.ask, this);
        this.addDebug = __bind(this.addDebug, this);
        this.cancel = __bind(this.cancel, this);
        this.show = __bind(this.show, this);
        this.updatePosition = __bind(this.updatePosition, this);
        this.getLocation = __bind(this.getLocation, this);
        this.log = __bind(this.log, this);
        _ref = IndexView.__super__.constructor.apply(this, arguments);
        return _ref;
      }

      IndexView.prototype.events = {
        'keyup #main-input': 'keyup',
        'webkitspeechchange #main-input': 'ask',
        'click .expand': 'expand',
        'click #cancel': 'cancel'
      };

      IndexView.prototype.initialize = function() {
        this.board = this.$('#board');
        this.loader = this.$('#loader');
        this.input = this.$('#main-input');
        this.form = this.$('#input-form');
        this.checkDates = false;
        return AppState.on('change:mainContext change:responderContext', this.resolver);
      };

      IndexView.prototype.render = function(options) {
        var init;
        this.input.focus();
        if (this.board.is(':empty')) {
          init = $('#init');
          init.fadeIn('slow');
          setTimeout((function() {
            return init.fadeOut('slow');
          }), 1000);
        }
        return this.getLocation();
      };

      IndexView.prototype.log = function() {
        return Util.log(arguments);
      };

      IndexView.prototype.getLocation = function() {
        return navigator.geolocation.getCurrentPosition(this.updatePosition);
      };

      IndexView.prototype.updatePosition = function(position) {
        return AppState.set({
          lat: position.coords.latitude,
          lon: position.coords.longitude
        });
      };

      IndexView.prototype.show = function(results) {
        var template, templateName;
        templateName = results.action != null ? results.action : 'bubbleout';
        template = $('#' + templateName + '-template').html();
        template = Handlebars.compile(template);
        this.board.append(template(results)).scrollTop(this.board.find(':last').offset().top);
        if (AppState.get('debug' === true)) {
          this.addDebug(results);
        }
        return this.loader.hide();
      };

      IndexView.prototype.cancel = function(e) {
        this.board.empty();
        AppState.set({
          mainContext: {},
          responderContext: {},
          history: []
        });
        this.form.removeClass('cancel');
        this.loader.hide();
        return this.input.focus();
      };

      IndexView.prototype.addDebug = function(results) {
        var debugDate, template;
        debugDate = AppState.get('debugData');
        if (debugData.request != null) {
          debugData.request = JSON.stringify(debugData.request, null, 4);
        }
        if (debugData.response != null) {
          debugData.response = JSON.stringify(debugData.response, null, 4);
        }
        if (!results) {
          results = {
            debug: debugData
          };
        } else {
          results.debug = debugData;
        }
        template = Handlebars.compile($('#debug-template').html());
        return this.board.find(':last').append(template(results));
      };

      IndexView.prototype.ask = function(e) {
        var classifier, input, template, text;
        input = $(e);
        text = input.val();
        input.val('');
        template = Handlebars.compile($('#bubblein-template').html());
        this.board.append(template(text)).scrollTop(this.board.find(':last').offset().top);
        this.form.addClass('cancel');
        if (AppState.get('inProgress') === true) {
          this.log('should disambiguate');
          return this.disambiguate(text);
        } else {
          classifier = new Classifier();
          return classifier.fetch({
            data: {
              query: text
            }
          });
        }
      };

      IndexView.prototype.keyup = function(e) {
        var history, pos, target, value;
        value = $(e.target).val();
        target = $(e.target);
        history = AppState.get('history');
        pos = AppState.get('pos');
        switch (e.which) {
          case 13:
            if (value) {
              this.ask(target);
              return AppState.set('pos', history.length);
            }
            break;
          case 38:
            if (pos > 0) {
              AppState.set('pos', pos - 1);
            }
            return target.val(history[pos]);
          case 40:
            if (pos < history.length) {
              AppState.set('pos', pos + 1);
            }
            return target.val(history[pos]);
        }
      };

      IndexView.prototype.disambiguate = function(payload) {
        var action, context, data, dis, field, text, type,
          _this = this;
        if (AppState.get('inProgress') === true) {
          action = 'active';
          context = AppState.get('responderContext');
          field = context.field;
          type = context.type;
          text = payload;
          data = {
            text: text,
            types: [type]
          };
          this.log('disambiguate user response', data);
        } else {
          this.log('disambiguate rez response');
          action = 'passive';
          context = AppState.get('mainContext');
          field = payload.field;
          type = payload.type;
          text = context.payload[field];
          data = {
            text: text,
            types: [type],
            device_info: Util.buildDeviceInfo()
          };
        }
        dis = new Disambiguator(data, {
          action: action
        });
        dis.on('done', function(model, response, options) {
          _this.log('disambiguator done', response, field, type);
          return _this.disambiguateResults(response, field, type);
        });
        return dis.post();
      };

      IndexView.prototype.disambiguateResults = function(response, field, type) {
        var datetime;
        this.log('disambiguate successHandler', response, field, type);
        this.checkDates = true;
        if (AppState.get('debug') === true && AppState.get('inProgress') === true) {
          this.addDebug();
        }
        if (response != null) {
          if ((response.date != null) || (response.time != null)) {
            datetime = Util.buildDatetime(response.date, response.time);
            response[type] = datetime[type];
            this.log('done handler', response);
          }
        }
        return AppState.set('mainContext.payload.' + field, response[type]);
      };

      IndexView.prototype.resolver = function(model, response, opts) {
        /*
        			here is where we need to make checks of whether to pass along data
        			to 'REZ' or resolve with the disambiguator
        */

        var context, datetime, dis, payload, posted, rez;
        if (response.status != null) {
          switch (response.status.toLowerCase()) {
            case 'disambiguate':
              this.log('resolver disambiguate', response);
              AppState.set('inProgress', false);
              return this.disambiguate(response);
            case 'in progress':
              this.log('resolver progress', response);
              AppState.set({
                inProgress: true,
                responderContext: response
              });
              return this.show(response);
            case 'complete':
            case 'completed':
              this.log('resolver complete', response);
              AppState.set({
                inProgress: false,
                responderContext: {}
              });
              if (response.actor == null) {
                return this.show(response);
              } else {
                context = AppState.get('mainContext');
                return dis = new Responder(context, {
                  action: 'actors'
                });
              }
          }
        } else {
          this.log('resolver response without status', response, AppState.get('mainContext'));
          payload = response.payload;
          if ((payload != null) && this.checkDates) {
            if ((payload.start_date != null) || (payload.start_time != null)) {
              datetime = Util.buildDatetime(payload.start_date, payload.start_time);
              this.log('datetime no status', datetime);
              if (payload.start_date != null) {
                payload.start_date = datetime.date;
              }
              if (payload.start_time != null) {
                payload.start_time = datetime.time;
              }
            }
            if ((payload.end_date != null) || (payload.end_time != null)) {
              datetime = Util.buildDatetime(payload.end_date, payload.end_time);
              if (payload.end_date != null) {
                payload.end_date = datetime.date;
              }
              if (payload.end_time != null) {
                payload.end_time = datetime.time;
              }
            }
          }
          AppState.set('mainContext', response);
          rez = new Responder(response, {
            action: 'audit'
          });
          posted = rez.post();
          return this.log(posted);
        }
      };

      return IndexView;

    })(Backbone.View);
  });

}).call(this);
