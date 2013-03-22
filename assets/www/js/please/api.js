/**
 * Manages communication with the Butler API.
 */
cordova.define('please/api', function(require, exports, module) {
    // var API_ENDPOINT = 'http://192.168.1.71:8080/butler';
    var API_ENDPOINT = "http://dev.liquid-helium.appspot.com/please";

    // var ask = function(message, callback) {
    //     $.get(API_ENDPOINT,
    //           {
    //               'format': 'json',
    //               'query': message
    //           }, callback, 'json');
    // };
    var ask = function (message, callback) {
    	$.ajax({
    		"url": API_ENDPOINT,
    		"type": 'POST',
    		"data": {"query": message},
    		"dataType": 'json',
    		success: function (response) {
    			console.log("RESPONSE: " + response)
    			callback(response);
    		},
    		error: function (response) {
    			console.log('WTF: ' + response);
    		}
    	})
    }

    exports.ask = ask;
});