define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq) {

     var vmUser = function () {
         var self = this;

         self.vmUserModels = {
             userModelsArray: ko.observableArray(),
             selectedOption: ko.observable()
         }

         //初始化活动动态模块数据
         self.vmUserModels.userModelsArray(getMaterial());
         self.EquipmentArray = ko.observableArray();
         self.selRoleName = ko.observable(false);
         var i = 0;
         self.vmUserModels.selectedOption.subscribe(function () {
             self.EquipmentArray(getUserModules(self.vmUserModels.selectedOption()));
         });

         self.userName = ko.observable('').extend({
             maxLength: { params: 50, message: "名称最大长度为50个字符" }
         });
         self.loginName = ko.observable('').extend({
             maxLength: { params: 50, message: "登录名最大长度为50个字符" },
             required: {
                 params: true,
                 message: "请输入登录名!"
             }
         });
         self.loginPass = ko.observable('').extend({
             minLength: { params: 6, message: "密码最小长度为6个字符" },
             maxLength: { params: 20, message: "密码最大长度为20个字符" },
             required: {
                 params: true,
                 message: "请输入登录密码!"
             }
         });

         self.reset = function () {
             vmUserModels.selectedOption(null);
             self.userName('');
             self.loginName('');
             self.loginPass('');
         }
         self.selTitle = ko.observable(false);
         self.flagC = ko.observable(false);
         var flag = false;
         self.VerifyLoginName = function () {
             var sendData = {
                 loginName: self.loginName()
             };
             $.ajax({
                 type: "POST",
                 url: "/Admin_Enterprise_User/VerifyLoginName",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                         flag = false;
                     };
                     if (jsonResult.code == '0') {
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {

                         }
                         }]);
                         flag = false;
                     } else if (jsonResult.code == '1') {
                         flag = true;
                     }
                 }
             })
         }

         self.Register = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0) {
                 self.VerifyLoginName();
                 if (!flag) {
                     return;
                 }
                 var sendData = {
                     diList: getEquipmentList(),
                     userName: self.userName(),
                     loginName: self.loginName(),
                     loginPass: self.loginPass()
                 };
                 $.ajax({
                     type: "POST",
                     url: "/Admin_Enterprise_User/AddSub",
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
                         }]);
                     }
                 })
             } else {
                 if (!self.vmUserModels.selectedOption()) {
                     self.selRoleName(true);
                 } else {
                     self.selRoleName(false);
                 }
                 self.errors.showAllMessages();
             }
         }
     }
     //获取设备字符串
     var getEquipmentList = function () {
         var arr = new Array();
         $("#equpmentLi").find("input[name='cbx']:checkbox").each(function () {
             if (this.checked == true) {
                 var EquipmentID = $(this).val();
                 if ($.inArray(EquipmentID, arr) == -1) {
                     arr.push(EquipmentID);
                 }
             }
         });
         return arr.join(",");
     }
     //获取产品
     var getMaterial = function () {
         var sendData = {
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Enterprise_User/GetMaterial",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 data = jsonResult.ObjList;
             }
         });
         return data;
     }
     //获取产品DI
     var getUserModules = function (id) {
         var sendData = {
             materialId: id
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Enterprise_User/GetSetDIList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 data = jsonResult.ObjList;
             }
         });
         return data;
     }
     //自定义绑定-复选框级联选择
     ko.bindingHandlers.checkBoxCascade = {
         init: function (element, valueAccessor, allBindingsAccessor) {
             $(element).on('click', "input[name='cbx']:checkbox", function (e) {
                 var status = true;
                 $(element).find("input[name='cbx']:checkbox").each(function () {
                     if (this.checked == false) {
                         status = false;
                         return false;
                     }
                 });
                 $(element).find("input[eleflag='allSelectBtn']").prop('checked', status);
             });
             $(element).on('click', "input[eleflag='allSelectBtn']", function (e) {
                 var obj = $(e.target);
                 $(element).find("input[name='cbx']:checkbox").each(function () {
                     $(this).prop('checked', obj.prop("checked"));
                 });
             });
         },
         update: function (element, valueAccessor, allBindingsAccessor) {

         }
     };
     vmUser.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     vmUser.prototype.close = function () {
         dialog.close(this);
     }
     vmUser.show = function () {
         var vmObj = new vmUser();
         vmObj.EquipmentArray(getUserModules(0));
         return dialog.show(vmObj);
     };
     return vmUser;
 });