define(['plugins/router', 'knockout', 'jquery', 'kindeditor.zh-CN', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui', 'jquery.querystring', 'bootbox'],
function (router, ko, $, kcn, kv, utils, loginInfo, dialog, jqueryui, qs, bootbox) {
    var vmObj;
    var vmRequestCount = function (id, eName, mName, rType, mSpec, applyCount) {
        var self = this;
//        self.selRepuestCount = ko.observable(false);
        self.id = ko.observable(id);
        self.eName = ko.observable(eName);
        self.mName = ko.observable(mName);
        self.rType = ko.observable(rType);
        self.mSpec = ko.observable(mSpec);
        self.applyCount = ko.observable(applyCount);
        var max = parseInt(self.applyCount());
        //        self.repuestCount = ko.observable('').extend({
        //            min: {
        //                params: 1,
        //                message: "数量最少为1！"
        //            },
        //            max: {
        //                params: max,
        //                message: "最多输入" + max + "！"
        //            },
        //            digit: {
        //                params: true,
        //                message: "审核数量为整数！"
        //            },
        //            required: {
        //                params: true,
        //                message: "请填写审核数量！"
        //            }
        //        });
        self.Register = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0) {
                //long rId, long requestCount
                var sendData = {
                    rId: self.id(),
                    requestCount: self.applyCount()
                }
                $.ajax({
                    type: "POST",
                    url: "/SysRequest/AuditNew",
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
    vmRequestCount.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    }
    vmRequestCount.prototype.close = function (a) {
        dialog.close(this, a);
    }
    vmRequestCount.show = function (id, eName, mName, rType, mSpec, applyCount) {
        vmObj = new vmRequestCount(id, eName, mName, rType, mSpec, applyCount);
        return dialog.show(vmObj);
    };
    return vmRequestCount;
});