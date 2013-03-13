/**
 * Manages communication with the Butler API.
 */
cordova.define('please/api', function(require, exports, module) {
    var API_ENDPOINT = 'http://192.168.1.71:8080/butler';

    var ask = function(message, callback) {
        $.get(API_ENDPOINT,
              {
                  'format': 'json',
                  'query': message
              }, callback, 'json');
    };

    exports.ask = ask;
});