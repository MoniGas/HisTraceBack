define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'jquery.querystring', 'utils', 'bootbox', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'bootstrap-datepicker'],
 function (router, ko, $, kv, qs, utils, bootbox, dialog, jqueryui, uploadify, bdp) {

     var getPath = function (ewm, type) {
         return "/ShowImage/CollectShowImg?ewm=" + ewm + "&type=" + type;
     }

     var getHref = function (ewm,type) {
         return "/ShowImage/CollectDownEWMImg?ewm=" + ewm + "&size=300" + "&type=" + type;
     }

     var getUrl = function (ewm) {
         var sendData = {
     }
     var data;
     $.ajax({
         type: "POST",
         url: "/Admin_CollectCode/GetEwmURL",
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