define(['plugins/dialog', 'knockout','jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'logininfo', 'jquery-ui'],
 function (dialog, ko,$, bdp, kv, utils, loginInfo, jqueryui) {

     var vmGreenhouse = function () {
         var self = this;
         self.name = ko.observable('').extend({
             maxLength: { params: 50, message: "名称最大长度为50个字符" },
             required: {
                 params: true,
                 message: "请输入生产基地名称!"
             }
         });
         self.selTitle = ko.observable(false);
         self.reset = function () {
             self.name('');
         }

         self.Register = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0) {
                 var sendData = {
                     name: self.name()
                 }

                 $.ajax({
                     type: "POST",
                     url: "/Admin_Greenhouse/Add",
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
         };
     }

     vmGreenhouse.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     vmGreenhouse.prototype.close = function () {
         dialog.close(this);
     }
     vmGreenhouse.prototype.closeOK = function (id) {
         dialog.close(this, id);
     }
     vmGreenhouse.show = function () {
         var vmObj = new vmGreenhouse();
         return dialog.show(vmObj);
     };
     return vmGreenhouse;
 });