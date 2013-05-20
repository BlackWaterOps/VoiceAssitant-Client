/**
 * Manages communication with the Butler API.
 */
cordova.define('please/api', function(require, exports, module) {
    // var API_ENDPOINT = 'http://192.168.1.71:8080/butler';
    // var API_ENDPOINT = "http://dev.liquid-helium.appspot.com/please";
        var API_ENDPOINT = "http://stremor-va.appspot.com/rest";


    // var ask = function(message, callback) {
    //     $.get(API_ENDPOINT,
    //           {
    //               'format': 'json',
    //               'query': message
    //           }, callback, 'json');
    // };
    var ask = function (message, context, callback) {
    	$.ajax({
    		"url": API_ENDPOINT,
    		"type": 'POST',
    		"data": {
                            "query": message,
                            "context": context
                        },
    		"dataType": 'json',
    		success: function (response) {
    			console.log("RESPONSE: " + response)
    			callback(response);
    		},
    		error: function (response, jqXHR, textStatus, errorThrown) {
                                    say("I'm sorry. I didn't understand that.");
    			console.log('WTF: ' + jqXHR + ' status: ' + textStatus + ' error: ' + errorThrown);
    		}
    	})
    }

    exports.ask = ask;
});