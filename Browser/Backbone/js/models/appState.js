// Generated by CoffeeScript 1.6.3
(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'util'], function(_, Backbone, Util) {
    var appState, _ref;
    appState = (function(_super) {
      __extends(appState, _super);

      function appState() {
        _ref = appState.__super__.constructor.apply(this, arguments);
        return _ref;
      }

      appState.prototype.defaults = {
        debug: true,
        debugData: {},
        lat: 0.00,
        lon: 0.00,
        inProgress: false,
        mainContext: {},
        responderContext: {},
        history: [],
        pos: history.length
      };

      return appState;

    })(Backbone.NestedModel);
    return new appState();
  });

}).call(this);
