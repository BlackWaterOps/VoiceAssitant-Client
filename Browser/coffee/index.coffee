class Please
	constructor: (options) ->
		@debug = true
		@debugData = { }
		@classifier = 'http://casper-cached.stremor-nli.appspot.com/v1'
		@disambiguator = 'http://casper-cached.stremor-nli.appspot.com/v1/disambiguate'
		@personal = 'http://stremor-pud.appspot.com/v1/disambiguate'
		@responder = 'http://rez.stremor-apier.appspot.com/v1/'
		@lat = 0.00
		@lon = 0.00
		@mainContext = { }
		@disambigContext = { }
		@history = [ ]
		@pos = @history.length
		@loader = $('#loader')
		@board = $('#board')
		@input = $('#main-input')
		@dateRegex = /\d{2,4}[-]\d{2}[-]\d{2}/i
		@timeRegex = /\d{1,2}[:]\d{2}[:]\d{2}/i
		@counter = 0
		
		Handlebars.registerHelper('elapsedTime', (dateString) =>
			results = @elapsedTimeHelper(dateString)
			return results.newDate + ' ' + results.newTime 
		)
		
		@currentState = 'init'

		# pretend presets is a list of 'special' times populated from a DB that stores user prefs
		@presets = 
			'after work': '18:00:00'
			'breakfast': '7:30:00'
			'lunch': '12:00:00'
		
		@init()
	init: =>
		@input.focus()
		.on('webkitspeechchange', @ask)
		.on('keyup', @keyup)

		$('body').on('click', '.expand', @expand)
		#.on('click', '.simulate', @simulate)
		
		$('#cancel').on('click', @cancel)

		if (@board.is(':empty'))
			init = $('#init')
			init.fadeIn('slow');
			setTimeout (->
				init.fadeOut 'slow'
			), 1000

		@getLocation()
		@registerListeners()

	registerListeners: =>
		$(document)
		.on('init', @auditor)
		.on('disambiguate', @disambiguatePassive)
		.on('disambiguate:personal', @disambiguatePersonal)
		.on('disambiguate:active', @disambiguateActive)
		.on('inprogress', @show)
		.on('completed', @actor)
		.on('error', @show)
		.on('debug', @addDebug)

	ask: (input) =>
		input = $(input)

		text = input.val()

		input.val('')
		
		template = Handlebars.compile($('#bubblein-template').html())
		
		@board.append(template(text)).scrollTop(@board.find('.bubble:last').offset().top)
		
		$('#input-form').addClass 'cancel'

		# TODO: CHECK CURRENT STATE INSTEAD
		if @currentState is 'inprogress'
			# @disambiguate text
			$(document).trigger(
				type: 'disambiguate:active'
				response: text
			)
		else
			data = query: text
					
			@requestHelper @classifier, "GET", data, (response) =>				
				# addDebug()
				# @resolver response
	
				$(document)
				.trigger($.Event('debug'))
				.trigger(
					type: 'init'
					response: response
				)	

	keyup: (e) =>	
		value = $(e.target).val()

		target = $(e.target)

		switch e.which
			when 13
				if value
					@ask target
					@history.push value
					@pos = @history.length
			when 38
				@pos -= 1 if @pos > 0
				target.val @history[@pos]

			when 40
				@pos += 1 if @pos < @history.length
				target.val @history[@pos]
				
	expand: (e) =>
	    e.preventDefault()
	    $(e.target).parent().next().toggle()

	cancel: (e) =>
		@board.empty()
		@mainContext = { }
		@disambigContext = { }
		@history = [ ]
		@currentState = 'init'

		$('#input-form').removeClass 'cancel'
		@loader.hide()
		@counter = 0
		@input.focus()

	store:
		createCookie: (k, v, d) ->
			exp = new Date()
			exp.setDate(exp.getDate() + d)
			val = escape(v) + ((exdays==null) ? "" : "; expires="+exp.toUTCString())
			document.cookie = k + "=" + v
			return true 

		get: (k) ->
			if window.Modernizr.localstorage
				$.parseJSON localStorage.getItem(k)
			else 
				# use cookies   
				i = 0
				while i < c.length
					x = c[i].substr(0, c[i].indexOf("="))
					y = c[i].substr(c[i].indexOf("=")+1)
					x = x.replace(/^\s+|\s+$/g,"")
					if x == k
						return unescape(y)
					c++
		
		set: (k, v) ->
			if window.Modernizr.localstorage
				localStorage.setItem k, JSON.stringify(v)
			else
				createCookie k, v, 365
		
		remove: (k) ->
			if window.Modernizr.localstorage
				localStorage.removeItem k
			else 
				createCookie k, v, -1
	
		clear: ->
			if window.Modernizr.localstorage
				localStorage.clear()
			else
				createCookie k, v, -1
	
	disambiguateSuccessHandler: (response, field, type) =>
		$(document).trigger($.Event('debug')) if @currentState is 'inprogress'	
			
		if response?
			# replace location operators
			@replaceLocation(response)

			# find & replace date time fields
			@replaceDates(response)

			# find & replace the specific field indicated in the response 
			if field.indexOf('.') isnt -1
				@findOrReplace(field, response[type])
			else
				@mainContext.payload[field] = response[type]
		
			# clone 'mainContext' so we don't pollute with unused_tokens
			request = $.extend({}, @mainContext)
			
			request.unused_tokens = response.unused_tokens if response.unused_tokens?
			
			@auditor(request)
		else
			console.log 'oops no responder response', results

	disambiguateActive: (e) =>
		field = @disambigContext.field

		type = @disambigContext.type
		
		text = e.response

		postData = 
			payload: text
			type: type
			types: [type]

		@requestHelper(@disambiguator + '/active', 'POST', postData, (response) =>
			@disambiguateSuccessHandler(response, field, type)
		)

	disambiguatePersonal: (e) =>
		# in the future we'll need to send a userid for personal data 
		data = e.response

		field = data.field

		# TODO: handle multi types?
		type = data.type

		if field.indexOf('.') isnt -1
			text = @findOrReplace(field)
		else
			text = @mainContext.payload[field]	

		postData = 
			types: [type]
			type: type
			payload: text

		@requestHelper(@personal, 'POST', postData, (response) =>
			@disambiguateSuccessHandler(response, field, type)
		)

	disambiguatePassive: (e) =>	
		data = e.response

		field = data.field

		# TODO: handle multi types
		type = data.type

		if field.indexOf('.') isnt -1
			text = @findOrReplace(field)
		else
			text = @mainContext.payload[field]	

		postData = 
			types: [type]
			type: type
			payload: text

		@requestHelper(@disambiguator + '/passive', 'POST', postData, (response) =>
			@disambiguateSuccessHandler(response, field, type)
		)

	# NOTE: data can be a jquery event object or a plain object
	auditor: (data) =>
		response = if data instanceof $.Event then data.response else data

		payload = response.payload

		@replaceLocation(payload) if payload?
		@replaceDates(payload) if payload?

		# this should only be set for init requests not disambiguate responses
		@mainContext = response

		@counter++

		@requestHelper(@responder + 'audit' , 'POST', response, @responderSuccessHandler) if @counter < 3

	responderSuccessHandler: (response) =>
		@currentState = response.status.replace(' ', '')

		@disambigContext = response if @currentState is 'inprogress'

		$(document).trigger(
			type: @currentState
			response: response
		)

	actor: (e) =>
		@disambigContext = { }

		data = e.response
		
		if data.actor is null or data.actor is undefined
			@show(data)
		else
			@requestHelper(@responder + 'actors/' + data.actor, 'POST', @mainContext, @show)
	
	addDebug: (e) =>
		return if @debug is false

		@debugData.request = JSON.stringify(@debugData.request, null, 4) if @debugData.request?
		@debugData.response = JSON.stringify(@debugData.response, null, 4) if @debugData.response?
		
		results = e.response

		if !results
			results = 
				debug: @debugData
		else
			results.debug = @debugData

		template = Handlebars.compile($('#debug-template').html())
		@board.find('.bubble:last').append(template(results))

	# NOTE: results can be a jquery event object or a plain object
	show: (results) =>
		results = results.response if results instanceof $.Event

		templateData = results.show.simple

		templateName = if templateData.link? then 'link' else 'bubbleout'		

		template = $('#' + templateName + '-template')
		
		template = Handlebars.compile(template.html())

		@board.append(template(templateData)).scrollTop(@board.find('.bubble:last').offset().top)

		$(document).trigger(
			type: 'debug'
			response: results
		)

		if results.show? and results.show.structured? and results.show.structured.template?
			templateData = results.show.structured.items
			template = results.show.structured.template.split(':')
			templateBase = template[0]
			templateType = template[1]
			templateName = if template[2]? then template[2] else templateType

			template = $('#' + templateType + '-template')

			template = $('#' + templateBase + '-template') if template.length is 0
			
			if template.length > 0
				template = Handlebars.compile(template.html())

				@board.find('.bubble:last').append(template(templateData)).scrollTop(@board.find('.bubble:last').offset().top)

		@loader.hide()
		@counter = 0

	getLocation: =>
		navigator.geolocation.getCurrentPosition @updatePosition
	
	updatePosition: (position) =>
		@lat = position.coords.latitude
		@lon = position.coords.longitude
	
	findOrReplace: (field, type = null) =>
		fields = field.split('.')
		found = null

		cursive = (obj) ->
			for key, val of obj		
				if fields.length > 1 and key is fields[0]
					fields.shift()
					cursive(val)
				else if key is fields[0]
					if type is null
						found = obj[key]
					else
						obj[key] = type
					return

		cursive(@mainContext)

		return found

	nameMap: (key) =>
		map = false
		if key.indexOf(@disambiguator) isnt -1
			map = "Disambiguator"
		else if key.indexOf(@classifier) isnt -1
			map = "Casper"
		else if key.indexOf(@responder) isnt -1
			map = "Rez"
		else if key.indexOf(@personal) isnt -1
			map = "Pud"

		return map

	buildDeviceInfo: =>
		clientDate = new Date()

		deviceInfo =
			"latitude": @lat,
			"longitude": @lon,
			"timestamp": clientDate.getTime() / 1000,
			"timeoffset": - clientDate.getTimezoneOffset() / 60

	requestHelper: (endpoint, type, data, doneHandler) =>
		data.device_info = @buildDeviceInfo() if @currentState is 'disambiguate'
	
		if @debug is true
			@debugData = 
				endpoint: endpoint
				type: type
				request: data

		endpointName = @nameMap(endpoint) 

		$.ajax(
			url: endpoint
			type: type
			data: if type is "POST" then JSON.stringify(data) else data 
			# contentType: "application/json"
			dataType: "json"
			timeout: 10000
			beforeSend: =>
				@log endpointName, ">", data
				@loader.show()
		).done((response, status) =>
			@log endpointName, "<", response
			
			if @debug is true
				@debugData.status = status
				@debugData.response = response

			doneHandler(response) if doneHandler?
		).fail((response, status) =>
			@error endpointName, "<", (if response.responseJSON? then response.responseJSON else response)
			
			if @debug is true
				@debugData.status = status
				@debugData.response = response
			
			@loader.hide()
		)

	operators = 
		"+": (left, right) -> parseInt(left, 10) + parseInt(right, 10)
		"-": (left, right) -> parseInt(left, 10) - parseInt(right, 10)

	toISOString: (dateObj) =>
		pad = (number) ->
			r = String number
			r = ('0' + r) if r.length is 1
	  
			return r
 
		( dateObj.getFullYear() + '-' + pad( dateObj.getMonth() + 1 ) + '-' + pad( dateObj.getDate() ) + 'T' + pad( dateObj.getHours() ) + ':' + pad( dateObj.getMinutes() ) + ':' + pad( dateObj.getSeconds() ) )
		# + '.' + String( (dateObj.getMilliseconds()/1000).toFixed(3) ).slice( 2, 5 )

	replaceLocation: (payload) =>
		if payload? and payload.location?
			switch payload.location
				when '#current_location'
					payload.location = @buildDeviceInfo()

		return	

	replaceDates: (payload) =>
		datetimes = [
			['date', 'time']
			['start_date', 'start_time']
			['end_date', 'end_time']
		]

		for pair in datetimes
			date = pair[0]
			time = pair[1]

			if payload[date]? or payload[time]?
				datetime = @buildDatetime(payload[date], payload[time])

				if datetime?
					payload[date] = datetime.date if payload[date]?
					payload[time] = datetime.time if payload[time]?

		return
		
	buildDatetime: (date, time) =>
		newDate = null

		newDate = @datetimeHelper(date) if date isnt null and date isnt undefined and @dateRegex.test(date) is false

		newDate = @datetimeHelper(time, newDate) if time isnt null and time isnt undefined and @timeRegex.test(time) is false

		return if not newDate?

		dateString = @toISOString(newDate).split('T')
		
		date: dateString[0]
		time: dateString[1]

	# #date-add: {[{#weekday:1}, [1, 'day']]}
	weekdayHelper: (dayOfWeek) =>
		date = new Date();

		currentDay = date.getDay()
		currentDate = date.getDate()

		# return date if currentDay is dayOfWeek
		
		offset = if currentDay < dayOfWeek then (dayOfWeek - currentDay) else (7 - (currentDay - dayOfWeek))
		
		date.setDate(currentDate + offset)

		return date

	fuzzyHelper: (datetime, isDate) =>
		date = new Date()

		label = null
		def = null

		for key, val of datetime
			label = val if key is 'label'
			def = val if key is 'default'

		presetLabel = @presets[label]
		preference = if presetLabel? then presetLabel else def
				
		if preference is null
			return

		splitSym = if isDate is true then '-' else ':'

		datetimeArr = preference.trim().split(splitSym)

		if isDate is true
			date.setFullYear datetimeArr[0]
			date.setMonth (datetimeArr[1] - 1)
			date.setDate datetimeArr[2]
		else
			date.setHours datetimeArr[0]
			date.setMinutes datetimeArr[1]
			date.setSeconds datetimeArr[2]

		return date
	
	newDateHelper: (datetime) =>
		if datetime.indexOf('now') isnt -1
			newDate = new Date();

		else if @dateRegex.test(datetime) is true
			split = datetime.split('-')
			newDate = new Date(split[0], (split[1]-1), split[2])

		else if @timeRegex.test(datetime) is true
			newDate = new Date();
			split = datetime.split(':')
			hours = newDate.getHours()
			minutes = newDate.getMinutes()
			seconds = newDate.getSeconds()

			if (hours > split[0]) or (hours is split[0] and minutes > split[1])
				# move date up one day
				newDate.setDate(newDate.getDate() + 1);

			newDate.setHours split[0]
			newDate.setMinutes split[1]
			newDate.setSeconds split[2]

		#@log 'newDate bottom', newDate

		return if newDate is null or newDate is undefined then new Date() else newDate

	# {'#time_add': [{'#time_fuzzy': {'label': 'dinner', 'default': '19:00:00'}}, 3600]}
	datetimeHelper: (dateOrTime, newDate = null) =>
		#@log dateOrTime

		dateOrTimeType = Object.prototype.toString.call(dateOrTime)

		switch (dateOrTimeType)
			when '[object String]'
				if newDate is null
					newDate = @newDateHelper dateOrTime

			when '[object Object]'
				for action, parsable of dateOrTime
					if action.indexOf('weekday') isnt -1
						return @weekdayHelper parsable
					else if action.indexOf('fuzzy') isnt -1
						isDate = if action.indexOf('date') isnt -1 then true else false 
						return @fuzzyHelper parsable, isDate
					else
						operator = if action.indexOf('add') isnt -1 then '+' else '-'

						parsableType = Object.prototype.toString.call(parsable)

						if parsableType is '[object Array]' # date partials
							for item in parsable
								itemType = Object.prototype.toString.call(item);

								if newDate is null 
									switch itemType
										when '[object String]' # 'now' or '2013-07-01'
											newDate = @newDateHelper item
										when '[object Object]' #weekday, #fuzzy operators
											for itemKey, itemValue of item
												if newDate is null
													if itemKey.indexOf('weekday') isnt -1
														newDate = @weekdayHelper itemValue
													else if itemKey.indexOf('fuzzy') isnt -1
														isDate = if itemKey.indexOf('date') isnt -1 then true else false 
														newDate = @fuzzyHelper itemValue, isDate
								
								else if itemType is '[object Number]' # dates to add to Date object 
									interval = item
									
									if interval is null 
										return

									if action.indexOf('time') isnt -1
										curr = newDate.getSeconds()
										time = operators[operator](curr, interval)
										newDate.setSeconds(time)
									else if action.indexOf('date') isnt -1
										curr = newDate.getDate()
										date = operators[operator](curr, interval)
										newDate.setDate(date)

		return newDate

	elapsedTimeHelper: (dateString) =>		
		dt = new Date(dateString.replace(/[T|Z]/g, ' '))
		mm = dt.getMonth() + 1
		dd = dt.getDate()
		yy = dt.getFullYear()
		hh = dt.getHours()
		min = dt.getMinutes()
		
		mm = ("0" + mm) if mm < 10
		dd = ("0" + dd) if dd < 10

		# new addition to parseDate
		hh = (hh - 12) if hh > 12
		hh = ("0" + hh) if hh < 10
		# end new addition
		min = ("0" + min) if min < 10
		
		pubdate = mm + "/" + dd + "/" + yy
		pubtime = hh + ":" + min
		origPubdate = pubdate
		origPubtime = pubtime

		ut = new Date()

		if (ut.getTime() - dt.getTime()) < 86400000
			pubdate = ""
			pTime = Math.round((((ut.getTime() - dt.getTime()) / 1000) / 60))
			if pTime < 60
				pubtime = "About " + pTime + " minutes ago"
			else
				pTime = Math.round((pTime / 60))
				if pTime is 1
					pubtime = "About " + pTime + " hour ago"
				else
					pubtime = "About " + pTime + " hours ago"
		
		oldDate: origPubtime
		oldTime: origPubdate
		newDate: pubdate
		newTime: pubtime 

	log: =>
		args = [ ]
		for argument in arguments
			argument = JSON.stringify(argument, null, " ") if typeof argument is 'object'
			args.push argument

		console.log args.join(" ")

	error: =>
		args = [ ]
		for argument in arguments
			argument = JSON.stringify(argument, null, " ") if typeof argument is 'object'
			args.push argument

		console.error args.join(" ")

new Please()                    