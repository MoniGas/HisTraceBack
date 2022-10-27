define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'jquery.fileDownload', './requestcodeewm', './requestcodesales'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, jfd, requestcodeewm, requestcodesales) {
     var moduleInfo = {
         moduleID: '10060',
         parentModuleID: '10001'
     }
     var levelId = 0;
     var mainCode = ko.observable('');
     var vmNewsModels = {
         newsModelsArray: ko.observableArray(),
         selectedAddOption: ko.observable(),
         selectedSelOption: ko.observable()
     }

     var selAddProduct = ko.observable(false);
     var i = 0;
     vmNewsModels.selectedAddOption.subscribe(function () {
         if (i == 0) { i = i + 1; return; }
         if (vmNewsModels.selectedAddOption()) {
             selAddProduct(false);
         }
         else {
             selAddProduct(true);
         }
     });

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
         //第一次装入当前的上一个月(格式yyyy-mm)  
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

     //     var num = ko.observable('').extend({
     //         required: {
     //             params: true,
     //             message: "请输入申请数量!"
     //         }
     //     });
     var num = ko.observable('').extend({
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
             message: "申请数量为整数！"
         },
         required: {
             params: true,
             message: "请填写申请数量！"
         }
     });
     var selNum = ko.observable(false);

     var selTitle = ko.observable(false);

     var ClearRequest = function () {
         var self = this;
         //         vmNewsModels.selectedAddOption = ko.observable();
         self.num('500');
         self.vmNewsModels.selectedAddOption(null);
     }

     // 申请产品二维码
     var AddRequest = function (data, event) {
         var currentObj = $(event.target);
         currentObj.blur();
         var errors = ko.validation.group(this);
         if (errors().length <= 0 && vmNewsModels.selectedAddOption()) {
             var sendData = {
                 mId: vmNewsModels.selectedAddOption(),
                 codeCount: num()
             }
             $.ajax({
                 type: "POST",
                 url: "/Admin_Request/Add",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (obj) {
                     if (loginInfo.isLoginTimeout(obj.code, obj.Msg, 1)) {
                         return;
                     }
                     dialog.showMessage(obj.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                     searchRequest(null, null);
                     ClearRequest();
                 }
             })
         } else {
             if (vmNewsModels.selectedAddOption()) {
                 selAddProduct(false);
             }
             else {
                 selAddProduct(true);
             }
             errors.showAllMessages();
         }
     }

     //搜索生产基地
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

     //刷新生产基地
     var refreshRequest = function (data, event) {
         location.reload();
     };

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
             mId: vmNewsModels.selectedSelOption(),
             mName: searchName()//vmNewsModels.searchName()
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
             },
             error: function (Error) {
                 alert(Error.toString());
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
         url: "/Admin_Request/SearchNameList",
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

 // 审核通过
 var audit = function (rId, type) {
     var sendData = {
         rId: ko.utils.unwrapObservable(rId),
         type: ko.utils.unwrapObservable(type)
     }
     var value;
     $.ajax({
         type: "POST",
         url: "/Admin_Request/Audit",
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
     })
 }

 var downloadFile = function (downLoadURL) {
     var jsonResult = loginInfo.isLoginTimeoutForServer();
     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
         return;
     };
     var downLoadURL = ko.utils.unwrapObservable(downLoadURL);
     $.fileDownload('/Admin_Request/DownloadFile?downLoadURL=' + downLoadURL)
                  .done(function () { alert('下载成功'); })
                       .fail(function () { alert('下载失败!'); });
 }

 var GetLevelId = function () {
     var sendData = {
 }
 $.ajax({
     type: "POST",
     url: "/Admin_Request/GetLevelId",
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
        case 1040000005: // 生成完成
            btnEwm.css({ "display": "" });
            if (levelId == 1) {
                btnRecord.css({ "display": "" });
            }
            btnPackaging.css({ "display": "" });
            break;
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
        case 1040000010: // 打包成功
            btnEwm.css({ "display": "" });
            if (levelId == 1) {
                btnRecord.css({ "display": "" });
            }
            btnDownload.css({ "display": "" });
            break;
        case 1040000011: // 打包失败
            btnEwm.css({ "display": "" });
            if (levelId == 1) {
                btnRecord.css({ "display": "" });
            }
            btnPackaging.css({ "display": "" });
            break;
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

var vm = {
    binding: function () {
        //初始化活动动态模块数据
        vmNewsModels.newsModelsArray(getNewsModules());
        //初初化导航状态
        //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
        // 获取登录用户权限，1.企业用户 2.监管部门 3.平台商
        GetLevelId();
        vm.vmRequest = getDataKO(1);

        //限制文本框只能输入正整数
        $("#txtCount").keyup(function () {
            //如果输入非数字，则替换为''，如果输入数字，则在每4位之后添加一个空格分隔
            //                 this.value = this.value.replace(/[^\d]/g, '');
        })

        document.onkeydown = function (e) {
            var theEvent = window.event || e;
            var code = theEvent.keyCode || theEvent.which;
            if (code == 13) {
                $("#btnSearch").click();
            }
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
        $('#custom_file_upload').uploadify({
            'uploader': '/lib/jQueryUpLoadify/Scripts/uploadify.swf',
            'script': '/lib/jQueryUpLoadify/Upload.ashx',
            'cancelImg': '', ///lib/jQueryUpLoadify/Scripts/uploadify-cancel.png
            'folder': '/attached',
            'multi': false,
            'auto': true,
            'fileExt': '*.jpg;*.gif;*.png',
            'fileDesc': 'Image Files (.JPG, .GIF, .PNG)',
            'queueID': 'custom-queue',
            'queueSizeLimit': 999,
            'simUploadLimit': 999,
            'buttonText': '选择文件',
            'removeCompleted': false,
            'onSelectOnce': function (event, data) {
                $('#status-message').text(data.filesSelected + ' 个文件加入上传队列');
            },
            'onComplete': function (event, queueId, fileObj, response, data) {
                //alert(response.split('|')[1]); //这里获取上传后的URL路径，用来在前台显示 
                vm.vmXunJian_Add.files = response.split('|')[1];
            },
            'onAllComplete': function (event, data) {
                $('#status-message').text(data.filesUploaded + ' 个文件已上传');
            }
        });
    },
    goBack: function () {
        router.navigateBack();
    },
    vmRequest: null,
    num: num,
    vmNewsModels: vmNewsModels,
    beginDate: beginDate,
    endDate: endDate,
    searchRequest: searchRequest,
    refreshRequest: refreshRequest,
    searchName: searchName,
    addName: addName,
    AddRequest: AddRequest,
    ClearRequest: ClearRequest,
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
    selAddProduct: selAddProduct,
    complementHistoryDate: complementHistoryDate,
    getNowFormatDate: getNowFormatDate
}
return vm;
});