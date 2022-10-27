define(['plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'logininfo', 'jquery-ui'],
 function (dialog, ko, $, bdp, kv, utils, loginInfo, jqueryui) {
    var vmOperationType = function () {
        var self = this;
        self.name = ko.observable('').extend({
            maxLength: { params: 10, message: "名称最大长度为10个字符" },
            required: {
                params: true,
                message: "请输入生产环节名称!"
            }
        });
        self.selTitle = ko.observable(false);
        //self.type = ko.observable(0);
        self.Memo = ko.observable('');
        self.reset = function () {
            self.memo('');
            self.name('');
        }

        self.Save = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0) {
                var sendData = {
                    memo: self.Memo(),
                    name: self.name()
                }

                $.ajax({
                    type: "POST",
                    url: "/Admin_OperationType/Add",
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
                                self.closeOK(jsonResult.ObjModel);
                            }
                        }
                        }]);
                    }
                })
            } else {
                self.errors.showAllMessages();
            }
        }
    }

    vmOperationType.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    }
    vmOperationType.prototype.close = function () {
        dialog.close(this);
    }
    vmOperationType.prototype.closeOK = function (id) {
        dialog.close(this, id);
    }
    vmOperationType.show = function () {
        var vmObj = new vmOperationType();
        return dialog.show(vmObj);
    };
    return vmOperationType;
});