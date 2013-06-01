cordova.define("cordova/plugin/calendarDialog",
  function(require, exports, module) {
    var exec = require("cordova/exec");

    /**
     * Constructor
     */
    function CalendarDialog() {
    }

    /**
     * Call a phone number
     *
     * @param {DOMString} number
     * @param {Object} successCallback
     * @param {Object} errorCallback
     */
    CalendarDialog.prototype.getDate = function (successCallback, errorCallback) {
        return exec(successCallback, errorCallback, "CalendarDialog", "getDate", []);
    };

    var calendarDialog = new CalendarDialog();
    module.exports = calendarDialog;
});

/**
 * Load CalendarDialog
 */

if(!window.plugins) {
    window.plugins = {};
}
if (!window.plugins.CalendarDialog) {
    window.plugins.CalendarDialog = cordova.require("cordova/plugin/calendarDialog");
}
