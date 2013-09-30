function eventPlugin() {}

eventPlugin.prototype.addEvent = function(title, location, notes, startDate, endDate, successCallback, errorCallback) {
  if (typeof errorCallback != "function") {
    console.log("eventPlugin.addEvent failure: errorCallback parameter must be a function");
    return
  }

  if (typeof successCallback != "function") {
    console.log("eventPlugin.addEvent failure: successCallback parameter must be a function");
    return
  }

  cordova.exec(
  successCallback, // called when signature capture is successful
  errorCallback, // called when signature capture encounters an error
  'EventPlugin', // Tell cordova that we want to run "PushNotificationPlugin"
  'addEvent', // Tell the plugin the action we want to perform
  [{
    "title": title,
    "description": notes,
    "eventLocation": location,
    "startTimeMillis": startDate.getTime(),
    "endTimeMillis": endDate.getTime()
  }]); // List of arguments to the plugin
};
eventPlugin.prototype.deleteEvent = function(title, location, notes, startDate, endDate, deleteAll, successCallback, errorCallback) {
  throw "NotImplemented";
}

eventPlugin.prototype.findEvent = function(title, location, notes, startDate, endDate, successCallback, errorCallback) {
  throw "NotImplemented";
}

eventPlugin.prototype.modifyEvent = function(title, location, notes, startDate, endDate, newTitle, newLocation, newNotes, newStartDate, newEndDate, successCallback, errorCallback) {
  throw "NotImplemented";
}


eventPlugin.install = function() {
  if (!window.plugins) {
    window.plugins = {};
  }

  window.plugins.eventPlugin = new eventPlugin();
  return window.plugins.eventPlugin;
};


cordova.addConstructor(eventPlugin.install);