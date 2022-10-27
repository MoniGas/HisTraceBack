define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'bootbox'],
 function (router, dialog, ko, $, bdp, kv, utils, bootbox) {


     var getPath = function (ewm, type) {
         return "/ShowImage/ShowImg?ewm=" + ewm+"&type="+type;
     }

     var getHref = function (ewm,type) {
         return "/ShowImage/DownEWMImg?ewm=" + ewm + "&size=300" + "&type=" + type;
     }

     var getUrl = function (ewm) {
         var sendData = {
     }
     var data;
     $.ajax({
         type: "POST",
         url: "/Public/GetIdcodeURL",
         contentType: "application/json;charset=utf-8", //必须有
         dataType: "json", //表示返回值类型，不必须
         data: JSON.stringify(sendData),
         async: false,
         success: function (jsonResult) {
             data = jsonResult + ewm;
         }
     })
     return data;
 }

 var vmEwm = function (ewm,type) {
     var self = this;
     //         alert(ewm);
     self.ewm = ko.observable(ewm);
     self.path = ko.observable(getPath(ewm, type));
     self.href = ko.observable(getHref(ewm,type, 300));
     self.idcodeUrl = ko.observable(getUrl(ewm));

     self.CheckVerify = function () {
         var verify = 0;
         var sendData = {}
         $.ajax({
             type: "POST",
             url: "/Login/GetEnterpriseVerify",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 verify = jsonResult.code;
             }
         });
         if (verify > 0) {
             return true;
         }
         else {
             return false;
         }
     }
 }
 vmEwm.prototype.binding = function () {
     $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
 }
 vmEwm.prototype.close = function () {
     dialog.close(this);
 }
 vmEwm.show = function (ewm,type) {
     var vmObj = new vmEwm(ewm,type);
     return dialog.show(vmObj);
 };
 return vmEwm;
});