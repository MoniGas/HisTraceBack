define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'jquery.querystring', 'utils', 'bootbox', 'plugins/dialog', 'jquery-ui', 'logininfo', 'bootstrap-datepicker'],
 function (router, ko, $, kv, qs, utils, bootbox, dialog, jqueryui, loginInfo, bdp) {
     var vmObj;
     var vmSetQcReportEdit = function () {
         var self = this;
         self.userName = ko.observable('').extend({
             maxLength: { params: 50, message: "登录名最大长度为50个字符" },
             required: {
                 params: true,
                 message: "请输入登录名！"
             }
         });
         self.pwd = ko.observable('').extend({
             maxLength: { params: 25, message: "密码最大长度为25个字符" },
             required: {
                 params: true,
                 message: "请输入密码！"
             }
         });
         self.batchModelsArray = [];
         self.selectedBatch = ko.observable();
         self.selbatchmodel = ko.observable(false);
         self.selectedBatch.subscribe(function () {
             if (self.selectedBatch()) {
                 self.selbatchmodel(false);
             }
             else {
                 self.selbatchmodel(true);
             }
         });
         self.dealerModelsArray = [];
         self.selectedDealer = ko.observable();
         self.selectedDealer.subscribe(function () {
             if (self.selectedDealer()) {
                 $("#DivSCDate").css({ "display": "" });
             }
             else {
                 $("#DivSCDate").css({ "display": "none" });
             }
         });
         self.pDate = ko.observable(utils.dateFormat(new Date(), 'yyyy-MM-dd'));
         self.scDate = ko.observable(false);
         self.cancel = function () {
             self.close(this);
         }
         self.Save = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0 && self.userName() != null && self.pwd() != null && self.userName() != "" && self.pwd() != "" && self.selectedBatch()) {
                 var batch = 0;
                 if (self.selectedBatch()) {
                     batch = self.selectedBatch();
                 }
                 var dealer = 0;
                 if (self.selectedDealer()) {
                     dealer = self.selectedDealer();
                 }
                 var sendData = {
                     userName: self.userName(),
                     pwd: self.pwd(),
                     batchNameID: batch,
                     dealerID: dealer,
                     scDate: self.pDate()
                 }
                 $.ajax({
                     type: "POST",
                     url: "/File/ActiveEWM",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (jsonResult) {
                         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                             return;
                         };
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                         self.close();
                     }
                 })
             }
             else {
                 if (self.pDate() == null || self.pDate() == "" || self.pDate() == "undefined") {
                     self.scDate(true);
                 }
                 else {
                     self.scDate(false);
                 }
                 if (!self.selectedBatch()) {
                     self.selbatchmodel(true);
                 }
                 self.errors.showAllMessages();
             }
         }
     }
     //获取批次模块
     var getBatchModules = function () {
         var sendData = {
         };
         var data;
         $.ajax({
             type: "POST",
             url: "/File/BatchList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                     return;
                 };
                 data = jsonResult.ObjList;
             }
         });
         return data;
     }
     var getDealerModules = function () {
         var sendData = {
         };
         var data;
         $.ajax({
             type: "POST",
             url: "/Public/GetDealer",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                     return;
                 };
                 data = jsonResult.ObjList;
             }
         });
         return data;
     }
     vmSetQcReportEdit.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
         /*************************************/
         $('#pDate').datepicker({
             language: 'cn',
             autoclose: true,
             todayHighlight: true
         });
     }
     vmSetQcReportEdit.prototype.close = function () {
         dialog.close(this);
     }
     vmSetQcReportEdit.show = function () {
         vmObj = new vmSetQcReportEdit();
         vmObj.batchModelsArray = getBatchModules();
         vmObj.dealerModelsArray = getDealerModules();
         $("#DivSCDate").css({ "display": "none" });
         return dialog.show(vmObj);
     };
     return vmSetQcReportEdit;
 });