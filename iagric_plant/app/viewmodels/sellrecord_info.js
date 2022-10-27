define(['plugins/router', 'knockout', 'jquery', 'utils', 'jquery.querystring', 'knockout.mapping', 'plugins/dialog', 'jqPaginator', 'jquery-ui', 'logininfo', 'jquery.uploadify', 'jquery.fileDownload'],
 function (router, ko, $, utils, qs, km, dialog, jq, jqueryui, loginInfo, uploadify, jfd) {
     //ajax获取数据
     var vmObj;

     var getData = function (pageIndex, code) {
         var sendData = {
             id: vmObj.id(),
//             Ewm: code,
//             EwmTableIdArray: vmObj.EwmTableIdArray(),
             pageIndex: pageIndex
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/UsageRecordCount/GetRecordDetail",
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
//         self.EwmTableIdArray = ko.observable(EwmTableIdArray);

         self.pageSize = ko.observable();
         self.totalCounts = ko.observable();
         self.pageIndex = ko.observable();

         self.ObjList = ko.observableArray();

         self.ViewType = function (Type) {
             Type = ko.utils.unwrapObservable(Type);
             if (Type == 1) {
                 return "套标组码";
             } else if (Type == 2) {
                 return "套标子码";
             } else if (Type == 3) {
                 return "单品码";
             }
         }

         self.searchInfo = function () {
             var list = getDataKO(1, self.code());
             updateData(list);
         };
         self.init = function () {
             var list = getDataKO(1, "");
             updateData(list);
         }
         //下载
         self.export = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             $.fileDownload('/UsageRecordCount/ExportTxt?id=' + self.id())
                  .done(function () {
                      alert('下载成功');
                  }).fail(function () { alert('下载失败!'); });
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