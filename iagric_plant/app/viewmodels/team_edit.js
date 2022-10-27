define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'jquery.querystring', 'utils', 'bootbox', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'logininfo'],
 function (router, ko, $, kv, qs, utils, bootbox, dialog, jqueryui, uploadify, loginInfo) {
     var vmObj;
     var vmTeamEdit = function (id) {
         var self = this;
         self.TeamName = ko.observable().extend({
             minLength: { params: 2, message: "名称最小长度为2个字符" },
             maxLength: { params: 100, message: "名称最大长度为100个字符" },
             required: {
                 params: true,
                 message: "请输入班组名称！"
             }
         });
         self.id = id;
         self.selTitle = ko.observable(false);
         self.Remark = ko.observable();
         self.init = function () {
             self.id = id;
             var sendData = {
                 id: self.id
             };
             $.ajax({
                 type: "POST",
                 url: "/Team/GetModel",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (jsonResult.code == 0) {
                         bootbox.alert({
                             title: "提示",
                             message: "获取数据失败！",
                             buttons: {
                                 ok: {
                                     label: '确定'
                                 }
                             }
                         });
                         return;
                     };
                     self.TeamName(jsonResult.ObjModel.TeamName);
                     self.Remark(jsonResult.ObjModel.Remark);
                 }
             })
         }
         self.cancel = function () {
             self.close(this);
         }
         self.Register = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0) {
                 var sendData = {
                     id: self.id,
                     TeamName: self.TeamName(),
                     Remark: editorTeam.html()
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Team/Edit",
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
                                 self.closeOK(sendData.TeamName, sendData.Remark);
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
     vmTeamEdit.prototype.binding = function () {
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
     vmTeamEdit.prototype.close = function () {
         dialog.close(this, "", "");
     }
     vmTeamEdit.prototype.closeOK = function (TeamName, Remark) {
         dialog.close(this, TeamName, Remark);
     }
     vmTeamEdit.show = function (id) {
         vmObj = new vmTeamEdit(id);
         vmObj.init(id);
         return dialog.show(vmObj);
     };
     return vmTeamEdit;
 });