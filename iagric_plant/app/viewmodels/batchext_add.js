define(['plugins/router', 'jquery', 'knockout.validation', 'utils','logininfo', 'plugins/dialog', 'knockout', 'jquery-ui'],
 function (router,$, kv, utils,loginInfo, dialog, ko, jqueryui) {
     var vmBatchExtAdd = function (id) {
         var self = this;
         self.BatchExtName = ko.observable('').extend({
             minLength: { params: 2, message: "名称最小长度为2个字符" },
             maxLength: { params: 50, message: "名称最大长度为50个字符" },
             required: {
                 params: true,
                 message: "请输入子批次号！"
             }
         });
         self.id = id;
         self.selTitle = ko.observable(false);
         self.calcel = function () {
             self.close(this);
         }
         self.Register = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0) {
                 var sendData = {
                     batchid: self.id,
                     batchExtName: self.BatchExtName()
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Admin_BatchExt/Add",
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
                                 self.closeOK(jsonResult);
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
     vmBatchExtAdd.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     vmBatchExtAdd.prototype.close = function () {
         dialog.close(this);
     }
     vmBatchExtAdd.prototype.closeOK = function (result) {
         dialog.close(this, result);
     }
     vmBatchExtAdd.show = function (id) {
         var vmObj = new vmBatchExtAdd(id);
         return dialog.show(vmObj);
     };
     return vmBatchExtAdd;

 });