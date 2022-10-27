define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'jquery.fileDownload', './requestcodeewm', './PackageAndDownload'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, jfd, requestcodeewm, PackageAndDownload) {
     var moduleInfo = {
         moduleID: '23001',
         parentModuleID: '10001'
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

     var beginDate = ko.observable(utils.dateFormat(getNowFormatDate()));
     var endDate = ko.observable(utils.dateFormat(getNowFormatDate()));

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
                         var list = GetDataKO(num);
                         UpdateData(list);
                         this.pageSize = parseInt(ViewModel.VmGenerateCode.pageSize());
                         this.totalCounts = parseInt(ViewModel.VmGenerateCode.totalCounts());
                     }
                 }
             });
         }
     };

     //ajax获取数据
     var GetData = function (pageIndex) {
         var sendData = {
             pageIndex: pageIndex,
             bd: beginDate(),
             ed: endDate()
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Request/Index",
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
         ViewModel.VmGenerateCode.ObjList(list.ObjList());
         ViewModel.VmGenerateCode.pageSize(list.pageSize());
         ViewModel.VmGenerateCode.totalCounts(list.totalCounts());
         ViewModel.VmGenerateCode.pageIndex(list.pageIndex());
     }

     //把获取的ajax数据转化为ko
     var GetDataKO = function (pageIndex) {
         var list = km.fromJS(GetData(pageIndex));
         return list;
     }
     var init = true;
     var onchangeData = function () {
         if (init == false) {
             var list = GetDataKO(1);
             UpdateData(list);
         }
         init = false;
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
             return Specification;
         }
     }

     var ViewEwmEncryption = function (EwmEncryption) {
         EwmEncryption = ko.utils.unwrapObservable(EwmEncryption);
         if (EwmEncryption == null) {
             return "-";
         } else if (EwmEncryption == true) {
             return "是";
         } else if (EwmEncryption == false) {
             return "否";
         }
     }

     var ViewFileEncryption = function (FileEncryption) {
         FileEncryption = ko.utils.unwrapObservable(FileEncryption);
         if (FileEncryption == null) {
             return "-";
         } else if (FileEncryption == true) {
             return "是";
         } else if (FileEncryption == false) {
             return "否";
         }
     }

     var ViewImageCounterfeit = function (ImageCounterfeit) {
         ImageCounterfeit = ko.utils.unwrapObservable(ImageCounterfeit);
         if (ImageCounterfeit == null) {
             return "-";
         } else if (ImageCounterfeit == false) {
             return "数字防伪";
         } else if (ImageCounterfeit == true) {
             return "图像防伪";
         }
     }

     var ViewDownLoadNum = function (DownLoadNum) {
         DownLoadNum = ko.utils.unwrapObservable(DownLoadNum);
         if (DownLoadNum == null) {
             return "-";
         } else {
             return DownLoadNum + "次";
         }
     }

     // 查看产品二维码
     var ProductEwm = function (mId) {
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         var id = ko.utils.unwrapObservable(mId);
         requestcodeewm.show(id);
     }

     var Download = function (Id, Type) {
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         Id = ko.utils.unwrapObservable(Id);
         PackageAndDownload.show(Id, Type).then(function () {
             var list = GetDataKO(1);
             UpdateData(list);
             //self.close();
         });
     }

     var DownloadFile = function (RequestCodeId, downLoadURL) {
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         RequestCodeId = ko.utils.unwrapObservable(RequestCodeId);
         downLoadURL = ko.utils.unwrapObservable(downLoadURL);
         $.fileDownload('/Admin_Request/DownloadFile?RequestCodeId=' + RequestCodeId + '&downLoadURL=' + downLoadURL)
                  .done(function () { alert('下载成功!'); })
                       .fail(function () { alert('下载失败，请稍后重试!'); });

         var sendData = {
             RequestCodeId: RequestCodeId
         }
         $.ajax({
             type: "POST",
             url: "/Admin_Request/UpdateDownLoadNum",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                     return;
                 };
                 if (jsonResult.code == 1) {
                     var list = GetDataKO(1);
                     UpdateData(list);
                 }
             }
         })

     }

     var GetStatus = function (status) {
         status = ko.utils.unwrapObservable(status);
         switch (status) {
             case 1040000001:
                 return "等待审核"; // 原未审核
             case 1040000005:
                 return "可下载"; // 原生成完成
             case 1040000006:
                 return "等待审核"; // 原生成失败
             case 1040000008:
                 return "等待审核"; // 原审批通过，等待生成
             case 1040000010:
                 return "可下载"; // 原打包完成
             case 1040000011:
                 return "等待审核"; // 原打包失败
         }
     }

     var mouseoverFun = function (data, event) {
         // 查看产品二维码
         var btnEwm = $("#btnEwm_" + data.RequestCode_ID());
         //打包 
         var btnPackaging = $("#btnPackaging_" + data.RequestCode_ID());
         //下载
         var btnDownload = $("#btnDownload_" + data.RequestCode_ID());
         // 下载设置
         var btnDownloadSetting = $("#btnDownloadSetting_" + data.RequestCode_ID());
         // 激活追溯
         var btnSellCode = $("#btnSellCode_" + data.RequestCode_ID());

         btnEwm.css({ "display": "none" });
         btnPackaging.css({ "display": "none" });
         btnDownload.css({ "display": "none" });
         btnDownloadSetting.css({ "display": "none" });
         btnSellCode.css({ "display": "none" });

         switch (data.Status()) {
             case 1040000005: // 生成完成
                 btnEwm.css({ "display": "" });
                 btnPackaging.css({ "display": "" });
                 break;
             case 1040000006: // 生成失败
                 btnGenerate.css({ "display": "" });
                 break;
             case 1040000010: // 打包成功
                 btnEwm.css({ "display": "" });
                 btnDownload.css({ "display": "" });
                 btnDownloadSetting.css({ "display": "" });
                 if (data.RequestCount() > data.saleCount()) {
                     btnSellCode.css({ "display": "" });
                 }
                 break;
         }
     }

     var mouseoutFun = function (data, event) {
         // 查看产品二维码
         var btnEwm = $("#btnEwm_" + data.RequestCode_ID());
         //打包 
         var btnPackaging = $("#btnPackaging_" + data.RequestCode_ID());
         //下载
         var btnDownload = $("#btnDownload_" + data.RequestCode_ID());
         // 下载设置
         var btnDownloadSetting = $("#btnDownloadSetting_" + data.RequestCode_ID());
         // 激活追溯
         var btnSellCode = $("#btnSellCode_" + data.RequestCode_ID());

         btnEwm.css({ "display": "none" });
         btnPackaging.css({ "display": "none" });
         btnDownload.css({ "display": "none" });
         btnDownloadSetting.css({ "display": "none" });
         btnSellCode.css({ "display": "none" });
     }

     var navigateTo = function (routeName, RequestCodeId) {
         // 保存RequestCodeId，用于销售时获取默认的开始码和结束码
         SaveRequestCodeId(ko.utils.unwrapObservable(RequestCodeId));

         switch (routeName) {
             case 'guide1':
                 router.navigate('#' + routeName + '?pc=10001');
                 break;
             default:
                 router.navigate('#' + routeName);
         }
     }

     var SaveRequestCodeId = function (RequestCodeId) {
         var sendData = {
             RequestCodeId: RequestCodeId
         }
         $.ajax({
             type: "POST",
             url: "/Admin_Request/SaveRequestCodeId",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                     return;
                 };
             }
         })
     }

     var ViewModel = {
         binding: function () {
             //初初化导航状态
             //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);

             ViewModel.VmGenerateCode = GetDataKO(1);

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
         VmGenerateCode: null,
         beginDate: beginDate,
         endDate: endDate,
         onchangeData: onchangeData,
         ViewType: ViewType,
         ViewSpecification: ViewSpecification,
         ViewUnit: ViewUnit,
         GetStatus: GetStatus,
         ProductEwm: ProductEwm,
         DownloadFile: DownloadFile,
         mouseoverFun: mouseoverFun,
         mouseoutFun: mouseoutFun,
         Download: Download,
         ViewEwmEncryption: ViewEwmEncryption,
         ViewFileEncryption: ViewFileEncryption,
         ViewImageCounterfeit: ViewImageCounterfeit,
         ViewDownLoadNum: ViewDownLoadNum,
         loginInfo: loginInfo,
         navigateTo: navigateTo

     }
     return ViewModel;
 });