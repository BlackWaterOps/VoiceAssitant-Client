class window.Please
	constructor: (options) ->
		@debug = true
		@debugData = { }
		@classifier = 'http://casper-cached.stremor-nli.appspot.com/v1'
		@disambiguator = 'http://casper-cached.stremor-nli.appspot.com/v1/disambiguate'
		@personal = 'http://stremor-pud.appspot.com/v1/'
		@responder = 'http://rez.stremor-apier.appspot.com/v1/'
		@lat = 33.4930947
		@lon = -111.928558
		@mainContext = null
		@disambigContext = null
		@history = [ ]
		@pos = @history.length
		@loader = $('.loader')
		@board = $('.board')
		@input = $('.main-input')
		@dateRegex = /\d{4}[-]\d{2}[-]\d{2}/i
		@timeRegex = /\d{2}[:]\d{2}[:]\d{2}/i
		@counter = 0
		@disableSpeech = false

		@currentState = 
			status: null
			origin: null

		# pretend presets is a list of 'special' times populated from a DB that stores user prefs
		@presets = 
			'after work': '18:00:00'
			'breakfast': '7:30:00'
			'lunch': '12:00:00'
		
		@init()

	init: =>
		@getLocation()
		@registerHandlebarHelpers()
		@registerListeners()

		@input.focus()
		.on('webkitspeechchange', @ask)
		.on('keyup', @keyup)

		$('body').on('click', '.expand', @expand)
				 .on('click', '.choice-item', @handleChoice)
		
		
		#.on('click', '.simulate', @simulate)
		
		$('#cancel').on('click', @cancel)

		if (@board.is(':empty'))
			init = $('.init')
			init.fadeIn('slow')
			setTimeout (->
				init.fadeOut 'slow'
			), 1000

	registerListeners: =>
		$(document)
		.on('init', @classify)
		.on('audit', @auditor)
		.on('disambiguate', @disambiguatePassive)
		.on('disambiguate:personal', @disambiguatePersonal)
		.on('disambiguate:active', @disambiguateActive)
		.on('disambiguate:candidate', @disambiguateCandidate)
		.on('restart', @replaceContext)
		.on('choice', @choose)
		.on('inprogress', @show)
		.on('completed', @actor)
		.on('error', @show)
		.on('debug', @addDebug)

	registerHandlebarHelpers: =>
		Handlebars.registerHelper('elapsedTime', (dateString) =>
			results = @elapsedTimeHelper(dateString)
			return results.newDate + ' ' + results.newTime 
		)
		
		Handlebars.registerHelper('flightDates', (dateString, type, options) =>
			return "--" if not dateString?
			
			formatted = @formatDate(dateString)

			am = formatted.am
			mm = formatted.month
			dd = formatted.date
			yy = formatted.year
			hh = formatted.hours
			min = formatted.minutes
			day = formatted.dayOfWeek
			mon = formatted.monthOfYear

			time = $('<span/>').addClass('formatted-time').text(hh + ":" + min + " " + am)
			date = $('<span/>').addClass('formatted-date').text(mm + "/" + dd + "/" + yy)

			if type is 'actual'
				result = time.get(0).outerHTML
			else
				result = date.get(0).outerHTML + "&nbsp;" + time.get(0).outerHTML 
			
			new Handlebars.SafeString(result)
		)

		Handlebars.registerHelper('eventDates', (dateString) =>
			# "ddd, MMM d, yyyy"
			# Wed, Jan 5, 2013 

			formatted = @formatDate(dateString)

			mm = formatted.month
			dd = formatted.date
			yy = formatted.year
			day = formatted.dayOfWeek
			mon = formatted.monthOfYear

			return day.substr(0, 3) + ", " + mon.substr(0, 3) + " " + dd + ", " + yy
		)

		Handlebars.registerHelper('stockShare', (direction, price_change, price_change_percent) =>
			change = $('<span/>').text(price_change + " " + price_change_percent + "%")

			change.addClass('stock-up') if direction is "up"
			change.addClass('stock-down') if direction is "down"
			
			result = change.get(0).outerHTML

			new Handlebars.SafeString(result)
		)

	ask: (input) =>
		if typeof input is 'string'
			text = input
		else
			input = $(input)

			text = input.val()

			input.val('')
		
		template = Handlebars.compile($('#bubblein-template').html())
		
		@board.append(template(text)).scrollTop(@board.find('.bubble:last').offset().top)
		
		$('.input-form').addClass('cancel')

		doc = $(document)

		if @currentState.state is 'inprogress' or (@currentState.state is 'error' and @disambigContext?)
			doc.trigger(
				type: 'disambiguate:active'
				response: text
			)
		else if @currentState.state is 'choice'
			doc.trigger(
				type: 'disambiguate:candidate'
				response: text
			)
		else
			doc.trigger(
				type: 'init'
				response: text
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

	clearContext: =>
		@mainContext = null
		@disambigContext = null
		@currentState = 
			state: null
			origin: null

	cancel: (e) =>
		@board.empty()
		@history = [ ]
		@clearContext()
		$('.input-form').removeClass 'cancel'
		@loader.hide()
		@counter = 0
		@input.focus()
		@clearChoiceList()

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
	
	replaceContext: (e) =>
		# @counter = 0
		
		$(document).trigger(
			type: 'audit'
			response: e.response.data
		)

	classify: (e) =>
		query = if e instanceof $.Event then e.response else e

		data = query: query
				
		@requestHelper(@classifier, "GET", data, (response) =>
			$(document)
			.trigger($.Event('debug'))
			.trigger(
				type: 'audit'
				response: @clientOperations(response, response.payload)
			)
		)

	disambiguateSuccessHandler: (response) =>
		debuggable = ['inprogress', 'error', 'choice']

		$(document).trigger($.Event('debug')) if debuggable.indexOf(@currentState.state) isnt -1				
			
		if response?
			# create a clone of the context to update so we can compare them later
			context = $.extend(true, {}, @mainContext)

			context = @clientOperations(context, response)

			field = @disambigContext.field

			type = @disambigContext.type

			# find & replace the specific field indicated in the response 
			if field.indexOf('.') isnt -1
				context = @replace(context, field, response[type])
			else
				context.payload[field] = response[type]
			
			$(document).trigger(
				type: 'audit'
				response: context
			)
		else
			console.log 'oops no responder response', results

	disambiguateActive: (e) =>
		# field = @disambigContext.field

		type = @disambigContext.type
		
		text = e.response

		postData = 
			payload: text
			type: type

		@requestHelper(@disambiguator + '/active', 'POST', postData, @disambiguateSuccessHandler)

	disambiguateCandidate: (e) =>
		# field = @disambigContext.field

		type = @disambigContext.type
		
		text = e.response

		list = @disambigContext.show.simple.list

		postData =
			payload: text
			type: type
			candidates: list

		@requestHelper(@disambiguator + '/candidate', 'POST', postData, @disambiguateSuccessHandler)
		
		###
		@requestHelper(@disambiguator + '/candidate', 'POST', postData, (response) =>
			@disambiguateSuccessHandler(response, field, type)
		)
		###

	disambiguatePersonal: (e) =>
		# in the future we'll need to send a userid for personal data 
		data = e.response

		field = data.field

		type = data.type

		text = if field.indexOf('.') isnt -1 then @find(@mainContext, field) else @mainContext.payload[field]

		postData = 
			type: type
			payload: text

		@requestHelper(@personal + 'disambiguate', 'POST', postData, @disambiguateSuccessHandler)

	disambiguatePassive: (e) =>	
		data = e.response

		field = data.field

		type = data.type

		text = if field.indexOf('.') isnt -1 then @find(@mainContext, field) else @mainContext.payload[field]

		postData = 
			type: type
			payload: text

		@requestHelper(@disambiguator + '/passive', 'POST', postData, @disambiguateSuccessHandler)

	choose: (e) =>
		data = e.response

		list = $('<ul/>')

		for item in data.show.simple.list
			listItem = $('<li/>').addClass('choice-item').data('choice', item).append($('<a/>').text(item.text))
			list.append(listItem)

		$('.list-slider').html(list)
		$('body').addClass('choice')

		# should create trigger
		@show(e)

	handleChoice: (e) =>						
		choice = $(e.currentTarget).data('choice')
		
		field = @disambigContext.field

		template = Handlebars.compile($('#bubblein-template').html())

		@board.append(template(choice.text)).scrollTop(@board.find('.bubble:last').offset().top)

		# type = @disambigContext.type
		
		# find & replace the specific field indicated in the response 
		if field.indexOf('.') isnt -1
			@mainContext = @replace(@mainContext, field, choice.data)
		else
			@mainContext.payload[field] = choice.data
		
		@requestHelper(@responder + 'audit' , 'POST', @mainContext, (response) =>
			$(document).trigger($.Event('debug'))

			@auditorSuccessHandler(response)
		)
	
	clearChoiceList: =>
		$('body').removeClass('choice').find('list-slider').empty()

	# NOTE: data can be a jquery event object or a plain object
	auditor: (e) =>
		# context = if data instanceof $.Event then data.response else data
		context = e.response
				
		isEqual = @isEqual(context, @mainContext)

		if not isEqual
			@mainContext = context

			# @counter++

			@requestHelper(@responder + 'audit' , 'POST', context, @auditorSuccessHandler)
		else
			console.log('potential request loop detected')

	auditorSuccessHandler: (response) =>
		state = response.status.replace(' ', '')

		crossCheck = state.split(':')[0]

		tempStates = ['disambiguate', 'inprogress', 'choice']
	
		@disambigContext = response if tempStates.indexOf(crossCheck) isnt -1

		@currentState = 
			state: state
			origin: 'auditor'

		$(document).trigger(
			type: @currentState.state
			response: response
		)

	actor: (e) =>
		@disambigContext = { }

		data = e.response
		
		if data.actor is null or data.actor is undefined
			@show(data)
		else
			endpoint = @responder + 'actors/' + data.actor

			if data.actor.indexOf('private') isnt -1
				data.actor = data.actor.replace('private:', '')
				endpoint = @personal + 'actors/' + data.actor

			@requestHelper(endpoint, 'POST', @mainContext, @actorResponseHandler)
	
	actorResponseHandler: (response) =>
		@clearContext()
		@show(response)

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

		if templateData.link?
			templateName = 'link'
		else if templateData.image?
			templateName = 'image'
		else
			templateName = 'bubbleout'

		template = $('#' + templateName + '-template')
		
		template = Handlebars.compile(template.html())

		# TODO: create template wrapper div to inject templates into
		@board.append(template(templateData)).scrollTop(@board.find('.bubble:last').offset().top)

		$(document).trigger(
			type: 'debug'
			response: results
		)

		if results.show? and results.show.structured? and results.show.structured.template?
			templateData = results.show.structured.items || results.show.structured.item
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
		
		if window isnt window.top
			# Chrome app
			window.top.postMessage(
				action: 'speak',
				speak: results.speak
				options: {}
			, '*')
		else
			# Cordova App
			$(document).trigger(
				type: 'speak'
				response: results.speak
			)

	getLocation: =>
		navigator.geolocation.getCurrentPosition @updatePosition
	
	updatePosition: (position) =>
		@lat = position.coords.latitude
		@lon = position.coords.longitude
	
	isEqual: (object1, object2) =>
		JSON.stringify(object1) is JSON.stringify(object2)

	reduce: (fun, iterable, initial) =>
		if iterable.length > 0
			initial = fun(initial, iterable[0])
			return @reduce(fun, iterable.slice(1), initial)
		else
			return initial
 
	find: (context, field) =>
		fields = field.split('.')
		
		if 'function' is typeof Array.prototype.reduce
			fields.reduce((prevVal, currVal, index, array) =>
				return prevVal[currVal] || null
			, context)
		else
			@reduce((obj, key) =>
				return obj[key] || null
			, fields, context)

	replace: (context, field, type) =>
		fields = field.split('.')

		last = fields.pop()

		field = fields.join('.')

		obj = @find(context, field)

		obj[last] = type

		return context		

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
		if endpoint.indexOf(@disambiguator) isnt -1 and endpoint.indexOf('passive') isnt -1
			data = $.extend(true, {}, data)
			data.device_info = @buildDeviceInfo()

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
			timeout: 20000
			beforeSend: =>
				@log endpointName, ">", data
				@clearChoiceList()
				@input.prop('readonly', true)
				@loader.show()
		).done((response, status) =>
			@log endpointName, "<", response
			@input.prop('readonly', false)

			if @debug is true
				@debugData.status = status
				@debugData.response = response

			doneHandler(response) if doneHandler?
		).fail((response, status) =>
			@error endpointName, "<", (if response.responseJSON? then response.responseJSON else response)
			@input.prop('readonly', false)

			if @debug is true
				@debugData.status = status
				@debugData.response = response
			
			@loader.hide()
			@mainContext = null
		)
	###
	operators = 
		"+": (left, right) -> parseInt(left, 10) + parseInt(right, 10)
		"-": (left, right) -> parseInt(left, 10) - parseInt(right, 10)
	###

	toISOString: (dateObj) =>
		pad = (number) ->
			r = String number
			r = ('0' + r) if r.length is 1
	  
			return r
 
		( dateObj.getFullYear() + '-' + pad( dateObj.getMonth() + 1 ) + '-' + pad( dateObj.getDate() ) + 'T' + pad( dateObj.getHours() ) + ':' + pad( dateObj.getMinutes() ) + ':' + pad( dateObj.getSeconds() ) )
		# + '.' + String( (dateObj.getMilliseconds()/1000).toFixed(3) ).slice( 2, 5 )

	# Note: 
	# data represents the payload object in response to classification
	# and represents the entire response object in response to disambiguation
	clientOperations: (context, data) =>
		if data?
			# replace location operators			
			data = @replaceLocation(data)

			# find & replace date time fields
			data = @replaceDates(data)

			# prepend (potential) unused_tokens to payload.{field}
			context = @prependTo(context, data) if data.unused_tokens?		

		return context

	prependTo: (context, data) =>
		prepend = data.unused_tokens.join(" ")

		field = data.prepend_to

		payloadField = context.payload[field]

		payloadField = if not payloadField? then "" else " " + payloadField

		context.payload[field] = prepend + payloadField

		return context

	replaceLocation: (payload) =>
		if payload? and payload.location?
			switch payload.location
				when '#current_location'
					payload.location = @buildDeviceInfo()

		return payload

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
				includeDate = true
				includeTime = true

				if not payload[date]?
					console.log("dont include " + date)
					includeDate = false

				if not payload[time]? or Object.prototype.toString.call(time) is "[object Array]"
					console.log("dont include " + time)
					includeTime = false

				datetime = @buildDatetime(payload[date], payload[time])

				payload[date] = datetime[0] if includeDate is true
					
				payload[time] = datetime[1] if includeTime is true

		return payload
	
	buildDatetime: (dateO, timeO) =>
		now = new Date()

		date = null
		time = null

		dateType = Object.prototype.toString.call(dateO)
		timeType = Object.prototype.toString.call(timeO)

		if dateType is "[object String]"
			if dateO is "#date_now"
				date = now
			else if dateRegex.test(dateO)
				dateSplit = dateO.split("-")

				date = new Date(dateSplit[0], dateSplit[1], dateSplit[2])
		else if dateType is "[object Object]"
			date = @parseDateObject(dateO, now)

		if timeType is "[object String]"
			if timeO is "#time_now"
				time = now
			else if @timeRegex.test(timeO)
				timeSplit = timeO.split(":")

				time = new Date(now.getFullYear(), now.getMonth(), now.getDate(), timeSplit[0], timeSplit[1])
		else if timeType is "[object Object]"
			hasDate = if date? then true else false
				
			baseDate = if hasDate then date else now

			ret = @parseTimeObject(timeO, baseDate, now)

			if (ret isnt null)
				time = ret

				date = ret if hasDate

		dString = null
		tString = null

		if date?
			console.log("date", date)
			date = @toISOString(date)
			dString = date.split("T")[0]

		if time?
			console.log("time", time)
			time = @toISOString(time)
			tString = time.split("T")[1]

		return [dString, tString]
		

	parseDateObject: (dateO, now) =>
		return @parseDateWeekdayObject(dateO, now) if dateO["#date_weekday"]?

		return @parseDateAddObject(dateO, now) if dateO["#date_add"]?

		return null

	parseDateWeekdayObject: (obj, now) =>
		dayOfWeek = obj["#date_weekday"]

		currentDay = now.getDay()
		currentDate = now.getDate()
		
		if dayOfWeek isnt currentDay
			offset = if currentDay < dayOfWeek then (dayOfWeek - currentDay) else (7 - (currentDay - dayOfWeek))
		
			now.setDate(currentDate + offset)

		return now

	parseDateAddObject: (obj, now) =>
		operands = obj["#date_add"]

		base = operands[0]

		baseDate = null

		if base["#date_weekday"]?
			baseDate = @parseDateObject(first, now)
		else if base is "#date_now"
			baseDate = now

		return null if baseDate is null

		baseDate.setDate(baseDate.getDate() + parseInt(operands[1], 10))

		return baseDate

	parseTimeObject: (timeO, baseDate, now) =>
		return @parseTimeAddObject(timeO, baseDate, now) if timeO["#time_add"]?

		return @parseTimeFuzzyObject(timeO, baseDate, now) if timeO["#fuzzy_time"]?

		return null

	parseTimeAddObject: (obj, baseDate, now) =>
		operands = obj["#time_add"]

		base = operands[0]

		baseDateTime = null

		if first["#fuzzy_time"]?
			baseDateTime = @parseTimeObject(first, baseDate, now)
		else if first is "#time_now"
			baseDateTime = now

		return null if baseDateTime is null

		baseDateTime.setSeconds(baseDateTime.getSeconds() + parseInt(operands[1], 10))

		return baseDateTime

	parseTimeFuzzyObject: (obj, baseDate, now) =>
		label = obj["label"]

		def = obj["default"]

		defaultSplit = def.split(":")

		defaultTime = new Date(now.getFullYear(), now.getMonth(), now.getDate(), defaultSplit[0], defaultSplit[1])

		time = @getFuzzyTimeValue(label, now)

		datetime = if time is null then defaultTime else time

		baseDate.setHours(datetime.getHours())
		baseDate.setMinutes(datetime.getMinutes())
		baseDate.setSeconds(datetime.getSeconds())

		return baseDate

	getFuzzyTimeValue: (label, now) =>
		presetLabel = @presets[label]
		
		if presetLabel?
			presetSplit = presetLabel.split(":")

			return new Date(now.getFullYear(), now.getMonth(), now.getDate(), presetSplit[0], presetSplit[1])

		return null


	###
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
	###

	elapsedTimeHelper: (dateString) =>
		formatted = @formatDate(dateString)

		pubdate = formatted.month + "/" + formatted.date + "/" + formatted.year 
		pubtime = formatted.hours + ":" + formatted.minutes

		origPubdate = pubdate
		origPubtime = pubtime

		dt = new Date(dateString)
		ut = new Date()

		dTime = dt.getTime()
		uTime = ut.getTime()

		if (uTime - dTime) < 86400000
			pubdate = ""
			pTime = Math.round((((uTime - dTime) / 1000) / 60))
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

	formatDate: (dateString) =>
		monthsOfTheYear = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]
		
		daysOfTheWeek = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"]

		dt = new Date(dateString.replace(/[TZ]/g, " ").trim())
		
		mm = dt.getMonth() + 1
		dd = dt.getDate()
		yy = dt.getFullYear()
		hh = dt.getHours()
		min = dt.getMinutes()
		day = dt.getDay()

		mm = ("0" + mm) if mm < 10
		dd = ("0" + dd) if dd < 10

		am = if (hh > 12) then "pm" else "am"
		hh = (hh - 12) if hh > 12
		hh = ("0" + hh) if hh < 10
		min = ("0" + min) if min < 10

		date = 
			year: yy
			month: mm
			monthOfYear: monthsOfTheYear[dt.getMonth()]
			date: dd
			day: day
			dayOfWeek: daysOfTheWeek[day]
			hours: hh
			minutes: min
			am: am

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
