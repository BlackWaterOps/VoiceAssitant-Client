define([
	'underscore',
	'backbone',
	'util'
], (_, Backbone, Util) ->
	class appState extends Backbone.NestedModel
		defaults:
			debug: false
			debugData: { }
			lat: 0.00
			lon: 0.00
			# sendDeviceInfo: false
			requestStatus: ''
			inProgress: false
			mainContext: { } 
			responderContext: { }
			history: [ ]
			pos: history.length

	return new appState()
)