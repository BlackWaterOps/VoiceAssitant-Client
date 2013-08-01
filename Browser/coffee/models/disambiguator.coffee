define([
	'underscore',
	'backbone',
	'models/base',
	'models/appState',
	'util'
], (_, Backbone, ModelBase, AppState, Util) ->
	class disambiguator extends ModelBase
		defaults:
			text: ''
			types: [ ]

		urlRoot: 'http://casper-cached.stremor-nli.appspot.com/v1/disambiguate/' 

		urlAction: null

		url: =>
			if @urlAction? then @urlRoot + @urlAction else @urlRoot

		initialize: (attributes, options) =>
			@urlAction = options.action if options? and options.action?
)