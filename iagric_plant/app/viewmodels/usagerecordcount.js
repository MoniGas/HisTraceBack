define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'jquery.fileDownload', './sellrecord_info'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, jfd, sellrecord_info) {
     var moduleInfo = {
         moduleID: '44000',
         parentModuleID: '40000'
     }

     var getNowFormatDate = function () {
         var date = new Date();
         var seperator1 = "-";
         var strYear = date.getFullYear();
         var strMonth = date.getMonth() + 1;
         var strDate = date.getDate();
         if (strMonth >= 1 && strMonth <= 9) {
             strMonth = "0" + strMonth;
         }
         if (strDate >= 0 && strDate <= 9) {
             strDate = "0" + strDate;
         }
         var currentdate = strYear + seperator1 + strMonth + seperator1 + strDate;
         return currentdate;
     }
     var beginDate = ko.observable(getNowFormatDate());
     var endDate = ko.observable(getNowFormatDate());
     //获取产品名称
     var GetMaterial = function (beginCode) {
         var beginCode = ko.utils.unwrapObservable(beginCode);
         var sendData = {
             beginCode: beginCode
         }

         var data;
         $.ajax({
             type: "POST",
             url: "/UsageRecordCount/GetMaterial",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                     return;
                 };
                 data = jsonResult.ObjModel.MaterialName;
             }
         });
         return data;
     }
     //筛选
     var search = function (data, event) {
         var list = GetDataKO(0,1);
         UpdateData(list);
     };
     //前7天
     var seven = function (data, event) {
         var list = GetDataKO(1, 1);
         UpdateData(list);
     };
     //前30天
     var lastMonth = function (data, event) {
         var list = GetDataKO(2, 1);
         UpdateData(list);
     };
     //前6个月
     var sixMonth = function (data, event) {
         var list = GetDataKO(3, 1);
         UpdateData(list);
     };
     //查看详情
//     var infoSellRecord = function (id, EwmTableIdArray, data, event) {
     var infoSellRecord = function (id, data, event) {
         var currentObj = $(event.target);
         currentObj.blur();

         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         id = ko.utils.unwrapObservable(id);
//         EwmTableIdArray = ko.utils.unwrapObservable(EwmTableIdArray);
         sellrecord_info.show(id);
     }
     //自定义绑定-分页控件
     ko.bindingHandlers.GenerateCodePagerBH = {
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
                         var list = GetDataKO(0,num);
                         UpdateData(list);
                         this.pageSize = parseInt(ViewModel.VmUsageRecord.pageSize());
                         this.totalCounts = parseInt(ViewModel.VmUsageRecord.totalCounts());
                     }
                 }
             });
         }
     };

     //ajax获取数据
     var GetData = function (index,pageIndex) {
         var sendData = {
             index: index,
             beginDate: beginDate(),
             endDate: endDate(),
             pageIndex: pageIndex
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/UsageRecordCount/Index",
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

     //分页、搜索时更新数据源
     var UpdateData = function (list) {
         ViewModel.VmUsageRecord.ObjList(list.ObjList());
         ViewModel.VmUsageRecord.pageSize(list.pageSize());
         ViewModel.VmUsageRecord.totalCounts(list.totalCounts());
         ViewModel.VmUsageRecord.pageIndex(list.pageIndex());
     }

     //把获取的ajax数据转化为ko
     var GetDataKO = function (index,pageIndex) {
         var list = km.fromJS(GetData(index,pageIndex));
         return list;
     }
     var init = true;
     var onchangeData = function () {
         if (init == false) {
             var list = GetDataKO(0,1);
             UpdateData(list);
         }
         init = false;
     }
     var mouseoverFun = function (data, event) {
         var self = $(event.target).closest('tr');

         var ShowAll = self.find("button[eleflag='ShowAll']");
         ShowAll.css({ "display": "" });
     }
     var mouseoutFun = function (data, event) {
         var self = $(event.target).closest('tr');

         var ShowAll = self.find("button[eleflag='ShowAll']");
         ShowAll.css({ "display": "none" });
     }
     var ViewModel = {
         binding: function () {
             //初初化导航状态
             //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);


             ViewModel.VmUsageRecord = GetDataKO(0,1);

             $('#date1').datepicker({
                 autoclose: true,
                 todayHighlight: true,
                 language: 'cn'
             });
             $('#date2').datepicker({
                 autoclose: true,
                 todayHighlight: true,
                 language: 'cn'
             });
         },
         goBack: function () {
             router.navigateBack();
         },
         VmUsageRecord: null,
         beginDate: beginDate,
         endDate: endDate,
         GetMaterial: GetMaterial,
         search: search,
         seven: seven,
         lastMonth: lastMonth,
         sixMonth: sixMonth,
         getNowFormatDate: getNowFormatDate,
         infoSellRecord: infoSellRecord,
         mouseoverFun: mouseoverFun,
         mouseoutFun: mouseoutFun,
         onchangeData: onchangeData
     }
     return ViewModel;
 });