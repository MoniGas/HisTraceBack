define(['plugins/dialog', 'plugins/router', 'knockout', 'knockout.mapping', 'jqPaginator', 'logininfo', "./operationtype_add", "./operationtype_edit", 'utils'],
function (dialog, router, ko, km, jq, loginInfo, operationtype_add, operationtype_edit, utils) {
    var moduleInfo = {
        moduleID: '10100',
        parentModuleID: '10001'
    }
    //添加生产环节
    var addOperationType = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        operationtype_add.show().then(function () {
            name('');
            var list = getDataKO(1, name());
            updateData(list);
        });
        //        router.navigate('#operationType_Add');
        //        searchOperationType();
    }

    var status = function (index) {
        var index = ko.utils.unwrapObservable(index);
        switch (index) {
            case 0:
                return "种植";
            case 1:
                return "加工";
            case 2:
                return "养殖";
        }
    }

    //编辑生产环节
    var editOperationType = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        //        router.navigate('#operationType_Edit?id=' + id);
        operationtype_edit.show(id).then(function () {
            var list = getDataKO(vm.vmOperationType.pageIndex(), name());
            updateData(list);
        });
    }

    var name = ko.observable();
    //ajax获取数据
    var getData = function (pageIndex, name) {
        var sendData = {
            pageIndex: pageIndex,
            operationName: name
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_OperationType/Index",
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

    //搜索生产环节
    var searchOperationType = function (data, event) {
        var list = getDataKO(1, name());
        updateData(list);
    };

    //刷新生产环节
    var refreshOperationType = function (data, event) {
        //searchTitle('');
        //getDataKO(1, "");
        location.reload();
    };

    //分页、搜索时更新数据源
    var updateData = function (list) {
        vm.vmOperationType.ObjList(list.ObjList());
        vm.vmOperationType.pageSize(list.pageSize());
        vm.vmOperationType.totalCounts(list.totalCounts());
        vm.vmOperationType.pageIndex(list.pageIndex());
    }

    //删除生产环节
    var delOperationType = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(id);
        dialog.showMessage("确定删除选择的数据吗？", '系统提示', [
        {
            title: '确定',
            callback: function () {
                var sendData = {
                    id: id
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_OperationType/Del",
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
                                    var currentPageRow = vm.vmOperationType.ObjList().length;
                                    var pageIndex = vm.vmOperationType.pageIndex();
                                    if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                        pageIndex = vm.vmOperationType.pageIndex() - 1;
                                    }

                                    var list = getDataKO(pageIndex, name());
                                    updateData(list);
                                    this.pageSize = parseInt(vm.vmOperationType.pageSize());
                                    this.totalCounts = parseInt(vm.vmOperationType.totalCounts());
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

    //自定义绑定-分页控件
    ko.bindingHandlers.operationTypePager = {
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
                        this.pageSize = parseInt(vm.vmOperationType.pageSize());
                        this.totalCounts = parseInt(vm.vmOperationType.totalCounts());
                    }
                }
            });
        }
    };

    var mouseoverFun = function (data, event) {
        // 删除
        var btnDel = $("#btnDel_" + data.Batch_ZuoYeType_ID());
        // 修改
        var btnEdit = $("#btnEdit_" + data.Batch_ZuoYeType_ID());

        btnDel.css({ "display": "" });
        btnEdit.css({ "display": "" });

    }

    var mouseoutFun = function (data, event) {
        // 删除
        var btnDel = $("#btnDel_" + data.Batch_ZuoYeType_ID());
        // 修改
        var btnEdit = $("#btnEdit_" + data.Batch_ZuoYeType_ID());

        btnDel.css({ "display": "none" });
        btnEdit.css({ "display": "none" });


    }

    var vm = {
        binding: function () {
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            //初始化brand列表数据
            vm.vmOperationType = getDataKO(1, "");
        },
        goBack: function () {
            router.navigateBack();
        },
        vmOperationType: null,
        addOperationType: addOperationType,
        searchOperationType: searchOperationType,
        name: name,
        refreshOperationType: refreshOperationType,
        delOperationType: delOperationType,
        editOperationType: editOperationType,
        status: status,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun
    }
    return vm;

});