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

		urlRoot: 'http://casper.stremor-nli.appspot.com/disambiguate/' 

		urlAction: null

		url: =>
			if @urlAction? then @urlRoot + @urlAction else @urlRoot

		initialize: (attributes, options) =>
			@urlAction = options.action if options? and options.action?
)