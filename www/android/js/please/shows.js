/**
 * Methods for displaying prompt and result information to the user
 */
cordova.define('please/shows', function(require, exports, module) {
  var list = function (response) {
    var data = response.show;
    var listContents = '<li>' + data.list.join('</li><li>') + '</li>';
    $('.listView ul').html(listContents);
    $('.listView p.prompt').text(data.text);

    $(document).one('sendQuery', function () {
      $('.listSlider').removeClass('active suspended');
    });

    setTimeout(function () {
        $('.listSlider').addClass('active');
    }, 500);
  };
  exports.list = list;

  var date = function () {
    $('.bubble.please:last .helperButtons').append('<a class="calendarLink">Show calendar</a>');

    $(document).one('sendQuery', function () {
      $('.calendarLink').remove();
    });
  };
  exports.date = date;

  var preformatted = function (response) {
    var template, source, html, item;
    var data = response.show;
    switch (data.template.toLowerCase()) {
      case "shopping":
        source = $('#shoppingTemplate').html();
      break;

      case "search":

      break;

      case "video":

      break;
    }

    template = Handlebars.compile(source);

    html = template(data);
    
    item = $('<div/>').addClass('preformatted').html(html).get(0).outerHTML; 

    say(response.speak, item);

  };
  exports.preformatted = preformatted;

  // couple of helper functions if needed when setting strings to attributes
  String.prototype.htmlEscape = function(str) {
    return String(str)
            .replace(/&/g, '&amp;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
  }

  String.prototype.htmlUnescape = function(str) {
    return String(value)
        .replace(/&quot;/g, '"')
        .replace(/&#39;/g, "'")
        .replace(/&lt;/g, '<')
        .replace(/&gt;/g, '>')
        .replace(/&amp;/g, '&');
  }
});
