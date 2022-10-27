define(['plugins/router', 'knockout.mapping', 'jquery', 'jqPaginator', 'plugins/dialog', 'knockout', 'jquery-ui', 'utils', 'jquery.fileDownload', 'logininfo', 'bootstrap-datepicker', 'knockout.validation'],
 function (router, km, $, jq, dialog, ko, jqueryui, utils, jfd, loginInfo, bdp, kv) {
     //ajax获取数据
     var initDate = true;
     var getData = function (pid, pageIndex) {
         var sendData = {
             pid: pid,
             pageIndex: pageIndex
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/SysSetAuditCount/Index",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                     return;
                 };
                 data = jsonResult;
             },
             error: function (Error) {
                 alert(Error);
             }
         })
         return data;
     }
     //分页、搜索时更新数据源
     var updateData = function (list) {
         vmObj.vmAuditCodeAdd.ObjList(list.ObjList());
         vmObj.vmAuditCodeAdd.pageSize(list.pageSize());
         vmObj.vmAuditCodeAdd.totalCounts(list.totalCounts());
         vmObj.vmAuditCodeAdd.pageIndex(list.pageIndex());
     }
     //把获取的ajax数据转化为ko
     var getDataKO = function (pid, pageIndex) {
         var list = km.fromJS(getData(pid, pageIndex));
         return list;
     }
     //自定义绑定-分页控件
     ko.bindingHandlers.CodeImgPager = {
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
                         var list = getDataKO(vmObj.pid, num);
                         updateData(list);
                         this.pageSize = parseInt(vmObj.vmAuditCodeAdd.pageSize());
                         this.totalCounts = parseInt(vmObj.vmAuditCodeAdd.totalCounts());
                     }
                 }
             });
         }
     };

     //定义主模型gensuitcode==auditcodeadd
     var auditcodeadd = function (pid) {
         var self = this;
         //码表编号
         self.pid = pid;
         self.auditcountall = ko.observable();
         self.auditedcount = ko.observable();
         self.auditedcountyu = ko.observable();
         self.zhuijiaCode = ko.observable().extend({
             min: {
                 params: 1,
                 message: "数量最少为1！"
             },
             digit: {
                 params: true,
                 message: "请输入整数！"
             },
             required: {
                 params: true,
                 message: "请输入追加数量！"
             }
         });
         self.selzhuijiaCode = ko.observable(false);
         self.vmAuditCodeAdd = ko.observableArray();
         self.onchange = function () {
             var list = getDataKO(pid, 1);
             self.a(list);
         }
         self.a = function (list) {
             self.vmAuditCodeAdd.ObjList(list.ObjList());
         }
         self.init = function (a) {
             var list = getDataKO(pid, 1);
             return list;
         }
         self.initmodel = function () {
             var sendData = {
                 pid: self.pid
             };
             $.ajax({
                 type: "POST",
                 url: "/SysSetAuditCount/GetModelCount",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     self.auditcountall(jsonResult.ObjModel.AuditCountAll);
                     self.auditedcount(jsonResult.ObjModel.AuditedCount);
                     self.auditedcountyu(jsonResult.ObjModel.AuditCountAll - jsonResult.ObjModel.AuditedCount);
                 }
             })
             //             alert(self.auditcountall);
         }
         self.calcel = function () {
             this.close();
         }
         //鼠标滑过************************************************************************/
         self.mouseoverFun = function (data, event) {
             var self = $(event.target).closest('tr');
             var ShowAll = self.find("button[eleflag='ShowAll']");
             ShowAll.css({ "display": "" });
         }
         //鼠标离开************************************************************************/
         self.mouseoutFun = function (data, event) {
             var self = $(event.target).closest('tr');
             var ShowAll = self.find("button[eleflag='ShowAll']");
             ShowAll.css({ "display": "none" });
         }
         //设置码************************************************************************/
         self.addcode = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0) {
                 var sendData = {
                     pid: self.pid,
                     zhuijiaCode: self.zhuijiaCode()
                 }
                 $.ajax({
                     type: "POST",
                     url: "/SysSetAuditCount/Add",
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
                                 var list = getDataKO(vmObj.pid, 1);
                                 updateData(list);
                                 self.initmodel();
                             }
                         }
                         }]);
                     }
                 })
             }
             else {
                 //                 if (self.zhuijiaCode() == null || self.zhuijiaCode() == undefined) {
                 //                     self.selzhuijiaCode(true)
                 //                 }
                 //                 else {
                 //                     self.selzhuijiaCode(false)
                 //                 }
                 self.errors.showAllMessages();
             }
         }
     }
     auditcodeadd.prototype.close = function (a) {
         //alert(this.province().code);
         dialog.close(this, a);
     }
     auditcodeadd.prototype.closeOK = function (id) {
         dialog.close(this, id);
     }
     var vmObj;
     auditcodeadd.show = function (pid) {
         vmObj = new auditcodeadd(pid);
         vmObj.vmAuditCodeAdd = vmObj.initmodel();
         vmObj.vmAuditCodeAdd = vmObj.init(0);
         return dialog.show(vmObj);
     };
     return auditcodeadd;
 });
