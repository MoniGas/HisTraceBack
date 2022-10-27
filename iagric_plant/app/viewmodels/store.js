define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', './store_add', './store_edit', './slotting_add', './slotting', 'plugins/dialog', 'utils', 'logininfo', './ewm_view'],
    function (router, ko, km, $, jq, store_add, store_edit, slotting_add, slotting, dialog, utils, loginInfo, ewm_view) {
        var moduleInfo = {
            moduleID: '33000',
            parentModuleID: '10001'
        }
        var searchTitle = ko.observable();

        //ajax获取数据
        var getData = function (pageIndex, storeName) {
            var sendData = {
                storeName: storeName,                              
                pageIndex: pageIndex
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/Store/Index",
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
            vm.vmStore.ObjList(list.ObjList());
            vm.vmStore.pageSize(list.pageSize());
            vm.vmStore.totalCounts(list.totalCounts());
            vm.vmStore.pageIndex(list.pageIndex());
        }
        //搜索仓库
        var searchStore = function (data, event) {
            var list = getDataKO(1, searchTitle());
            updateData(list);
        };
        //添加仓库
        var addStore = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            store_add.show().then(function () {
                searchTitle('');
                var list = getDataKO(1, "");
                updateData(list);
            });
        };
        //编辑仓库
        var editStore = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            var id = ko.utils.unwrapObservable(id);
            store_edit.show(id).then(function () {
                var list = getDataKO(vm.vmStore.pageIndex(), searchTitle());
                updateData(list);
            });
        };
        //添加货位
        var addSlotting = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            var id = ko.utils.unwrapObservable(id);
            slotting_add.show(id).then(function () {
                var list = getDataKO(vm.vmStore.pageIndex(), searchTitle());
                updateData(list);
            });
        };
        //查看仓库码
        var showCodeInfo = function (ewm) {
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            var ewm = ko.utils.unwrapObservable(ewm);
            ewm_view.show(ewm);
        }
        //货位管理
        var storeSlotting = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            var id = ko.utils.unwrapObservable(id);
            slotting.show(id).then(function () {
                var list = getDataKO(vm.vmStore.pageIndex(), searchTitle());
                updateData(list);
            });
        };
        var delStore = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var id = ko.utils.unwrapObservable(id);
            dialog.showMessage("确定删除该仓库吗？", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id
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
                                        var currentPageRow = vm.vmStore.ObjList().length;
                                        var pageIndex = vm.vmStore.pageIndex();
                                        if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                            pageIndex = vm.vmStore.pageIndex() - 1;
                                        }
                                        var list = getDataKO(pageIndex, searchTitle());
                                        updateData(list);
                                        this.pageSize = parseInt(vm.vmStore.pageSize());
                                        this.totalCounts = parseInt(vm.vmStore.totalCounts());
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
        var getDataKO = function (pageIndex, storeName) {
            var list = km.fromJS(getData(pageIndex, storeName));
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
                            this.pageSize = parseInt(vm.vmStore.pageSize());
                            this.totalCounts = parseInt(vm.vmStore.totalCounts());
                        }
                    }
                });
            }
        };
        var mouseoverFun = function (data, event) {
            var self = $(event.target).closest('tr');

            var ShowAll = self.find("button[eleflag='ShowAll']");
            ShowAll.css({ "display": "" });
        }
        var mouseoutFun = function (data, event) {
            var self = $(event.target).closest('tr');

            var ShowAll = self.find("button[eleflag='ShowAll']");
            ShowAll.css({ "display": "none" });
        }
        var vm = {
            binding: function () {
                //初初化导航状态
                //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
                //初始化dealer列表数据
                vm.vmStore = getDataKO(1, "");
                updateData(vm.vmStore);
            },
            goBack: function () {
                router.navigateBack();
            },
            vmStore: null,
            searchTitle: searchTitle,
            searchStore: searchStore,
            addStore: addStore,
            editStore: editStore,
            addSlotting: addSlotting,
            storeSlotting: storeSlotting,
            delStore: delStore,
            mouseoverFun: mouseoverFun,
            mouseoutFun: mouseoutFun,
            showCodeInfo: showCodeInfo
        }
        return vm;
    });