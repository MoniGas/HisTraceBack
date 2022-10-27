define(['plugins/router', 'knockout', 'jquery', 'utils', 'jquery.querystring', 'knockout.mapping', 'plugins/dialog', 'jqPaginator', 'jquery-ui','logininfo'],
 function (router, ko,$, utils, qs, km, dialog, jq, jqueryui,loginInfo) {
     //ajax获取数据
     var vmObj;
     var getData = function (pageIndex, code) {
         var sendData = {
             checkId: vmObj.id(),
             pageIndex: pageIndex
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Material_OnlineOrder/GetOrderList",
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
     var getDataKO = function (pageIndex, code) {
         var list = km.fromJS(getData(pageIndex, code));
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
     ko.bindingHandlers.sellinfoPager = {
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
                         var list = getDataKO(num);
                         updateData(list);
                         this.pageSize = parseInt(vmObj.pageSize());
                         this.totalCounts = parseInt(vmObj.totalCounts());
                     }
                 }
             });
         }
     };
     var vmSellInfo = function (id) {
         var self = this;
         self.code = ko.observable();
         self.id = ko.observable(id);

         self.pageSize = ko.observable();
         self.totalCounts = ko.observable();
         self.pageIndex = ko.observable();

         self.ObjList = ko.observableArray();

         self.searchInfo = function () {
             var list = getDataKO(1, self.code());
             updateData(list);
         };
         self.init = function () {
             var list = getDataKO(1, "");
             updateData(list);
         }
     }

     vmSellInfo.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     vmSellInfo.prototype.close = function () {
         dialog.close(this);
     }
     vmSellInfo.show = function (id) {
         vmObj = new vmSellInfo(id);
         vmObj.init();
         return dialog.show(vmObj);
     };

     return vmSellInfo;

 });