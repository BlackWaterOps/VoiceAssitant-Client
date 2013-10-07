class Cordova
    constructor: (options) ->
        @please = new Please()
        
        # turn off debugging for phone
        @please.debug = false

        opts =
            lines: 12 # The number of lines to draw
            length: 7 # The length of each line
            width: 5 # The line thickness
            radius: 10 # The radius of the inner circle
            color: '#999' # #rbg or #rrggbb
            speed: 1 # Rounds per second
            trail: 100 # Afterglow percentage
            shadow: true # Whether to render a shadow
        
        $.fn.spin = (spinOpts) ->
            this.each(->
                $this = $(this)
                spinner = $this.data('spinner')

                spinner.stop() if (spinner) 
                if opts isnt false
                    opts = $.extend(
                        color: $this.css('color')
                    , opts)

                    spinner = new Spinner(opts).spin(this)
                    $this.data('spinner', spinner)
            )
            
            return this

        $('.spinner').remove()

        document.addEventListener("deviceready", @deviceReady, false)

    deviceReady: =>
        window.plugins.tts.startup(@startupWin, @startupFail)
        window.plugins.speechrecognizer.init(@speechInitOk, @speechInitFail)
        
        $(document).on('speak', @speak)

        myScroll = new iScroll('wrapper', {checkDOMChanges: true})
        hite = $('#wrapper').innerHeight()
        @refreshiScroll()

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
      
            @refreshiScroll();
    
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

                    console.log('your query was ' + query)

                    # call please
                    @please.ask(query)

                    @refreshiScroll()

    speak: (e) =>
        window.plugins.tts.speak(e.response);

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