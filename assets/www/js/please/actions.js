/**
 * Actions triggered by the Butler API
 */
cordova.define('please/actions', function(require, exports, module) {
    exports.web = function(payload) {
        window.open(payload);
    };
});