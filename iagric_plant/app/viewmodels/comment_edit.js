define(['plugins/router', 'knockout', 'jquery', 'logininfo', 'knockout.validation', 'jquery.querystring', 'utils', 'plugins/dialog'],
function (router, ko, $, loginInfo, kv, qs, utils, dialog) {
    var vmComment = function (commentId) {
        var self = this;
        self.commentId = ko.observable("");
        self.orderNum = ko.observable("");
        self.materialName = ko.observable("");
        self.linkMan = ko.observable("");
        self.linkPhone = ko.observable("");
        self.level = ko.observable("");
        self.content = ko.observable("");
        self.time = ko.observable("");
        self.reComment = ko.observable('').extend({
            maxLength: { params: 300, message: "回复最大长度为300个字符" },
            required: {
                params: true,
                message: "请填写回复信息!"
            }
        });
        self.selReComment = ko.observable(false);

        self.init = function (commentId) {
            self.commentId(commentId);
            var sendData = {
                commentId: commentId
            }
            $.ajax({
                type: "POST",
                url: "/Admin_Comment/Info",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                        return;
                    };
                    if (jsonResult.code == 1) {
                        self.orderNum(jsonResult.ObjModel.OrderNum);
                        self.materialName(jsonResult.ObjModel.MaterialFullName);
                        self.linkMan = ko.observable(jsonResult.ObjModel.LinkMan);
                        self.linkPhone = ko.observable(jsonResult.ObjModel.LinkPhone);
                        self.level = ko.observable(jsonResult.ObjModel.textLevel);
                        self.content = ko.observable(jsonResult.ObjModel.Content);
                        self.time = ko.observable(jsonResult.ObjModel.AddTime);
                    }
                }
            })
        }
        self.Add = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0) {
                var sendData = {
                    commentId: self.commentId(),
                    content: self.reComment()
                };
                $.ajax({
                    type: "POST",
                    url: "/Admin_Comment/Add",
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
    };
    vmComment.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    }
    vmComment.prototype.close = function () {
        dialog.close(this);
    }
    vmComment.show = function (commentId) {
        var vmObj = new vmComment();
        vmObj.init(commentId);
        return dialog.show(vmObj);
    };
    return vmComment;
});