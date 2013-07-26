cordova.define("cordova/plugin/appInstallOffer",
  function(require, exports, module) {
    var exec = require("cordova/exec");

    /**
     * Constructor
     */
    function AppInstallOffer() {
    }

    /**
     * Offer to install an application
     *
     * @param {DOMString} title Title of prompt dialog
     * @param {DOMString} message Message in prompt dialog
     * @param {DOMString} package Name of package
     * @param {Object} successCallback
     * @param {Object} errorCallback
     */
    AppInstallOffer.prototype.prompt = function (title, message, package,
                                               successCallback, errorCallback) {
        return exec(successCallback, errorCallback, "AppInstallOffer", "prompt", [title, message, package]);
    };

    var appInstallOffer = new AppInstallOffer();
    module.exports = appInstallOffer;
});

/**
 * Load AndroidLauncher
 */

if(!window.plugins) {
    window.plugins = {};
}
if (!window.plugins.AppInstallOffer) {
    window.plugins.AppInstallOffer = cordova.require("cordova/plugin/appInstallOffer");
}
