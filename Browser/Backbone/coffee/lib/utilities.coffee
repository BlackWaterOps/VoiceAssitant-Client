define([
  'underscore',
  'backbone',
  'models/appState'
], (_, Backbone, AppState) ->
	utilities =
		operators:
			"+": (left, right) -> parseInt(left, 10) + parseInt(right, 10)
			"-": (left, right) -> parseInt(left, 10) - parseInt(right, 10)

		requestHelper: (endpoint, type, data, successHandler, errorHandler) =>
			if AppState.sendDeviceInfo is true
				data.device_info = @buildDeviceInfo()
				AppState.sendDeviceInfo = false

			$.ajax(
				url: endpoint
				type: type
				data: if type is "POST" then JSON.stringify(data) else data 
				# contentType: "application/json"
				dataType: "json"
				timeout: 10000
				beforeSend: =>
					console.log endpoint, type, data
					AppState.set 'requestStatus', 'beforeSend'
				).done((response) =>
					AppState.set 'requestStatus', 'done'
					successHandler(response) if successHandler?
				).fail((response) =>
					AppState.set 'requestStatus', 'fail'
					console.log '* POST fail', response, response.getResponseHeader()
					errorHandler(response) if errorHandler?
				).always((response) =>
					AppState.set 'requestStatus', 'complete'
				)
		
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
		
		newDateHelper: (datetime) =>
			@dateRegex = /\d{2,4}[-]\d{2}[-]\d{2}/i
			@timeRegex = /\d{1,2}[:]\d{2}[:]\d{2}/i

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

		datetimeHelper: (dateOrTime, newDate = null) =>
			console.log dateOrTime

			if _.isString(dateOrTime)
				console.log 'is string'
				newDate = @newDateHelper(dateOrTime) if _.isNull(newDate)	

			else if _.isObject(dateOrTime)
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

						if _.isArray(parsable) # date partials
							for item in parsable
								if _.isNull(newDate) 
									console.log 'step 2', 'set datetime'

									if _.isString(item) # 'now' or '2013-07-01' 
										newDate = @newDateHelper item
									else if _.isObject(item) #weekday, #fuzzy operators
										for itemKey, itemValue of item
											if newDate is null
												if itemKey.indexOf('weekday') isnt -1
													newDate = @weekdayHelper itemValue
												else if itemKey.indexOf('fuzzy') isnt -1
													isDate = if itemKey.indexOf('date') isnt -1 then true else false 
													newDate = @fuzzyHelper itemValue, isDate
								
								else if _.isNumber(item) # dates to add to Date object 
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

		toISOString: (dateObj) =>
			pad = (number) ->
				r = String number
				r = ('0' + r) if r.length is 1
				return r
			
			return ( dateObj.getFullYear() + '-' + pad( dateObj.getMonth() + 1 ) + '-' + pad( dateObj.getDate() ) + 'T' + pad( dateObj.getHours() ) + ':' + pad( dateObj.getMinutes() ) + ':' + pad( dateObj.getSeconds() ) )
			# + '.' + String( (dateObj.getMilliseconds()/1000).toFixed(3) ).slice( 2, 5 )

		buildDatetime: (date, time) =>
			dateObj = null

			console.log 'start date parsing'
			dateObj = @datetimeHelper(date) if date isnt null 
			
			console.log 'start time parsing'
			dateObj = @datetimeHelper(time, dateObj) if time isnt null

			dateString = @toISOString(dateObj).split('T')
			
			date: dateString[0]
			time: dateString[1]

		buildDeviceInfo: =>
			clientDate = new Date()

			deviceInfo =
				"latitude": AppState.get('lat')
				"longitude": AppState.get('lon')
				"timestamp": ( clientDate.getTime() / 1000 )
				"timeoffset": - clientDate.getTimezoneOffset() / 60
)