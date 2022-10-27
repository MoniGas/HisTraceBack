define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'jquery.fileDownload', 'jquery.querystring', "./codestatis_threshold"],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, jfd, qs, codestatis_threshold) {
     var moduleInfo = {
         moduleID: '11032',
         parentModuleID: '10000'
     }

     var addThreshold = function (data, event) {
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         codestatis_threshold.show();
         //        router.navigate('#operationType_Add');
         //        searchOperationType();
     }
     //生成码编号
     var materialName = ko.observable('');
     var codeAllCount = ko.observable('');
     var enterpriseName = ko.observable('');
     var newDate = new Date();
     var complementHistoryDate = function () {
         var date = new Date();
         var seperator1 = "-";
         var strYear = date.getFullYear();
         var strMonth = date.getMonth() + 1;
         var strDate = date.getDate() - 1;
         if (strMonth >= 1 && strMonth <= 9) {
             strMonth = "0" + strMonth;
         }
         if (strDate >= 0 && strDate <= 9) {
             strDate = "0" + strDate;
         }
         if (strMonth == 1) {
             strYear--;
             strMonth = 12;
         }
         var currentdate = strYear + seperator1 + strMonth + seperator1 + strDate;
         return currentdate;
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
     var beginDate = ko.observable(complementHistoryDate());
     var endDate = ko.observable(getNowFormatDate());
     //搜索
     var searchCode = function (data, event) {
         var list;
         if ($("#tab1").is(":hidden")) {
             list = GetDataKO(1, 1);
             UpdateData(list, 1);
         } else {
             list = GetDataKO(1, 0);
             UpdateData(list, 0);
         }
     };
     var onchangeData = function () {
         //         var list = GetDataKO(1);
         //         UpdateData(list);
         var list;
         if ($("#tab1").is(":hidden")) {
             list = GetDataKO(1, 1);
             UpdateData(list, 1);
         } else {
             list = GetDataKO(1, 0);
             UpdateData(list, 0);
         }
     }
     //套标/单品
     var ViewSuit = function (GuiGe) {
         GuiGe = ko.utils.unwrapObservable(GuiGe);
         if (GuiGe.length == 0 || GuiGe == 0) {
             return "---";
         } else {
             return GuiGe;
         }
     }
     var ViewType = function (Type) {
         Type = ko.utils.unwrapObservable(Type);
         if (Type == 1 || Type == 4) {
             return "礼盒码";
         } else if (Type == 3 || Type == 6) {
             return "产品码";
         } else if (Type == 5 || Type == 7) {
             return "箱码";
         } else if (Type == 9) {
             return "套标码";
         } else if (Type == 10) {
             return "农药码";
         }
     }
     //套/个
     var ViewNum = function (totalNum, type) {
         totalNum = ko.utils.unwrapObservable(totalNum);
         type = ko.utils.unwrapObservable(type);
         if (type == 2) {
             return totalNum + "套";
         } else {
             return totalNum + "个";
         }
     }
     //自定义绑定-分页控件
     ko.bindingHandlers.SetCodeStatisPagerBH = {
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
                         //                         var list = GetDataKO(num);
                         //                         UpdateData(list);
                         var list;
                         list = GetDataKO(num, 0);
                         UpdateData(list, 0);
                         this.pageSize = parseInt(ViewModel.VmCodeStatisC.pageSize());
                         this.totalCounts = parseInt(ViewModel.VmCodeStatisC.totalCounts());

                     }
                 }
             });
         }
     };
     //分页控件C
     ko.bindingHandlers.SetCodeStatisPagerBHC = {
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
                         //                         var list = GetDataKO(num);
                         //                         UpdateData(list);
                         var list;
                         list = GetDataKO(num, 1);
                         UpdateData(list, 1);
                         this.pageSize = parseInt(ViewModel.VmCodeStatis.pageSize());
                         this.totalCounts = parseInt(ViewModel.VmCodeStatis.totalCounts());
                     }
                 }
             });
         }
     };
     //日期格式化
     var ChangeDateFormat = function (val) {
         if (val != null) {
             var date = new Date(parseInt(val.replace("/Date(", "").replace(")/", ""), 10));
             //月份为0-11，所以+1，月份小于10时补个0
             var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
             var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
             return date.getFullYear() + "-" + month + "-" + currentDate;
         }
         return "";
     }

     //鼠标滑过
     var mouseoverFun = function (data, event) {
         var self = $(event.target).closest('tr');
         var ShowAll = self.find("button[eleflag='ShowAll']");
         ShowAll.css({ "display": "" });
     }
     //鼠标离开
     var mouseoutFun = function (data, event) {
         var self = $(event.target).closest('tr');
         var ShowAll = self.find("button[eleflag='ShowAll']");
         ShowAll.css({ "display": "none" });
     }

     //Tab页控制
     var TopSubPageClick = function (index, data, event) {
         if (index == 1) {
             $("#li1").addClass("active");
             $("#li0").removeClass("active");
         }
         else {
             $("#li0").addClass("active");
             $("#li1").removeClass("active");
         }
         //        getDataKO(1, index, "");
         var list = GetDataKO(1, index);
         UpdateData(list, index);
     }

     //ajax获取数据

     //分页、搜索时更新数据源
     var UpdateData = function (list, index) {
         if (index == 0) {
             ViewModel.VmCodeStatis.ObjList(list.ObjList());
             ViewModel.VmCodeStatis.pageSize(list.pageSize());
             ViewModel.VmCodeStatis.totalCounts(list.totalCounts());
             ViewModel.VmCodeStatis.pageIndex(list.pageIndex());
         } else {
             ViewModel.VmCodeStatisC.ObjList(list.ObjList());
             ViewModel.VmCodeStatisC.pageSize(list.pageSize());
             ViewModel.VmCodeStatisC.totalCounts(list.totalCounts());
             ViewModel.VmCodeStatisC.pageIndex(list.pageIndex());
         }
     }
     //ajax获取数据
     var GetData = function (pageIndex) {
         var sendData = {
             pageIndex: pageIndex,
             mlx: $("#mlx").val(),
             materialName: materialName(),
             beginDate: beginDate(),
             endDate: endDate()
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/CodeStatis/Index",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                     return;
                 };
                 data = jsonResult;
                 codeAllCount(jsonResult.ObjModel.TotalNum);
                 enterpriseName(jsonResult.ObjModel.EnterpriseName);
             },
             error: function (Error) { alert(Error); }
         })
         return data;
     }
     //ajax获取数据ContinueCode
     var GetContinueCodeData = function (pageIndex) {
         var sendData = {
             pageIndex: pageIndex,
             //mlx: $("#mlx").val(),
             //materialName: materialName(),
             beginDate: beginDate(),
             endDate: endDate()
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/CodeStatis/GetContinneCodeRecord",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                     return;
                 };
                 data = jsonResult;
                 codeAllCount(jsonResult.ObjModel.TotalNum);
                 enterpriseName(jsonResult.ObjModel.EnterpriseName);
             },
             error: function (Error) { alert(Error); }
         })
         debugger
         return data;
     }

     //把获取的ajax数据转化为ko
     var GetDataKO = function (pageIndex, index) {
         if (index == 1) {
             $("#tab2").show();
             $("#szfz").hide();
             $("#tab1").hide();
             $("#lab1").hide();
             $("#spa1").hide();
             $("#dis2").hide();
             $("#txt1").hide();
             $("#mlx").hide();
             $("#SetCodeStatisPager").hide();
             $("#SetCodeStatisPagerC").show();
             var list = km.fromJS(GetContinueCodeData(pageIndex));
         }
         else {
             $("#lab1").show();
             $("#spa1").show();
             $("#dis2").show();
             $("#txt1").show();
             $("#tab1").show();
             $("#mlx").show();
             $("#tab2").hide();
             $("#szfz").show();
             $("#SetCodeStatisPager").show();
             $("#SetCodeStatisPagerC").hide();
             var list = km.fromJS(GetData(pageIndex));
         }

         return list;
     }
     var ViewModel = {
         binding: function () {
             //初初化导航状态
             ViewModel.VmCodeStatisC = GetDataKO(1, 1);
             ViewModel.VmCodeStatis = GetDataKO(1, 0);
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
         VmCodeStatis: null,
         VmCodeStatisC: null,
         searchCode: searchCode,
         //     matype: matype,
         materialName: materialName,
         codeAllCount: codeAllCount,
         enterpriseName: enterpriseName,
         beginDate: beginDate,
         endDate: endDate,
         onchangeData: onchangeData,
         mouseoverFun: mouseoverFun,
         mouseoutFun: mouseoutFun,
         complementHistoryDate: complementHistoryDate,
         getNowFormatDate: getNowFormatDate,
         ViewType: ViewType,
         ViewSuit: ViewSuit,
         ViewNum: ViewNum,
         TopSubPageClick: TopSubPageClick,
         addThreshold: addThreshold
     }
     return ViewModel;
 });