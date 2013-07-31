define([
	'underscore',
	'backbone',
	'models/base',
	'models/appState'
], (_, Backbone, ModelBase, AppState) ->
	class responder extends ModelBase
		defaults:
			model: ''
			action: ''
			payload: { }

		urlRoot: 'http://rez.stremor-apier.appspot.com/v1/' 

		urlAction: null

		url: =>
			if @urlAction? then @urlRoot + @urlAction else @urlRoot

		initialize: (attributes, options) ->
			console.log 'responder init', attributes, options

			if options? and options.action?
				switch options.action
					when 'audit'
						@urlAction = options.action

					when 'actors'
						@urlAction = options.action + '/' + options.actor

		parse: (response, options) ->
			# take response and set it to responderContext
			if response? and response.status? and response.status isnt 'completed'
				AppState.set 'responderContext', response

			# clear out action so we don't get audit/actor confusion
			@urlAction is null if @urlAction?

			return response

)