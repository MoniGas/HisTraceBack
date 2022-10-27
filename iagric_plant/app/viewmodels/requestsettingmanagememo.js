define(['plugins/router', 'jquery', 'knockout.validation', 'utils', 'plugins/dialog', 'knockout', 'jquery-ui', 'logininfo'],
function (router, $, kv, utils, dialog, ko, jqueryui, loginInfo) {
    var maxCount = 0;
    var type = 0;
    var vmBatchPart = function () {
        var self = this;
        self.SettingId = ko.observable(0);
        self.RequestId = ko.observable(0);
        self.MaterialName = ko.observable('');
        self.BatchName = ko.observable('');
        self.CodeCount = ko.observable('');
        self.Memo = ko.observable('');
        self.cancel = function () {
            self.close(this);
        }
        self.AddSetting = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0) {
                var sendData = {
                    subId: self.SettingId(),
                    memo: self.Memo()
                };
                $.ajax({
                    type: "POST",
                    url: "/Admin_RequestCodeSetting/AddEditMemo",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        if (jsonResult.code != 0) {
                            dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                        }
                        else {
                            dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { self.close(); } }]);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
            else {
                self.errors.showAllMessages();
            }
        }
        self.init = function (subId) {
            var sendData = {
                subId: subId
            };
            $.ajax({
                type: "POST",
                url: "/Admin_RequestCodeSetting/SettingMemo",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    if (jsonResult.code != 0) {
                        dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                    }
                    else {
                        self.MaterialName(jsonResult.ObjModel.materialName);
                        self.BatchName(jsonResult.ObjModel.zbatchName);
                        self.CodeCount(jsonResult.ObjModel.remaining);
                        self.RequestId(jsonResult.ObjModel.requestId);
                        type = jsonResult.ObjModel.type;
                        self.Memo(jsonResult.ObjModel.memo);
                    }
                },
                error: function (e) {
                }
            });
        }
    }
    vmBatchPart.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
        if (type == 9) {
            $("#lable1").text("套标数量：");
        }
        else {
            $("#lable1").text("码数量：");
        }
    }
    vmBatchPart.prototype.close = function () {
        dialog.close(this);
    }
    vmBatchPart.show = function (subId) {
        var vmObj = new vmBatchPart();
        vmObj.init(subId);
        vmObj.SettingId(subId);
        return dialog.show(vmObj);
    };
    return vmBatchPart;
});
