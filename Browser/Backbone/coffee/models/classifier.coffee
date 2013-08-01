define([
	'underscore',
	'backbone',
	'models/base',
	'models/appState'
], (_, Backbone, ModelBase, AppState) ->
	class classifer extends ModelBase	
		urlRoot: 'http://casper-cached.stremor-nli.appspot.com/v1'

		parse: (response, options) ->
			# take response and set it to mainContext
			AppState.set 'mainContext', response

			return response
)