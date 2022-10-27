define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'utils', 'logininfo', 'jquery.querystring', 'kindeditor.zh-CN', 'plugins/dialog', 'jquery-ui'],
 function (router, ko, $, kv, utils, loginInfo, qs, kcn, dialog, jqueryui) {

     var vmShowActiveStatus = function () {
         var self = this;
         /******************黑名单码***********************/
         //         self.property = ko.observableArray([]);
         //         self.propertyName = ko.observable();


         /*****************************************/

         self.setDate = ko.observable('aaaa');
         self.LicenseCode = ko.observable('');
         self.init = function () {
             $.ajax({
                 type: "POST",
                 url: "/EnterpriseSwitch/GetIsActive",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 async: false,
                 success: function (jsonResult) {
                     if (jsonResult.code == "-1") {
                         $("#activeStatus").removeAttr("checked");
                         //  $("#activeStatus").attr("checked", "checked");
                     } else {
                         $("#activeStatus").attr("checked", "checked");
                     }
                 }
             });
         }
         self.GetLicense = function () {
             $.ajax({
                 type: "POST",
                 url: "/EnterpriseSwitch/GetLicense",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 async: false,
                 success: function (jsonResult) {
                     if (jsonResult.code == "1") {
                         self.LicenseCode(jsonResult.ObjModel.LicenseCode);
                         self.setDate(jsonResult.ObjModel.StrLicenseEndDate);
                     } else {
                     }
                 }
             });
         }
         self.SaveInfo = function (data, event) {

             $.ajax({
                 type: "POST",
                 url: "/EnterpriseSwitch/SwitchActive",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 async: false,
                 success: function (jsonResult) {
                     if (jsonResult.code == "1") {
                         //                         window.location.reload();
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                             //window.location.reload();
                         }
                         }]);
                     }
                 }
             });
         }
     }
     var vm = {
         activate: function () {
         },
         binding: function () {
             //初始化导航状态
             vm.vmShowActiveStatus = new vmShowActiveStatus();
             vm.vmShowActiveStatus.init();
             vm.vmShowActiveStatus.GetLicense();
         },
         vmShowActiveStatus: null
     }
     return vm;
 });