define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'logininfo', 'plugins/dialog', 'utils'], function (router, ko, km, $, jq, loginInfo, dialog, utils) {
    var moduleInfo = {
        moduleID: '11020',
        parentModuleID: '10000'
    }

    var vmBrandModels = {
        AdvertModelsArray: ko.observableArray(),
        selectedOption: ko.observable()
    }
    var flag = ko.observable(false);
    var searchTitle = ko.observable();
    var brandEnterpriseStatus = ko.observable(-1);
    //ajax获取数据
    var getData = function (pageIndex) {
        var sendData = {
            pageIndex: pageIndex,
            searchName: searchTitle(),
            brandEnterpriseStatus: brandEnterpriseStatus()
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/SysBrand/QYBrandAuditEnterprise",
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
    var onchange = function (data, even) {
        var list = getDataKO(1);
        updateData(list);
    }
    // 审核通过
    var resetPass = function (id, data, event) {
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        dialog.showMessage("确定审核通过？", '系统提示', [
        {
            title: '确定',
            callback: function () {
                var sendData = {
                    id: ko.utils.unwrapObservable(id)
                }
                $.ajax({
                    type: "POST",
                    url: "/SysBrand/Audit",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                            if (jsonResult.code != 0) {
                                searchBrand();
                            }
                            //                            getLoginInfo(ko.utils.unwrapObservable(id));
                        }
                        }]);
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
    //把获取的ajax数据转化为ko
    var getDataKO = function (pageIndex) {
        var list = km.fromJS(getData(pageIndex));
        return list;
    }
    //显示状态
    var replace = function (value) {
        var flag = "";
        var value = ko.utils.unwrapObservable(value);
        switch (value) {
            case 0:
                flag = "未审核";
                return flag;
            case 1:
                flag = "审核通过";
                return flag;
                //            case -1:
                //                flag = "审核不通过";
                //                return flag;
        }
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
                        var list = getDataKO(num);

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
        //审核通过
        var btnReset = $("#btnReset_" + data.Brand_Enterprise_ID());

        ShowHand.css({ "display": "" });
        btnReset.css({ "display": "none" });

        switch (data.BrandEnterpriseStatus()) {
            case 0: // 未审核
                btnReset.css({ "display": "" });
                break;
        }
    }
    var mouseoutFun = function (data, event) {
        var self = $(event.target).closest('tr');
        var ShowHand = self.find("div[eleflag='ShowHand']");
        var btnReset = $("#btnReset_" + data.Brand_Enterprise_ID());

        ShowHand.css({ "display": "none" });
        btnReset.css({ "display": "none" });
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
        brandEnterpriseStatus: brandEnterpriseStatus,
        searchBrand: searchBrand,
        onchange: onchange,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun,
        resetPass: resetPass,
        flag: flag,
        replace: replace
    }
    return vm;
});