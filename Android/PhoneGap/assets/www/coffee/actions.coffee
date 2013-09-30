class Actions
    constructor: ->
        @monthsOfTheYear = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]
        @dayOfTheWeek = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]

        @phoneCall = null
        
        ###
        cordova.define('please/actions', (require, exports, module) =>
            @phoneCall = require('cordova/plugin/phonecall')
            exports.calendar = @calendar
            exports.time = @time
            exports.call = @call
            exports.web = @web
            exports.link = @link
            exports.reminder = @reminder
            exports.alarm = @alarm
            exports.images = @images
            exports.sms = @sms
            exports.email = @email
            exports.capture_image = @capture_image
            exports["capture-image"] = @capture_image
            exports.capture_audio = @capture_audio
            exports["capture-audio"] = @capture_audio
            exports.capture_video = @capture_video
            exports["capture-video"] = @capture_video
            exports.clear_log = @clear_log
            exports.locate = @locate
            exports.directions = @directions
            exports.app_launch = @app_launch
            exports.app_view = @app_view
            exports.contacts = @contacts
        ###

    dateFromString: (date, time) ->
        time = "12:00:00 PM" if not time? 
        
        new Date(date + " " + time)
    
    createEventSubject: (subject, person, location) ->
        subject = "" if not subject?

        subject += " with " + person if person not null

        subject += " at " + location if location not null and location not ""
    
        return subject

    calendar: (payload) -> 
        sDate = dateFromString(payload.date, payload.time)
        ###
        if (payload.time == null) {
            payload.time = "12:00:00 PM"
        }
        sDate = new Date(payload.date + " " + payload.time)
        ###
        temp = sDate.getTime()
        temp += (payload.duration * 3600000)
        eDate = new Date(temp)
        
        payload.location = "" if not payload.location? is null

        memo = createEventSubject(payload.subject, payload.person, payload.location)
        
        ###
        memo = payload.subject
        if (payload.person !== null) {
            memo += " with " + payload.person
        }
        if ((payload.location !== null) && (payload.location !== "")) {
            memo += " at " + payload.location
        }
        ###
        successHandler = (success) ->
        errorHandler = (error) ->

        window.plugins.calendarPlugin.createEvent(memo, payload.location, "", sDate, eDate, successHandler, errorHandler)
    
    time: ->
        # theDate = new Date()
        # hours = theDate.getHours()
        # if (hours < 12) {
        #    ampm = "AM"
        # } else {
        #     ampm = "PM"
        # }
        # mins = theDate.getMinutes()
        # if (mins < 10) {
        #     mins = "0" + mins
        # }
        # seconds = theDate.getSeconds()
        # if (seconds < 10) {
        #     seconds = "0" + seconds
        # }
        # dayOfTheWeek = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]
        # day = dayOfTheWeek[theDate.getDay()]
        # monthsOfTheYear = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]
        # month = monthsOfTheYear[theDate.getMonth()]
        # date = theDate.getDate()
        # year = theDate.getFullYear()
        # say("It is now " + hours + ":" + mins + " " + ampm + " on " + day + ", " + month + " " +  date + ", " + year + ".")

    call: (payload) ->
        PhoneCall.call(payload.phone)
    
    web: (payload) ->
        window.open(payload.url, '_blank', 'location=yes')
    
    link: (payload) ->
        if payload.url?
            $('.bubble:last').append('<a href="' + payload.url + '" class="showMore extLink">Click for more</a>')
    
    reminder: (payload) ->
        regex = /alarm/i

        if payload.seconds?
           date = new Date()
           date.setSeconds(date.getSeconds() + payload.seconds)
        else if payload.datetime?   
            date = new Date(payload.datetime)
        else
            date = new Date()

        payload.ticker = "" if not payload.ticker?
        payload.message = "" if not payload.message? or not payload.ticker?

        window.plugins.localNotification.add(
            date: date,
            ticker: payload.ticker,
            message: payload.message,
            repeatDaily: false,
            id: 999
        )

        if payload.query not undefined and regex.test("alarm")
            say("a reminder alarm has been set for you")
        else
            say("a reminder has been set for you")
    
    alarm: (payload) ->
        payload.datetime = payload.datetime.replace("T", " ")
        
        date = new Date(payload.datetime)
        now = new Date()

        # if date is next day, pass off to reminder.
        if date.getFullYear() > now.getFullYear()
            reminder(payload)
            return
        else if date.getMonth() > now.getMonth()
            reminder(payload)
            return
        else if date.getDate() > now.getDate()
            reminder(payload)
            return

        successHandler = ->
            say("an alarm has been set for you")

        errorHandler = (msg) ->
            console.log(msg)
            say("Sorry, I could not set your alarm")

        window.plugins.alarmClockPlugin.addAlarm(date, "Please Alarm", false, successHandler, errorHandler)
    
    images: (payload) ->
        for path in payload.url
            $('.bubble:last').append('<img class="galleryImg" src="' + path + '" />')

        refreshiScroll()

    sms: (payload) ->
        successHandler = (success) ->
        errorHandler = (error) ->

        cordova.exec(successHandler, errorHandler, "SendSms", "SendSms", [payload.phone,payload.message])

    email: (payload) ->
        subject = payload.subject
        message = payload.message
        address = payload.address

        window.plugins.emailComposer.showEmailComposer(subject, message, [address], [], [], false, [])

   capture_image: (payload) ->
        successHandler = (imageData) ->
            path = imageData[0].fullPath
            img = "<img src=\"" + path + "\" />"
            
            # $('.console').append('<div class="bubble please">' + '<img src="' + path + '" />' + '</div>')
            
            say('That is a beautiful picture!', img)

        errorHandler = (error) ->

        navigator.device.capture.captureImage(successHandler, errorHandler, {limit:1})

   capture_audio: (payload) ->
        captureSuccess = (media) ->
            path = media[0].fullPath
            audio = "<audio src=\"" + path + "\" controls></audio>"
            
            # $('.console').append('<div class="bubble please">' + '<audio src="' +path + '"controls ></audio>' + '</div>')
            
            say('That sounded great!', audio)
        
        captureError () ->
            say('Sorry, I could not launch the audio recorder')

        navigator.device.capture.captureAudio(captureSuccess, captureError, {limit:1})

   capture_video: (payload) ->
        captureSuccess = (media) ->
            path = media[0].fullPath
            video = "<video src=\"" + path + "\" controls></video>"
            
            # $('.console').append('<div class="bubble please">' + '<video src="' +path + '" controls ></video>' + '</div>')

            say('That was a funny video!', video)
        
        captureError = () ->
            say('Sorry, I could not launch the video recorder')

        navigator.device.capture.captureVideo(captureSuccess, captureError, {limit:1})

   clear_log: (payload) ->
        $('.console').html("")
        $('.listSlider').removeClass('active suspended')
        .find('.prompt').empty().end()
        .find('ul').empty()
        # say('Let me know how I can be of assistance.')

   locate: (payload) ->
        oops = false
        defaultLocation = 
            coords:
                latitude: 33.620632, 
                longitude: -111.92565

        navigator.geolocation.getCurrentPosition(geoSuccess, geoFail, {timeout:10000, enableHighAccuracy:true})
        
        geoSuccess = (pos) ->
            lat = pos.coords.latitude
            lng = pos.coords.longitude
            
            say("I found you!") if not oops
        
            $('#gMapPos').attr('id', '')
            $('.bubble:last').append('<div id="gMapPos"></div>')
            latlng = new google.maps.LatLng(lat,lng)
            options = 
                zoom: 16, 
                center: latlng, 
                disableDefaultUI: true, 
                mapTypeId: google.maps.MapTypeId.ROADMAP, 
                draggable: false

            map = new google.maps.Map(document.getElementById('gMapPos'), options)
            uRHere = new google.maps.Marker({ position: latlng, map: map, title: 'You are here' })

        geoFail = () ->
           say("You must be hiding, because I can't find you. But I did find Stremor headquarters in Scottsdale.")
           oops = true
           geoSuccess(defaultLocation)

    directions: (payload) -> 
        directionsDisplay
        directionsService = new google.maps.DirectionsService()
        myDestination = payload.location
        defaultLocation = 
            coords: 
                latitude: 33.620632, 
                longitude: -111.92565

        oops = false

        navigator.geolocation.getCurrentPosition(navSuccess, navFail, {timeout:10000, enableHighAccuracy:true})
        $('#gMap').attr('id', '')
        $('#wrapper').spin(opts)

        navSuccess = (pos) ->
            say("Here is the route that I found: ") if not oops
            
            directionsDisplay = new google.maps.DirectionsRenderer()
            lat = pos.coords.latitude
            lng = pos.coords.longitude
            originPoint = new google.maps.LatLng(lat, lng)
            mapOptions = 
                zoom:7,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                center: originPoint,
                draggable: false

          $('.bubble:last').append('<div id="gMap"></div>')
          map = new google.maps.Map(document.getElementById("gMap"), mapOptions)
          directionsDisplay.setMap(map)
          calcRoute(originPoint, myDestination)

        calcRoute = (startPt, endPt) ->
            start = startPt
            end = endPt
            request =
                origin: start,
                destination: end,
                travelMode: google.maps.TravelMode.DRIVING

            directionsService.route(request, (result, status) ->
                if (status == google.maps.DirectionsStatus.OK)
                    directionsDisplay.setDirections(result)
            )

            refreshiScroll()

        navFail = () ->
            say("This is sad, but I'm having trouble with my GPS. Here's how to get there from Scottsdale.")
            oops = true
            navSuccess(defaultLocation)

    app_launch: (payload) ->
        window.plugins.AndroidLauncher.launch(payload.package, null, null)

    app_view: (payload) ->
        window.plugins.AndroidLauncher.view(payload.uri, null, (b) ->
            ###
            if ( payload.uri.indexOf('waze') != -1 ) {
                window.plugins.AppInstallOffer.prompt('Waze not found',
                                                      "We couldn't find Waze on your phone. Please install it.",
                                                      "com.waze")
            }
            ###
        )

    contacts: ->
        console.log("contacts")

        find = ["id", "displayName", "name", "nickname", "phoneNumbers", "emails", "addresses", "ims", "birthday", "note", "categories", "urls"]

        success = (contacts) ->
            for contact in contacts
                console.log(JSON.stringify(contact))        

        error = (msg) ->
            console.log(msg)

        navigator.contacts.find(["*"], success, error, {})
