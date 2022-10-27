define(['plugins/router', 'knockout.mapping', 'jquery', 'jqPaginator', 'plugins/dialog', 'knockout', 'jquery-ui', 'utils', 'jquery.fileDownload', 'logininfo', 'jquery.uploadify', 'bootstrap-datepicker', 'knockout.validation', './slotting_edit', './slotting_view'],
 function (router, km, $, jq, dialog, ko, jqueryui, utils, jfd, loginInfo, uploadify, bdp, kv, slotting_edit, slotting_view) {
     //ajax获取数据
     var initDate = true;
     var getData = function (id, pageIndex) {
         var sendData = {
             storeId: id,
             pageIndex: pageIndex
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/Store/SlottingList",
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
         vmObj.vmStore.ObjList(list.ObjList());
         vmObj.vmStore.pageSize(list.pageSize());
         vmObj.vmStore.totalCounts(list.totalCounts());
         vmObj.vmStore.pageIndex(list.pageIndex());
     }
     //把获取的ajax数据转化为ko
     var getDataKO = function (id, pageIndex) {
         var list = km.fromJS(getData(id, pageIndex));
         return list;
     }
     //自定义绑定-分页控件
     ko.bindingHandlers.storePager = {
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
                         var list = getDataKO(vmObj.id, num);
                         updateData(list);
                         this.pageSize = parseInt(vmObj.vmStore.pageSize());
                         this.totalCounts = parseInt(vmObj.vmStore.totalCounts());
                     }
                 }
             });
         }
     };

     //定义主模型gensuitcode==store
     var store = function (id) {
         var self = this;
         self.id = id;
         self.vmStore = ko.observableArray();
         self.onchange = function () {
             var list = getDataKO(id, 1);
             self.a(list);
         }
         self.a = function (list) {
             self.vmStore.ObjList(list.ObjList());
         }
         self.init = function (a) {
             var list = getDataKO(id, 1);
             return list;
         }
         self.delStore = function (id, data, event) {
             dialog.showMessage("确定删除选择的数据吗？", '系统提示', [
             {
                 title: '确定',
                 callback: function () {
                     var sendData = {
                         id: ko.utils.unwrapObservable(id)
                     }
                     $.ajax({
                         type: "POST",
                         url: "/Store/Delete",
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
                                         var currentPageRow = vmObj.vmStore.ObjList().length;
                                         var pageIndex = vmObj.vmStore.pageIndex();
                                         if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                             pageIndex = vmObj.vmStore.pageIndex() - 1;
                                         }
                                         var list = getDataKO(vmObj.id, pageIndex);
                                         updateData(list);
                                         this.pageSize = parseInt(vmObj.vmStore.pageSize());
                                         this.totalCounts = parseInt(vmObj.vmStore.totalCounts());

                                     }
                                 }
                                 }]);
                             }
                         }
                     })
                 }
             },
            {
                title: '取消',
                callback: function () {
                }
            }
            ]);
         }
        self.editStore = function (id, data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             var jsonResult = loginInfo.isLoginTimeoutForServer();
             if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                 return;
             };
             var id = ko.utils.unwrapObservable(id);
             slotting_edit.show(id).then(function () {
                 var list = getDataKO(self.id, 1);
                 updateData(list);
             });
         }
         self.showCodeInfo = function (EwmUrl) {
             var jsonResult = loginInfo.isLoginTimeoutForServer();
             if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                 return;
             };
             var EwmUrl = ko.utils.unwrapObservable(EwmUrl);
             slotting_view.show(EwmUrl);
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

     }
     //绑定属性************************************************************************/
     store.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     store.prototype.close = function (a) {
         //alert(this.province().code);
         dialog.close(this, a);
     }
     store.prototype.closeOK = function (id) {
         dialog.close(this, id);
     }
     var vmObj;
     store.show = function (id) {
         vmObj = new store(id);
         vmObj.vmStore = vmObj.init(0);
         return dialog.show(vmObj);
     };
     return store;
 });
