define([
  'underscore',
  'backbone',
  'models/appState'
], (_, Backbone, AppState) ->
	class Utilities
		constructor: ->
			@dateRegex = /\d{2,4}[-]\d{2}[-]\d{2}/i
			@timeRegex = /\d{1,2}[:]\d{2}[:]\d{2}/i

		operators:
			"+": (left, right) -> parseInt(left, 10) + parseInt(right, 10)
			"-": (left, right) -> parseInt(left, 10) - parseInt(right, 10)

		log: =>
			args = [ ]
			for argument in arguments
				argument = JSON.stringify(argument) if typeof argument is 'object'
				args.push argument
				
			console.log args.join(" ")
				
		weekdayHelper: (dayOfWeek) =>
			@log 'weekday helper', dayOfWeek

			date = new Date();

			currentDay = date.getDay()
			currentDate = date.getDate()

			# return date if currentDay is dayOfWeek
			
			offset = if currentDay < dayOfWeek then (dayOfWeek - currentDay) else (7 - (currentDay - dayOfWeek))
			
			date.setDate(currentDate + offset)

			return date

		fuzzyHelper: (datetime, isDate) =>
			@log 'fuzzy helper', datetime, isDate

			date = new Date()

			@log 'handle fuzzy date or time'
			# label = null
			# def = null

			for key, val of datetime
				@log key, val
				label = val if key is 'label'
				def = val if key is 'default'

			# presetLabel = @presets[label]
			# @log 'presetLabel', presetLabel
			# preference = if presetLabel? then presetLabel else def
			
			preference = def

			@log 'use', preference
			
			if preference is null
				@log 'useTime error' 
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
			@log 'newDate', datetime
			if datetime.indexOf('now') isnt -1
				@log 'is now'
				newDate = new Date();

			else if @dateRegex.test(datetime) is true
				@log 'is date string'
				split = datetime.split('-')
				newDate = new Date(split[0], (split[1]-1), split[2])

			else if @timeRegex.test(datetime) is true
				@log 'is time'
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

			@log 'newDate bottom', newDate

			return if newDate is null or newDate is undefined then new Date() else newDate

		datetimeHelper: (dateOrTime, newDate = null) =>
			@log dateOrTime

			if _.isString(dateOrTime)
				@log 'is string'
				newDate = @newDateHelper(dateOrTime) if _.isNull(newDate)	

			else if _.isObject(dateOrTime)
				@log 'is object'
				for action, parsable of dateOrTime
					@log 'step 1', action, parsable

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
									@log 'step 2', 'set datetime'

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
									@log 'step 3', 'parse array group'

									interval = item
									
									if interval is null 
										@log 'frag error', interval
										return

									if action.indexOf('time') isnt -1
										curr = newDate.getSeconds()
										time = @operators[operator](curr, interval)
										newDate.setSeconds(time)
									else if action.indexOf('date') isnt -1
										curr = newDate.getDate()
										date = @operators[operator](curr, interval)
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

			@log 'start date parsing'
			dateObj = @datetimeHelper(date) if date isnt null 
			
			@log 'start time parsing'
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

	return new Utilities()
)