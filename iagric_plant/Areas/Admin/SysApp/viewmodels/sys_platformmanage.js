define(['plugins/dialog', 'plugins/router', 'knockout', 'knockout.mapping', 'jqPaginator', 'utils', 'logininfo', "./sys_addplatform", "./auditcodecount"],
function (dialog, router, ko, km, jq, utils, loginInfo, sys_addplatform, auditcodecount) {
    var moduleInfo = {
        moduleID: '11100',
        parentModuleID: '10000'
    }

    var name = ko.observable();
    //ajax获取数据
    var getData = function (pageIndex, name) {
        var sendData = {
            pageIndex: pageIndex,
            name: name
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/SysPlatForm/Index",
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

    //把获取的ajax数据转化为ko
    var getDataKO = function (pageIndex, name) {
        var list = km.fromJS(getData(pageIndex, name));
        return list;
    }

    //搜索生产基地
    var searchPlatForm = function (data, event) {
        var list = getDataKO(1, name());
        updateData(list);
    };

    //分页、搜索时更新数据源
    var updateData = function (list) {
        vm.vmPlatForm.ObjList(list.ObjList());
        vm.vmPlatForm.pageSize(list.pageSize());
        vm.vmPlatForm.totalCounts(list.totalCounts());
        vm.vmPlatForm.pageIndex(list.pageIndex());
    }
    //关联监管部门
    var addPlatForm = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        sys_addplatform.show().then(function () {
            name('');
            searchPlatForm();
        });
    }
    //设置审核码数量
    var AuditCodeCount = function (pid, data, event) {
//        var currentObj = $(event.target);
//        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var pid = ko.utils.unwrapObservable(pid);
        auditcodecount.show(pid).then(function () {
        });
    };
    //自定义绑定-分页控件
    ko.bindingHandlers.greenhousePagerBH = {
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
                        var list = getDataKO(num, name());
                        updateData(list);
                        this.pageSize = parseInt(vm.vmPlatForm.pageSize());
                        this.totalCounts = parseInt(vm.vmPlatForm.totalCounts());
                    }
                }
            });
        }
    };

    var mouseoverFun = function (data, event) {
        // 设置审核码数量
        var btnSeeCode = $("#btnSeeCode_" + data.PRRU_PlatForm_ID());
        btnSeeCode.css({ "display": "" });
        //        var btnDel = $("#btnDel_" + data.Enterprise_Info_ID());
        //        btnDel.css({ "display": "" });
    }

    var mouseoutFun = function (data, event) {
        //设置审核码数量
        var btnSeeCode = $("#btnSeeCode_" + data.PRRU_PlatForm_ID());
        btnSeeCode.css({ "display": "none" });
        // 删除
        //        var btnDel = $("#btnDel_" + data.Enterprise_Info_ID());
        //        btnDel.css({ "display": "none" });
    }

    var vm = {
        binding: function () {
            //初初化导航状态
            loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            //初始化brand列表数据
            vm.vmPlatForm = getDataKO(1, "");
        },
        goBack: function () {
            router.navigateBack();
        },
        vmPlatForm: null,
        addPlatForm: addPlatForm,
        AuditCodeCount: AuditCodeCount,
        searchPlatForm: searchPlatForm,
        name: name,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun,
        loginInfo: loginInfo
    }
    return vm;

});