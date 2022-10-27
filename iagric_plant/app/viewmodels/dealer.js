define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', './dealer_add', './dealer_edit', "./dealer_showmap", 'plugins/dialog', 'utils', 'logininfo', './dealerUpload'],
    function (router, ko, km, $, jq, dealer_add, dealer_edit, dealer_showmap, dialog, utils, loginInfo, uploadexcel) {
        var moduleInfo = {
            moduleID: '10090',
            parentModuleID: '10001'
        }
        var searchTitle = ko.observable();

        //ajax获取数据
        var getData = function (pageIndex, dealerName) {
            var sendData = {
                pageIndex: pageIndex,
                dealerName: dealerName
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/Admin_Dealer/Index",
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
            vm.vmDealer.ObjList(list.ObjList());
            vm.vmDealer.pageSize(list.pageSize());
            vm.vmDealer.totalCounts(list.totalCounts());
            vm.vmDealer.pageIndex(list.pageIndex());
        }
        //搜索经销商
        var searchDealer = function (data, event) {
            var list = getDataKO(1, searchTitle());
            updateData(list);
        };
        var addDealer = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            dealer_add.show().then(function () {
                searchTitle('');
                var list = getDataKO(1, "");
                updateData(list);
            });
        };
        var editDealer = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            var id = ko.utils.unwrapObservable(id);
            dealer_edit.show(id).then(function () {
                var list = getDataKO(vm.vmDealer.pageIndex(), searchTitle());
                updateData(list);
            });
        };
        var showMap = function (location, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            var location = ko.utils.unwrapObservable(location);
            dealer_showmap.show(location);
        };
        var delDealer = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var id = ko.utils.unwrapObservable(id);
            dialog.showMessage("确定删除该经销商吗？", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin_Dealer/Delete",
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
                                        var currentPageRow = vm.vmDealer.ObjList().length;
                                        var pageIndex = vm.vmDealer.pageIndex();
                                        if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                            pageIndex = vm.vmDealer.pageIndex() - 1;
                                        }
                                        var list = getDataKO(pageIndex, searchTitle());
                                        updateData(list);
                                        this.pageSize = parseInt(vm.vmDealer.pageSize());
                                        this.totalCounts = parseInt(vm.vmDealer.totalCounts());
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
        //把获取的ajax数据转化为ko
        var getDataKO = function (pageIndex, dealerName) {
            var list = km.fromJS(getData(pageIndex, dealerName));
            return list;
        }
        //自定义绑定-分页控件
        ko.bindingHandlers.dealerPager = {
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
                            var list = getDataKO(num, searchTitle());
                            updateData(list);
                            this.pageSize = parseInt(vm.vmDealer.pageSize());
                            this.totalCounts = parseInt(vm.vmDealer.totalCounts());
                        }
                    }
                });
            }
        };
        var mouseoverFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var ShowMap = self.find("button[eleflag='ShowMap']");
            var ShowHand = self.find("div[eleflag='ShowHand']");

            ShowMap.css({ "display": "" });
            ShowHand.css({ "display": "" });
        }
        var mouseoutFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var ShowMap = self.find("button[eleflag='ShowMap']");
            var ShowHand = self.find("div[eleflag='ShowHand']");

            ShowMap.css({ "display": "none" });
            ShowHand.css({ "display": "none" });
        }

        //导入Excel
        var interExcel = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            uploadexcel.show().then(function () {
                var list = getDataKO(vm.vmDealer.pageIndex(), searchTitle());
                updateData(list);
            });
        }
        var vm = {
            binding: function () {
                //初初化导航状态
                //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
                //初始化dealer列表数据
                vm.vmDealer = getDataKO(1, "");
                updateData(vm.vmDealer);
            },
            goBack: function () {
                router.navigateBack();
            },
            vmDealer: null,
            searchTitle: searchTitle,
            searchDealer: searchDealer,
            addDealer: addDealer,
            editDealer: editDealer,
            delDealer: delDealer,
            mouseoverFun: mouseoverFun,
            mouseoutFun: mouseoutFun,
            showMap: showMap,
            interExcel: interExcel
        }
        return vm;
    });