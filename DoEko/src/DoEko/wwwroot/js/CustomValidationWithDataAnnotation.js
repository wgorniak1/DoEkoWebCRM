$(function () {
    "use strict";

    $.validator.addMethod('nip',
        function (value, element, parameters) {
            "use strict";
            var nip = value;
            if (value.charAt(0) === "7") {
                return true;
            }
            else {
                return true;
            }
        });

    $.validator.unobtrusive.adapters.add('nip', [], function (options) {
        options.rules.nip = {};
        options.messages['nip'] = options.message;
        alert("adapters.add");
    });
});