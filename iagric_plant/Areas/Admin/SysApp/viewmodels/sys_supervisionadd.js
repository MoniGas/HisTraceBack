define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'jquery-ui'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, jqueryui) {
     var isValidateCity = false;
     var isValidateArea = false;
     var getProvinces = function () {
         var data;
         $.ajax({
             type: "POST",
             url: "/Public/GetSheng",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             async: false,
             success: function (jsonResult) {
                 data = jsonResult;
             }
         });
         return data;
     }
     var vmUser = function () {
         var self = this;
         self.companyName = ko.observable('').extend({
             maxLength: { params: 50, message: "名称最大长度为50个字符" },
             required: {
                 params: true,
                 message: "请输入单位名称!"
             }
         });
         self.centerAddress = ko.observable('').extend({
             maxLength: { params: 50, message: "编号最大长度为50个字符" },
             required: {
                 params: true,
                 message: "请输入单位地址!"
             }
         });
         self.linkMan = ko.observable('').extend({
             maxLength: { params: 25, message: "登录名最大长度为25个字符" },
             required: {
                 params: true,
                 message: "请输入联系人!"
             }
         });
         self.linkPhone = ko.observable('').extend({
             axLength: { params: 30, message: "联系电话最大长度为30个字符!" },
             required: { params: true, message: "请输入联系电话!" },
             pattern: { params: /(^[0-9]{3,4}\-[0-9]{7,8}\-[0-9]{3,4}$)|(^[0-9]{3,4}\-[0-9]{7,8}$)|(^[0-9]{7,8}\-[0-9]{3,4}$)|(^[0-9]{7,11}$)/, message: "联系电话格式不正确!" }
         });
         self.complaintPhone = ko.observable('').extend({
             maxLength: { params: 30, message: "投诉电话最大长度为30个字符!" },
//             required: { params: true, message: "请输入投诉电话!" },
             pattern: { params: /(^[0-9]{3,4}\-[0-9]{7,8}\-[0-9]{3,4}$)|(^[0-9]{3,4}\-[0-9]{7,8}$)|(^[0-9]{7,8}\-[0-9]{3,4}$)|(^[0-9]{7,11}$)/, message: "投诉电话格式不正确!" }
         });

         self.postalCode = ko.observable('').extend({
             maxLength: { params: 6, message: "邮编最大长度为6个字符" },
//             required: {
//                 params: true,
//                 message: "请输入邮编!"
//             },
             digit: {
                 params: true,
                 message: "邮编格式不正确！"
             }
         });

         self.email = ko.observable('').extend({
             maxLength: { params: 50, message: "邮箱最大长度为50个字符!" },
//             required: { params: true, message: "请输入邮箱!" },
             email: { params: true, message: "邮箱格式不正确!" }
         });

         self.webURL = ko.observable('').extend({
             maxLength: { params: 100, message: "网站最大长度为100个字符" }
//             required: {
//                 params: true,
//                 message: "请输入网站!"
//             }
         });
         self.province = ko.observable('');
         self.city = ko.observable('');
         self.area = ko.observable('');
         self.provinces = [];
         self.citys = ko.observableArray();
         self.areas = ko.observableArray();
         self.selDealerssq = ko.observable(false);
         self.province.subscribe(function () {
             var defaultItem = { AddressName: '暂无相应市', Address_ID: '-1' };
             self.city(undefined);
             if (!self.province()) {
                 //self.areas(undefined);
                 self.citys(defaultItem);
                 return;
             }
             var selectedCode = self.province().Address_ID;
             var sendData = {
                 pid: selectedCode,
                 level: 2
             };
             $.ajax({
                 type: "POST",
                 url: "/Public/GetSub",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 async: false,
                 data: JSON.stringify(sendData),
                 success: function (jsonResult) {
                     self.citys(jsonResult);
                     if (jsonResult[0] == undefined) {
                         isValidateCity = false;
                         isValidateArea = false;
                         self.selDealerssq(false);
                     }
                     else {
                         isValidateCity = true;
                         isValidateArea = true;
                         self.selDealerssq(true);
                     }
                 }
             });
         });
         self.city.subscribe(function () {
             var defaultItem = { AddressName: '暂无相应区/县', Address_ID: '-1' };
             self.area(undefined);
             if (!self.city()) {
                 self.areas(defaultItem);
                 return;
             }
             var selectedCode = self.city().Address_ID;
             var sendData = {
                 pid: selectedCode,
                 level: 3
             };
             $.ajax({
                 type: "POST",
                 url: "/Public/GetSub",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 async: false,
                 data: JSON.stringify(sendData),
                 success: function (jsonResult) {
                     self.areas(jsonResult);
                     if (jsonResult[0] == undefined) {
                         isValidateArea = false;
                         self.selDealerssq(false);
                     }
                     else {
                         isValidateArea = true;
                         self.selDealerssq(true);
                     }
                 }
             });
         });
         self.area.subscribe(function () {
             if (!self.area() || self.city() == undefined) {
                 self.selDealerssq(true);
                 return;
             }
             self.selDealerssq(false);
         });
         self.provinces = getProvinces();

         self.Save = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0) {
                 var sendData = {
                     companyName: self.companyName(),
                     province: self.province().Address_ID,
                     city: self.city().Address_ID,
                     area: self.area().Address_ID,
                     centerAddress: self.centerAddress(),
                     linkMan: self.linkMan(),
                     linkPhone: self.linkPhone(),
                     complaintPhone: self.complaintPhone(),
                     postalCode: self.postalCode(),
                     email: self.email(),
                     webURL: self.webURL()
                 }
                 $.ajax({
                     type: "POST",
                     url: "/SysSupervision/Add",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (jsonResult) {
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                             if (jsonResult.code == 1) {
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

     vmUser.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     vmUser.prototype.close = function () {
         dialog.close(this);
     }
     vmUser.show = function () {
         var vmObj = new vmUser();
         return dialog.show(vmObj);
     };
     return vmUser;
 });