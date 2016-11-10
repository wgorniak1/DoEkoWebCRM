$(function () {
    "use strict";
    $('.wg-panel-header-button').click(function (e) { event.stopPropagation(); });
});

jQuery.fn.fadeOutAndRemove = function (speed) {
    $(this).fadeOut(speed, function () {
        $(this).remove();
    })
}