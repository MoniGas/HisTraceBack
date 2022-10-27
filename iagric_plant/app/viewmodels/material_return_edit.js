define(['plugins/router', 'knockout', 'jquery', 'logininfo', 'knockout.validation', 'jquery.querystring', 'utils', 'plugins/dialog'],
function (router, ko, $, loginInfo, kv, qs, utils, dialog) {
    var vmOrder = function (ExpressComp, ExpressNum) {
        var self = this;
        self.kdcomp = ko.observable(ExpressComp);
        self.kdnum = ko.observable(ExpressNum);
    };
    vmOrder.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    }
    vmOrder.prototype.close = function () {
        dialog.close(this);
    }
    vmOrder.show = function (ExpressComp, ExpressNum) {
        //alert(ExpressComp + ExpressNum);
        var vmObj = new vmOrder(ExpressComp, ExpressNum);
        return dialog.show(vmObj);
    };
    return vmOrder;
});