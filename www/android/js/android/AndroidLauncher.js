cordova.define("cordova/plugin/androidLauncher",
  function(require, exports, module) {
    var exec = require("cordova/exec");

    /**
     * Constructor
     */
    function AndroidLauncher() {
    }

    /**
     * Launch an intent with ACTION_VIEW and some URI
     *
     * @param {DOMString} URI
     * @param {Object} successCallback
     * @param {Object} errorCallback
     */
    AndroidLauncher.prototype.view = function (uri, successCallback, errorCallback) {
        return exec(successCallback, errorCallback, "AndroidLauncher", "view", [uri]);
    };

    /**
     * Launch an application by package name
     *
     * @param {DOMString} packageName
     * @param {Object} successCallback
     * @param {Object} errorCallback
     */
    AndroidLauncher.prototype.launch = function (packageName, successCallback, errorCallback) {
        return exec(successCallback, errorCallback, "AndroidLauncher", "launch", [packageName]);
    };

    var androidLauncher = new AndroidLauncher();
    module.exports = androidLauncher;
});

/**
 * Load AndroidLauncher
 */

if(!window.plugins) {
    window.plugins = {};
}
if (!window.plugins.AndroidLauncher) {
    window.plugins.AndroidLauncher = cordova.require("cordova/plugin/androidLauncher");
}
