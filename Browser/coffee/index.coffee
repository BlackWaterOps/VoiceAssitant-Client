class Please
	constructor: (options) ->
		@debug = true
		@debugData = { }
		@classifier = 'http://casper-cached.stremor-nli.appspot.com/'
		@disambiguator = 'http://casper-cached.stremor-nli.appspot.com/disambiguate'
		@responder = 'http://rez.stremor-apier.appspot.com/'
		@lat = 0.00
		@lon = 0.00
		@sendDeviceInfo = false
		@inProgress = false
		@mainContext = { }
		@disambigContext = { }
		@history = [ ]
		@pos = history.length
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
		
		if (@board.is(':empty'))
			init = $('#init')
			init.fadeIn('slow');
			setTimeout (->
				init.fadeOut 'slow'
			), 1000

		@getLocation()
		
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
	
	buildDeviceInfo: =>
		clientDate = Date()

		device = "device": 
			"type": "web-client",
			"lat": 0.00,
			"lon": 0.00,
			"timestamp": clientDate.getTime() / 1000,
			"timeoffset": - clientDate.getTimezoneOffset() / 60

	getLocation: =>
		navigator.geolocation.getCurrentPosition @updatePosition
	
	updatePosition: (position) =>
		@lat = position.coords.latitude
		@lon = position.coords.longitude
		
	disambiguate: (payload) =>
		if @inProgress is true
			endpoint = @disambiguator + "/active"

			field = @disambigContext.field

			type = @disambigContext.type
			
			text = payload

			data = 
				text: text
				types: [type]

			console.log 'disambiguate user response', data
		else
			console.log 'disambiguate rez response'

			endpoint = @disambiguator + "/passive"

			@sendDeviceInfo = true

			field = payload.field
			# TODO: handle multi types
			type = payload.type

			text = @mainContext.payload[field]

			data = 
				text: text
				types: [type]

		successHandler = (results) =>
			console.log 'successHandler', results
			# gross hack to stop the resolver from 
			checkDates = true

			if @debug is true and @inProgress is true
				@addDebug(null, 'in')
				
			if results?
				if results.date? or results.time?
					datetime = @buildDatetime(results.date, results.time)
				
					results[type] = datetime[type]

					checkDates = false

					console.log 'successhandler', results

			@mainContext.payload[field] = results[type]
			
			@resolver @mainContext, checkDates

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
					console.log 'resolver disambiguate', response
					@inProgress = false
					@disambiguate response
				when 'in progress'
					@counter = 0
					@inProgress = true
					# store response so @disambiguate can get to it after @show
					@disambigContext = response
					console.log 'resolver progress', response
					# display text to user and get response
					@show response
				when 'complete', 'completed'
					console.log 'resolver complete', response
					@counter = 0
					@inProgress = false
					@disambigContext = { }
					if response.actor is null or response.actor is undefined
						@show response
					else
						@requestHelper @responder + response.actor, 'POST', @mainContext, @show
		else  
			console.log 'resolver response without status', response 
			payload = response.payload

			if payload? and checkDates
				if payload.start_date? or payload.start_time?
					datetime = @buildDatetime payload.start_date, payload.start_time

					console.log 'datetime no status', datetime

					payload.start_date = datetime.date if payload.start_date?
					payload.start_time = datetime.time if payload.start_time?

				if payload.end_date? or payload.end_time?
					datetime = @buildDatetime payload.end_date, payload.end_time

					payload.end_date = datetime.date if payload.end_date?
					payload.end_time = datetime.time if payload.end_time?


			# this needs to be an append and not an override
			@mainContext = response

			@counter++

			@requestHelper @responder, "POST", response, @resolver if @counter < 3

	addDebug: (results, bubble) =>
		@debugData.request = JSON.stringify(@debugData.request, null, 4) if @debugData.request?
		@debugData.response = JSON.stringify(@debugData.response, null, 4) if @debugData.response?
		@debugData.bubble = bubble
		
		if !results
			results = 
				debug:@debugData
		else
			results.debug = @debugData

		template = Handlebars.compile($('#debug-template').html())
		@board.append(template(results)).scrollTop(@board.height())

	ask: (input) =>
		console.log 'ask'
		input = $(input)

		text = input.val()

		input.val('')
		
		template = Handlebars.compile($('#bubblein-template').html())
		
		@board.append template(text)
	
		if @inProgress is true
			console.log 'should disambiguate'
			@disambiguate text
		else
			data = query: text
					
			@requestHelper @classifier, "GET", data, (response) =>
				if @debug is true
					@addDebug(null, 'in')

				@resolver response

	keyup: (e) =>
		value = $(e.target).val()

		target = $(e.target)

		switch e.which
			when 13
				if value
					@ask target
					# history.push value
					@pos = history.length
			when 38
				pos -= 1 if pos > 0
				target.val history[pos]

			when 40
				pos += 1 if pos < history.length
				target.val history[pos]
				
	expand: (e) =>
	    e.preventDefault()
	    $(e.target).parent().next().toggle()

	# simulate: (e) =>
	#     data = JSON.stringify(
	#         data: @lastResponse
	#     )

	#     doneHandler = (response) =>
	#         console.log '* POST success'
	#         @board.append(@templates.simulate(response)).scrollTop(@board.height());
	#         @loader.hide();

	#     @requestHelper 'http://stremor-va.appspot.com/simulate', data, doneHandler
	
	show: (results) =>
		# Handlebars.compile($('#bubblein-template').html())
		templateName = if results.action? then results.action else 'bubbleout'

		template = $('#' + templateName + '-template').html()

		template = Handlebars.compile(template)

		@board.append(template(results)).scrollTop(@board.height())

		@addDebug(results, 'out') if @debug is true
			
		@loader.hide()

	requestHelper: (endpoint, type, data, doneHandler) =>
		if @sendDeviceInfo is true
			clientDate = new Date()
			# TODO: APPEND THIS DATA
			data.device_info =
				"latitude": @lat,
				"longitude": @lon,
				"timestamp": clientDate.getTime() / 1000,
				"timeoffset": - clientDate.getTimezoneOffset() / 60
			
			@sendDeviceInfo = false

		if @debug is true
			@debugData = 
				endpoint: endpoint
				type: type
				request: data

		$.ajax(
			url: endpoint
			type: type
			data: if type is "POST" then JSON.stringify(data) else data 
			# contentType: "application/json"
			dataType: "json"
			timeout: 10000
			beforeSend: =>
				console.log endpoint, type, data
				@loader.show()
		).done((response, status) =>
			if @debug is true
				@debugData.status = status
				@debugData.response = response

			doneHandler(response) if doneHandler?
		).fail((response, status) =>
			console.log '* POST fail', response, response.getResponseHeader()
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

		console.log 'start date parsing', date
		newDate = @datetimeHelper(date) if date isnt null and date isnt undefined

		console.log 'start time parsing', time
		newDate = @datetimeHelper(time, newDate) if time isnt null and time isnt undefined

		console.log 'buildDatetime datestring', newDate

		dateString = @toISOString(newDate).split('T')
		
		console.log 'buildDatetime datestring', dateString

		date: dateString[0]
		time: dateString[1]

	# #date-add: {[{#weekday:1}, [1, 'day']]}
	weekdayHelper: (dayOfWeek) =>
		console.log 'weekday helper', dayOfWeek

		date = new Date();

		currentDay = date.getDay()
		currentDate = date.getDate()

		# return date if currentDay is dayOfWeek
		
		offset = if currentDay < dayOfWeek then (dayOfWeek - currentDay) else (7 - (currentDay - dayOfWeek))
		
		date.setDate(currentDate + offset)

		return date

	fuzzyHelper: (datetime, isDate) =>
		console.log 'fuzzy helper', datetime, isDate

		date = new Date()

		console.log 'handle fuzzy date or time'
		label = null
		def = null

		for key, val of datetime
			console.log key, val
			label = val if key is 'label'
			def = val if key is 'default'

		presetLabel = @presets[label]
		console.log 'presetLabel', presetLabel
		preference = if presetLabel? then presetLabel else def
		
		console.log 'use', preference
		
		if preference is null
			console.log 'useTime error' 
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
	
	newDate: (datetime) =>
		console.log 'newDate', datetime
		if datetime.indexOf('now') isnt -1
			console.log 'is now'
			newDate = new Date();

		else if @dateRegex.test(datetime) is true
			console.log 'is date string'
			split = datetime.split('-')
			newDate = new Date(split[0], (split[1]-1), split[2])

		else if @timeRegex.test(datetime) is true
			console.log 'is time'
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

		console.log 'newDate bottom', newDate

		return if newDate is null or newDate is undefined then new Date() else newDate

	# {'#time_add': [{'#time_fuzzy': {'label': 'dinner', 'default': '19:00:00'}}, 3600]}
	datetimeHelper: (dateOrTime, newDate = null) =>
		console.log dateOrTime

		dateOrTimeType = Object.prototype.toString.call(dateOrTime)

		switch (dateOrTimeType)
			when '[object String]'
				console.log 'is string'
				if newDate is null
					newDate = @newDate dateOrTime

			when '[object Object]'
				console.log 'is object'
				for action, parsable of dateOrTime
					console.log 'step 1', action, parsable

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
									console.log 'step 2', 'set datetime'

									switch itemType
										when '[object String]' # 'now' or '2013-07-01'
											newDate = @newDate item
										when '[object Object]' #weekday, #fuzzy operators
											for itemKey, itemValue of item
												if newDate is null
													if itemKey.indexOf('weekday') isnt -1
														newDate = @weekdayHelper itemValue
													else if itemKey.indexOf('fuzzy') isnt -1
														isDate = if itemKey.indexOf('date') isnt -1 then true else false 
														newDate = @fuzzyHelper itemValue, isDate
								
								else if itemType is '[object Number]' # dates to add to Date object 
									console.log 'step 3', 'parse array group'

									interval = item
									
									if interval is null 
										console.log 'frag error', interval
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
	
	###   
	datetimeHelper: (dateOrTime, newDate = null) =>
		console.log dateOrTime

		for action, parsable of dateOrTime
			console.log 'step 1', action, parsable

			if action.indexOf('fuzzy') is -1
				operator = if action.indexOf('add') isnt -1 then '+' else '-'

			parsableType = Object.prototype.toString.call(parsable)

			if parsableType is '[object Array]' # date partials
				for item in parsable
					itemType = Object.prototype.toString.call(item);

					if newDate is null 
						console.log 'step 2', 'set datetime'

						switch itemType
							when '[object String]' # 'now' or '2013-07-01'
								newDate = if item is "now" then new Date() else new Date(item)
							when '[object Object]' # weekday operator {#weekday:1}
								for itemKey, itemValue of item
									newDate = @weekdayHelper itemValue if itemKey.indexOf("weekday") isnt -1 and newDate is null
					else if itemType is '[object Array]' # dates to add to Date object 
						console.log 'step 3', 'parse array group'

						interval = null
						type = null

						for frag in item
							interval = frag if typeof frag is 'number' and not interval?
							type = frag if typeof frag is 'string' and not type?

						if interval is null or type is null
							console.log 'frag error', interval, type
							return

						if type.indexOf('second') isnt -1
							curr = newDate.getSeconds()
							time = operators[operator](curr, interval)
							newDate.setSeconds(time)
						else if type.indexOf('minute') isnt -1
							curr = newDate.getMinutes()
							time = operators[operator](curr, interval)
							newDate.setMinutes(time)
						else if type.indexOf('hour') isnt -1
							curr = newDate.getHours()
							time = operators[operator](curr, interval)
							newDate.setHours(time)
						else if type.indexOf('day') isnt -1
							curr = newDate.getDate()
							date = operators[operator](curr, interval)
							newDate.setDate(date)

			else if parsableType is '[object Object]' # if item is an object we must have a 'special' time
				console.log 'step 2', 'handle fuzzy date or time'
				label = null
				def = null

				for key, val of parsable
					console.log key, val
					label = val if key is 'label'
					def = val if key is 'default'

				presetLabel = @presets[label]
				console.log 'presetLabel', presetLabel
				useTime = if presetLabel? then presetLabel else def
				console.log 'useTime', useTime
				if useTime is null
					console.log 'useTime error' 
					return

				useTimeArr = useTime.trim().split(':')

				newDate.setHours useTimeArr[0]
				newDate.setMinutes useTimeArr[1]
				newDate.setSeconds useTimeArr[2]

		return newDate
	###

new Please()                    