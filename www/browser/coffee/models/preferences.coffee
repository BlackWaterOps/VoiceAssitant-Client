define([
	'underscore',
	'backbone'
], (_, Backbone) ->
	preferences = Backbone.Model.extend(
	
		lawnchair: new Lawnchair(
			name: 'preferences'
			adapters: ['webkit-sqlite', 'dom']
		,() ->
			$(document).trigger 'dbLoaded-preferences'
		)
	)
)