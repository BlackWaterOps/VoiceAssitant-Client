define([
	'underscore',
	'backbone',
	'models/appState',
	'util'
], (_, Backbone, AppState, Util) ->
	class ModelBase extends Backbone.Model		
		debug: AppState.get('debug')

		setDebugData: (data) =>
			# this has the potential for omitting needed properties 
			data.request = _.omit(data.request, _.keys(data.response)) 

			AppState.set('debugData', data) if @debug is true 

		post: (attributes, options) =>
			options = { } if not options?

			options.contentType = 'application/x-www-form-urlencoded'
			options.type = 'POST'

			Backbone.Model.prototype.save.call(this, attributes, options)

		sync: (method, model, options) =>
			# console.log 'sync', method, model, options

			original = Backbone.sync.previous || Backbone.sync
			
			request = original.call(Backbone, method, model, options)
		
			request.done((response, textStatus, jqXHR) =>
				# Util.log 'done', response
				@setDebugData(
					endpoint: (model.url() || model.urlRoot)
					request: (options.data || model.attributes)
					response: response
					status: textStatus
				)
				@.trigger 'done', model, response, options
			).fail((jqXHR, textStatus, errorThrown) =>
				Util.log 'fail', textStatus
				@setDebugData(
					endpoint: (model.url() || model.urlRoot)
					request: (options.data || model.attributes)
					response: errorThrown
					status: textStatus
				)
			)
		 
			return request
)