define(['plugins/router','jquery', 'knockout.validation', 'jquery.querystring', 'utils','logininfo', 'plugins/dialog', 'knockout', 'jquery-ui'],
 function (router,$, kv, qs, utils,loginInfo, dialog, ko, jqueryui) {
     var id = 0;
     var vmBatchExtEdit = function () {
         var self = this;
         self.id;
         self.bid;
         self.BatchExtName = ko.observable().extend({
             minLength: { params: 2, message: "名称最小长度为2个字符" },
             maxLength: { params: 100, message: "名称最大长度为100个字符" },
             required: {
                 params: true,
                 message: "请输入子批次号！"
             }
         });
         self.selTitle = ko.observable(false);
         self.init = function (id, bid) {
             self.id = id;
             self.bid = bid;
             var sendData = {
                 id: self.id,
                 bid: self.bid
             };
             $.ajax({
                 type: "POST",
                 url: "/Admin_BatchExt/GetModel",
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
                     self.BatchExtName(jsonResult.ObjModel.BatchExtName);
                 }
             })
         }
         self.Save = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0) {
                 var sendData = {
                     id: self.id,
                     bid: self.bid,
                     batchExtName: self.BatchExtName()
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Admin_BatchExt/Edit",
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
                                 self.closeOK(sendData.batchExtName);
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
     vmBatchExtEdit.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     vmBatchExtEdit.prototype.close = function () {
         dialog.close(this);
     }
     vmBatchExtEdit.prototype.closeOK = function (batchName) {
         dialog.close(this, batchName);
     }
     vmBatchExtEdit.show = function (id, bid) {
         var vmObj = new vmBatchExtEdit();
         vmObj.init(id, bid);
         return dialog.show(vmObj);
     };
     return vmBatchExtEdit;

 });