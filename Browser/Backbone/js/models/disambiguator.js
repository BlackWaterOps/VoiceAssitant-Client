// Generated by CoffeeScript 1.6.3
(function() {
  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; },
    __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'models/base', 'models/appState', 'util'], function(_, Backbone, ModelBase, AppState, Util) {
    var disambiguator, _ref;
    return disambiguator = (function(_super) {
      __extends(disambiguator, _super);

      function disambiguator() {
        this.initialize = __bind(this.initialize, this);
        this.url = __bind(this.url, this);
        _ref = disambiguator.__super__.constructor.apply(this, arguments);
        return _ref;
      }

      disambiguator.prototype.defaults = {
        text: '',
        types: []
      };

      disambiguator.prototype.urlRoot = 'http://casper-cached.stremor-nli.appspot.com/v1/disambiguate/';

      disambiguator.prototype.urlAction = null;

      disambiguator.prototype.url = function() {
        if (this.urlAction != null) {
          return this.urlRoot + this.urlAction;
        } else {
          return this.urlRoot;
        }
      };

      disambiguator.prototype.initialize = function(attributes, options) {
        if ((options != null) && (options.action != null)) {
          return this.urlAction = options.action;
        }
      };

      return disambiguator;

    })(ModelBase);
  });

}).call(this);
