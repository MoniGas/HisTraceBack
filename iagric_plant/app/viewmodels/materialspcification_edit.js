define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'jquery.querystring', 'utils', 'bootbox', 'plugins/dialog', 'jquery-ui', 'logininfo'],
 function (router, ko, $, kv, qs, utils, bootbox, dialog, jqueryui, loginInfo) {
     var array = new Array();
     var vmObj;
     var vmMaterialSpecEdit = function (id) {
         var self = this;
         self.MaterialSpcificationName = ko.observable().extend({
             minLength: { params: 1, message: "规格最小长度为1个字符" },
             maxLength: { params: 50, message: "规格最大长度为50个字符" },
             required: {
                 params: true,
                 message: "请输入产品规格！"
             }
         });
         self.MaterialSpcificationCode = ko.observable('').extend({
             required: {
                 params: true,
                 message: "请输入规格编码！"
             },
             pattern: {
                 params: /^\d{3}$/,
                 message: "请输入3位数字的规格编码！"
             }
         });
         self.Value = ko.observable('').extend({
             pattern: {
                 params: /^\d+[\.]?\d{0,2}$/g,
                 message: "必须是数字，并且最多两位小数！"
             },
             min: {
                 params: 0.001,
                 message: "规格重量最少为0.001！"
             },
//             max: {
//                 params: 5000,
//                 message: "规格最多为5000！"
//             },
             required: {
                 params: true,
                 message: "请输入规格重量！"
             }
         });
         self.SelName = ko.observable(false);
         self.id = id;
         self.selMaterialSpcificationName = ko.observable(false);
         self.selMaterialSpcificationCode = ko.observable(false);

         self.init = function () {
             var sendData = {
                 id: self.id
             };
             $.ajax({
                 type: "POST",
                 url: "/Admin_MaterialSpcification/GetModel",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     //alert(jsonResult.code);
                     //var obj = JSON.parse(jsonResult);
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
                     self.Value(jsonResult.ObjModel.Value);
                     self.MaterialSpcificationName(jsonResult.ObjModel.MaterialSpcificationName);
                     self.MaterialSpcificationCode(jsonResult.ObjModel.MaterialSpcificationCode);
                 }
             });
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
                     Value: self.Value(),
                     maSName: self.MaterialSpcificationName(),
                     maSCode: self.MaterialSpcificationCode()
                 }
                 //alert(JSON.stringify(sendData));
                 $.ajax({
                     type: "POST",
                     url: "/Admin_MaterialSpcification/Edit",
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
                                 self.closeOK(sendData.maSName, sendData.maSCode);
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
     vmMaterialSpecEdit.prototype.close = function () {
         //alert(this.province().code);
         dialog.close(this, "", "", 1);
     }
     vmMaterialSpecEdit.prototype.closeOK = function (maSName, maSCode) {
         dialog.close(this, maSName, maSCode, 2);
     }
     vmMaterialSpecEdit.show = function (id) {
         vmObj = new vmMaterialSpecEdit(id);
         vmObj.init();
         return dialog.show(vmObj);
     };
     return vmMaterialSpecEdit;

 });