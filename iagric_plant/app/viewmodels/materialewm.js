define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'bootbox', 'bootstrap-datepicker', 'jquery.querystring'],
 function (router, dialog, ko, $, bdp, kv, utils, bootbox, bdp, qs) {


     var getPath = function (ewm) {
         return "/ShowImage/ShowImgMaterial?ewm=" + ewm;
     }
     var vmEwm = function (ewm) {
         var self = this;
         self.ewm = ko.observable(ewm);
         var curDate = new Date();
         //获取前一天日期
         self.AddDate = ko.observable(utils.dateFormat(new Date(curDate.getTime() - 24 * 60 * 60 * 1000), 'yyyyMMdd').substring(2, 8));
         self.path = ko.observable(getPath(ewm));

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