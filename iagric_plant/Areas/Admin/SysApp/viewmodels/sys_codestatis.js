define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'jquery.fileDownload', 'jquery.querystring'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, jfd, qs) {
     var moduleInfo = {
         moduleID: '11032',
         parentModuleID: '10000'
     }
     //生成码编号
     var eid = ko.observable();
     var materialName = ko.observable('');
     var codeAllCount = ko.observable('');
     var enterpriseName = ko.observable('');
     var newDate = new Date();
     var complementHistoryDate = function () {
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
         var list = GetDataKO(1);
         UpdateData(list);
     };
     var onchangeData = function () {
         var list = GetDataKO(1);
         UpdateData(list);
     }
     //套标/单品
     var ViewSuit = function (GuiGe) {
         GuiGe = ko.utils.unwrapObservable(GuiGe);
         if (GuiGe.length == 0) {
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
                     var list = GetDataKO(num);
                     UpdateData(list);
                     this.pageSize = parseInt(ViewModel.VmCodeStatis.pageSize());
                     this.totalCounts = parseInt(ViewModel.VmCodeStatis.totalCounts());
                 }
             }
         });
     }
 };
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
 //ajax获取数据

 //分页、搜索时更新数据源
 var UpdateData = function (list) {
     ViewModel.VmCodeStatis.ObjList(list.ObjList());
     ViewModel.VmCodeStatis.pageSize(list.pageSize());
     ViewModel.VmCodeStatis.totalCounts(list.totalCounts());
     ViewModel.VmCodeStatis.pageIndex(list.pageIndex());
 }
 //ajax获取数据
 var GetData = function (pageIndex) {
     var sendData = {
         eid: eid,
         pageIndex: pageIndex,
         mlx: $("#mlx").val(),
         materialName: materialName(),
         beginDate: beginDate(),
         endDate: endDate()
     }
     var data;
     $.ajax({
         type: "POST",
         url: "/SysCodeStatis/Index",
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

 //把获取的ajax数据转化为ko
 var GetDataKO = function (pageIndex) {
     var list = km.fromJS(GetData(pageIndex));
     return list;
 }
 var ViewModel = {
     binding: function () {
         //初初化导航状态
         loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
         eid = qs.querystring("eid");
         ViewModel.VmCodeStatis = GetDataKO(1);

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
     searchCode: searchCode,
     VmMaType: VmMaType,
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
     ViewNum: ViewNum
 }
 return ViewModel;
});