define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'logininfo', 'jquery-ui'],
 function (router, dialog, ko, $, bdp, kv, utils, loginInfo, jqueryui) {
     var vmGreenhouse = function () {
         var self = this;
         self.selIsCheck = ko.observable(false);
         self.Account = ko.observable('').extend({
             maxLength: { params: 50, message: "最大长度为50个字！" },
             required: { params: true, message: "请输入企业支付宝账号！" }
         });
         self.selAccount = ko.observable(false);
         self.AccountName = ko.observable('').extend({
             maxLength: { params: 50, message: "最大长度为50个字！" },
             required: { params: true, message: "请输入企业支付宝账号户主姓名！" }
         });
         self.selAccountName = ko.observable(false);
         self.LinkPhone = ko.observable('').extend({
             maxLength: { params: 20, message: "最大长度为20个字！" },
             required: { params: true, message: "请输入下单通知手机！" }
         });
         self.opS = ko.observable();
         self.opE = ko.observable();
         self.init = function () {
             $.ajax({
                 type: "POST",
                 url: "/Admin_EnterpriseInfo/GetAccount",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 async: false,
                 success: function (jsonResult) {
                     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                         return;
                     };
                     if (jsonResult.ObjModel == null) {
                         self.Account('');
                         self.AccountName('');
                         self.LinkPhone('');
                     }
                     else {
                         self.Account(jsonResult.ObjModel.AccountNum);
                         self.AccountName(jsonResult.ObjModel.AccountName);
                         self.LinkPhone(jsonResult.ObjModel.LinkPhone);
                     }
                 }
             });
             $.ajax({
                 type: "POST",
                 url: "/Admin_EnterpriseInfo/Index",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 async: false,
                 success: function (jsonResult) {
                     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                         return;
                     };
                     var op = "";
                     var cl = "";
                     if (jsonResult.ObjModel.IsOpenShop == true) {
                         op = "开通商城";
                         cl = "关闭商城";
                         self.opS(op);
                         self.opE(cl);
                     }
                     else {
                         op = "关闭商城";
                         cl = "开通商城";
                         self.opS(op);
                         self.opE(cl);
                     }
                     //                     self.opS(jsonResult.ObjModel.Success);
                     //                     self.opE(jsonResult.ObjModel.Wrong);
                 }
             })
         }
         self.Register = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.opS() == "关闭商城") {
                 if (self.errors().length <= 0 && document.getElementById("ChkIsAgree").checked) {
                     var sendData = {
                         accountNum: self.Account(),
                         accountName: self.AccountName(),
                         linkPhone: self.LinkPhone()
                     };
                     $.ajax({
                         type: "POST",
                         url: "/Admin_EnterpriseInfo/OpenShop",
                         contentType: "application/json;charset=utf-8", //必须有
                         dataType: "json", //表示返回值类型，不必须
                         data: JSON.stringify(sendData),
                         async: false,
                         success: function (jsonResult) {
                             if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                                 return;
                             };
                             dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                                 self.init();
                             }
                             }]);
                         }
                     })
                 } else {
                     self.selIsCheck(!document.getElementById("ChkIsAgree").checked);
                     self.errors.showAllMessages();
                 }
             }
             else {
                 if (self.errors().length <= 0) {
                     var sendData = {
                         accountNum: self.Account(),
                         accountName: self.AccountName(),
                         linkPhone: self.LinkPhone()
                     };
                     $.ajax({
                         type: "POST",
                         url: "/Admin_EnterpriseInfo/OpenShop",
                         contentType: "application/json;charset=utf-8", //必须有
                         dataType: "json", //表示返回值类型，不必须
                         data: JSON.stringify(sendData),
                         async: false,
                         success: function (jsonResult) {
                             if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                                 return;
                             };
                             dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                                 self.init();
                             }
                             }]);
                         }
                     })
                 } else {
                     //self.selIsCheck(!document.getElementById("ChkIsAgree").checked);
                     self.errors.showAllMessages();
                 }
             }
         };
     }
     var vm = {
         activate: function () {
         },
         binding: function () {
             //初始化导航状态
             //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
             vm.vmGreenhouse = new vmGreenhouse();
             vm.vmGreenhouse.init();
             //             vm.vmGreenhouse.openStatusS();


         },
         vmGreenhouse: null
     }
     return vm;
 });