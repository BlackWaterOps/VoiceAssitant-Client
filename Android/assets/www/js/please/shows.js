/**
 * Methods for displaying prompt and result information to the user
 */
cordova.define('please/shows', function(require, exports, module) {
  var list = function (data) {
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
});
