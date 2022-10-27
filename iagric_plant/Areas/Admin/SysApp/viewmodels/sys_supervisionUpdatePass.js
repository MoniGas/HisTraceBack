define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'jquery-ui'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, jqueryui) {

    var vmUserPas = function (id) {

        var self = this;
        self.oldpwd = ko.observable('').extend({
            maxLength: { params: 50, message: "名称最大长度为50个字符" },
            required: {
                params: true,
                message: "请输入旧密码！"
            }
        });
        self.newpwd = ko.observable('').extend({
            minLength: { params: 6, message: "密码最小长度为6个字符" },
            maxLength: { params: 20, message: "名称最大长度为20个字符" },
            required: {
                params: true,
                message: "请输入新密码！"
            }
        });
        self.surepwd = ko.observable('').extend({
            minLength: { params: 6, message: "密码最小长度为6个字符" },
            maxLength: { params: 20, message: "名称最大长度为20个字符" },
            required: {
                params: true,
                message: "请确认新密码！"
            }
        });
        self.goBack = function () {
            router.navigateBack();
        }
        self.Save = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);

            if (self.errors().length <= 0) {
                var sendData = {
                    oldpwd: self.oldpwd(),
                    newpwd: self.newpwd(),
                    surepwd: self.surepwd()
                }

                $.ajax({
                    type: "POST",
                    url: "/SysSupervision/UpdatePass",
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
                                self.close();
                                location.href = "/Admin/";
                            }
                        }
                        }]);
                    }
                });
            } else {

                self.errors.showAllMessages();
            }
        }
    }
    vmUserPas.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    }
    vmUserPas.prototype.close = function () {
        //alert(this.province().code);
        dialog.close(this);
    }
    vmUserPas.show = function (id) {
        var vmObj = new vmUserPas(id);
        return dialog.show(vmObj);
    };
    return vmUserPas;
});