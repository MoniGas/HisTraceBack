define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'bootstrap-datepicker', './material_return_edit', 'plugins/dialog', 'utils', 'jquery.fileDownload', 'logininfo'],
    function (router, ko, km, $, jq, bdp, material_return_edit, dialog, utils, jfd, loginInfo) {
        var moduleInfo = {
            moduleID: '29000',
            parentModuleID: '10001'
        }

        var name = ko.observable();
        var bDate = ko.observable();
        var eDate = ko.observable();

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
                url: "/Admin_ReturnMaterial/Index",
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
            vm.vmOrder.ObjList(list.ObjList());
            vm.vmOrder.pageSize(list.pageSize());
            vm.vmOrder.totalCounts(list.totalCounts());
            vm.vmOrder.pageIndex(list.pageIndex());
        }
        //搜索经销商
        var searchOrder = function (data, event) {
            var list = getDataKO(name(), bDate(), eDate(), vm.vmOrder.pageIndex());
            updateData(list);
        };
        var ExpressInfo = function (ExpressComp, ExpressNum, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            var ExpressComp = ko.utils.unwrapObservable(ExpressComp);
            var ExpressNum = ko.utils.unwrapObservable(ExpressNum);
            material_return_edit.show(ExpressComp, ExpressNum);
        }
        var editOrder = function (id, status, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            var Message = '确定已经收到消费者的商品了吗？';
            if (status == 8) {
                Message = '确定不同意消费者的退货申请吗？';
            }
            else if (status == 9) {
                Message = '确定同意消费者的退货申请吗？';
            }
            dialog.showMessage(Message, '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        OrderNum: id(),
                        Status: status
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin_ReturnMaterial/EditStatus",
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
                                        var currentPageRow = vm.vmOrder.ObjList().length;
                                        var pageIndex = vm.vmOrder.pageIndex();
                                        if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                            pageIndex = vm.vmOrder.pageIndex() - 1;
                                        }
                                        var list = getDataKO(name(), bDate(), eDate(), vm.vmOrder.pageIndex());
                                        updateData(list);
                                        this.pageSize = parseInt(vm.vmOrder.pageSize());
                                        this.totalCounts = parseInt(vm.vmOrder.totalCounts());
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
        };
        var trueDelOrder = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var id = ko.utils.unwrapObservable(id);
            dialog.showMessage("确定删除该订单吗？", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin_Material_OnlineOrder/TrueDel",
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
                                        var currentPageRow = vm.vmOrder.ObjList().length;
                                        var pageIndex = vm.vmOrder.pageIndex();
                                        if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                            pageIndex = vm.vmOrder.pageIndex() - 1;
                                        }
                                        var list = getDataKO(name(), bDate(), eDate(), vm.vmOrder.pageIndex());
                                        updateData(list);
                                        this.pageSize = parseInt(vm.vmOrder.pageSize());
                                        this.totalCounts = parseInt(vm.vmOrder.totalCounts());
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
        var disOrder = function () {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            material_order_del.show().then(function () {
                var list = getDataKO(name(), bDate(), eDate(), vm.vmOrder.pageIndex());
                updateData(list);
            });
        };
        //把获取的ajax数据转化为ko
        var getDataKO = function (name, bDate, eDate, pageIndex) {
            var status = -1;
            if (statusArray.selectstatus() && statusArray.selectstatus().statusId != undefined) {
                status = statusArray.selectstatus().statusId;
            }
            else {
                status = -1;
            }
            //alert(status);
            var list = km.fromJS(getData(name, status, bDate, eDate, pageIndex));
            return list;
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
                            vm.vmOrder.pageIndex(num);
                            var list = getDataKO(name(), bDate(), eDate(), vm.vmOrder.pageIndex());
                            updateData(list);
                            this.pageSize = parseInt(vm.vmOrder.pageSize());
                            this.totalCounts = parseInt(vm.vmOrder.totalCounts());
                        }
                    }
                });
            }
        };
        var mouseoverFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var ShowInfo = self.find("div[eleflag='ShowInfo']");
            var ShowAgree = self.find("div[eleflag='ShowAgree']");

            if (data.OrderType() == 7 || data.OrderType() == 8 || data.OrderType() == 9) {
                ShowAgree.css({ "display": "" });
            }
            if (data.OrderType() == 10) {
                ShowInfo.css({ "display": "" });
            }
        }
        var mouseoutFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var ShowInfo = self.find("div[eleflag='ShowInfo']");
            var ShowAgree = self.find("div[eleflag='ShowAgree']");

            ShowInfo.css({ "display": "none" });
            ShowAgree.css({ "display": "none" });
        }
        var exportExcel = function () {
            var status = -1;
            if (statusArray.selectstatus() && statusArray.selectstatus().statusId != undefined) {
                status = statusArray.selectstatus().statusId;
            }
            else {
                status = -1;
            }

            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                return;
            };
            $.fileDownload('/Admin_ReturnMaterial/ExportExcel?name=' + $("#searchName").val() + '&status=' + status + '&bDate=' + $("#bDate").val() + '&eDate=' + $("#eDate").val())
                  .done(function () { alert('下载成功'); })
                       .fail(function () { alert('下载失败!'); });
            return;
        }

        var getstatus = function () {
            var data;
            $.ajax({
                type: "POST",
                url: "/Admin_ReturnMaterial/GetStatus",
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
        var statusArray = {
            statuss: ko.observableArray(),
            selectstatus: ko.observableArray()
        }

        statusArray.selectstatus.subscribe(function () {
            searchOrder(null, null);
        });
        var vm = {
            binding: function () {
                //初初化导航状态
                //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);

                statusArray.statuss(getstatus());

                //初始化dealer列表数据
                vm.vmOrder = getDataKO("", "", "", 1);
                updateData(vm.vmOrder);

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
            },
            goBack: function () {
                router.navigateBack();
            },
            vmOrder: null,
            statusArray: statusArray,
            name: name,
            status: status,
            bDate: bDate,
            eDate: eDate,
            searchOrder: searchOrder,
            editOrder: editOrder,
            disOrder: disOrder,
            mouseoverFun: mouseoverFun,
            mouseoutFun: mouseoutFun,
            trueDelOrder: trueDelOrder,
            exportExcel: exportExcel,
            ExpressInfo: ExpressInfo
        }
        return vm;
    });