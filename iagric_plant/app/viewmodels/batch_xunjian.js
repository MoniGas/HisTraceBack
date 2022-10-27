define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'plugins/dialog', 'knockout', 'jquery-ui', './batch_xunjian_edit', 'utils','logininfo'],
 function (router, ko, km, $, jq, dialog, ko, jqueryui, batch_xunjian_edit, utils,loginInfo) {

     //ajax获取数据
     var getData = function (bid, beid, pageIndex) {
         var sendData = {
             batchid: bid,
             batchextid: beid,
             pageIndex: pageIndex
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Batch_XunJian/Index",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 //                var obj = JSON.parse(jsonResult)
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                     return;
                 };
                 data = jsonResult;
             }
         })
         return data;
     }
     //分页、搜索时更新数据源
     var updateData = function (list) {
         vmObj.vmBatch_XunJian.ObjList(list.ObjList());
         vmObj.vmBatch_XunJian.pageSize(list.pageSize());
         vmObj.vmBatch_XunJian.totalCounts(list.totalCounts());
         vmObj.vmBatch_XunJian.pageIndex(list.pageIndex());
     }
     var search = function (data, event) {
         var list = getDataKO(bid(), beid(), 1);
         updateData(list);
     };
     //把获取的ajax数据转化为ko
     var getDataKO = function (bid, beid, pageIndex) {
         var list = km.fromJS(getData(bid, beid, pageIndex));
         return list;
     }

     //自定义绑定-分页控件
     ko.bindingHandlers.batch_XunJianPager = {
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
                 first: '<li class="first"><a href="javascript:;">首页页</a></li>',
                 last: '<li class="last"><a href="javascript:;">尾页</a></li>',
                 prev: '<li class="prev"><a href="javascript:;">上一页</a></li>',
                 next: '<li class="next"><a href="javascript:;">下一页</a></li>',
                 page: '<li class="page"><a href="javascript:;">{{page}}</a></li>',
                 onPageChange: function (num, type) {
                     //alert(type + '：' + num);
                     if (type == 'change') {
                         var list = getDataKO(vmObj.bid, vmObj.beid, num);
                         updateData(list);
                         this.pageSize = parseInt(vmObj.vmBatch_XunJian.pageSize());
                         this.totalCounts = parseInt(vmObj.vmBatch_XunJian.totalCounts());
                     }
                 }
             });
         }
     };
     var batchxunjian = function (bid, beid) {
         var self = this;
         self.bid = bid;
         self.beid = beid;
         self.vmBatch_XunJian = ko.observableArray();
         self.init = function () {
             //             self.vmBatch_XunJian = getDataKO(bid, beid, 1);
             var list = getDataKO(bid, beid, 1);
             return list;
         }
         self.updatedata = function (list) {
             self.vmBatch_XunJian.ObjList(list.ObjList());
         }
         self.editXunJian = function (id, data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             var jsonResult = loginInfo.isLoginTimeoutForServer();
             if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                 return;
             };
             var id = ko.utils.unwrapObservable(id);
             batch_xunjian_edit.show(id).then(function () {
                 var list = getDataKO(bid, beid, 1);
                 self.updatedata(list);
             });
         }
         self.delXunJian = function (id, data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             var id = ko.utils.unwrapObservable(id);
             dialog.showMessage("确定删除选择的数据吗?", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin_Batch_XunJian/Del",
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
                                        var currentPageRow = vmObj.vmBatch_XunJian.ObjList().length;
                                        var pageIndex = vmObj.vmBatch_XunJian.pageIndex();
                                        if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                            pageIndex = vmObj.vmBatch_XunJian.pageIndex() - 1;
                                        }
                                        var list = getDataKO(vmObj.bid, vmObj.beid, pageIndex);
                                        updateData(list);
                                        this.pageSize = parseInt(vmObj.vmBatch_XunJian.pageSize());
                                        this.totalCounts = parseInt(vmObj.vmBatch_XunJian.totalCounts());
                                    }
                                }
                                }]);
                            }
                        }
                    });
                }
            },
            {
                title: '取消',
                callback: function () {
                }
            }
            ]);
         }
     }
     batchxunjian.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     batchxunjian.prototype.close = function () {
         var listCount = vmObj.vmBatch_XunJian.ObjList().length;
         //         var listCount = vmObj.vmBatch_XunJian.totalCounts();
         dialog.close(this, listCount);
     }
     var vmObj;
     batchxunjian.show = function (bid, beid) {
         vmObj = new batchxunjian(bid, beid);
         vmObj.vmBatch_XunJian = vmObj.init();
         return dialog.show(vmObj);
     };
     return batchxunjian;
 });