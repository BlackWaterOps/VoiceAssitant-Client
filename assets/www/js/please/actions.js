/**
 * Actions triggered by the Butler API
 */
cordova.define('please/actions', function(require, exports, module) {
    var PhoneCall = require('cordova/plugin/phonecall');
    exports.call = function(payload) {
        PhoneCall.call(payload);
    };

    exports.web = function(payload) {
        window.open(payload);
    };
});