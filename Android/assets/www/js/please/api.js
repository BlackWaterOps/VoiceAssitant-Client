/**
 * Manages communication with the API.
 */
cordova.define('please/api', function(require, exports, module) {
    var API_ENDPOINT = "http://stremor-va.appspot.com/rest";

    if (isDebugging === true) {
        API_ENDPOINT.replace("stremor-va", "dev.stremor-va");
    }

    var ask = function (message, context, callback) {
        var data = JSON.stringify({
            "query": message,
            "context": context
        });

        $.ajax({
            "url": API_ENDPOINT,
            "type": 'POST',
            "data": data,
            "dataType": 'json',
            timeout: 10000,
            success: function (response) {
                console.log("RESPONSE", response, JSON.stringify(response));
                callback(response);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                message = 'The Please server could not process your request.';

                if ( textStatus == 'timeout' )
                    message = "I'm having trouble communicating with the Please server. " +
                      "Please try again later.";

                say(message, null);
                console.log('WTF: ' + jqXHR + ' status: ' + textStatus + ' error: ' + errorThrown);
            }
        });
    }

    exports.ask = ask;
});