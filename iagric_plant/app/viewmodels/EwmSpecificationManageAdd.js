define(['plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'logininfo', 'jquery-ui'],
 function (Dialog, Ko, $, Bdp, Kv, Utils, LoginInfo, JqueryUi) {

     var ViewModelSpecification = function () {
         var self = this;
         self.Value = Ko.observable('').extend({
             digit: {
                 params: true,
                 message: "请输入数字正整数！"
             },
             min: {
                 params: 1,
                 message: "规格最少为1！"
             },
             max: {
                 params: 99,
                 message: "规格最多为99！"
             },
             required: {
                 params: true,
                 message: "请输入规格数量!"
             }
         });
         self.selTitle = Ko.observable(false);
         self.GuiGe = Ko.observable('').extend({
             maxLength: { params: 50, message: "规格最大长度为50个字符" },
             required: { params: true, message: "请输入规格名称!" }
         });
         self.selguige = Ko.observable(false);
         self.reset = function () {
             self.Value('');
             self.GuiGe('');
         }

         self.Register = function (data, event) {
             var CurrentObj = $(event.target);
             CurrentObj.blur();
             self.errors = Ko.validation.group(self);
             if (self.errors().length <= 0) {
                 var sendData = {
                     Value: self.Value(),
                     GuiGe: self.GuiGe()
                 }

                 $.ajax({
                     type: "POST",
                     url: "/Admin_Specification/AddSave",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (jsonResult) {
                         if (LoginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                             return;
                         };
                         Dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
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

     ViewModelSpecification.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     ViewModelSpecification.prototype.close = function () {
         Dialog.close(this);
     }
     ViewModelSpecification.prototype.closeOK = function (Id) {
         Dialog.close(this, Id);
     }
     ViewModelSpecification.show = function () {
         var vmObj = new ViewModelSpecification();
         return Dialog.show(vmObj);
     };
     return ViewModelSpecification;
 });