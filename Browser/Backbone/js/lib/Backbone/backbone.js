define([
	'backbone.core', 
	'backbone.nested'
	//'backbone.lawnchair'
], function(Backbone) {
	Backbone.View.prototype.parent = Backbone.Model.prototype.parent = Backbone.Collection.prototype.parent = function(attribute, options) {
  
		/**
		 *  Call this inside of the child initialize method.  If it's a view, it will extend events also.
		 *  this.parent('inherit', this.options);  <- A views params get set to this.options
		 */
		if(attribute == "inherit") {
	    	this.parent('initialize', options); // passes this.options to the parent initialize method
	    
	    	// extends child events with parent events
	    	if(this.events) { $.extend(this.events, this.parent('events')); this.delegateEvents(); }
	    
	    	return;
	  	}

		/**
		 *  Call other parent methods and attributes anywhere else.
		 *  this.parent('parentMethodOrOverriddenMethod', params) <- called anywhere or inside overridden method
		 *  this.parent'parentOrOverriddenAttribute') <- call anywhere
		 */		
		return (_.isFunction(this.constructor.__super__[attribute])) ?
	    this.constructor.__super__[attribute].apply(this, _.rest(arguments)) :
	    this.constructor.__super__[attribute];
	};
	
	return Backbone;
});