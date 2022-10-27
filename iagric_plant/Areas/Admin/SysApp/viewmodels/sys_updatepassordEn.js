define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'utils', 'logininfo', 'knockout.validation'],
function (dialog, ko, jqueryui, km, utils, loginInfo, kv) {
    var moduleInfo = {
        moduleID: '11032',
        parentModuleID: '11030'
    }

    var vm = function (id, name, count) {
        var self = this;
        self.selsurepwd = ko.observable(false);
        self.eId = ko.observable(id);
        self.eName = ko.observable(name);
        self.surepwd = ko.observable('').extend({
            minLength: { params: 6, message: "密码最小长度为6个字符" },
            maxLength: { params: 50, message: "名称最大长度为20个字符" },
            required: {
                params: true,
                message: "请输入重置的密码！"
            }
        });

        self.save = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0) {
                var sendData = {
                    eId: self.eId(),
                    password: self.surepwd()
                }
                $.ajax({
                    type: "POST",
                    url: "/SysEnterpriseManage/SetPassWord",
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
    vm.show = function (id, name) {
        var vmObj = new vm(id, name);
        return dialog.show(vmObj);
    };

    return vm;
})