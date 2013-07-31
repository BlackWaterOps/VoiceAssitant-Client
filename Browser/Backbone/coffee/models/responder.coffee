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

		url: (action) =>
			if @urlAction? then @urlRoot + @urlAction else @urlRoot

		initialize: (attributes, options) ->
			console.log 'responder init', attributes, options

			if options? and options.action?
				switch options.action
					when 'audit'
						@urlAction = options.action

					when 'actor'
						@urlAction = options.action + '/' + attributes.actor

		parse: (response, options) ->
			# take response and set it to responderContext
			AppState.set 'responderContext', response

			return response

)