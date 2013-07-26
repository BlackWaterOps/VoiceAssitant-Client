define([
	'underscore',
	'backbone',
	'models/appState'
], (_, Backbone, AppState) ->
	class responder extends Backbone.Model
		defaults:
			model: ''
			action: ''
			payload: { }

		urlRoot: 'http://clever.stremor-x.appspot.com/' 

		initialize: (attrs, options) ->
	
		parse: (response, options) ->
			# take response and set it to responderContext
			AppState.set 'responderContext', response

			return response

		###
		Backbone.lawnchair overrides Backbone.sync but saves a reference to 
		the original sync method at Backbone.sync.previous
		### 
		sync: Backbone.sync.previous
)