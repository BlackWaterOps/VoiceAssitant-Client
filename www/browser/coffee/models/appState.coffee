define([
	'underscore',
	'backbone',
	'util'
], (_, Backbone, Util) ->
	class appState extends Backbone.Model
		defaults:
			lat: 0.00
			lon: 0.00
			sendDeviceInfo: false
			requestStatus: ''
			inProgress: false
			mainContext: null # instance of Classifier Model
			responderContext: null # instance of Responder Model
			history: [ ]
			pos: history.length

	return new appState()
)