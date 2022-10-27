define(['jquery'], function ($) {
    var request;

    request = {
        querystring: function (name) {
            var match;
            if (name === "") return "";
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            match = new RegExp("[\\?&]" + name + "=([^&#]*)").exec(window.location.href);
            return match === null ? "" : decodeURIComponent(match[1].replace(/\+/g, " "));
        }
    };
    return request;
});

