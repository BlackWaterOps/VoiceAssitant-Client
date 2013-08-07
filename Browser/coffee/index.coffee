class Please
	constructor: (options) ->
		@debug = true
		@debugData = { }
		@classifier = 'http://casper-cached.stremor-nli.appspot.com/v1'
		@disambiguator = 'http://casper-cached.stremor-nli.appspot.com/v1/disambiguate'
		@responder = 'http://rez.stremor-apier.appspot.com/v1/'
		@lat = 0.00
		@lon = 0.00
		@sendDeviceInfo = false
		@inProgress = false
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

	log: =>
		args = [ ]
		for argument in arguments
			argument = JSON.stringify(argument, null, " ") if typeof argument is 'object'
			args.push argument

		console.log args.join(" ")

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
	
	getLocation: =>
		navigator.geolocation.getCurrentPosition @updatePosition
	
	updatePosition: (position) =>
		@lat = position.coords.latitude
		@lon = position.coords.longitude
	
	cancel: (e) =>
		@board.empty()
		@mainContext = { }
		@disambigContext = { }
		@history = [ ]

		$('#input-form').removeClass 'cancel'
		@loader.hide()
		@input.focus()

	disambiguate: (payload) =>
		if @inProgress is true
			endpoint = @disambiguator + "/active"

			field = @disambigContext.field

			type = @disambigContext.type
			
			text = payload

			data = 
				text: text
				types: [type]

			console.log 'user response', field, type

			#@log 'disambiguate user response', data
		else
			#@log 'disambiguate rez response'

			endpoint = @disambiguator + "/passive"

			@sendDeviceInfo = true

			field = payload.field

			# TODO: handle multi types
			type = payload.type

			console.log field, type

			text = @mainContext.payload[field]

			data = 
				text: text
				types: [type]

		successHandler = (results) =>
			# #@log 'successHandler', results
			# gross hack to stop the resolver from 
			checkDates = true

			if @debug is true and @inProgress is true
				@addDebug()
				
			if results?
				if results.date? or results.time?
					datetime = @buildDatetime(results.date, results.time)
				
					results[type] = datetime[type]

					checkDates = false

					#@log 'successhandler', results


				if field.indexOf('.') isnt -1
					fields = field.split('.')

					cursive = (obj) ->
						for key, val of obj				
							if fields.length > 1 and key is fields[0]
								fields.shift()
								cursive(val)
							else if key is fields[0]
								obj[key] = results[type]
								return

					cursive(@mainContext)
				else
					@mainContext.payload[field] = results[type]
			
				@resolver @mainContext, checkDates
			else
				console.log 'oops no responder response', results

		@requestHelper endpoint, "POST", data, successHandler

	# TODO: resolver logic can be split into two function, classifier responses & responder responses
	resolver: (response, checkDates = true) =>
		###
		here is where we need to make checks of whether to pass along data
		to 'REZ' or resolve with the disambiguator
		###
		if response.status?            
			switch response.status.toLowerCase()
				when 'disambiguate'
					# #@log 'resolver disambiguate', response
					@inProgress = false
					@disambiguate response
				when 'in progress'
					@counter = 0
					@inProgress = true
					# store response so @disambiguate can get to it after @show
					@disambigContext = response
					# #@log 'resolver progress', response
					# display text to user and get response
					@show response
				when 'complete', 'completed'
					# #@log 'resolver complete', response
					@counter = 0
					@inProgress = false
					@disambigContext = { }
					if response.actor is null or response.actor is undefined
						@show response
					else
						@requestHelper @responder + 'actors/' + response.actor, 'POST', @mainContext, @show
		else  
			# #@log 'resolver response without status', response 
			payload = response.payload

			if payload? and checkDates
				if payload.start_date? or payload.start_time?
					datetime = @buildDatetime payload.start_date, payload.start_time

					#@log 'datetime no status', datetime

					payload.start_date = datetime.date if payload.start_date?
					payload.start_time = datetime.time if payload.start_time?

				if payload.end_date? or payload.end_time?
					datetime = @buildDatetime payload.end_date, payload.end_time

					payload.end_date = datetime.date if payload.end_date?
					payload.end_time = datetime.time if payload.end_time?


			# this needs to be an append and not an override
			@mainContext = response

			@counter++

			@requestHelper @responder + 'audit' , "POST", response, @resolver if @counter < 3

	addDebug: (results) =>
		@debugData.request = JSON.stringify(@debugData.request, null, 4) if @debugData.request?
		@debugData.response = JSON.stringify(@debugData.response, null, 4) if @debugData.response?
		
		if !results
			results = 
				debug:@debugData
		else
			results.debug = @debugData

		template = Handlebars.compile($('#debug-template').html())
		@board.find('.bubble:last').append(template(results))

	ask: (input) =>
		input = $(input)

		text = input.val()

		input.val('')
		
		template = Handlebars.compile($('#bubblein-template').html())
		
		@board.append(template(text)).scrollTop(@board.find('.bubble:last').offset().top)
		
		$('#input-form').addClass 'cancel'

		if @inProgress is true
			# #@log 'should disambiguate'
			@disambiguate text
		else
			data = query: text
					
			@requestHelper @classifier, "GET", data, (response) =>
				@addDebug() if @debug is true
			
				@resolver response

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

	# simulate: (e) =>
	#     data = JSON.stringify(
	#         data: @lastResponse
	#     )

	#     doneHandler = (response) =>
	#         #@log '* POST success'
	#         @board.append(@templates.simulate(response)).scrollTop(@board.height());
	#         @loader.hide();

	#     @requestHelper 'http://stremor-va.appspot.com/simulate', data, doneHandler
	
	show: (results) =>
		console.log results
		# Handlebars.compile($('#bubblein-template').html())
		templateName = if results.action? then results.action else 'bubbleout'

		template = $('#' + templateName + '-template').html()

		template = Handlebars.compile(template)

		@board.append(template(results)).scrollTop(@board.find('.bubble:last').offset().top)

		@addDebug(results) if @debug is true
			
		@loader.hide()

	mapper: (key) =>
		map = false
		if key.indexOf(@classifier) isnt -1
			map = "Casper"
		else if key.indexOf(@disambiguator) isnt -1
			map = "Disambiguator"
		else if key.indexOf(@responder) isnt -1
			map = "Rez"
		
		return map

	buildDeviceInfo: =>
		clientDate = new Date()

		deviceInfo =
			"latitude": @lat,
			"longitude": @lon,
			"timestamp": clientDate.getTime() / 1000,
			"timeoffset": - clientDate.getTimezoneOffset() / 60

	requestHelper: (endpoint, type, data, doneHandler) =>
		if @sendDeviceInfo is true
			data.device_info = @buildDeviceInfo()
			@sendDeviceInfo = false

		if @debug is true
			@debugData = 
				endpoint: endpoint
				type: type
				request: data

		endpointMap = @mapper(endpoint) 

		$.ajax(
			url: endpoint
			type: type
			data: if type is "POST" then JSON.stringify(data) else data 
			# contentType: "application/json"
			dataType: "json"
			timeout: 10000
			beforeSend: =>
				@log endpointMap, ">", data
				@loader.show()
		).done((response, status) =>
			@log endpointMap, "<", response
			if @debug is true
				@debugData.status = status
				@debugData.response = response

			doneHandler(response) if doneHandler?
		).fail((response, status) =>
			@log endpointMap, "<", response
			
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

	buildDatetime: (date, time) =>
		newDate = null

		newDate = @datetimeHelper(date) if date isnt null and date isnt undefined

		newDate = @datetimeHelper(time, newDate) if time isnt null and time isnt undefined

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

new Please()                    