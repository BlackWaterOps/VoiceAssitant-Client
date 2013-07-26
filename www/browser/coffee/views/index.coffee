define([
	'underscore',
	'backbone',
	'util',
	'models/appState',
	'models/classifier',
	'models/responder',
	'handlebars'
], (_, Backbone, Util, AppState, Classifier, Responder) ->
	class IndexView extends Backbone.View
		events:
			'keyup #main-input': 'keyup'
			'webkitspeechchange #main-input': 'ask'

		initialize: () ->
			@board = $('#board')
			@loader = $('#loader')
			@disambiguator = 'http://casper.stremor-nli.appspot.com/disambiguate'

			@templates = 
            	bubblein: Handlebars.compile($('#bubblein-template').html())
            	bubbleout: Handlebars.compile($('#bubbleout-template').html())
            	simulate: Handlebars.compile($('#simulate-template').html())
            	shopping: Handlebars.compile($('#shopping-template').html())
            	images: Handlebars.compile($('#images-template').html())
            	list: Handlebars.compile($('#list-template').html())
            	web: Handlebars.compile($('#web-template').html())

            AppState.on 'change:mainContext', @resolver
            AppState.on 'change:requestStatus', @requestStatus

		render: (options) ->
			if (@board.is(':empty'))
				init = $('#init')
				init.fadeIn('slow')
				setTimeout (->
					init.fadeOut 'slow'
				), 1000

		show: (data) =>
			@board.append(@templates.bubbleout(data)).scrollTop(@board.height())
			@loader.hide()

		ask: (e) ->
        	input = $(e)

        	text = input.val()

        	input.val('')
        
        	@board.append @templates.bubblein(text)

	        if AppState.inProgress is true
	            console.log 'should disambiguate'
	            # @disambiguate text
	            dis = new Disambiguator(
	            	text: text
	            )
	        else
	            # data = query: text
	            # @requestHelper @classifier, "GET", data, @resolver

	            classifier = new Classifier()
	            classifier.fetch(data: query: text)

		keyup: (e) ->
			value = $(e.target).val()

			target = $(e.target)
			
			history = AppState.get 'history'

			switch e.which
				when 13
					if value
	            		@ask target
	            		# history.push value
						AppState.set 'pos', history.length
				when 38
					pos -= 1 if pos > 0

					target.val history[pos]

				when 40
					pos += 1 if pos < history.length
					target.val history[pos]
		
		disambiguate: (payload) =>
			if AppState.get('inProgress') is true
				endpoint = @disambiguator + '/active'
				context = AppState.get 'responderContext'
				field = context.field
				type = AppState.responderContext.type
				text = payload
				data = 
					text: text
					types: [type]
				console.log 'disambiguate user response', data
			else
				console.log 'disambiguate rez response'
				endpoint = @disambiguator + '/passive'
				context = AppState.get 'mainContext'

				field = payload.field
				# TODO: handle multi types
				type = payload.type
				text = context.get('payload')[field]
				data = 
					text: text
					types: [type]

			Util.requestHelper endpoint, 'POST', data, @disambiguateResults		

		disambiguateResults: (response) =>
			if response?
				if (response.date? and typeof response.date is 'object') or (response.time? and typeof response.time is 'object')
                	datetime = Util.buildDatetime(response.date, response.time)
                
                	response[type] = datetime[type]

                	console.log 'done handler', response

            context = AppState.get 'mainContext'

            payload =  context.get 'payload'

            payload[field] = response[type]

            context.set 'payload', payload

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
						console.log 'resolver disambiguate', response
						
						dis = new Disambiguator()
						
						# 'save' is misleading. It's really just a 'POST'
						# the model is being misused in a sense
						dis.save response
					when 'in progress'
						console.log 'resolver progress', response
						
						# store response so @disambiguate can get to it after @show
						@AppState.set 
							inProgress: true
							responderContext: response

						# display text to user and get response
						@show response
					when 'complete', 'completed'
						console.log 'resolver complete', response
						
						AppState.set 
							inProgress: false
							responderContext: { }

						@show response  
			else  
				console.log 'resolver response without status', response, AppState.get('mainContext')

				payload = response.payload

				if payload?
					if (payload.start_date? and typeof payload.start_date is 'object') or (payload.start_time? and typeof payload.start_time is 'object')
						datetime = @buildDatetime payload.start_date, payload.start_time

						payload.start_date = datetime.date if payload.start_date?
						payload.start_time = datetime.time if payload.start_time?

					if (payload.end_date? and typeof payload.end_date is 'object') or (payload.end_time? and typeof payload.end_time is 'object')
						datetime = @buildDatetime payload.end_date, payload.end_time

						payload.end_date = datetime.date if payload.end_date?
						payload.end_time = datetime.time if payload.end_time?
				
				# this needs to be an append and not an override
				AppState.set 'mainContext', response

				responder = new Responder()

				# 'save' is misleading. It's really just a 'POST'
				responder.save(response)

		requestStatus: (model, response, opts) =>
			if response is 'beforeSend' then @loader.show() else @loader.hide()
)