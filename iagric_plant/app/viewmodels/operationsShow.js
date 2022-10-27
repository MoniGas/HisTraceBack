define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui', 'knockout.mapping'],
    function (router, ko, $, kv, utils, loginInfo, dialog, jqueryui, km) {
        var vmObj;
        var vmOperation = function (outProperty) {
            var self = this;
            self.propertybak = ko.observableArray(outProperty);
            self.selectprocessOps = ko.observableArray(outProperty);
        }
        vmOperation.prototype.btnUpEvent = function (data, event) {
            var index = 0;

            for (var i = 0; i < vmObj.selectprocessOps().length; i++) {
                if (data.opName == vmObj.selectprocessOps()[i].opName) {
                    index = i;
                }
            }
            if (index != 0) {
                var up = vmObj.selectprocessOps()[index - 1];
                vmObj.selectprocessOps()[index] = up;
                vmObj.selectprocessOps()[index - 1] = data;
            }
            vmObj.selectprocessOps(vmObj.selectprocessOps());
        }
        vmOperation.prototype.btnDownEvent = function (data, event) {
            var index = 0;
            for (var i = 0; i < vmObj.selectprocessOps().length; i++) {
                if (data.opName == vmObj.selectprocessOps()[i].opName) {
                    index = i;
                }
            }
            if (index < vmObj.selectprocessOps().length - 1) {
                var up = vmObj.selectprocessOps()[index + 1];
                vmObj.selectprocessOps()[index] = up;
                vmObj.selectprocessOps()[index + 1] = data;
            }
            vmObj.selectprocessOps(vmObj.selectprocessOps());
        }
        vmOperation.prototype.close = function () {
            dialog.close(this, vmObj.propertybak());
        }
        vmOperation.prototype.closeOK = function () {
            dialog.close(this, vmObj.selectprocessOps());
        }
        vmOperation.show = function (outProperty) {
            vmObj = new vmOperation(outProperty);
            return dialog.show(vmObj);
        };
        return vmOperation;
    });