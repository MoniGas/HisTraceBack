define(['plugins/router', 'knockout', 'jquery', 'kindeditor.zh-CN', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'jquery.querystring', 'bootbox'],
function (router, ko, $, kcn, kv, utils, loginInfo, dialog, jqueryui, uploadify, qs, bootbox) {
    var vmObj;
    var vmTeamAdd = function () {
        var self = this;
        self.TeamName = ko.observable('').extend({
            minLength: { params: 2, message: "名称最小长度为2个字符" },
            maxLength: { params: 50, message: "名称最大长度为50个字符" },
            required: {
                params: true,
                message: "请输入班组名称！"
            }
        });
        self.selTitle = ko.observable(false);
        self.Remark = ko.observable('');
        self.cancle = function () {
            self.close();
        }
        self.Register = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0) {
                var sendData = {
                    TeamName: self.TeamName(),
                    Remark: editorTeam.html()
                }
                $.ajax({
                    type: "POST",
                    url: "/Team/Add",
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
    vmTeamAdd.prototype.binding = function () {
         editorTeam = KindEditor.create("#txtInfos", {
            cssPath: '/lib/kindeditor/plugins/code/prettify.css',
            uploadJson: '/lib/kindeditor/upload_json.ashx',
            fileManagerJson: '/lib/kindeditor/file_manager_json.ashx',
            allowFileManager: true,
            afterCreate: function () { },
            afterBlur: function () { this.sync(); }
        });
        editorTeam.html(vmObj.Remark());
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    }
    vmTeamAdd.prototype.close = function (a) {
        dialog.close(this, a);
    }
    vmTeamAdd.prototype.closeOK = function (id) {
        dialog.close(this, id);
    }
    vmTeamAdd.show = function () {
        vmObj = new vmTeamAdd();
        return dialog.show(vmObj);
    };
    return vmTeamAdd;
});