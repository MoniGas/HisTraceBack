define(['plugins/router', 'knockout', 'jquery', 'kindeditor.zh-CN', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui'],
function (router, ko, $, kcn, kv, utils, loginInfo, dialog, jqueryui) {
    var vmObj;
    var vmMaterialSpecAdd = function () {
        var self = this;
        self.Value = ko.observable('').extend({
            pattern: {
                params: /^\d+[\.]?\d{0,2}$/g,
                message: "必须是数字，并且最多两位小数！"
            },
            min: {
                params: 0.001,
                message: "规格重量最少为0.001！"
            },
//            max: {
//                params: 5000,
//                message: "规格最多为5000！"
//            },
            required: {
                params: true,
                message: "请输入规格数量!"
            }
        });
        self.selTitle = ko.observable(false);
        self.MaterialSpcificationName = ko.observable('').extend({
            minLength: { params: 1, message: "规格名称最小长度为1个字符" },
            maxLength: { params: 50, message: "规格名称最大长度为50个字符" },
            required: {
                params: true,
                message: "请输入规格名称！"
            }
        });
        self.MaterialSpcificationCode = ko.observable('').extend({
            required: {
                params: true,
                message: "请输入规格编码！"
            },
            pattern: {
                params: /^\d{3}$/,
                message: "请输入3位数字的规格编码！"
            }
        });
        self.selMaterialSpcificationName = ko.observable(false);
        self.selMaterialSpcificationCode = ko.observable(false);
        self.cancle = function () {
            self.close();
        }
        self.Register = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0) {
                var sendData = {
                    Value: self.Value(),
                    maSName: self.MaterialSpcificationName(),
                    maSCode: self.MaterialSpcificationCode()
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_MaterialSpcification/Add",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                            return;
                        };
                        dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                            if (jsonResult.code != 0) {
                                self.closeOK(jsonResult.ObjModel);
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
    vmMaterialSpecAdd.prototype.close = function (a) {
        //alert(this.province().code);
        dialog.close(this, a);
    }
    vmMaterialSpecAdd.prototype.closeOK = function (id) {
        dialog.close(this, id);
    }
    vmMaterialSpecAdd.show = function () {
        vmObj = new vmMaterialSpecAdd();
        return dialog.show(vmObj);
    };
    return vmMaterialSpecAdd;
});