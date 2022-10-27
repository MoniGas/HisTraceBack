define(['plugins/router', 'knockout', 'jquery', 'utils', 'jquery.querystring', 'knockout.mapping', 'plugins/dialog', 'jqPaginator', 'jquery-ui', 'logininfo', 'bootstrap-datepicker'],
 function (router, ko, $, utils, qs, km, dialog, jq, jqueryui, loginInfo, bdp) {
     //ajax获取数据
     var vmObj;

     var getData = function (pageIndex, bbDate, eeDate) {
         var sendData = {
             gpId: vmObj.id(),
             sDate: bbDate,
             eeDate: eeDate,
             pageIndex: pageIndex
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Greenhouses_Probe/Data",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                     return;
                 };
                 data = jsonResult;
             }
         });
         return data;
     }
     //把获取的ajax数据转化为ko
     var getDataKO = function (pageIndex, bbDate, eeDate) {
         var data = getData(pageIndex, bbDate, eeDate);
         var list = km.fromJS(data);
         return list;
     }
     //分页、搜索时更新数据源
     var updateData = function (list) {
         vmObj.ObjList(list.ObjList());
         vmObj.pageSize(list.pageSize());
         vmObj.totalCounts(list.totalCounts());
         vmObj.pageIndex(list.pageIndex());
     }
     //自定义绑定-分页控件
     ko.bindingHandlers.prodedataPager = {
         init: function (element, valueAccessor, allBindingsAccessor) {
         },
         update: function (element, valueAccessor, allBindingsAccessor) {
             var value = valueAccessor();
             var allBindings = allBindingsAccessor();
             var pageSize = parseInt(ko.utils.unwrapObservable(value));
             var totalCounts = parseInt(ko.utils.unwrapObservable(allBindings.totalCounts));
             var pageIndex = parseInt(ko.utils.unwrapObservable(allBindings.pageIndex));

             $(element).jqPaginator({
                 totalCounts: totalCounts,
                 pageSize: pageSize,
                 visiblePages: 10,
                 currentPage: pageIndex,
                 first: '<li class="first"><a href="javascript:;">首页</a></li>',
                 last: '<li class="last"><a href="javascript:;">尾页</a></li>',
                 prev: '<li class="prev"><a href="javascript:;">上一页</a></li>',
                 next: '<li class="next"><a href="javascript:;">下一页</a></li>',
                 page: '<li class="page"><a href="javascript:;">{{page}}</a></li>',
                 onPageChange: function (num, type) {
                     if (type == 'change') {
                         var list = getDataKO(num, vmObj.bbDate(), vmObj.eeDate());
                         updateData(list);
                         this.pageSize = parseInt(vmObj.pageSize());
                         this.totalCounts = parseInt(vmObj.totalCounts());
                     }
                 }
             });
         }
     };
     var vmProdeData = function (id) {
         var self = this;
         self.id = ko.observable(id);

         self.bbDate = ko.observable();
         self.eeDate = ko.observable();

         self.pageSize = ko.observable();
         self.totalCounts = ko.observable();
         self.pageIndex = ko.observable();

         self.ObjList = ko.observableArray();

         self.searchInfo = function () {
             var list = getDataKO(1, self.bbDate(), self.eeDate());
             updateData(list);
         };
         self.init = function () {
             var list = getDataKO(1, "", "");
             updateData(list);
         }
     }

     vmProdeData.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });

         $('#bbDate').datepicker({
             language: 'cn',
             autoclose: true,
             todayHighlight: true
         });
         $('#eeDate').datepicker({
             language: 'cn',
             autoclose: true,
             todayHighlight: true
         });
     }
     vmProdeData.prototype.close = function () {
         dialog.close(this);
     }
     vmProdeData.show = function (id) {
         vmObj = new vmProdeData(id);
         vmObj.init();
         return dialog.show(vmObj);
     };

     return vmProdeData;

 });