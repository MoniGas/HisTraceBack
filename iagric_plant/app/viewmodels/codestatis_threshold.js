define(['plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'logininfo', 'jquery-ui'],
 function (dialog, ko, $, bdp, kv, utils, loginInfo, jqueryui) {
     var vmThreshold = function () {
         var self = this;
         self.init = function () {
             //页面载入获取阀值
             $.ajax({
                 type: "Post",
                 url: "/CodeStatis/GetThreshold",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 async: false,
                 success: function (jsonResult) {

                     console.log(jsonResult.ObjModel.Threshold);
                     self.count(jsonResult.ObjModel.Threshold);
                 }
             })
         };
         self.count = ko.observable('').extend({
             required: {
                 params: true,
                 message: "请输入阀值!"
             },
             pattern: {
                 params: /^\d+[\.]?\d{0,2}$/g,
                 message: "请输入数字！"
             }
         });
         self.selTitle = ko.observable(false);
         self.type = ko.observable(0);

         self.reset = function () {
             self.type(0);
             self.count('');
         }

         self.Save = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0) {
                 var sendData = {
                     count: self.count()
                 }

                 $.ajax({
                     type: "POST",
                     url: "/CodeStatis/SetThreshold",
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
                                 self.closeOK(1);
                             }
                         }
                         }]);
                         self.close(1);
                     }
                 })
             } else {
                 self.errors.showAllMessages();
             }
         }
     }

     vmThreshold.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     vmThreshold.prototype.close = function () {
         dialog.close(this);
     }
     vmThreshold.prototype.closeOK = function (id) {
         dialog.close(this, id);
     }
     vmThreshold.show = function () {
         var vmObj = new vmThreshold();
         vmObj.init();
         return dialog.show(vmObj);
     };
     return vmThreshold;
 });