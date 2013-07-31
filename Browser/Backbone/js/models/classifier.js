// Generated by CoffeeScript 1.6.3
(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'models/base', 'models/appState'], function(_, Backbone, ModelBase, AppState) {
    var classifer, _ref;
    return classifer = (function(_super) {
      __extends(classifer, _super);

      function classifer() {
        _ref = classifer.__super__.constructor.apply(this, arguments);
        return _ref;
      }

      classifer.prototype.urlRoot = 'http://casper-cached.stremor-nli.appspot.com/v1';

      classifer.prototype.fetch = function(query) {
        var debug;
        console.log('classifier fetch', query);
        debug = AppState.get('debug');
        return Backbone.Model.prototype.fetch.call(this, query);
      };

      classifer.prototype.parse = function(response, options) {
        AppState.set('mainContext', response);
        return response;
      };

      return classifer;

    })(ModelBase);
  });

}).call(this);
