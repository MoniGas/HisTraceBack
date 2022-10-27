define(['plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'logininfo', 'jquery-ui'],
 function (dialog, ko, $, bdp, kv, utils, loginInfo, jqueryui) {

     var vmOperationType = function (id) {
         var self = this;
         self.name = ko.observable('').extend({
             maxLength: { params: 50, message: "名称最大长度为50个字符" },
             required: {
                 params: true,
                 message: "请输入生产环节名称!"
             }
         });
         self.selTitle = ko.observable(false);
         self.Memo = ko.observable('');

         self.init = function () {
             var sendData = {
                 id: id
             }
             $.ajax({
                 type: "POST",
                 url: "/Admin_OperationType/SearchData",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (jsonResult.code == 0) {
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {

                         }
                         }
                     ]);
                     } else if (jsonResult.code == 1) {
                         //                         self.vmModels.selectedOption(jsonResult.ObjModel.type);
                         self.Memo(jsonResult.ObjModel.Memo);
                         self.name(jsonResult.ObjModel.OperationTypeName);
                     }
                 }
             })
         }

         self.Register = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0) {
                 var sendData = {
                     id: id,
                     memo: self.Memo(),
                     name: self.name()
                 }
                 //alert(JSON.stringify(sendData));
                 $.ajax({
                     type: "POST",
                     url: "/Admin_OperationType/Edit",
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
                                 self.close();
                             }
                         }
                         }
                     ]);
                     }
                 })
             } else {
                 self.errors.showAllMessages();
             }
         }
     }

     vmOperationType.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     vmOperationType.prototype.close = function () {
         dialog.close(this);
     }
     vmOperationType.show = function (id) {
         var vmObj = new vmOperationType(id);
         vmObj.init();
         return dialog.show(vmObj);
     };
     return vmOperationType;
 });