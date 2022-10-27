define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'logininfo', './regionalbrand_add', 'utils'], function (router, ko, km, $, jq, loginInfo, regionalbrand_add, utils) {
    var moduleInfo = {
        moduleID: '12001',
        parentModuleID: '10001'
    }
    var flag = ko.observable(false);
    var mName = ko.observable();

    //ajax获取数据
    var getData = function (pageIndex, mName) {
        var sendData = {
            pageIndex: pageIndex,
            mName: mName

        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Brand/RequestBrandEnterprise",
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
        vm.vmregionbrand.ObjList(list.ObjList());
        vm.vmregionbrand.pageSize(list.pageSize());
        vm.vmregionbrand.totalCounts(list.totalCounts());
        vm.vmregionbrand.pageIndex(list.pageIndex());
    }
    //查询条件
    var searchRB = function (data, event) {
        var list = getDataKO(1, mName());
        updateData(list);
    };
    //申请加入区域品牌
    var addregionalbrand = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        regionalbrand_add.show().then(function () {
            searchRB();
        });
    };
    //把获取的ajax数据转化为ko
    var getDataKO = function (pageIndex, mName) {
        var list = km.fromJS(getData(pageIndex, mName));
        return list;
    }
    //自定义绑定-分页控件
    ko.bindingHandlers.regionbrandPager = {
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
                        var list = getDataKO(num, mName());
                        updateData(list);
                        this.pageSize = parseInt(vm.vmregionbrand.pageSize());
                        this.totalCounts = parseInt(vm.vmregionbrand.totalCounts());
                    }
                }
            });
        }
    };
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
            case -1:
                flag = "审核不通过";
                return flag;
        }
    }
    var vm = {
        binding: function () {
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            //初始化列表数据
            vm.vmregionbrand = getDataKO(1, "");
            updateData(vm.vmregionbrand);
        },
        goBack: function () {
            router.navigateBack();
        },
        vmregionbrand: null,
        searchRB: searchRB,
        mName: mName,
        addregionalbrand: addregionalbrand,
        flag: flag,
        replace: replace
    }
    return vm;
});