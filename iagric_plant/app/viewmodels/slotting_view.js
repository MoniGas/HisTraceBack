define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'bootbox'],
 function (router, dialog, ko, $, bdp, kv, utils, bootbox) {


     var getPath = function (ewm) {
         return "/ShowImage/ShowImg?ewm=" + ewm;
     }

     var getHref = function (ewm) {
         return "/ShowImage/DownEWMImg?ewm=" + ewm + "&size=300";
     }

     var getUrl = function (ewm) {
         var sendData = {
     }
     var data;
     $.ajax({
         type: "POST",
         url: "/Public/GetIdcodeURL?ewm="+ewm,
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

 var vmEwm = function (ewm) {
     var self = this;
     //         alert(ewm);
     self.ewm = ko.observable(ewm);
     self.path = ko.observable(getPath(ewm));
     self.href = ko.observable(getHref(ewm, 300));
     self.idcodeUrl = ko.observable(getUrl(ewm));

 }
 vmEwm.prototype.binding = function () {
     $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
 }
 vmEwm.prototype.close = function () {
     dialog.close(this);
 }
 vmEwm.show = function (ewm) {
     var vmObj = new vmEwm(ewm);
     return dialog.show(vmObj);
 };
 return vmEwm;
});