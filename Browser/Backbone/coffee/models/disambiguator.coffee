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

		url: (action) =>
			if @urlAction? then @urlRoot + @urlAction else @urlRoot

		initialize: (attributes, options) =>
			@urlAction = options.action if options? and options.action?

		# parse: (response) =>
		# 	Util.log 'successHandler', results
			
		# 	# move this to AppState
		# 	@checkDates = true

		# 	# what to do with this
		# 	@addDebug() if AppState.get('debug') is true and AppState.get('inProgress') is true
				
		# 	if response?
		# 		if (response.date? or response.time?)
		# 			datetime = Util.buildDatetime(response.date, response.time)
				
		# 			response[type] = datetime[type]

		# 			Util.log 'done handler', response

		# 	context = AppState.get 'mainContext'

		# 	payload =  context.payload

		# 	# 
		# 	payload[field] = response[type]

		# 	context.payload = payload

		# 	# this should trigger resolver!!
		# 	AppState.set 'mainContext', context

)