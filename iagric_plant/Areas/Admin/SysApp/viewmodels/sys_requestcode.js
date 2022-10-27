define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'jquery.fileDownload', './sys_requestcodeewm', './sys_requestcodesales', './wait', './sys_requestcount'],
 function (router, dialog, ko, $, bdp, kv, utils, loginInfo, km, jq, jfd, requestcodeewm, requestcodesales, wait, sys_requestcount) {
     var moduleInfo = {
         moduleID: '11060',
         parentModuleID: '10000'
     }
     var levelId = 0;
     var mainCode = ko.observable('');
     var vmNewsModels = {
         newsModelsArray: ko.observableArray(),
         selectedSelOption: ko.observable()
     }

     var selAddProduct = ko.observable(false);
     var i = 0;

     var newDate = new Date();
     //     var beginDate = ko.observable(newDate.getFullYear() - 1 + "-" + newDate.getMonth() + "-" + newDate.getDate());
     //     var endDate = ko.observable(utils.dateFormat(new Date(), 'yyyy-MM-dd'));
     var complementHistoryDate = function () {
         var complDate = [];
         var curDate = new Date();
         var y = curDate.getFullYear();
         var m = curDate.getMonth() + 1;
         var d = curDate.getDate();
         if (m >= 1 && m <= 9) {
             m = "0" + m;
         }
         if (d >= 0 && d <= 9) {
             d = "0" + d;
         }
         //第一次装入当前的前一个月(格式yyyy-MM-dd)  
         complDate[0] = y + "-" + ((m - 1).toString().length == 1 ? "0" + (m - 1) : (m - 1)) + "-" + d;
         if (m == 1) {
             //到1月后,后推一年  
             y--;
             m = 12; //再从12月往后推  
             complDate[0] = y + "-" + (m.toString().length == 1 ? "0" + m : m) + "-" + d;
         }
         return complDate;
     }
     var beginDate = ko.observable(complementHistoryDate());
     var getNowFormatDate = function () {
         var date = new Date();
         var seperator1 = "-";
         var year = date.getFullYear();
         var month = date.getMonth() + 1;
         var strDate = date.getDate();
         if (month >= 1 && month <= 9) {
             month = "0" + month;
         }
         if (strDate >= 0 && strDate <= 9) {
             strDate = "0" + strDate;
         }
         var currentdate = year + seperator1 + month + seperator1 + strDate;
         return currentdate;
     }
     var endDate = ko.observable(getNowFormatDate());
     var addName = ko.observable('');
     var searchName = ko.observable('');
     var materialName = ko.observable('');

     var selNum = ko.observable(false);

     var selTitle = ko.observable(false);


     var searchRequest = function (data, event) {
         var list = getDataKO(1);
         updateData(list);
     };
     //分页、搜索时更新数据源
     var updateData = function (list) {
         vm.vmRequest.ObjList(list.ObjList());
         vm.vmRequest.pageSize(list.pageSize());
         vm.vmRequest.totalCounts(list.totalCounts());
         vm.vmRequest.pageIndex(list.pageIndex());
     }

     //把获取的ajax数据转化为ko
     var getDataKO = function (pageIndex) {
         var list = km.fromJS(getData(pageIndex));
         return list;
     }

     //ajax获取数据
     var getData = function (pageIndex) {
         var sendData = {
             pageIndex: pageIndex,
             bd: beginDate(),
             ed: endDate(),
             eName: searchName(),
             mName: materialName()
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/SysRequest/Index",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                     return;
                 };
                 data = jsonResult;
             },
             error: function (Error) {
                 alert(JSON.parse(Error));
             }
         })
         return data;
     }

     //获取活动动态模块
     var getNewsModules = function () {
         var sendData = {
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/SysRequest/SearchNameList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                     return;
                 };
                 data = jsonResult.ObjList;
             }
         });
         return data;
     }

     //自定义绑定-分页控件
     ko.bindingHandlers.requestPagerBH = {
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
                         this.pageSize = parseInt(vm.vmRequest.pageSize());
                         this.totalCounts = parseInt(vm.vmRequest.totalCounts());
                     }
                 }
             });
         }
     };
     // 查看产品二维码
     var productEwm = function (mId) {
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         var id = ko.utils.unwrapObservable(mId);
         requestcodeewm.show(id);
     }
     // 查看销售记录
     var salesRecord = function (mId) {
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         var id = ko.utils.unwrapObservable(mId);
         requestcodesales.show(id);
     }

     var wait = function (type) {
         if (ko.utils.unwrapObservable(type) == 1) {
             if (!$("#html1").isMasked()) {
                 $("#html1").mask({ spinner: { lines: 10, length: 5, width: 3, radius: 10 }, 'label': "加载中..." });
             }
         }
         else {

             if ($("#html1").isMasked()) {
                 $("#html1").unmask();
             }
         }
     }

     // 审核通过
     var audit = function (rId, type, eName, mName, rType, mSpec, applyCount) {
         a(rId, type, eName, mName, rType, mSpec, applyCount);
         wait(0);
     }

     var a = function (rId, type, eName, mName, rType, mSpec, applyCount) {
         if (ko.utils.unwrapObservable(type) == 3) {
             var rid = ko.utils.unwrapObservable(rId);
             var eName = ko.utils.unwrapObservable(eName);
             var mName = ko.utils.unwrapObservable(mName);
             var rType = ko.utils.unwrapObservable(rType);
             var mSpec = ko.utils.unwrapObservable(mSpec);
             var applyCount = ko.utils.unwrapObservable(applyCount);
             sys_requestcount.show(rid, eName, mName, rType, mSpec, applyCount).then(function () {
                 searchRequest(null, null);
             });
         }
         else {
             var sendData = {
                 rId: ko.utils.unwrapObservable(rId),
                 type: ko.utils.unwrapObservable(type)
             }
             var value;
             $.ajax({
                 type: "POST",
                 url: "/SysRequest/Audit",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                         return;
                     }
                     else {
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                             if (jsonResult.code != 0) {
                                 searchRequest(null, null);
                             }
                         }
                         }]);
                     }

                 }
             });
         }
     }

     var downloadFile = function (downLoadURL) {
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         var downLoadURL = ko.utils.unwrapObservable(downLoadURL);
         $.fileDownload('/SysRequest/DownloadFile?downLoadURL=' + downLoadURL)
                  .done(function () { alert('下载成功'); })
                       .fail(function () { alert('下载失败!'); });
     }

     var GetLevelId = function () {
         var sendData = {
         }
         $.ajax({
             type: "POST",
             url: "/SysRequest/GetLevelId",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (obj) {
                 levelId = obj.ObjModel.PRRU_PlatFormLevel_ID;
                 mainCode(obj.ObjModel.MainCode);
             }
         })
     }

     var getStatus = function (status) {
         var status = ko.utils.unwrapObservable(status);
         switch (status) {
             case 1040000001:
                 return "未审核";
             case 1040000005:
                 return "生成完成";
             case 1040000006:
                 return "生成失败";
             case 1040000008:
                 return "审批通过，等待生成";
             case 1040000010:
                 return "打包成功";
             case 1040000011:
                 return "打包失败";
         }
     }

     var visibleType = function (status, type) {
         var status = ko.utils.unwrapObservable(status);
         var type = ko.utils.unwrapObservable(type);
         var flag = false;
         switch (type) {
             case 1: // 查看产品二维码
                 if (status == 1040000005 || status == 1040000010 || status == 1040000011) {
                     flag = true;
                 }
                 return flag;
             case 2: //查看销售记录
                 if ((status == 1040000005 || status == 1040000010 || status == 1040000011) && levelId == 1) {
                     flag = true;
                 }
                 return flag;
             case 3: //打包 
                 if (status == 1040000005 || status == 1040000011) {
                     flag = true;
                 }
                 return flag;
             case 4: //下载
                 if (status == 1040000010) {
                     flag = true;
                 }
                 return flag;
             case 5: //审核通过
                 if (status == 1040000001 && levelId == 2) {
                     flag = true;
                 }
                 return flag;
             case 6: //手动生成
                 if ((status == 1040000008 || status == 1040000006) && levelId == 2) {
                     flag = true;
                 }
                 return flag;
         }
     }

     var mouseoverFun = function (data, event) {
         // 查看产品二维码
         var btnEwm = $("#btnEwm_" + data.RequestCode_ID());
         //查看销售记录
         var btnRecord = $("#btnRecord_" + data.RequestCode_ID());
         //打包 
         var btnPackaging = $("#btnPackaging_" + data.RequestCode_ID());
         //下载
         var btnDownload = $("#btnDownload_" + data.RequestCode_ID());
         //审核通过
         var btnAudit = $("#btnAudit_" + data.RequestCode_ID());
         //手动生成
         var btnGenerate = $("#btnGenerate_" + data.RequestCode_ID());

         btnEwm.css({ "display": "none" });
         btnRecord.css({ "display": "none" });
         btnPackaging.css({ "display": "none" });
         btnDownload.css({ "display": "none" });
         btnAudit.css({ "display": "none" });
         btnGenerate.css({ "display": "none" });

         switch (data.Status()) {
             case 1040000001: // 未审核
                 if (levelId == 2 || levelId == 3) {
                     btnAudit.css({ "display": "" });
                 }
                 break;
             //        case 1040000005: // 生成完成                                       
             //            btnEwm.css({ "display": "" });                                       
             //            if (levelId == 1) {                                       
             //                btnRecord.css({ "display": "" });                                       
             //            }                                       
             //            btnPackaging.css({ "display": "" });                                       
             //            break;                                       
             case 1040000006: // 生成失败
                 if (levelId == 2 || levelId == 3) {
                     btnGenerate.css({ "display": "" });
                 }
                 break;
             case 1040000008: // 审批通过，等待生成
                 if (levelId == 2 || levelId == 3) {
                     btnGenerate.css({ "display": "" });
                 }
                 break;
             //        case 1040000010: // 打包成功                                       
             //            btnEwm.css({ "display": "" });                                       
             //            if (levelId == 1) {                                       
             //                btnRecord.css({ "display": "" });                                       
             //            }                                       
             //            btnDownload.css({ "display": "" });                                       
             //            break;                                       
             //        case 1040000011: // 打包失败                                       
             //            btnEwm.css({ "display": "" });                                       
             //            if (levelId == 1) {                                       
             //                btnRecord.css({ "display": "" });                                       
             //            }                                       
             //            btnPackaging.css({ "display": "" });                                       
             //            break;                                       
         }
     }
     var mouseoutFun = function (data, event) {
         // 查看产品二维码
         var btnEwm = $("#btnEwm_" + data.RequestCode_ID());
         //查看销售记录
         var btnRecord = $("#btnRecord_" + data.RequestCode_ID());
         //打包 
         var btnPackaging = $("#btnPackaging_" + data.RequestCode_ID());
         //下载
         var btnDownload = $("#btnDownload_" + data.RequestCode_ID());
         //审核通过
         var btnAudit = $("#btnAudit_" + data.RequestCode_ID());
         //手动生成
         var btnGenerate = $("#btnGenerate_" + data.RequestCode_ID());

         btnEwm.css({ "display": "none" });
         btnRecord.css({ "display": "none" });
         btnPackaging.css({ "display": "none" });
         btnDownload.css({ "display": "none" });
         btnAudit.css({ "display": "none" });
         btnGenerate.css({ "display": "none" });
     }
     var onchangeEvent = function () {
         searchRequest();
     }

     var onchangeBeginData = function () {
         searchRequest();
     }

     var onchangeEndData = function () {
         searchRequest();
     }

     var ViewType = function (Type) {
         Type = ko.utils.unwrapObservable(Type);
         if (Type == 1) {
             return "套标产品码";
         } else if (Type == 2) {
             return "单品产品码";
         }
     }
     var ViewUnit = function (Type) {
         Type = ko.utils.unwrapObservable(Type);
         if (Type == 1) {
             return "套";
         } else if (Type == 2) {
             return "个";
         }
     }

     var ViewSpecification = function (Specification) {
         Specification = ko.utils.unwrapObservable(Specification);
         if (Specification == 0 || Specification == null) {
             return "-";
         } else {
             //return Specification + "瓶/箱";
             return Specification
         }
     }

     var vm = {
         binding: function () {
             //初始化活动动态模块数据
             vmNewsModels.newsModelsArray(getNewsModules());
             //初初化导航状态
             loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
             // 获取登录用户权限，1.企业用户 2.监管部门 3.平台商
             GetLevelId();
             vm.vmRequest = getDataKO(1);

             //限制文本框只能输入正整数
             $("#txtCount").keyup(function () {
                 //如果输入非数字，则替换为''，如果输入数字，则在每4位之后添加一个空格分隔
                 //                 this.value = this.value.replace(/[^\d]/g, '');
             })

             document.onkeydown = function (e) {
                 //            var theEvent = window.event || e;
                 //            var code = theEvent.keyCode || theEvent.which;
                 //            if (code == 13) {
                 //                $("#btnSearch").click();
                 //            }
             }
         },
         bindingComplete: function () {
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
         compositionComplete: function (view) {

         },
         goBack: function () {
             router.navigateBack();
         },
         vmRequest: null,
         vmNewsModels: vmNewsModels,
         beginDate: beginDate,
         endDate: endDate,
         searchRequest: searchRequest,
         searchName: searchName,
         materialName: materialName,
         addName: addName,
         productEwm: productEwm,
         salesRecord: salesRecord,
         audit: audit,
         downloadFile: downloadFile,
         visibleType: visibleType,
         getStatus: getStatus,
         mainCode: mainCode,
         mouseoverFun: mouseoverFun,
         mouseoutFun: mouseoutFun,
         onchangeEvent: onchangeEvent,
         onchangeBeginData: onchangeBeginData,
         onchangeEndData: onchangeEndData,
         selTitle: selTitle,
         selNum: selNum,
         wait: wait,
         selAddProduct: selAddProduct,
         ViewUnit: ViewUnit,
         ViewSpecification: ViewSpecification,
         ViewType: ViewType,
         complementHistoryDate: complementHistoryDate,
         getNowFormatDate: getNowFormatDate
     }
     return vm;
 });