
define(['plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'logininfo', 'jquery-ui'],
 function (dialog, ko, $, bdp, kv, utils, loginInfo, jqueryui) {
 var data=ko.observable('')
     var vmOperationType = function (id) {
         var self = this;

         self.init = function (settingId) {
             var sendData = {
                 settingId: settingId
             }
             $.ajax({
                 type: "POST",
                 url: "/RequestCodeMa/PacketEwm",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                         return;
                     };
                     data = jsonResult;
                 }
             })
             return data;
         }

     }

     vmOperationType.prototype.binding = function () {
         $("#redPacketFrame").attr("src", data);
     }
     vmOperationType.prototype.close = function () {
         dialog.close(this);
     }
     vmOperationType.show = function (id) {
         var vmObj = new vmOperationType(id);
         data = vmObj.init(id).url;
         return dialog.show(vmObj);
     };
     return vmOperationType;
 });