define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'logininfo', './origin_add', './origin_edit', 'plugins/dialog', 'utils'],
function (router, ko, km, $, jq, loginInfo, origin_add, origin_edit, dialog, utils) {
    var moduleInfo = {
        moduleID: '31000',
        parentModuleID: '10001'
    }

    var vmOriginModels = {
        AdvertModelsArray: ko.observableArray(),
        selectedOption: ko.observable()
    }

    var searchTitle = ko.observable();
    //ajax获取数据
    var getData = function (pageIndex, originName) {
        var sendData = {
            pageIndex: pageIndex,
            originName: originName
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/AdminOrigin/Index",
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
        vm.vmOrigin.ObjList(list.ObjList());
        vm.vmOrigin.pageSize(list.pageSize());
        vm.vmOrigin.totalCounts(list.totalCounts());
        vm.vmOrigin.pageIndex(list.pageIndex());
    }
    //搜索原料
    var searchOrigin = function (data, event) {
        var list = getDataKO(1,searchTitle());
        updateData(list);
    };
    var addOrigin = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        origin_add.show().then(function () {
            searchTitle('');
            var list = getDataKO(1, "");
            updateData(list);
        });
    };
    //编辑原料信息
    var editOrigin = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        origin_edit.show(id).then(function (OriginName, Descriptions, originImgInfo, type) {
                var list = getDataKO(vm.vmOrigin.pageIndex(), '');
                updateData(list);
        });
    };
    //删除原料
    var delOrigin = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(id);
        dialog.showMessage("确定删除选择的数据吗?", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        originId: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/AdminOrigin/Delete",
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
                                        var currentPageRow = vm.vmOrigin.ObjList().length;
                                        var pageIndex = vm.vmOrigin.pageIndex();
                                        if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                            pageIndex = vm.vmOrigin.pageIndex() - 1;
                                        }
                                        var list = getDataKO(pageIndex, searchTitle());
                                        updateData(list);
                                        this.pageSize = parseInt(vm.vmOrigin.pageSize());
                                        this.totalCounts = parseInt(vm.vmOrigin.totalCounts());
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
        var getDataKO = function (pageIndex, originName) {
        var list = km.fromJS(getData(pageIndex,originName));
        return list;
    }
    //自定义绑定-分页控件
    ko.bindingHandlers.originPager = {
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
                        this.pageSize = parseInt(vm.vmOrigin.pageSize());
                        this.totalCounts = parseInt(vm.vmOrigin.totalCounts());
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
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            //初始化Origin列表数据
            vm.vmOrigin = getDataKO(1,'');
            updateData(vm.vmOrigin);
        },
        goBack: function () {
            router.navigateBack();
        },
        vmOrigin: null,
        searchTitle: searchTitle,
        searchOrigin: searchOrigin,
        addOrigin: addOrigin,
        editOrigin: editOrigin,
        delOrigin: delOrigin,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun
    }
    return vm;
});