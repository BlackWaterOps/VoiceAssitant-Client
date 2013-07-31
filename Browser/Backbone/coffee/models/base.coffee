define([
	'underscore',
	'backbone',
	'models/appState',
	'util'
], (_, Backbone, AppState, Util) ->
	class ModelBase extends Backbone.Model		
		debug: AppState.get('debug')

		setDebugData: (data) =>
			if @debug is true
				debugData = AppState.get 'debugData'
				_.extend(debugData, data)
				AppState.set 'debugData', debugData

		post: (attributes, options) =>
			options = { } if not options?

			options.contentType = 'application/x-www-form-urlencoded'
			options.type = 'POST'

			Backbone.Model.prototype.save.call(this, attributes, options)

		sync: (method, model, options) =>
			# Util.log 'sync', method, model, options

			original = Backbone.sync.previous || Backbone.sync

			@setDebugData(
				request: model.attributes, 
				endpoint: model.url
			)
							
			request = original.call(Backbone, method, model, options)
			
			request.done((response, textStatus, jqXHR) =>
				Util.log 'done', response
				@setDebugData(response: response, status: textStatus)
				@.trigger 'done', model, response, options
			).fail((jqXHR, textStatus, errorThrown) =>
				Util.log 'fail', textStatus
				@setDebugData(response: errorThrown, status: textStatus)
			)
		 
			return request
)