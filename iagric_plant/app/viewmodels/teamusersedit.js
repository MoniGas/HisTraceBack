define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'jquery.querystring', 'utils', 'bootbox', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'logininfo'],
 function (router, ko, $, kv, qs, utils, bootbox, dialog, jqueryui, uploadify, loginInfo) {
     var vmObj;

     var vmTeamUsersEdit = function () {
         var self = this;
         self.id;
         self.userName = ko.observable().extend({
             maxLength: { params: 10, message: "名称最大长度为10个字符!" },
             required: { params: true, message: "请输入人员名称!" }
         });
         self.userPhone = ko.observable().extend({
             maxLength: { params: 30, message: "联系电话最大长度为30个字符!" },
//             required: { params: true, message: "请输入联系电话!" },
             pattern: { params: /(^[0-9]{3,4}\-[0-9]{7,8}\-[0-9]{3,4}$)|(^[0-9]{3,4}\-[0-9]{7,8}$)|(^[0-9]{7,8}\-[0-9]{3,4}$)|(^[0-9]{7,11}$)/, message: "电话格式不正确!" }
         });
         self.userNumber = ko.observable().extend({
             maxLength: { params: 30, message: "工牌号最大长度为20个字符!" },
             //             required: { params: true, message: "请输入联系电话!" },
             pattern: { params: /(^[A-Za-z0-9]+$)/, message: "工牌号格式不正确!" }
         });
//         self.userNumber = ko.observable('');
         self.init = function (id) {
             self.id = id;
             var sendData = {
                 id: self.id
             };
             $.ajax({
                 type: "POST",
                 url: "/TeamUsers/GetModel",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     self.userName(jsonResult.ObjModel.UserName);
                     self.userPhone(jsonResult.ObjModel.UserPhone);
                     self.userNumber(jsonResult.ObjModel.UserNumber);
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
                     userName: self.userName(),
                     userPhone: self.userPhone(),
                     userNumber:self.userNumber()
                 }
                 $.ajax({
                     type: "POST",
                     url: "/TeamUsers/Edit",
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
             } else {
                 self.errors.showAllMessages();
             }
         }
     }
     vmTeamUsersEdit.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     vmTeamUsersEdit.prototype.close = function () {
         dialog.close(this);
     }
     vmTeamUsersEdit.show = function (id) {
         vmObj = new vmTeamUsersEdit();
         vmObj.init(id);
         return dialog.show(vmObj);
     };
     return vmTeamUsersEdit;

 });