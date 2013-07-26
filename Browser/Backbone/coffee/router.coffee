define([
	'underscore',
	'backbone',
	'views/index'
], (_, Backbone, Home) ->
	
	appRouter = Backbone.Router.extend(
		routes:
			'': 'home'
		
		home: ->
			home = new Home(
				el: $('body')
			)

			home.render()
	)

	initialize = ->
		router = new appRouter()

		Backbone.history.start()

	 
	initialize: initialize
)