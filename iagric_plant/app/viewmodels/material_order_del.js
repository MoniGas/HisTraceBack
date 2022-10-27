define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'bootstrap-datepicker', 'plugins/dialog', 'utils', 'logininfo', 'jquery.querystring', 'knockout.validation'],
    function (router, ko, km, $, jq, bdp, dialog, utils, loginInfo, qs, kv) {
        var vmObj;

        var getstatus = function () {
            var data;
            $.ajax({
                type: "POST",
                url: "/Admin_Material_OnlineOrder/GetStatus",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必
                async: false,
                success: function (jsonResult) {
                    data = jsonResult.ObjList;
                },
                error: function (Error) {
                    alert(Error);
                }
            })
            return data;
        }

        //ajax获取数据
        var getData = function (name, status, bDate, eDate, pageIndex) {
            var sendData = {
                name: name,
                status: status,
                bDate: bDate,
                eDate: eDate,
                pageIndex: pageIndex
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/Admin_Material_OnlineOrder/DelIndex",
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
        var updateData = function (list) {
            //alert(list.pageIndex());
            vmObj.ObjList(list.ObjList());
            vmObj.pageSize(list.pageSize());
            vmObj.totalCounts(list.totalCounts());
            vmObj.pageIndex(list.pageIndex());
        }
        //自定义绑定-分页控件
        ko.bindingHandlers.orderPager = {
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
                            var list = getDataKO(vmObj.name(), vmObj.bDate(), vmObj.eDate(), vmObj.pageIndex());
                            updateData(list);
                            this.pageSize = parseInt(vmObj.pageSize());
                            this.totalCounts = parseInt(vmObj.totalCounts());
                        }
                    }
                });
            }
        };
        //把获取的ajax数据转化为ko
        var getDataKO = function (name, bDate, eDate, pageIndex) {
            var status = -1;
            if (vmObj.statusArray.selectstatus() && vmObj.statusArray.selectstatus().statusId != undefined) {
                status = vmObj.statusArray.selectstatus().statusId;
            }
            else {
                status = -1;
            }
            var list = km.fromJS(getData(name, status, bDate, eDate, pageIndex));
            return list;
        }
        var vmOrder = function () {
            var self = this;

            self.name = ko.observable();
            self.bDate = ko.observable();
            self.eDate = ko.observable();
            self.statusArray = {
                statuss: ko.observableArray(),
                selectstatus: ko.observableArray()
            }

            self.pageSize = ko.observable();
            self.totalCounts = ko.observable();
            self.pageIndex = ko.observable();

            self.ObjList = ko.observableArray();

            self.searchOrder = function () {
                var list = getDataKO(self.name(), self.bDate(), self.eDate(), self.pageIndex());
                updateData(list);
            }

            self.init = function () {


                self.statusArray.statuss(getstatus());

                var list = getDataKO("", "", "", 1);
                updateData(list);

            }


            self.mouseoverFun = function (data, event) {
                var self = $(event.target).closest('tr');
                var ShowHand = self.find("div[eleflag='ShowHand']");

                ShowHand.css({ "display": "" });
            }
            self.mouseoutFun = function (data, event) {
                var self = $(event.target).closest('tr');
                var ShowHand = self.find("div[eleflag='ShowHand']");

                ShowHand.css({ "display": "none" });
            }
            self.delOrder = function (id, data, event) {
                var currentObj = $(event.target);
                currentObj.blur();
                var id = ko.utils.unwrapObservable(id);
                dialog.showMessage("确定显示该订单吗？", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin_Material_OnlineOrder/Del",
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
                                        var currentPageRow = vmObj.ObjList().length;
                                        var pageIndex = vmObj.pageIndex();
                                        if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                            pageIndex = vmObj.pageIndex() - 1;
                                        }
                                        var list = getDataKO(vmObj.name(), vmObj.bDate(), vmObj.eDate(), vmObj.pageIndex());
                                        updateData(list);
                                        this.pageSize = parseInt(vmObj.pageSize());
                                        this.totalCounts = parseInt(vmObj.totalCounts());
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
        };
        vmOrder.prototype.binding = function () {
            $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });

            $('#bDate').datepicker({
                language: 'cn',
                autoclose: true,
                todayHighlight: true
            });
            $('#eDate').datepicker({
                language: 'cn',
                autoclose: true,
                todayHighlight: true
            });

        }
        vmOrder.prototype.close = function () {
            dialog.close(this);
        }
        vmOrder.show = function () {
            vmObj = new vmOrder();
            vmObj.init();
            return dialog.show(vmObj);
        };
        return vmOrder;
    });