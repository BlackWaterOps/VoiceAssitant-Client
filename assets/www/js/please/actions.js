/**
 * Actions triggered by the Butler API
 */
cordova.define('please/actions', function(require, exports, module) {
    var Calendar = require('cordova/plugin/calendar');
    var PhoneCall = require('cordova/plugin/phonecall');

    exports['calendar.addEvent'] = function(payload) {
        Calendar.createEvent(payload.title, payload.location, payload.notes,
                             payload.startDate, payload.endDate);
    };

    exports.call = function(payload) {
        PhoneCall.call(payload);
    };

    exports.web = function(payload) {
        window.open(payload);
    };
});