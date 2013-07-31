define([
	'underscore',
	'backbone',
	'models/base',
	'models/appState'
], (_, Backbone, ModelBase, AppState) ->
	class classifer extends ModelBase	
		urlRoot: 'http://casper.stremor-nli.appspot.com/'
		
		fetch: (query) ->
			console.log 'classifier fetch', query
			debug = AppState.get 'debug'

			Backbone.Model.prototype.fetch.call(this, query)

		parse: (response, options) ->
			# take response and set it to mainContext
			AppState.set 'mainContext', response

			return response
)