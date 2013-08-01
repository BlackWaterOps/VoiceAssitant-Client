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
			@board = @.$('#board')
			@loader = @.$('#loader')
			@input = @.$('#main-input')
			@form = @.$('#input-form')

			@checkDates = false
			AppState.on 'change:mainContext change:responderContext', @resolver

		render: (options) ->
			@input.focus()

			if (@board.is(':empty'))
				init = $('#init')
				init.fadeIn('slow')
				setTimeout (->
					init.fadeOut 'slow'
				), 1000

			@getLocation()

			@log
				results: 'results'
				data: 'somedata'
				object: 
					one: 'one'
					two: 'two'
					three: 'three'
			, 'string'
			, ['a','b','c','d']

		log: =>
			Util.log arguments

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

			@board.append(template(results)).scrollTop(@board.find('.bubble:last').offset().top)

			@addDebug()
				
			@loader.hide()

		cancel: (e) =>
			@board.empty()
			AppState.set
				mainContext: { }
				responderContext: { }
				history: [ ]

			@form.removeClass 'cancel'
			@loader.hide()
			@input.focus()

		addDebug: =>
			if AppState.get('debug') is true
				debugData = AppState.get('debugData')

				debugData.request = JSON.stringify(debugData.request, null, 4) if debugData.request?
				debugData.response = JSON.stringify(debugData.response, null, 4) if debugData.response?
				
				template = Handlebars.compile($('#debug-template').html())
				
				@board.find('.bubble:last').append(template(debugData))

		ask: (e) =>
			input = $(e)

			text = input.val()

			input.val('')
			
			template = Handlebars.compile($('#bubblein-template').html())
		
			@board.append(template(text)).scrollTop(@board.find('.bubble:last').offset().top)

			@form.addClass 'cancel'

			@loader.show()

			if AppState.get('inProgress') is true
				#@log 'should disambiguate'
				@disambiguate text
			else
				#@log AppState.get 'inProgress'

				classifier = new Classifier()

				classifier.on('done', @addDebug)

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
				type = context.type
				text = payload
				data = 
					text: text
					types: [type]
				#@log 'disambiguate user response', data
			else
				#@log 'disambiguate rez response'
				action = 'passive'
				context = AppState.get 'mainContext'

				field = payload.field
				type = payload.type
				text = context.payload[field]
				data = 
					text: text
					types: [type]
					device_info: Util.buildDeviceInfo()

			disambiguator = new Disambiguator(data, action: action)

			disambiguator.on('done', (model, response, options) =>
				#@log 'disambiguator done', response, field, type
				@disambiguateResults response, field, type
			)

			disambiguator.post()

		disambiguateResults: (response, field, type) =>
			#@log 'disambiguate successHandler', response, field, type
			
			@checkDates = true

			@addDebug() if AppState.get('inProgress') is true
				
			if response?
				if (response.date? or response.time?)
					datetime = Util.buildDatetime(response.date, response.time)
				
					response[type] = datetime[type]

					#@log 'done handler', response

			AppState.set 'mainContext.payload.' + field, response[type]

			# payload[field] = response[type]

			# this should trigger resolver!!
			# AppState.set 'mainContext', context

		resolver: (model, response, opts) =>
			###
			here is where we need to make checks of whether to pass along data
			to 'REZ' or resolve with the disambiguator
			###
			if response.status?
				switch response.status.toLowerCase()
					when 'disambiguate'
						#@log 'resolver disambiguate', response
						AppState.set 'inProgress', false

						@disambiguate response
					when 'in progress'
						#@log 'resolver progress', response
						
						# store response so @disambiguate can get to it after @show
						AppState.set 
							inProgress: true
							responderContext: response

						# display text to user and get response
						@show response
					when 'complete', 'completed'
						#@log 'resolver complete', response
						
						mc = AppState.get 'mainContext'
						rc = AppState.get 'responderContext'

						AppState.set 
							inProgress: false
							responderContext: { }
							mainContext: { }
						, silent: true	

						if not response.actor?
							@show response
						else
							disambiguator = new Responder(mc, action: 'actors', actor: rc.actor)

							disambiguator.on('done', (model, response, options) =>
								@show response
							)

							disambiguator.post()
			else  
				#@log 'resolver response without status', response, AppState.get('mainContext')

				payload = response.payload

				#TODO: put these dates and disambig response dates into one function
				if payload? and @checkDates
					if payload.start_date? or payload.start_time?
						datetime = Util.buildDatetime payload.start_date, payload.start_time

						#@log 'datetime no status', datetime

						payload.start_date = datetime.date if payload.start_date?
						payload.start_time = datetime.time if payload.start_time?

					if payload.end_date? or payload.end_time?
						datetime = Util.buildDatetime payload.end_date, payload.end_time

						payload.end_date = datetime.date if payload.end_date?
						payload.end_time = datetime.time if payload.end_time?
				

				AppState.set 'mainContext', response

				#rez = new Responder(response, action: 'audit')

				#posted = rez.post()

				#@log posted
)