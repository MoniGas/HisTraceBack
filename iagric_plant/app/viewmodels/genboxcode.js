define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'jquery.fileDownload'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, jfd) {
     var moduleInfo = {
         moduleID: '32000',
         parentModuleID: '10001'
     }
     var SelType = ko.observable(false);
     var SelTypeModel = {
         SelTypeArray: ko.observable(),
         SelectedId: ko.observable()
     }
     var VmMaterial = {
         MaterialArray: ko.observableArray(),
         SelectedId: ko.observable()
     }
     VmMaterial.SelectedId.subscribe(function () {
         if (VmMaterial.SelectedId()) {
             SelMaterial(false);
             $.post("/Admin_Material/Info", { id: VmMaterial.SelectedId() }, function (jsonResult) {
                 //                 if (jsonResult.ObjModel.PackCount != 0 && jsonResult.ObjModel.PackCount != null) {
                 //                     PackNumber(jsonResult.ObjModel.PackCount);
                 //                 }
                 //                 else {
                 //                     PackNumber('');
                 //                 }
             });
         } else {
             SelMaterial(true);
         }
     });
     //产品型号
     //     var SpecificationsModel = {
     //         SpecificationsArray: ko.observableArray(),
     //         SelectedId: ko.observable()
     //     }
     var searchName = ko.observable('');
     //数量
     var Number = ko.observable('').extend({
         min: {
             params: 1,
             message: "数量最少为1！"
         },
         max: {
             params: 100000,
             message: "最多输入100000！"
         },
         digit: {
             params: true,
             message: "生成数量为整数！"
         },
         required: {
             params: true,
             message: "请填写生成数量！"
         }
     });
     //     //计划装箱/装盒数量
     //     var PackNumber = ko.observable('').extend({
     //         min: {
     //             params: 1,
     //             message: "数量最少为1！"
     //         },
     //         max: {
     //             params: 100,
     //             message: "最多输入100！"
     //         },
     //         digit: {
     //             params: true,
     //             message: "包装数量为整数！"
     //         }
     //     });
     //20190114生成包材码加上获取企业简码按钮
     var TraceEnMainCode = ko.observable('');
     //获取企业信息
     var GetEnterpriseModel = function () {
         var sendData = {
         }
         $.ajax({
             type: "POST",
             url: "/Admin_EnterpriseInfo/GetEnterpriseModel",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 TraceEnMainCode(jsonResult.ObjModel.TraceEnMainCode);
             }
         })
     }
     //下载次数
     var ViewDownLoadNum = function (DownLoadNum) {
         DownLoadNum = ko.utils.unwrapObservable(DownLoadNum);
         if (DownLoadNum == null) {
             return "-";
         } else {
             return DownLoadNum + "次";
         }
     }
     //下载地址
     var GetDownUrl = function (downloadUrl) {
         downloadUrl = ko.utils.unwrapObservable(downloadUrl);
         if (downloadUrl == null || downloadUrl == "") {
             return "javascript:alert('正在生成，请稍后再试！')";
         }
         else {
             return downloadUrl;
         }
     }
     //状态
     var GetStatus = function (status) {
         status = ko.utils.unwrapObservable(status);
         switch (status) {
             case 1040000001:
                 return "未审核"; // 原未审核
             case 1040000005:
                 return "生成"; // 原生成完成
             case 1040000006:
                 return "生成失败"; // 原生成失败
             case 1040000008:
                 return "审批通过，等待生成"; // 
             case 1040000010:
                 return "可下载"; // 
             case 1040000011:
                 return "打包失败"; // 
         }
     }
     // 审核通过
     var audit = function (rId, type) {
         a(rId, type);
         //         wait(0);
     }

     var a = function (rId, type) {
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
                             var list = GetDataKO(1);
                             UpdateData(list);
                         }
                     }
                     }]);
                 }

             }
         })
     }
     //下载
     var DownloadFile = function (RequestCodeId, downLoadURL) {
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         RequestCodeId = ko.utils.unwrapObservable(RequestCodeId);
         downLoadURL = ko.utils.unwrapObservable(downLoadURL);
         $.fileDownload('/Admin_Request/DownloadFile?RequestCodeId=' + RequestCodeId + '&downLoadURL=' + downLoadURL)
                  .done(function () { alert('下载成功!'); })
                       .fail(function () { alert('下载失败!'); });

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
     //     var endDate = ko.observable(utils.dateFormat(new Date(), 'yyyy-MM-dd'));
     //     var SelSpecification = ko.observable(false);
     var SelMaterial = ko.observable(false);
     var SelType = ko.observable(false);
     //     var SelMaterialSpec = ko.observable(false);

     //获取产品
     var GetMaterialList = function () {
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Request/SearchNameList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
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

     //     //获取规格
     //     var GetSpecificationsList = function () {
     //         var data;
     //         $.ajax({
     //             type: "POST",
     //             url: "/Admin_Specification/GetSelectList",
     //             contentType: "application/json;charset=utf-8", //必须有
     //             dataType: "json", //表示返回值类型，不必须
     //             async: false,
     //             success: function (jsonResult) {
     //                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
     //                     return;
     //                 };
     //                 data = jsonResult.ObjList;
     //                 //alert(JSON.stringify(data));
     //             }
     //         });
     //         return data;
     //     }
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
     //鼠标滑过
     var mouseoverFun = function (data, event) {
         var self = $(event.target).closest('tr');
         var btnPackaging = $("#btnPackaging_" + data.RequestCode_ID());
         btnPackaging.css({ "display": "none" });
         //下载
         var btnDownload = $("#btnDownload_" + data.RequestCode_ID());
         btnDownload.css({ "display": "none" });

         switch (data.Status()) {
             case 1040000005: // 打包成功
                 btnPackaging.css({ "display": "" });
                 break;
             case 1040000010: // 打包成功
                 btnDownload.css({ "display": "" });
                 break;
         }
     }
     //鼠标离开
     var mouseoutFun = function (data, event) {
         var self = $(event.target).closest('tr');
         var btnPackaging = $("#btnPackaging_" + data.RequestCode_ID());
         btnPackaging.css({ "display": "none" });
         //下载
         var btnDownload = $("#btnDownload_" + data.RequestCode_ID());
         btnDownload.css({ "display": "none" });

     }
     //ajax获取数据
     var GetData = function (pageIndex) {
         var sendData = {
             pageIndex: pageIndex,
             bd: beginDate(),
             ed: endDate(),
             type: 4
             //         mName: searchName()
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Request/IndexBox",
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
     /*******************************获取企业简码4位简码20190114新加*****************************************/
     var GetEntepriseMainJCode = function (data, event) {
         var currentObj = $(event.target);
         currentObj.blur();
         var sendData = {
         }
         $.ajax({
             type: "POST",
             url: "/Public/EditEnJMainCode",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                     return;
                 };
                 dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                     if (jsonResult.code != 0) {
                         //获取企业信息
                         GetEnterpriseModel();
                         VmMaterial.MaterialArray(GetMaterialList());
                         SelTypeModel.SelTypeArray([{ "Text": "箱码", "Value": 5 }, { "Text": "礼盒码", "Value": 1}]);
                         $("#DivSpecifications").css({ "display": "none" });
                         $("#SpanGe").css({ "display": "" });
                         $("#SpanTao").css({ "display": "none" });
                         ViewModel.VmGenerateCode = GetDataKO(1);
                     }
                 }
                 }]);
             },
             error: function (Error) {
             }
         })
     }
     var Generate = function () {
         var currentObj = $(event.target);
         currentObj.blur();
         var errors = ko.validation.group(this);
         var val = $('input:radio[name="CodeTypeCheck"]:checked').val();
         if (val == 2) {
             if (TraceEnMainCode() == null) {
                 alert("请先获取企业简码！");
                 return;
             }
         }
         if (errors().length <= 0 && VmMaterial.SelectedId() && SelTypeModel.SelectedId()) {
             var sendData = {
                 codeAttribute: ko.utils.unwrapObservable(SelTypeModel.SelectedId),
                 //                 GuiGeID: SpecificationsModel.SelectedId(),
                 Material_ID: VmMaterial.SelectedId(),
                 RequestCount: Number(),
                 codeOfType: $("input[name='CodeTypeCheck']:checked").val()
                 //                 PackNumber: PackNumber()
             }
             $.ajax({
                 type: "POST",
                 url: "/Admin_Request/GeneratePackCode",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                         return;
                     };
                     dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                         if (jsonResult.code == 1) {
                             var list = GetDataKO(1);
                             UpdateData(list);
                         }
                     }
                     }]);
                 }
             })
         } else {
             if (!VmMaterial.SelectedId()) {
                 SelMaterial(true);
             }
             errors.showAllMessages();
         }
     }

     var ViewType = function (Type) {
         Type = ko.utils.unwrapObservable(Type);
         if (Type == 1) {
             return "盒码";
         } else if (Type == 4) {
             return "盒码";
         } else if (Type == 5) {
             return "箱码";
         } else if (Type == 7) {
             return "箱码";
         }
     }

     var ViewUnit = function (Type) {
         Type = ko.utils.unwrapObservable(Type);
         if (Type == 1) {
             return "个";
         } else if (Type == 2) {
             return "个";
         } else if (Type == 4) {
             return "个";
         } else if (Type == 5) {
             return "个";
         }
     }

     var ViewSpecification = function (Specification) {
         Specification = ko.utils.unwrapObservable(Specification);
         if (Specification == 0 || Specification == null) {
             return "-";
         } else {
             return Specification;
         }
     }

     var ViewModel = {
         binding: function () {
             //初初化导航状态
             //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
             //获取企业信息20190114添加
             GetEnterpriseModel();
             VmMaterial.MaterialArray(GetMaterialList());
             //             SpecificationsModel.SpecificationsArray(GetSpecificationsList());
             SelTypeModel.SelTypeArray([{ "Text": "箱码", "Value": 5 }, { "Text": "礼盒码", "Value": 1}]);
             $("#DivSpecifications").css({ "display": "none" });
             $("#SpanGe").css({ "display": "" });
             $("#SpanTao").css({ "display": "none" });

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
         SelTypeModel: SelTypeModel,
         SelType: SelType,
         VmMaterial: VmMaterial,
         //         SpecificationsModel: SpecificationsModel,
         Number: Number,
         //         PackNumber: PackNumber,
         beginDate: beginDate,
         endDate: endDate,
         onchangeData: onchangeData,
         Generate: Generate,
         ViewType: ViewType,
         //         SelSpecification: SelSpecification,
         SelMaterial: SelMaterial,
         ViewSpecification: ViewSpecification,
         ViewUnit: ViewUnit,
         SelType: SelType,
         //         SelMaterialSpec: SelMaterialSpec,
         searchName: searchName,
         ViewDownLoadNum: ViewDownLoadNum,
         GetDownUrl: GetDownUrl,
         GetStatus: GetStatus,
         audit: audit,
         DownloadFile: DownloadFile,
         mouseoverFun: mouseoverFun,
         mouseoutFun: mouseoutFun,
         TraceEnMainCode: TraceEnMainCode,
         GetEntepriseMainJCode: GetEntepriseMainJCode
     }
     return ViewModel;
 });