define([
	'underscore',
	'backbone',
	'util',
	'models/appState',
	'models/classifier',
	'models/responder',
	'models/disambiguator',
	'handlebars'
], (_, Backbone, Util, AppState, Classifier, Responder, Disambiguator) ->
	class IndexView extends Backbone.View
		events:
			'keyup #main-input': 'keyup'
			'webkitspeechchange #main-input': 'ask'
			'click .expand': 'expand'
			'click #cancel': 'cancel'

		initialize: () ->
			@board = $('#board')
			@loader = $('#loader')
			@input = $('#main-input')
			@checkDates = false

			AppState.on 'change:mainContext change:responderContext', @resolver
			
			# TODO: figure out best way to get a callback init'd
			# Disambiguator.on 'sync', @disambiguateResults

		render: (options) ->
			@input.focus()

			if (@board.is(':empty'))
				init = $('#init')
				init.fadeIn('slow')
				setTimeout (->
					init.fadeOut 'slow'
				), 1000

			@getLocation()

		log: =>
			args = [ ]
			for argument in arguments
				argument = JSON.stringify(argument) if typeof argument is 'object'
				args.push argument

			console.log args.join(" ")

		getLocation: =>
			navigator.geolocation.getCurrentPosition @updatePosition
	
		updatePosition: (position) =>
			AppState.set 
				lat: position.coords.latitude
				lon: position.coords.longitude

		show: (results) =>
			# Handlebars.compile($('#bubblein-template').html())
			templateName = if results.action? then results.action else 'bubbleout'

			template = $('#' + templateName + '-template').html()

			template = Handlebars.compile(template)

			@board.append(template(results)).scrollTop(@board.find(':last').offset().top)

			@addDebug(results) if AppState.get 'debug' is true
				
			@loader.hide()

		cancel: (e) =>
			@board.empty()
			AppState.set
				mainContext: { }
				responderContext: { }
				history: [ ]

			$('#input-form').removeClass 'cancel'
			@loader.hide()
			@input.focus()

		addDebug: (results) =>
			debugDate = AppState.get 'debugData'

			debugData.request = JSON.stringify(debugData.request, null, 4) if debugData.request?
			debugData.response = JSON.stringify(debugData.response, null, 4) if debugData.response?
			
			if !results
				results = 
					debug: debugData
			else
				results.debug = debugData

			template = Handlebars.compile($('#debug-template').html())
			@board.find(':last').append(template(results))

		ask: (e) =>
			input = $(e)

			text = input.val()

			input.val('')
			
			template = Handlebars.compile($('#bubblein-template').html())
		
			@board.append(template(text)).scrollTop(@board.find(':last').offset().top)

			if AppState.get 'inProgress' is true
				@log 'should disambiguate'
				@disambiguate text
			else
				classifier = new Classifier()
				classifier.fetch(data: query: text)

		keyup: (e) =>
			value = $(e.target).val()

			target = $(e.target)
			
			history = AppState.get 'history'

			pos = AppState.get 'pos'

			switch e.which
				when 13
					if value
						@ask target
						# history.push value
						AppState.set('pos', history.length)
				when 38
					AppState.set('pos', pos - 1) if pos > 0
					target.val history[pos]

				when 40
					AppState.set('pos', pos + 1) if pos < history.length
					target.val history[pos]
		
		disambiguate: (payload) =>
			if AppState.get('inProgress') is true
				action = 'active'
				context = AppState.get 'responderContext'
				field = context.field
				type = AppState.get('responderContext').type
				text = payload
				data = 
					text: text
					types: [type]
				@log 'disambiguate user response', data
			else
				@log 'disambiguate rez response'
				action = 'passive'
				context = AppState.get 'mainContext'

				field = payload.field
				type = payload.type
				text = context.get('payload')[field]
				data = 
					text: text
					types: [type]
					device_info: Util.buildDeviceInfo()

			dis = new Disambiguator(data, action: action)

			dis.post()

		disambiguateResults: (response) =>
			@log 'successHandler', results
			
			@checkDates = true

			@addDebug() if AppState.get('debug') is true and AppState.get('inProgress') is true
				
			if response?
				if (response.date? or response.time?)
					datetime = Util.buildDatetime(response.date, response.time)
				
					response[type] = datetime[type]

					@log 'done handler', response

			context = AppState.get 'mainContext'

			payload =  context.payload

			payload[field] = response[type]

			context.payload = payload

			# this should trigger resolver!!
			AppState.set 'mainContext', context

		resolver: (model, response, opts) =>
			###
			here is where we need to make checks of whether to pass along data
			to 'REZ' or resolve with the disambiguator
			###
			if response.status?
				switch response.status.toLowerCase()
					when 'disambiguate'
						@log 'resolver disambiguate', response
						AppState.set 'inProgress', false

						@disambiguate response
					when 'in progress'
						@log 'resolver progress', response
						
						# store response so @disambiguate can get to it after @show
						AppState.set 
							inProgress: true
							responderContext: response

						# display text to user and get response
						@show response
					when 'complete', 'completed'
						@log 'resolver complete', response
						
						AppState.set 
							inProgress: false
							responderContext: { }

						if response.actor is null or response.actor is undefined
							@show response
						else
							context = AppState.get('mainContext')
							dis = new Responder(context, action: 'actors')
			else  
				@log 'resolver response without status', response, AppState.get('mainContext')

				payload = response.payload

				#TODO: put these dates and disambig response dates into one function
				if payload? and @checkDates
					if payload.start_date? or payload.start_time?
						datetime = Util.buildDatetime payload.start_date, payload.start_time

						@log 'datetime no status', datetime

						payload.start_date = datetime.date if payload.start_date?
						payload.start_time = datetime.time if payload.start_time?

					if payload.end_date? or payload.end_time?
						datetime = Util.buildDatetime payload.end_date, payload.end_time

						payload.end_date = datetime.date if payload.end_date?
						payload.end_time = datetime.time if payload.end_time?
				
				AppState.set 'mainContext', response

				rez = new Responder(response, action: 'audit')

				posted = rez.post()

				@log posted
)