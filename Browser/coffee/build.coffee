(
	appDir: '../'
	baseUrl: '../js'
	dir: '../build'
	modules: [

	]
	optimizeCss: 'standard'
	removeCombined: true
	paths:
		'underscore': 'lib/underscore.min'
		'backbone': 'lib/Backbone/backbone'
		'backbone.core': 'lib/Backbone/backbone.min'
		'backbone.nested': 'lib/Backbone/backbone.nested.min'
		'backbone.lawnchair': 'lib/Backbone/backbone.lawnchair'
		'lawnchair': 'lib/Lawnchair/lawn.clippings'
		'lawnchair.core': 'lib/Lawnchair/Lawnchair.min'
		'lawnchair.webkit': 'lib/Lawnchair/adapters/webkit-sqlite.min'
		'lawnchair.dom': 'lib/Lawnchair/adapters/dom.min'
		'lawnchair.query': 'lib/Lawnchair/plugins/query.min'
		'handlebars': 'lib/handlebars'
		'util': 'lib/utilities'
	shim:
		'underscore':
			exports: '_'
		
		'backbone.core':
			deps: ['jquery', 'underscore']
			exports: 'Backbone'
		
		'backbone.lawnchair': ['backbone.core']
		
		'backbone.nested': ['backbone.core']

		'lawnchair.core': 
			exports: 'Lawnchair'
		
		'lawnchair.webkit': ['lawnchair.core']
		'lawnchair.dom': ['lawnchair.core']
		'lawnchair.query': ['lawnchair.core']
		'handlebars':
			exports: 'Handlebars'
			
		'util': ['jquery']
)