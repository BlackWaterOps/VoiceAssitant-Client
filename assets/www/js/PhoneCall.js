/*
 * cordova is available under *either* the terms of the modified BSD license *or* the
 * MIT License (2008). See http://opensource.org/licenses/alphabetical for full text.
 *
 * Copyright (c) 2011, IBM Corporation
 */
/*
 * cordova is available under *either* the terms of the modified BSD license *or* the
 * MIT License (2008). See http://opensource.org/licenses/alphabetical for full text.
 * 
 * Copyright (c) 2011, IBM Corporation
 *
 * Modified by Murray Macdonald (murray@workgroup.ca) on 2012/05/30 to add pitch(), speed(), stop(), and interrupt() methods.
 */
cordova.define("cordova/plugin/phonecall",
  function(require, exports, module) {
    var exec = require("cordova/exec");
    
    /**
     * Constructor
     */
    function PhoneCall() {
    }
    
    /**
     * Call a phone number
     * 
     * @param {DOMString} number
     * @param {Object} successCallback
     * @param {Object} errorCallback
     */
    PhoneCall.prototype.call = function(number, successCallback, errorCallback) {
         return exec(successCallback, errorCallback, "PhoneCall", "call", [number]);
    };

    var phonecall = new PhoneCall();
    module.exports = phonecall;
});

/**
 * Load PhoneCall
 */

if(!window.plugins) {
    window.plugins = {};
}
if (!window.plugins.PhoneCall) {
    window.plugins.PhoneCall = cordova.require("cordova/plugin/phonecall");
}