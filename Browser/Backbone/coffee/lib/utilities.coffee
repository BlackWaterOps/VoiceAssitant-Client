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
				clientDate = new Date()

				#TODO: wrap ajax request with location request callback
				data.device_info =
					lat: AppState.lat,
					lon: AppState.lon,
					timestamp: clientDate.getTime() / 1000,
					timeoffset: - clientDate.getTimezoneOffset() / 60
            
				AppState.sendDeviceInfo = false

				data = JSON.stringify(data) if type is "POST"

				$.ajax(
					url: endpoint
					type: type
					data: data
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
			date = new Date()
			
			currentDay = date.getDay()
			currentDate = date.getDate()

			return date if currentDay is dayOfWeek
        
			offset = if currentDay < dayOfWeek then (dayOfWeek - currentDay) else (7 - (currentDay - dayOfWeek))
        
			date.setDate(currentDate + offset)

			return date
		
		fuzzyHelper: (datetime, isDate) =>
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

		datetimeHelper: (dateOrTime, newDate = null) =>
        	console.log dateOrTime

        	for action, parsable of dateOrTime
            	console.log 'step 1', action, parsable

            	operator = if action.indexOf('add') isnt -1 then '+' else '-'

            	parsableType = Object.prototype.toString.call(parsable)

            	if parsableType is '[object Array]' # date partials
                	for item in parsable
                    	itemType = Object.prototype.toString.call(item);

                    	if newDate is null 
                        	console.log 'step 2', 'set datetime'

                        	switch itemType
                            	when 'string' # 'now' or '2013-07-01'
                                	newDate = if item is 'now' then new Date() else new Date(item)
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
			dateObj = datetimeHelper(date) if date isnt null 
			
			console.log 'start time parsing'
			dateObj = datetimeHelper(time, dateObj) if time isnt null

			dateString = toISOString(dateObj).split('T')
			
			date: dateString[0]
			time: dateString[1]
)