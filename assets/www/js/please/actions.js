/**
 * Actions triggered by the Butler API
 */
cordova.define('please/actions', function(require, exports, module) {
    var Calendar = window.plugins.calendarPlugin;
    var PhoneCall = require('cordova/plugin/phonecall');
    var Reminder = window.plugins.localNotification;

    exports['calendar.addEvent'] = function(payload) {
        Calendar.createEvent(
                payload.title, 
                payload.location, 
                payload.notes,
                payload.startDate, 
                payload.endDate
            );
    };

    exports.call = function(payload) {
        PhoneCall.call(payload);
    };

    exports.web = function(payload) {
        window.open(payload);
    };

    exports.reminder = function (payload) {
        var now = new Date();
        now.setSeconds(now.getSeconds() + payload.seconds);
        Reminder.add({
            date: now,
            ticker: payload.ticker,
            message: payload.message,
            repeatDaily: false,
            id: 999
        })
    }

    exports.sms = function (payload) {
         cordova.exec(
            function(success) {}, 
            function(error) {}, 
            "SendSms", 
            "SendSms", 
            [
                payload.number, 
                payload.message
            ]);
    };

});