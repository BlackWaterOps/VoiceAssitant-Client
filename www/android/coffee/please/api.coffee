###
Manages communication with the APIS.
###
cordova.define 'please/api', (require, exports, module) =>
    
    CLASSIFIER = 'http://casper-cached.stremor-x.appspot.com/'
    BUILDER = ''
    DISAMBIGUATOR = 'http://casper-cached.stremor-x.appspot.com/disambiguate'
    RESPONDER = 'http://clever.stremor-x.appspot.com/'

    makeRequest = (endpoint, type, data, successHandler, errorHandler) =>
        $.ajax(
            url: endpoint
            type: type
            data: data
            dataType: 'json'
            timeout: 10000
        ).done(
            successHandler if successHandler?
        ).fail(
            errorHandler if errorHandler?
        )

    ask = (message, context, callback) =>
        data = JSON.stringify(
            "query": message
            "context": context
        )

        makeRequest CLASSIFIER, 'GET', data, callback

        # $.ajax(
        #     "url": API_ENDPOINT
        #     "type": 'POST'
        #     "data": data
        #     "dataType": 'json',
        #     timeout: 10000,
        #     success: function (response) {
        #         console.log("RESPONSE", response, JSON.stringify(response));
        #         callback(response);
        #     },
        #     error: function (jqXHR, textStatus, errorThrown) {
        #         message = 'The Please server could not process your request.';

        #         if ( textStatus == 'timeout' )
        #             message = "I'm having trouble communicating with the Please server. " +
        #               "Please try again later.";

        #         say(message, null);
        #         console.log('WTF: ' + jqXHR + ' status: ' + textStatus + ' error: ' + errorThrown);
        #     }
        # });

    exports.ask = ask

    disambiguate = (payload, callback) =>

        makeRequest DISAMBIGUATOR, 'POST', data, callback

    exports.disambiguate = disambiguate    

    respond = (payload, callback) =>

        makeRequest RESPONDER, 'POST', data, callback

    exports.respond = respond