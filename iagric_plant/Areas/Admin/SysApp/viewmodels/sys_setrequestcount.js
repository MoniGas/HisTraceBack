define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'utils', 'logininfo', 'knockout.validation'],
function (dialog, ko, jqueryui, km, utils, loginInfo, kv) {
    var moduleInfo = {
        moduleID: '11032',
        parentModuleID: '11030'
    }

    var vm = function (id, name, count) {
        var self = this;
        self.selCount = ko.observable(false);
        self.eId = ko.observable(id);
        self.eName = ko.observable(name);
        self.count = ko.observable(count).extend({
            min: {
                params: 1,
                message: "数量最少为1！"
            },
            digit: {
                params: true,
                message: "申请数量为整数！"
            },
            required: {
                params: true,
                message: "请填写申请数量！"
            }
        });

        self.save = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0) {
                var sendData = {
                    eId: self.eId(),
                    count: self.count()
                }
                $.ajax({
                    type: "POST",
                    url: "/SysEnterpriseManage/SetRequestCount",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                            return;
                        };
                        dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                            if (jsonResult.code == 1) {
                                self.close();
                            }
                        }
                        }]);
                    }
                })
            }
            else {
                self.errors.showAllMessages();
            }
        }
    }

    vm.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    }
    vm.prototype.close = function () {
        dialog.close(this);
    }
    vm.show = function (id, name, count) {
        var vmObj = new vm(id, name, count);
        return dialog.show(vmObj);
    };

    return vm;
})