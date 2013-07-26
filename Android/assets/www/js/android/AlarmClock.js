function alarmClockPlugin() {}

alarmClockPlugin.prototype.addAlarm = function (date, message, skip, success, error) {
  if (!date instanceof Date) {
    console.log("alarmClockPlugin.add failure: date parameter must be a Date");
    return;
  }

  if (typeof error != "function") {
    console.log("alarmClockPlugin.addAlarm failure: error parameter must be a function");
    return;
  }

  if (typeof success != "function") {
    console.log("alarmClockPlugin.addAlarm failure: success parameter must be a function");
    return;
  }

  dateObj = {
    "hour": date.getHours(),
    "minutes": date.getMinutes()
  }

  var args = [{
    "date": dateObj,
    "message": message,
    "skip": skip
  }];

  console.log(JSON.stringify(args));

  cordova.exec(success, error, 'AlarmClockPlugin', 'add', args);
};

alarmClockPlugin.prototype.cancelAlarm = function (id, successCallback, errorCallback) {
    throw "Not yet implemented";
};

alarmClockPlugin.install = function() {
  if (!window.plugins) {
    window.plugins = {};
  }

  window.plugins.alarmClockPlugin = new alarmClockPlugin();
  return window.plugins.alarmClockPlugin;
};

cordova.addConstructor(alarmClockPlugin.install);