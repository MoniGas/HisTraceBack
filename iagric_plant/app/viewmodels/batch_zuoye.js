define(['plugins/router', 'knockout.mapping', 'jquery', 'jqPaginator', 'plugins/dialog', 'knockout', 'jquery-ui', './batch_zuoye_edit', 'utils', 'logininfo'],
 function (router, km, $, jq, dialog, ko, jqueryui, batch_zuoye_edit, utils, loginInfo) {
     //ajax获取数据
     self.opid = 0;
     var getData = function (bid, beid, opid, pageIndex) {
         var sendData = {
             batchid: bid,
             opid: opid,
             batchextid: beid,
             pageIndex: pageIndex
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Batch_ZuoYe/Index",
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
         vmObj.vmBatch_ZuoYe.ObjList(list.ObjList());
         vmObj.vmBatch_ZuoYe.pageSize(list.pageSize());
         vmObj.vmBatch_ZuoYe.totalCounts(list.totalCounts());
         vmObj.vmBatch_ZuoYe.pageIndex(list.pageIndex());
     }
     var search = function (data, event) {
         if (vmObj.opTypeModelsArray.selectedModel()) {
             opid = vmObj.opTypeModelsArray.selectedModel().Batch_ZuoYeType_ID;
         }
         var list = getDataKO(bid, beid, opid, 1);
         updateData(list);
     };
     //把获取的ajax数据转化为ko
     var getDataKO = function (bid, beid, opid, pageIndex) {
         var list = km.fromJS(getData(bid, beid, opid, pageIndex));
         return list;
     }
     //自定义绑定-分页控件
     ko.bindingHandlers.batchZuoYePager = {
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
                         var list = getDataKO(vmObj.bid, vmObj.beid, opid, num);

                         updateData(list);
                         this.pageSize = parseInt(vmObj.vmBatch_ZuoYe.pageSize());
                         this.totalCounts = parseInt(vmObj.vmBatch_ZuoYe.totalCounts());
                     }
                 }
             });
         }
     };
     var batchzuoye = function (bid, beid, zuoyeName) {
         var self = this;
         self.bid = bid;
         self.beid = beid;
         self.opid = opid;
         self.vmBatch_ZuoYe = ko.observableArray();
         self.opTypeModelsArray = [];
         self.selectedModel = ko.observable();
         self.homeDefaultZuoYeName = zuoyeName;

         self.onchange = function () {
             var list = getDataKO(bid, beid, self.selectedModel(), 1);
             self.a(list);
         }

         self.a = function (list) {
             self.vmBatch_ZuoYe.ObjList(list.ObjList());
         }

         self.init = function (a) {
             var list = getDataKO(bid, beid, a, 1);
             return list;
         }
         self.calcel = function () {
             this.close();
         }
         self.editZuoYe = function (id, data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             var jsonResult = loginInfo.isLoginTimeoutForServer();
             if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                 return;
             };
             var id = ko.utils.unwrapObservable(id);
             batch_zuoye_edit.show(id).then(function () {
                 var list = getDataKO(bid, beid, 0, 1);
                 self.a(list);
                 var currentGongXuItem = self.vmBatch_ZuoYe.ObjList()[0];
                 if (currentGongXuItem != undefined) {
                     self.homeDefaultZuoYeName = currentGongXuItem.OperationTypeName();
                 }
             });
         }
         self.delBatch_ZuoYe = function (id, data, event) {
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
                        url: "/Admin_Batch_ZuoYe/Delete",
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
                                        var currentPageRow = vmObj.vmBatch_ZuoYe.ObjList().length;
                                        var pageIndex = vmObj.vmBatch_ZuoYe.pageIndex();
                                        if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                            pageIndex = vmObj.vmBatch_ZuoYe.pageIndex() - 1;
                                        }
                                        var list = getDataKO(vmObj.bid, vmObj.beid, vmObj.opid, pageIndex);
                                        updateData(list);
                                        this.pageSize = parseInt(vmObj.vmBatch_ZuoYe.pageSize());
                                        this.totalCounts = parseInt(vmObj.vmBatch_ZuoYe.totalCounts());

                                        var currentGongXuItem = self.vmBatch_ZuoYe.ObjList()[0];
                                        //alert(currentGongXuItem);
                                        if (currentGongXuItem != undefined) {
                                            self.homeDefaultZuoYeName = currentGongXuItem.OperationTypeName();
                                        }
                                        else {
                                            self.homeDefaultZuoYeName = '无';
                                        }
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
     //获取生产/加工类型模块
     var getNewsModules = function () {
         var sendData = {

         };
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Batch_ZuoYe/OpTypeListAll",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 data = jsonResult.ObjList;
             },
             error: function (e) {

             }
         });
         return data;
     }
     batchzuoye.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     batchzuoye.prototype.close = function () {
         dialog.close(this, this.homeDefaultZuoYeName);
     }
     var vmObj;
     batchzuoye.show = function (bid, beid, zuoyeName) {
         vmObj = new batchzuoye(bid, beid, zuoyeName);
         vmObj.opTypeModelsArray = getNewsModules();
         //     vmObj.init(0);
         vmObj.vmBatch_ZuoYe = vmObj.init(0);
         return dialog.show(vmObj);
     };
     return batchzuoye;
 });
