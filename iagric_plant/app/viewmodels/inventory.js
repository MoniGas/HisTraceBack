define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'plugins/dialog', 'utils', 'logininfo'],
    function (router, ko, km, $, jq, dialog, utils, loginInfo) {
        var moduleInfo = {
            moduleID: '61500',
            parentModuleID: '61000'
        }
        var searchTitle = ko.observable();
        var vmStoreList = {
            StoreArray: ko.observableArray(),
            SelectedId: ko.observable()
        }
        var getStoreModules = function () {
            var data;
            var sendData = {
                pageIndex: 1
            }
            $.ajax({
                type: "POST",
                url: "/Store/SelectStore",
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
        //ajax获取数据
        var getData = function (pageIndex, storeId, maName) {
            var sendData = {
                pageIndex: pageIndex,
                storeId: storeId,
                maName: maName
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/Store/GetInventory",
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
        //搜索仓库库存
        var searchStore = function (data, event) {
            var list = getDataKO(1, vmStoreList.SelectedId(), searchTitle());
            updateData(list);
        };
        //把获取的ajax数据转化为ko
        var getDataKO = function (pageIndex, storeId, maName) {
            var list = km.fromJS(getData(pageIndex, storeId, maName));
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
                            var list = getDataKO(num, vmStoreList.SelectedId(), searchTitle());
                            updateData(list);
                            this.pageSize = parseInt(vm.vmStore.pageSize());
                            this.totalCounts = parseInt(vm.vmStore.totalCounts());
                        }
                    }
                });
            }
        };
        var vm = {
            binding: function () {
                //初初化导航状态
                //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
                //初始化dealer列表数据
                vmStoreList.StoreArray(getStoreModules());
                vm.vmStore = getDataKO(1, -1, "");
                updateData(vm.vmStore);
            },
            goBack: function () {
                router.navigateBack();
            },
            vmStore: null,
            searchTitle: searchTitle,
            searchStore: searchStore,
            vmStoreList: vmStoreList
        }
        return vm;
    });