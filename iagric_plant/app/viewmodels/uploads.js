define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', './uploads_add', './uploads_edit', 'plugins/dialog', 'utils', 'logininfo'],
function (router, ko, km, $, jq, uploads_add, uploads_edit, dialog, utils, loginInfo) {
    var searchTitle = ko.observable();
    var moduleInfo = {
        moduleID: '10200',
        parentModuleID: '10001'
    }

    //ajax获取数据
    var getData = function (pageIndex, uploadsName) {
        var sendData = {
            pageIndex: pageIndex,
            uploadsName: uploadsName
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Uploads/Index",
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
        vm.vmUploads.ObjList(list.ObjList());
        vm.vmUploads.pageSize(list.pageSize());
        vm.vmUploads.totalCounts(list.totalCounts());
        vm.vmUploads.pageIndex(list.pageIndex());
    }
    //搜索产品
    var searchUploads = function (data, event) {
        var list = getDataKO(1, searchTitle());
        updateData(list);
    };
    //上传
    var addUploads = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        uploads_add.show().then(function () {
            searchTitle("");
            var list = getDataKO(1, "");
            updateData(list);
        });
    }
    //编辑
    var editUploads = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        uploads_edit.show(id).then(function () {
            var list = getDataKO(vm.vmUploads.pageIndex(), searchTitle());
            updateData(list);
        });
    };
    //把获取的ajax数据转化为ko
    var getDataKO = function (pageIndex, uploadsName) {
        var list = km.fromJS(getData(pageIndex, uploadsName));
        return list;
    }
    //自定义绑定-分页控件
    ko.bindingHandlers.uploadsPager = {
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
                        this.pageSize = parseInt(vm.vmUploads.pageSize());
                        this.totalCounts = parseInt(vm.vmUploads.totalCounts());
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
            //初始化dealer列表数据
            vm.vmUploads = getDataKO(1, "");
            updateData(vm.vmUploads);
        },
        goBack: function () {
            router.navigateBack();
        },
        vmUploads: null,
        searchTitle: searchTitle,
        searchUploads: searchUploads,
        addUploads: addUploads,
        editUploads: editUploads,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun
    }
    return vm;
});