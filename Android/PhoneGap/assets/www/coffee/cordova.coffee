class Cordova
    constructor: (options) ->
        @please = new Please()
                
        document.addEventListener("deviceready", @deviceReady, false)

    deviceReady: =>
        window.plugins.tts.startup(@startupWin, @startupFail)
        window.plugins.speechrecognizer.init(@speechInitOk, @speechInitFail)
        
        # myScroll = new iScroll('wrapper', {checkDOMChanges: true})
        # hite = $('#wrapper').innerHeight()
        return

    refreshiScroll: =>
        setTimeout( =>
            myScroll.refresh()
            myScroll.scrollToElement('#board div:last-child', 250) if $('#board').innerHeight() > hite 
        , 0)
        # $('.spinner').remove()

   	startupWin: (result) =>
        # When result is equal to STARTED we are ready to play
        if result is TTS.STARTED
            console.log('cordova startupWin')
            $('.control').on('fastClick', '.micbutton', @initQuery)
      
            # @refreshiScroll();
    
    initQuery: =>
        window.plugins.tts.stop()
        @recognizeSpeech()

    recognizeSpeech: =>
        requestCode = 1234
        maxMatches = 1
        promptString = "Please say a command"  # optional
        language = "en-US"                     # optional
        
        window.plugins.speechrecognizer.startRecognize(@speechOk, @speechFail, requestCode, maxMatches, promptString, language)

    speechOk: (results) =>
        if results
            respObj = JSON.parse(results)
            if respObj
                matches = respObj.speechMatches.speechMatch

                if matches.length > 0
                    query = matches[0]
                    query = @cleanQuery(query)
                    # echo(query) # need to trigger speak from please and handle in this file

                    # call please
                    @please.ask(query)

    cleanQuery: (query) =>
        query.replace('needa', 'need a')

    speechInitOk: =>
        console.log('speech recognizer is ready')
    
    speechInitFail: (m) =>
        console.log('speech recognizer failed')
    
    speechFail: (message) =>
        console.log("speechFail: " + message)
    
    startupFail: (result) =>
        console.log("Startup failure: " + result)

new Cordova()