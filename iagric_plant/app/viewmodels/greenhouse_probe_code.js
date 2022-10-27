define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'knockout.mapping'],
 function (router, dialog, ko, $, bdp, kv, utils, km) {

     var vmObj;

     var getPath = function (ewm) {
         return "/ShowImage/ShowImg?ewm=" + ewm;
     }

     var getHref = function (ewm) {
         return "/ShowImage/DownEWMImg?ewm=" + ewm + "&size=300";
     }

     var getData = function () {
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Greenhouses_Probe/ShowEwm",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             async: false,
             success: function (jsonResult) {
                 data = jsonResult;
             }
         })
         return data;
     }

     var vmEwm = function () {
         var self = this;
         self.ObjList = ko.observableArray();

         self.init = function () {
             var list = getDataKO(); 
             updateData(list);
         }
     }
     //把获取的ajax数据转化为ko
     var getDataKO = function () {
         var data = getData();
         var list = km.fromJS(data);
         return list;
     }
     //分页、搜索时更新数据源
     var updateData = function (list) {
         vmObj.ObjList(list.ObjList());
     }
     vmEwm.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     vmEwm.prototype.close = function () {
         dialog.close(this);
     }
     vmEwm.show = function () {
         vmObj = new vmEwm();
         vmObj.init();
         return dialog.show(vmObj);
     }
     return vmEwm;
 });