define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'logininfo', './sys_brandadd', './sys_brandedit', 'plugins/dialog', 'utils'], function (router, ko, km, $, jq, loginInfo, sys_brandadd, sys_brandedit, dialog, utils) {
    var moduleInfo = {
        moduleID: '11010',
        parentModuleID: '10000'
    }

    var vmBrandModels = {
        AdvertModelsArray: ko.observableArray(),
        selectedOption: ko.observable()
    }

    var searchTitle = ko.observable();
    //ajax获取数据
    var getData = function (pageIndex) {
        var sendData = {
            pageIndex: pageIndex,
            brandName: searchTitle()
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/SysBrand/Index",
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
        vm.vmBrand.ObjList(list.ObjList());
        vm.vmBrand.pageSize(list.pageSize());
        vm.vmBrand.totalCounts(list.totalCounts());
        vm.vmBrand.pageIndex(list.pageIndex());
    }
    //搜索品牌
    var searchBrand = function (data, event) {
        var list = getDataKO(1);
        updateData(list);
    };
    var addBrand = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        sys_brandadd.show().then(function () {
            searchBrand();
        });
        //        router.navigate('#brand_Add');
    };
    //编辑品牌信息
    var editBrand = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        sys_brandedit.show(id).then(function (BrandName, Descriptions, logo, type) {
            if (type == 2) {
                data.BrandName(BrandName);
                data.Descriptions(Descriptions);
                data.Logo(logo);
            }
        });
        //        sys_brandedit.show(id).then(function () {
        //            searchBrand();
        //        });
    };
    //删除品牌
    var delBrand = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(id);
        dialog.showMessage("确定删除选择的数据吗?", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        brandId: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/SysBrand/Delete",
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
                                        var currentPageRow = vm.vmBrand.ObjList().length;
                                        var pageIndex = vm.vmBrand.pageIndex();
                                        if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                            pageIndex = vm.vmBrand.pageIndex() - 1;
                                        }
                                        var list = getDataKO(pageIndex, searchTitle());
                                        updateData(list);
                                        this.pageSize = parseInt(vm.vmBrand.pageSize());
                                        this.totalCounts = parseInt(vm.vmBrand.totalCounts());
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
    var getDataKO = function (pageIndex) {
        var list = km.fromJS(getData(pageIndex));
        return list;
    }
    //自定义绑定-分页控件
    ko.bindingHandlers.brandPager = {
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
                    //alert(type + '：' + num);
                    if (type == 'change') {
                        var list = getDataKO(num, searchTitle());

                        updateData(list);
                        this.pageSize = parseInt(vm.vmBrand.pageSize());
                        this.totalCounts = parseInt(vm.vmBrand.totalCounts());
                    }
                }
            });
        }
    };
    var mouseoverFun = function (data, event) {
        var self = $(event.target).closest('tr');
        var ShowHand = self.find("div[eleflag='ShowHand']");

        ShowHand.css({ "display": "" });
    }
    var mouseoutFun = function (data, event) {
        var self = $(event.target).closest('tr');
        var ShowHand = self.find("div[eleflag='ShowHand']");

        ShowHand.css({ "display": "none" });
    }
    var vm = {
        binding: function () {
            //初初化导航状态
            loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            //初始化brand列表数据
            vm.vmBrand = getDataKO(1);
            updateData(vm.vmBrand);
        },
        goBack: function () {
            router.navigateBack();
        },
        vmBrand: null,
        searchTitle: searchTitle,
        searchBrand: searchBrand,
        addBrand: addBrand,
        editBrand: editBrand,
        delBrand: delBrand,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun
    }
    return vm;
});