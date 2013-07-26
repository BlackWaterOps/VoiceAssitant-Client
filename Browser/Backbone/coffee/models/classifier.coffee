define([
	'underscore',
	'backbone',
	'models/appState'
], (_, Backbone, AppState) ->
	class classifer extends Backbone.Model		
		urlRoot: 'http://casper-cached.stremor-nli.appspot.com/'

		# initialize: (attrs, options) ->
	
		parse: (response, options) ->
			# take response and set it to mainContext
			AppState.set 'mainContext', response

			return response

		###
		Backbone.lawnchair overrides Backbone.sync but saves a reference to 
		the original sync method at Backbone.sync.previous
		### 
		sync: Backbone.sync.previous

)